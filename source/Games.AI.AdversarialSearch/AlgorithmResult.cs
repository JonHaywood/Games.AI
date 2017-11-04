
using Games.AI.Search;

namespace Games.AI.AdversarialSearch
{
    public class AlgorithmResult
    {
        public AlgorithmResult(bool solved, IAction action, AlgorithmStatistics statistics, string debugOutput = null)
        {
            Solved = solved;
            Action = action;
            Statistics = statistics;
            DebugOutput = debugOutput;
        }

        public bool Solved { get; private set; }
        public IAction Action { get; private set; }
        public AlgorithmStatistics Statistics { get; private set; }
        public string DebugOutput { get; private set; }
    }
}
