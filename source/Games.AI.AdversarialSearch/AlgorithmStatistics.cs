using System.Text;

namespace Games.AI.AdversarialSearch
{
    public class AlgorithmStatistics
    {
        public double ElapsedTimeInSeconds { get; set; }
        public int VisitedStateCount { get; set; }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.AppendLine(string.Format("Ellapsed time in seconds: {0}", ElapsedTimeInSeconds));
            output.AppendLine(string.Format("Visited State Count: {0}", VisitedStateCount));
            return output.ToString();
        }
    }
}
