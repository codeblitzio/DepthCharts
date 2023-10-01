using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Options;
using CodingTest.DepthCharts.Repositories;
using CodingTest.DepthCharts.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CodingTest.DepthCharts.Tests.Validators;

public class AddPlayerHandlerCommandValidatorTests
{
    readonly IRepository _repository;
    readonly IOptions<AppOptions> _options;
    readonly IValidator<AddPlayerCommand> _sut;

    public AddPlayerHandlerCommandValidatorTests()
	{
        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions
        {
            Sport = "MLB"
        });

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        _sut = new AddPlayerCommandValidator(_repository);
    }

    [Fact]
    public async Task Command_Is_Valid()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                PlayerId = 1,
                Name = "Bob",
                PositionId = "SP"
            },
            PositionId = "SP",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Depth()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                PlayerId = 1,
                Name = "Bob",
                PositionId = "SP"
            },
            PositionId = "SP"
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_Null_Player()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Player_Id()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                Name = "Bob",
                PositionId = "SP"
            },
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Player_Name()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                PlayerId = 1,
                PositionId = "SP"
            },
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Player_Position()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                PlayerId = 1,
                Name = "Bob",
            },
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_Invalid_Position()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                PlayerId = 1,
                Name = "Bob",
                PositionId = "SP"
            },
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Position()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                PlayerId = 1,
                Name = "Bob",
                PositionId = "SP"
            },
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_Negative_Depth()
    {
        //arrange 
        var command = new AddPlayerCommand
        {
            Player = new AddPlayerCommand.PlayerObj
            {
                PlayerId = 1,
                Name = "Bob",
                PositionId = "SP"
            },
            PositionId = "QB",
            Depth = -1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }
}
