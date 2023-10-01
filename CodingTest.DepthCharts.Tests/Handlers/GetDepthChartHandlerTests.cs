using AutoMapper;
using CodingTest.DepthCharts.Handlers;
using CodingTest.DepthCharts.Mappers;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Options;
using CodingTest.DepthCharts.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CodingTest.DepthCharts.Tests.Handlers;

public class GetDepthChartHandlerTests
{
	readonly ILogger<GetDepthChartHandler> _logger;
	readonly IRepository _repository;
    readonly IOptions<AppOptions> _options;
    readonly IMapper _mapper;


    public GetDepthChartHandlerTests()
	{
        // moq the logger
		_logger = new Mock<ILogger<GetDepthChartHandler>>().Object;

        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions { Sport = "MLB" });

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        // use the concrete mapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Get_Depth_Chart()
    {
        //arrange
        var position = await _repository.GetPositionAsync("SP", default);
        var players = position.Players.ToList();

        players.Add(new Entities.Player { PlayerId = 1, Name = "Bob", PositionId = "SP" });
        players.Add(new Entities.Player { PlayerId = 2, Name = "Alice", PositionId = "SP" });

        position.Players = players;

        position = await _repository.GetPositionAsync("C", default);
        players = position.Players.ToList();

        players.Add(new Entities.Player { PlayerId = 3, Name = "Charlie", PositionId = "SP" });

        position.Players = players;

        var query = new GetDepthChartQuery();

        var sut = new GetDepthChartHandler(_repository, _mapper, _logger);

        // act
        var result = await sut.Handle(query, default);

        // assert
        Assert.True(result.Positions.Count() == 2);
        Assert.True(result.Positions.ElementAt(0).Players.ElementAt(0) == 1);
        Assert.True(result.Positions.ElementAt(0).Players.ElementAt(1) == 2);
        Assert.True(result.Positions.ElementAt(1).Players.ElementAt(0) == 3);
    }
}
