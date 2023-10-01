using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Repositories;
using FluentValidation;

namespace CodingTest.DepthCharts.Validators;

public class AddPlayerCommandValidator : AbstractValidator<AddPlayerCommand>
{
    public AddPlayerCommandValidator(IRepository repository)
	{
        RuleFor(command => command.Player).NotNull();

        RuleFor(command => command.Player.PlayerId).GreaterThan(0).When(command => command.Player != null);
        RuleFor(command => command.Player.PositionId).NotEmpty().When(command => command.Player != null);
        RuleFor(command => command.Player.Name).NotEmpty().When(command => command.Player != null);

        RuleFor(command => command.Depth).GreaterThan(-1).When(command => command.Depth != null);

        RuleFor(command => command.PositionId).NotEmpty();

        RuleFor(command => command.PositionId).MustAsync(async (id, cancellation) =>
        {
            var positionIds = await repository.GetPositionIdsAsync(cancellation);
            return positionIds.Any(positionId => positionId == id);
        }).When(command => command.PositionId != null).WithMessage("The 'PositionId' is invalid.");
    }
}
