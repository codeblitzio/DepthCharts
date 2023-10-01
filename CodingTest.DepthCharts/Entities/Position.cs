namespace CodingTest.DepthCharts.Entities
{
	public class Position
	{
        public string PositionId { get; set; }
        public string Sport { get; set; }
        public IEnumerable<Player> Players { get; set; }
    }
}
