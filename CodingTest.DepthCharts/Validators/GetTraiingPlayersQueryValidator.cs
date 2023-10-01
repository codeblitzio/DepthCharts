using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Repositories;
using FluentValidation;

namespace CodingTest.DepthCharts.Validators;

public class GetTrailingPlayersQueryValidator : AbstractValidator<GetTrailingPlayersQuery>
{
    public GetTrailingPlayersQueryValidator(IRepository repository)
	{
        RuleFor(query => query.PlayerId).GreaterThan(0);

        RuleFor(command => command.PositionId).NotEmpty();

        RuleFor(command => command.PositionId).MustAsync(async (id, cancellation) =>
        {
            var positionIds = await repository.GetPositionIdsAsync(cancellation);
            return positionIds.Any(positionId => positionId == id);
        }).When(command => command.PositionId != null).WithMessage("The 'PositionId' is invalid.");
    }
}
