using System.Linq;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Options;
using CodingTest.DepthCharts.Repositories;
using CodingTest.DepthCharts.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Sdk;

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
            Sport = "NFL"
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
                PositionId = "QB"
            },
            PositionId = "QB",
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
                PositionId = "QB"
            },
            PositionId = "QB"
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
        Assert.Contains(result.Errors, e => e.ErrorMessage == "'Player' must not be empty.");
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
                PositionId = "QB"
            },
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "'Player Player Id' must be greater than '0'.");
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
                PositionId = "QB"
            },
            PositionId = "QB",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "'Player Name' must not be empty.");
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
        Assert.Contains(result.Errors, e => e.ErrorMessage == "'Player Position Id' must not be empty.");
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
                PositionId = "QB"
            },
            PositionId = "SP",
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "The 'PositionId' is invalid.");
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
                PositionId = "QB"
            },
            Depth = 1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "'Position Id' must not be empty.");
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
                PositionId = "QB"
            },
            PositionId = "QB",
            Depth = -1
        };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "'Depth' must be greater than '-1'.");
    }
}
