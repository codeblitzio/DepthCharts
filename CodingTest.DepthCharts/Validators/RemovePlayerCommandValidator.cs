using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Repositories;
using FluentValidation;

namespace CodingTest.DepthCharts.Validators;

public class RemovePlayerCommandValidator : AbstractValidator<RemovePlayerCommand>
{
    public RemovePlayerCommandValidator(IRepository repository)
	{
        RuleFor(command => command.PlayerId).GreaterThan(0);

        RuleFor(command => command.PositionId).NotEmpty();

        RuleFor(command => command.PositionId).MustAsync(async (id, cancellation) =>
        {
            var positionIds = await repository.GetPositionIdsAsync(cancellation);
            return positionIds.Any(positionId => positionId == id);
        }).When(command => command.PositionId != null).WithMessage("The 'PositionId' is invalid.");
    }
}
