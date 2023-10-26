using AutoMapper;
using CodingTest.DepthCharts.Handlers;
using CodingTest.DepthCharts.Mappers;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Options;
using CodingTest.DepthCharts.Repositories;
using CodingTest.DepthCharts.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CodingTest.DepthCharts.Tests.Acceptance;

public class AcceptanceTests
{
    readonly IRepository _repository;
    readonly IOptions<AppOptions> _options;
    readonly IMapper _mapper;
    readonly AddPlayerHandler _addPlayerHandler;
    readonly GetDepthChartHandler _getDepthChartHandler;
    readonly GetTrailingPlayersHandler _getTrailingPlayerHandler;

    public AcceptanceTests()
    {
        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions());

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        // use the concrete mapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        _mapper = config.CreateMapper();


        _addPlayerHandler = new AddPlayerHandler(
            _repository,
            _mapper,
            new AddPlayerCommandValidator(_repository),
            new Mock<ILogger<AddPlayerHandler>>().Object);

        _getDepthChartHandler = new GetDepthChartHandler(
            _repository,
            _mapper,
            new Mock<ILogger<GetDepthChartHandler>>().Object);

        _getTrailingPlayerHandler = new GetTrailingPlayersHandler(
             _repository,
            _mapper,
            new GetTrailingPlayersQueryValidator(_repository),
            new Mock<ILogger<GetTrailingPlayersHandler>>().Object);
    }

    [Fact]
    public async Task NFL_Test()
    {
        // arrange
        _options.Value.Sport = "NFL";

        var _bob = new AddPlayerCommand.PlayerObj { PlayerId = 1, Name = "Bob", PositionId = "WR" };
        var _alice = new AddPlayerCommand.PlayerObj { PlayerId = 2, Name = "Alice", PositionId = "WR" };
        var _charlie = new AddPlayerCommand.PlayerObj { PlayerId = 3, Name = "Charlie", PositionId = "WR" };

        var addPlayerCommand1 = new AddPlayerCommand { Player = _bob, PositionId = "WR", Depth = 0 };
        var addPlayerCommand2 = new AddPlayerCommand { Player = _alice, PositionId = "WR", Depth = 0 };
        var addPlayerCommand3 = new AddPlayerCommand { Player = _charlie, PositionId = "WR", Depth = 2 };
        var addPlayerCommand4 = new AddPlayerCommand { Player = _bob, PositionId = "KR", Depth = 2 };

        var trailingPlayerQuery = new GetTrailingPlayersQuery { PlayerId = 2, PositionId = "WR" };

        // act
        await _addPlayerHandler.Handle(addPlayerCommand1, default);
        await _addPlayerHandler.Handle(addPlayerCommand2, default);
        await _addPlayerHandler.Handle(addPlayerCommand3, default);
        await _addPlayerHandler.Handle(addPlayerCommand4, default);

        var depthChartResult = await _getDepthChartHandler.Handle(new GetDepthChartQuery(), default);
        var trailingPlayersResult = await _getTrailingPlayerHandler.Handle(trailingPlayerQuery, default);

        // assert
        Assert.Equal(2, depthChartResult.Positions.Count());
        Assert.Equal("WR", depthChartResult.Positions.ElementAt(0).PositionId);
        Assert.Equal(2, depthChartResult.Positions.ElementAt(0).Players.ElementAt(0));
        Assert.Equal(1, depthChartResult.Positions.ElementAt(0).Players.ElementAt(1));
        Assert.Equal(3, depthChartResult.Positions.ElementAt(0).Players.ElementAt(2));
        Assert.Equal("KR", depthChartResult.Positions.ElementAt(1).PositionId);
        Assert.Equal(1, depthChartResult.Positions.ElementAt(1).Players.ElementAt(0));

        Assert.Equal(2, trailingPlayersResult.Players.Count());
        Assert.Equal(1, trailingPlayersResult.Players.ElementAt(0));
        Assert.Equal(3, trailingPlayersResult.Players.ElementAt(1));
    }

    [Fact]
    public async Task MLB_Test()
    {
        // arrange
        _options.Value.Sport = "MLB";

        var _bob = new AddPlayerCommand.PlayerObj { PlayerId = 1, Name = "Bob", PositionId = "SP" };
        var _alice = new AddPlayerCommand.PlayerObj { PlayerId = 2, Name = "Alice", PositionId = "SP" };
        var _charlie = new AddPlayerCommand.PlayerObj { PlayerId = 3, Name = "Charlie", PositionId = "SP" };

        var addPlayerCommand1 = new AddPlayerCommand { Player = _bob, PositionId = "SP", Depth = 0 };
        var addPlayerCommand2 = new AddPlayerCommand { Player = _alice, PositionId = "SP", Depth = 0 };
        var addPlayerCommand3 = new AddPlayerCommand { Player = _charlie, PositionId = "SP", Depth = 2 };
        var addPlayerCommand4 = new AddPlayerCommand { Player = _bob, PositionId = "C", Depth = 2 };

        var trailingPlayerQuery = new GetTrailingPlayersQuery { PlayerId = 2, PositionId = "SP" };

        // act
        await _addPlayerHandler.Handle(addPlayerCommand1, default);
        await _addPlayerHandler.Handle(addPlayerCommand2, default);
        await _addPlayerHandler.Handle(addPlayerCommand3, default);
        await _addPlayerHandler.Handle(addPlayerCommand4, default);

        var depthChartResult = await _getDepthChartHandler.Handle(new GetDepthChartQuery(), default);
        var trailingPlayersResult = await _getTrailingPlayerHandler.Handle(trailingPlayerQuery, default);

        // assert
        Assert.Equal(2, depthChartResult.Positions.Count());
        Assert.Equal("SP", depthChartResult.Positions.ElementAt(0).PositionId);
        Assert.Equal(2, depthChartResult.Positions.ElementAt(0).Players.ElementAt(0));
        Assert.Equal(1, depthChartResult.Positions.ElementAt(0).Players.ElementAt(1));
        Assert.Equal(3, depthChartResult.Positions.ElementAt(0).Players.ElementAt(2));
        Assert.Equal("C", depthChartResult.Positions.ElementAt(1).PositionId);
        Assert.Equal(1, depthChartResult.Positions.ElementAt(1).Players.ElementAt(0));

        Assert.Equal(2, trailingPlayersResult.Players.Count());
        Assert.Equal(1, trailingPlayersResult.Players.ElementAt(0));
        Assert.Equal(3, trailingPlayersResult.Players.ElementAt(1));
    }
}