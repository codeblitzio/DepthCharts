using MediatR;

namespace CodingTest.DepthCharts.Messages;

public class GetTrailingPlayersQuery : IRequest<GetTrailingPlayersResult>
{
    public int PlayerId { get; set; }
    public string PositionId { get; set; }
}
