using ClosedCV.Application.Abstractions.Messaging;
using ClosedCV.Domain.SharedKernel;
using FluentValidation;
using FluentValidation.Results;

namespace ClosedCV.Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class CommandBaseHandler<TCommand>(
        IEnumerable<IValidator<TCommand>> validators,
        ICommandHandler<TCommand> handler
    )
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(validators, command);

            if (validationFailures.Length == 0)
            {
                return await handler.HandleAsync(command, cancellationToken);
            }

            return CreateValidationError(validationFailures);
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
        IEnumerable<IValidator<TCommand>> validators,
        ICommandHandler<TCommand, TResponse> handler
    )
        : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(validators, command);

            if (validationFailures.Length == 0)
            {
                return await handler.HandleAsync(command, cancellationToken);
            }

            return CreateValidationError(validationFailures);
        }
    }

    // Helper methods
    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
        IEnumerable<IValidator<TCommand>> validators,
        TCommand command
    )
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context)));
        ValidationFailure[] failures = [.. results.Where(v => !v.IsValid).SelectMany(v => v.Errors)];
        return failures;
    }

    private static ValidationError CreateValidationError(ValidationFailure[] errors)
    {
        return new([.. errors.Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))]);
    }
}
