using CodingTest.DepthCharts.Entities;
using CodingTest.DepthCharts.Exceptions;
using CodingTest.DepthCharts.Options;
using Microsoft.Extensions.Options;

namespace CodingTest.DepthCharts.Repositories;

public class MockRepository : IRepository
{
    readonly IOptions<AppOptions> _options;
    readonly ILogger<MockRepository> _logger;

    public MockRepository(IOptions<AppOptions> options, ILogger<MockRepository> logger)
    {
        _options = options;
        _logger = logger;
    }

    // New sports can be added by seeding a Positions table as per below.
    // To achieve scalablity positions are associated with a sport.
    // The preferred sport is configured in appsettings.json i.e "MLB" or "NFL".
    // No further code changes are required.

    private readonly List<Position> _positions = new()
    {
        new Position { Sport = "NFL", PositionId = "QB", Players = new List<Player>() },
        new Position { Sport = "NFL", PositionId = "WR", Players = new List<Player>() },
        new Position { Sport = "NFL", PositionId = "RB", Players = new List<Player>() },
        new Position { Sport = "NFL", PositionId = "TE", Players = new List<Player>() },
        new Position { Sport = "NFL", PositionId = "K",  Players = new List<Player>() },
        new Position { Sport = "NFL", PositionId = "P",  Players = new List<Player>() },
        new Position { Sport = "NFL", PositionId = "KR", Players = new List<Player>() },
        new Position { Sport = "NFL", PositionId = "PR", Players = new List<Player>() },

        new Position { Sport = "MLB", PositionId = "SP", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "RP", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "C",  Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "1B", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "2B", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "3B", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "SS", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "LF", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "RF", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "CF", Players = new List<Player>() },
        new Position { Sport = "MLB", PositionId = "DH", Players = new List<Player>() }
    };

    public async Task<IEnumerable<string>> GetPositionIdsAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await Task.FromResult(_positions.Where(p => p.Sport == _options.Value.Sport).Select(p => p.PositionId));
        }
        catch (Exception ex)
        {
            // we don't really need exception handling in the mock repository
            // but here's an example of how to use it in a real one

            // log the exception with serilog
            _logger.LogError(ex, "Error retrieving PostiionIds from repository");

            // throw an appropriate exception to be handled by the hellang middleware
            // a problem details response summarising the error will be returned
            throw new RepositoryException("Error retrieving PostiionIds from repository");
        }
    }

    public async Task<IEnumerable<Position>> GetPositionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await Task.FromResult(_positions.Where(p => p.Sport == _options.Value.Sport));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Positions from repository");

            throw new RepositoryException("Error retrieving Postitions from repository");
        }
    }

    public async Task<Position> GetPositionAsync(string positionId, CancellationToken cancellationToken)
    {
        try
        {
            return await Task.FromResult(_positions.Where(p => p.Sport == _options.Value.Sport).Single(p => p.PositionId == positionId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Position {positionId} from repository", positionId);

            throw new RepositoryException("Error retrieving Postition from repository");
        }
    }

    public async Task SavePositionAsync(Position position, CancellationToken cancellationToken)
    {
        try
        {
            // for the mock there nothing to do here, as we're storing in-memory
            await Task.CompletedTask;

            // for a real repo the player list may have been significantly shuffled
            // it'd be more performant to update the list atomically rather than issuing many individual updates
        }
        catch (Exception ex)
        {
            // note structured logging
            _logger.LogError(ex, "Error saving Position {@position} to repository", position);

            throw new RepositoryException("Error saving Position to repository");
        }
    }
}

