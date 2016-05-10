using System.Text;

namespace Games.AI.UninformedSearch
{
    public class AlgorithmStatistics
    {
        public double ElapsedTimeInSeconds { get; set; }
        public int VisitedNodeCount { get; set; }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.AppendLine(string.Format("Ellapsed time in seconds: {0}", ElapsedTimeInSeconds));
            output.AppendLine(string.Format("Visited Note Count: {0}", VisitedNodeCount));
            return output.ToString();
        }
    }
}
