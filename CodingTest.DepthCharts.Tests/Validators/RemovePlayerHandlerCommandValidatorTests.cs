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

public class RemovePlayerHandlerCommandValidatorTests
{
    readonly IRepository _repository;
    readonly IOptions<AppOptions> _options;
    readonly IValidator<RemovePlayerCommand> _sut;

    public RemovePlayerHandlerCommandValidatorTests()
	{
        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions
        {
            Sport = "MLB"
        });

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        _sut = new RemovePlayerCommandValidator(_repository);
    }

    [Fact]
    public async Task Command_Is_Valid()
    {
        //arrange 
        var command = new RemovePlayerCommand { PlayerId = 1, PositionId = "SP" };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Player_Id()
    {
        //arrange 
        var command = new RemovePlayerCommand { PositionId = "SP" };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Position()
    {
        //arrange 
        var command = new RemovePlayerCommand { PlayerId = 1 };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_Invalid_Position()
    {
        //arrange 
        var command = new RemovePlayerCommand { PlayerId = 1, PositionId = "QB" };

        // act
        var result = await _sut.TestValidateAsync(command);

        // assert
        Assert.False(result.IsValid);
    }
}
