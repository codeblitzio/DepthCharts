using AutoMapper;
using CodingTest.DepthCharts.Exceptions;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Repositories;
using FluentValidation;
using MediatR;

namespace CodingTest.DepthCharts.Handlers;

public class GetTrailingPlayersHandler : IRequestHandler<GetTrailingPlayersQuery, GetTrailingPlayersResult>
{
    readonly IRepository _repository;
    readonly IMapper _mapper;
    readonly IValidator<GetTrailingPlayersQuery> _validator;
    readonly ILogger<GetTrailingPlayersHandler> _logger;

    public GetTrailingPlayersHandler(IRepository repository, IMapper mapper, IValidator<GetTrailingPlayersQuery> validator,
        ILogger<GetTrailingPlayersHandler> logger)
	{
        _repository = repository;
        _mapper = mapper;
        _validator = validator; 
        _logger = logger;
    }

    public async Task<GetTrailingPlayersResult> Handle(GetTrailingPlayersQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetTrailingDepthChartQuery {@query}", query);

        // use fluent validations

        var validation = await _validator.ValidateAsync(query, cancellationToken);

        if (!validation.IsValid)
        {
            // the helland middleware will return a problem details response summarising the failure
            throw new BadRequestException($"{validation.ToString(" ~ ")}");
        }

        // retrieve the position associated with the request

        var position = await _repository.GetPositionAsync(query.PositionId, cancellationToken);

        var players = position.Players.ToList();

        var index = players.FindIndex(p => p.PlayerId == query.PlayerId);

        // return a not found problem details if the player wasn't found
        if (index == -1)
        {
            throw new BadRequestException("The Player was not found");
        }

        players.RemoveRange(0, index + 1);

        var result = new GetTrailingPlayersResult
        {
            Players = players.Select(p => p.PlayerId)
        };

        _logger.LogInformation("Handled GetTrailingDepthChartQuery {@query}", query);

        return result;
    }
}
