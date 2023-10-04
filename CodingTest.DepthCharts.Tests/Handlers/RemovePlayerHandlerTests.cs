using AutoMapper;
using CodingTest.DepthCharts.Exceptions;
using CodingTest.DepthCharts.Handlers;
using CodingTest.DepthCharts.Mappers;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Options;
using CodingTest.DepthCharts.Repositories;
using CodingTest.DepthCharts.Validators;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CodingTest.DepthCharts.Tests.Handlers;

public class RemovePlayerHandlerTests
{
	readonly ILogger<RemovePlayerHandler> _logger;
	readonly IRepository _repository;
    readonly IValidator<RemovePlayerCommand> _validator;
    readonly IOptions<AppOptions> _options;
    readonly IMapper _mapper;


    public RemovePlayerHandlerTests()
	{
        // moq the logger
		_logger = new Mock<ILogger<RemovePlayerHandler>>().Object;

        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions { Sport = "NFL" });

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        // use the concret validator
        _validator = new RemovePlayerCommandValidator(_repository);

        // use the concrete mapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Remove_Player()
    {
        //arrange
        var position = await _repository.GetPositionAsync("QB", default);
        var players = position.Players.ToList();

        players.Add(new Entities.Player { PlayerId = 1, Name = "Bob", PositionId = "QB" });
        players.Add(new Entities.Player { PlayerId = 2, Name = "Alice", PositionId = "QB" });
        players.Add(new Entities.Player { PlayerId = 3, Name = "Charlie", PositionId = "QB" });

        position.Players = players;

        var command = new RemovePlayerCommand { PlayerId = 2, PositionId = "QB" };

        var sut = new RemovePlayerHandler(_repository, _mapper, _validator, _logger);

        // act
        await sut.Handle(command, default);

        // assert
        Assert.Equal(2, position.Players.Count());
        Assert.Equal(1, position.Players.ElementAt(0).PlayerId);
        Assert.Equal(3, position.Players.ElementAt(1).PlayerId);
    }

    [Fact]
    public async Task Add_Player_Throws_BadRequestException()
    {
        //arrange 
        var command = new RemovePlayerCommand { PlayerId = 1, PositionId = "SP" };

        var sut = new RemovePlayerHandler(_repository, _mapper, _validator, _logger);

        // act and asset
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => sut.Handle(command, default));
        Assert.Equal("The 'PositionId' is invalid.", ex.Message);
    }
}
