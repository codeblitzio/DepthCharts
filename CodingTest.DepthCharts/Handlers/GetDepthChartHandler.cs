using AutoMapper;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Repositories;
using MediatR;

namespace CodingTest.DepthCharts.Handlers;

public class GetDepthChartHandler : IRequestHandler<GetDepthChartQuery, GetDepthChartResponse>
{
    readonly IRepository _repository;
    readonly IMapper _mapper;
    readonly ILogger<GetDepthChartHandler> _logger;

    public GetDepthChartHandler(IRepository repository, IMapper mapper, ILogger<GetDepthChartHandler> logger)
	{
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetDepthChartResponse> Handle(GetDepthChartQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetFullDepthChartQuery {@query}", query);

        var positions = new List<GetDepthChartResponse.Position>();

        var entityPositions = await _repository.GetPositionsAsync(cancellationToken);

        foreach(var entityPosition in entityPositions)
        {
            positions.Add(new GetDepthChartResponse.Position
            {
                PositionId = entityPosition.PositionId,
                Players = entityPosition.Players.Select(p => p.PlayerId)
            });
        }

        var result = new GetDepthChartResponse
        {
            Positions = positions.Where(p => p.Players.Any())
        };

        _logger.LogInformation("Handled GetFullDepthChartQuery {@query}", query);

        return result;
    }
}
