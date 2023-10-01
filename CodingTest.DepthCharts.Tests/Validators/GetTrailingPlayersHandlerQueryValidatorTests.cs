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

public class GetTrailingPlayersHandlerQueryValidatorTests
{
    readonly IRepository _repository;
    readonly IOptions<AppOptions> _options;
    readonly IValidator<GetTrailingPlayersQuery> _sut;

    public GetTrailingPlayersHandlerQueryValidatorTests()
	{
        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions
        {
            Sport = "MLB"
        });

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        _sut = new GetTrailingPlayersQueryValidator(_repository);
    }

    [Fact]
    public async Task Command_Is_Valid()
    {
        //arrange 
        var query = new GetTrailingPlayersQuery { PlayerId = 1, PositionId = "SP" };

        // act
        var result = await _sut.TestValidateAsync(query);

        // assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Player_Id()
    {
        //arrange 
        var query = new GetTrailingPlayersQuery { PositionId = "SP" };

        // act
        var result = await _sut.TestValidateAsync(query);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_No_Position()
    {
        //arrange 
        var query = new GetTrailingPlayersQuery { PlayerId = 1 };

        // act
        var result = await _sut.TestValidateAsync(query);

        // assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Command_Has_Invalid_Position()
    {
        //arrange 
        var query = new GetTrailingPlayersQuery { PlayerId = 1, PositionId = "QB" };

        // act
        var result = await _sut.TestValidateAsync(query);

        // assert
        Assert.False(result.IsValid);
    }
}
