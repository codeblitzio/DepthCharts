using MediatR;

namespace CodingTest.DepthCharts.Messages;

public class AddPlayerCommand : IRequest
{
    public class PlayerObj
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public string PositionId { get; set; }
    }

    public PlayerObj Player { get; set; }
	public string PositionId { get; set; }
	public int? Depth { get; set; }
}

