
using CommandPatternDemo.Core.Commands;
using CommandPatternDemo.Core.Interfaces;
using CommandPatternDemo.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CommandPatternDemo.Infrastructure
{
    public class CommandHandler : ICommandHandler
    {
        private readonly CommandContext _context;
        private readonly ITransactionManager _transactionManager;
        private readonly ICacheProvider _cacheProvider;
        private readonly AuthorizationHandler _authHandler;
        private readonly DbContext _dbContext;

        public CommandHandler(
            CommandContext context,
            ITransactionManager transactionManager,
            ICacheProvider cacheProvider,
            AuthorizationHandler authHandler,
            DbContext dbContext)
        {
            _context = context;
            _transactionManager = transactionManager;
            _cacheProvider = cacheProvider;
            _authHandler = authHandler;
            _dbContext = dbContext;
        }

        public async Task<object> HandleAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command is IAuthorizationCommand authCommand)
            {
                if (!_authHandler.HasPermissions(_context.User, authCommand.RequiredPermissions))
                    throw new UnauthorizedAccessException("User lacks required permissions");
            }

            if (command is ICacheableCommand cacheCommand)
            {
                return await _cacheProvider.GetOrSetAsync(
                    cacheCommand.CacheKey,
                    () => ExecuteCommandWithAudit(command),
                    cacheCommand.CacheDuration);
            }

            return await ExecuteCommandWithAudit(command);
        }

        private async Task<object> ExecuteCommandWithAudit<TCommand>(TCommand command) where TCommand : ICommand
        {
            var auditLog = new AuditLog
            {
                CommandType = typeof(TCommand).Name,
                CommandData = System.Text.Json.JsonSerializer.Serialize(command),
                UserId = _context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                ExecutedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _transactionManager.ExecuteInTransactionAsync(async () =>
                {
                    _context.Logger.LogInformation($"Executing command {typeof(TCommand).Name}");
                    var commandResult = await command.ExecuteAsync(_context);
                    await _context.DbContext.SaveChangesAsync();
                    return commandResult;
                });

                auditLog.IsSuccessful = true;
                await SaveAuditLog(auditLog);
                return result;
            }
            catch (Exception ex)
            {
                auditLog.IsSuccessful = false;
                auditLog.ErrorMessage = ex.Message;
                await SaveAuditLog(auditLog);
                throw;
            }
        }

        private async Task SaveAuditLog(AuditLog log)
        {
            _dbContext.Set<AuditLog>().Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}