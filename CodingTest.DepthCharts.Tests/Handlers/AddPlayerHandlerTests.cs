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

public class AddPlayerHandlerTests
{
	readonly ILogger<AddPlayerHandler> _logger;
	readonly IRepository _repository;
    readonly IValidator<AddPlayerCommand> _validator;
    readonly IOptions<AppOptions> _options;
    readonly IMapper _mapper;

    readonly AddPlayerCommand.PlayerObj _bob = new() { PlayerId = 1, Name = "Bob", PositionId = "SP" };
    readonly AddPlayerCommand.PlayerObj _alice = new() { PlayerId = 2, Name = "Alice", PositionId = "SP" };
    readonly AddPlayerCommand.PlayerObj _charlie = new() { PlayerId = 3, Name = "Charlie", PositionId = "SP" };


    public AddPlayerHandlerTests()
	{
        // moq the logger
		_logger = new Mock<ILogger<AddPlayerHandler>>().Object;

        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions { Sport = "MLB" });

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        // use the concret validator
        _validator = new AddPlayerCommandValidator(_repository);

        // use the concrete mapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Add_Simgle_Player()
    {
        //arrange 
        var command = new AddPlayerCommand{ Player = _bob, PositionId = "SP", Depth = 1 };

        var sut = new AddPlayerHandler(_repository, _mapper, _validator, _logger);

        // act
        await sut.Handle(command, default);

        // assert
        var position = await _repository.GetPositionAsync("SP", default);

        Assert.True(position.Players.Count() == 1);
        Assert.True(position.Players.ElementAt(0).PlayerId == 1);
    }


    [Fact]
    public async Task Add_Two_Players_With_Same_Depth()
    {
        //arrange 
        var command1 = new AddPlayerCommand { Player = _bob, PositionId = "SP", Depth = 0 };
        var command2 = new AddPlayerCommand { Player = _alice, PositionId = "SP", Depth = 0 };

        var sut = new AddPlayerHandler(_repository, _mapper, _validator, _logger);

        // act
        await sut.Handle(command1, default);
        await sut.Handle(command2, default);

        // assert
        var position = await _repository.GetPositionAsync("SP", default);

        Assert.True(position.Players.Count() == 2);
        Assert.True(position.Players.ElementAt(0).PlayerId == 2);
        Assert.True(position.Players.ElementAt(1).PlayerId == 1);
    }

    [Fact]
    public async Task Add_Two_Players_With_Different_Depths()
    {
        //arrange 
        var command1 = new AddPlayerCommand { Player = _bob, PositionId = "SP", Depth = 0 };
        var command2 = new AddPlayerCommand { Player = _alice, PositionId = "SP", Depth = 1 };

        var sut = new AddPlayerHandler(_repository, _mapper, _validator, _logger);

        // act
        await sut.Handle(command1, default);
        await sut.Handle(command2, default);

        // assert
        var position = await _repository.GetPositionAsync("SP", default);

        Assert.True(position.Players.Count() == 2);
        Assert.True(position.Players.ElementAt(0).PlayerId == 1);
        Assert.True(position.Players.ElementAt(1).PlayerId == 2);
    }

    [Fact]
    public async Task Add_Two_Players_With_No_Depths()
    {
        //arrange 
        var command1 = new AddPlayerCommand { Player = _bob, PositionId = "SP" };
        var command2 = new AddPlayerCommand { Player = _alice, PositionId = "SP" };

        var sut = new AddPlayerHandler(_repository, _mapper, _validator, _logger);

        // act
        await sut.Handle(command1, default);
        await sut.Handle(command2, default);

        // assert
        var position = await _repository.GetPositionAsync("SP", default);

        Assert.True(position.Players.Count() == 2);
        Assert.True(position.Players.ElementAt(0).PlayerId == 1);
        Assert.True(position.Players.ElementAt(1).PlayerId == 2);
    }

    [Fact]
    public async Task Add_Player_With_Too_High_Depth()
    {
        //arrange 
        var command = new AddPlayerCommand { Player = _bob, PositionId = "SP", Depth = 99 };

        var sut = new AddPlayerHandler(_repository, _mapper, _validator, _logger);

        // act
        await sut.Handle(command, default);

        // assert
        var position = await _repository.GetPositionAsync("SP", default);

        Assert.True(position.Players.Count() == 1);
        Assert.True(position.Players.ElementAt(0).PlayerId == 1);
    }

    [Fact]
    public async Task Add_Player_Throws_BadRequestException()
    {
        //arrange 
        var command = new AddPlayerCommand { Player = _bob, PositionId = "QB", Depth = 0 };

        var sut = new AddPlayerHandler(_repository, _mapper, _validator, _logger);

        // act and asset
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => sut.Handle(command, default));
        Assert.Equal("The 'PositionId' is invalid.", ex.Message);
    }
}
