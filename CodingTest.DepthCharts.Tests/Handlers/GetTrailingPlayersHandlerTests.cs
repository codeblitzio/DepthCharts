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

public class GetTrailingPlayersHandlerTests
{
	readonly ILogger<GetTrailingPlayersHandler> _logger;
	readonly IRepository _repository;
    readonly IValidator<GetTrailingPlayersQuery> _validator;
    readonly IOptions<AppOptions> _options;
    readonly IMapper _mapper;


    public GetTrailingPlayersHandlerTests()
	{
        // moq the logger
		_logger = new Mock<ILogger<GetTrailingPlayersHandler>>().Object;

        // create a concrete options
        _options = Microsoft.Extensions.Options.Options.Create(new AppOptions { Sport = "NFL" });

        // the main repo is a concrete mock, let's reuse
        _repository = new Repositories.MockRepository(_options, new Mock<ILogger<Repositories.MockRepository>>().Object);

        // use the concret validator
        _validator = new GetTrailingPlayersQueryValidator(_repository);

        // use the concrete mapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task Get_Trailing_Players()
    {
        //arrange
        var position = await _repository.GetPositionAsync("QB", default);
        var players = position.Players.ToList();

        players.Add(new Entities.Player { PlayerId = 1, Name = "Bob", PositionId = "QB" });
        players.Add(new Entities.Player { PlayerId = 2, Name = "Alice", PositionId = "QB" });
        players.Add(new Entities.Player { PlayerId = 3, Name = "Charlie", PositionId = "QB" });

        position.Players = players;

        var query = new GetTrailingPlayersQuery { PlayerId = 1, PositionId = "QB" };

        var sut = new GetTrailingPlayersHandler(_repository, _mapper, _validator, _logger);

        // act
        var result = await sut.Handle(query, default);

        // assert
        Assert.Equal(2, result.Players.Count());
        Assert.Equal(2, result.Players.ElementAt(0));
        Assert.Equal(3, result.Players.ElementAt(1));
    }

    [Fact]
    public async Task Get_Trailing_Players_Throws_BadRequestException()
    {
        //arrange
        var query = new GetTrailingPlayersQuery { PlayerId = 1, PositionId = "SP" };

        var sut = new GetTrailingPlayersHandler(_repository, _mapper, _validator, _logger);

        // act and asset
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => sut.Handle(query, default));
        Assert.Equal("The 'PositionId' is invalid.", ex.Message);
    }
}
