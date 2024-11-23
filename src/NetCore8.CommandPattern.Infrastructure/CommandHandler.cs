using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetCore8.CommandPattern.Core.Commands.Interfaces;
using NetCore8.CommandPattern.Core.Commands;
using NetCore8.CommandPattern.Core.Interfaces;
using NetCore8.CommandPattern.Core.Metrics;
using NetCore8.CommandPattern.Core.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json; 
using NetCore8.CommandPattern.Infrastructure;

public class CommandHandler : ICommandHandler
{
    private readonly CommandContext _context;
    private readonly ITransactionManager _transactionManager;
    private readonly ICacheProvider _cacheProvider;
    private readonly AuthorizationHandler _authHandler;
    private readonly IEventPublisher _eventPublisher;
    private readonly DbContext _dbContext;
    private readonly ILogger<CommandHandler> _logger;
    private readonly IMetricsRegistry _metrics;

    public CommandHandler(
        CommandContext context,
        ITransactionManager transactionManager,
        ICacheProvider cacheProvider,
        AuthorizationHandler authHandler,
        IEventPublisher eventPublisher,
        DbContext dbContext,
        ILogger<CommandHandler> logger,
        IMetricsRegistry metrics)
    {
        _context = context;
        _transactionManager = transactionManager;
        _cacheProvider = cacheProvider;
        _authHandler = authHandler;
        _eventPublisher = eventPublisher;
        _dbContext = dbContext;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<object> HandleAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var correlationId = Guid.NewGuid().ToString();
        var commandType = typeof(TCommand).Name;
        var stopwatch = Stopwatch.StartNew();

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["correlationId"] = correlationId,
            ["commandType"] = commandType
        });

        try
        {
            _logger.LogInformation(
                "Starting command execution. Type: {CommandType}, Data: {@Command}",
                commandType,
                command);

            if (command is IValidatableCommand validatable)
            {
                _logger.LogDebug("Validating command {CommandType}", commandType);
                await validatable.ValidateAsync(_context);
            }

            if (command is IAuthorizationCommand authCommand)
            {
                _logger.LogDebug("Checking authorization for command {CommandType}", commandType);
                if (!_authHandler.HasPermissions(_context.User, authCommand.RequiredPermissions))
                {
                    _logger.LogWarning(
                        "Authorization failed for command {CommandType}. User: {User}, Required Permissions: {@Permissions}",
                        commandType,
                        _context.User?.Identity?.Name,
                        authCommand.RequiredPermissions);
                    throw new UnauthorizedAccessException();
                }
            }

            object result;
            if (command is IRetryableCommand retryable)
            {
                result = await HandleWithRetryAsync(command, retryable);
            }
            else if (command is ICacheableCommand cacheable)
            {
                result = await HandleCacheableCommandAsync(command, cacheable);
            }
            else
            {
                result = await ExecuteCommandWithAudit(command);
            }

            stopwatch.Stop();
            _metrics.RecordCommandDuration<TCommand>(stopwatch.Elapsed);

            _logger.LogInformation(
                "Command completed successfully. Type: {CommandType}, Duration: {Duration}ms",
                commandType,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metrics.IncrementCommandFailure<TCommand>();

            _logger.LogError(
                ex,
                "Command failed. Type: {CommandType}, Duration: {Duration}ms, Error: {Error}",
                commandType,
                stopwatch.ElapsedMilliseconds,
                ex.Message);

            throw;
        }
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
                _logger.LogWarning(
                    "Retry attempt {Attempt}/{MaxRetries} for command {CommandType}. Error: {Error}",
                    attempts,
                    retryable.MaxRetries,
                    typeof(TCommand).Name,
                    ex.Message);

                await Task.Delay(retryable.RetryDelay);
            }
        }
    }

    private async Task<object> HandleCacheableCommandAsync<TCommand>(TCommand command, ICacheableCommand cacheable)
        where TCommand : ICommand
    {
        _logger.LogDebug(
            "Attempting to get cached result for command {CommandType} with key {CacheKey}",
            typeof(TCommand).Name,
            cacheable.CacheKey);

        return await _cacheProvider.GetOrSetAsync(
            cacheable.CacheKey,
            () => ExecuteCommandWithAudit(command),
            cacheable.CacheDuration);
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
        await _dbContext.Set<AuditLog>().AddAsync(log);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Audit log created. CommandType: {CommandType}, Success: {IsSuccessful}, User: {UserId}",
            log.CommandType,
            log.IsSuccessful,
            log.UserId);
    }
}