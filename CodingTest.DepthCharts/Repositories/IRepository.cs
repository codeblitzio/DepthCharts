using CodingTest.DepthCharts.Entities;

namespace CodingTest.DepthCharts.Repositories;

public interface IRepository
{
    Task<IEnumerable<string>> GetPositionIdsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Position>> GetPositionsAsync(CancellationToken cancellationToken);
    Task<Position> GetPositionAsync(string positionId, CancellationToken cancellationToken);
    Task SavePositionAsync(Position position, CancellationToken cancellationToken);
}
