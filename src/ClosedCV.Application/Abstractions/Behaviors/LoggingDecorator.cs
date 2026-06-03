using ClosedCV.Application.Abstractions.Messaging;
using ClosedCV.Domain.SharedKernel;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace ClosedCV.Application.Abstractions.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class QueryHandler<TQuery, TResponse>(
        ILogger<QueryHandler<TQuery, TResponse>> logger,
        IQueryHandler<TQuery, TResponse> handler
    )
        : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            logger.LogInformation("Processing query {query}", queryName);

            Result<TResponse> result = await handler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed query {query}", queryName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed query {query} with error", queryName);
                }
            }

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ILogger<CommandBaseHandler<TCommand>> logger,
        ICommandHandler<TCommand> handler
    )
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Processing command {command}", commandName);

            Result result = await handler.HandleAsync(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {command}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed command {command} with error", commandName);
                }
            }

            return result;
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
        ILogger<CommandHandler<TCommand, TResponse>> logger,
        ICommandHandler<TCommand, TResponse> handler
    )
        : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Processing command {command}", commandName);

            Result<TResponse> result = await handler.HandleAsync(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {command}", commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed command {command} with error", commandName);
                }
            }

            return result;
        }
    }
}
