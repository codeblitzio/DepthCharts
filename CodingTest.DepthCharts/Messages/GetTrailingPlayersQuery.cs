using MediatR;

namespace CodingTest.DepthCharts.Messages;

public class GetTrailingPlayersQuery : IRequest<GetTrailingPlayersResponse>
{
    public int PlayerId { get; set; }
    public string PositionId { get; set; }
}
