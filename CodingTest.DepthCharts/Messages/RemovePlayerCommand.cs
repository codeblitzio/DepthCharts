using MediatR;

namespace CodingTest.DepthCharts.Messages;

public class RemovePlayerCommand : IRequest<Unit>
{
	public int PlayerId { get; set; }
	public string PositionId { get; set; }
}
