using MediatR;

namespace CodingTest.DepthCharts.Messages;

public class RemovePlayerCommand : IRequest
{
	public int PlayerId { get; set; }
	public string PositionId { get; set; }
}
