
using Microsoft.EntityFrameworkCore;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Interfaces;
using NetCore8.CommandPattern.Core.Models;
using System.Security.Claims;
using System.Text.Json;

namespace NetCore8.CommandPattern.Infrastructure
{
    public class CommandHandler : ICommandHandler
    {
        private readonly CommandContext _context;
        private readonly ITransactionManager _transactionManager;
        private readonly ICacheProvider _cacheProvider;
        private readonly AuthorizationHandler _authHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly DbContext _dbContext;

        public CommandHandler(
            CommandContext context,
            ITransactionManager transactionManager,
            ICacheProvider cacheProvider,
            AuthorizationHandler authHandler,
            IEventPublisher eventPublisher,
            DbContext dbContext)
        {
            _context = context;
            _transactionManager = transactionManager;
            _cacheProvider = cacheProvider;
            _authHandler = authHandler;
            _eventPublisher = eventPublisher;
            _dbContext = dbContext;
        }

        public async Task<object> HandleAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command is IValidatableCommand validatable)
                await validatable.ValidateAsync(_context);

            if (command is IAuthorizationCommand authCommand)
                if (!_authHandler.HasPermissions(_context.User, authCommand.RequiredPermissions))
                    throw new UnauthorizedAccessException();

            if (command is IRateLimitedCommand rateLimit)
            {
                // Rate limit implementation would go here
            }

            if (command is IRetryableCommand retryable)
                return await HandleWithRetryAsync(command, retryable);

            if (command is ICacheableCommand cacheable)
                return await _cacheProvider.GetOrSetAsync(
                    cacheable.CacheKey,
                    () => ExecuteCommandWithAudit(command),
                    cacheable.CacheDuration);

            return await ExecuteCommandWithAudit(command);
        }

        private async Task<object> HandleWithRetryAsync<TCommand>(TCommand command, IRetryableCommand retryable) 
            where TCommand : ICommand
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    return await ExecuteCommandWithAudit(command);
                }
                catch (Exception ex) when (attempts < retryable.MaxRetries && retryable.ShouldRetry(ex))
                {
                    attempts++;
                    await Task.Delay(retryable.RetryDelay);
                }
            }
        }

        private async Task<object> ExecuteCommandWithAudit<TCommand>(TCommand command) where TCommand : ICommand
        {
            var auditLog = new AuditLog
            {
                CommandType = typeof(TCommand).Name,
                CommandData = JsonSerializer.Serialize(command),
                UserId = _context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                ExecutedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _transactionManager.ExecuteInTransactionAsync(async () =>
                {
                    var commandResult = await command.ExecuteAsync(_context);
                    if (command is IBatchCommand<object> batchCommand)
                    {
                        // Batch processing implementation would go here
                    }
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