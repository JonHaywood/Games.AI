
namespace Games.AI.AdversarialSearch
{
    public class AlgorithmResult
    {
        public AlgorithmResult(bool solved, IAction action, AlgorithmStatistics statistics)
        {
            Solved = solved;
            Action = action;
            Statistics = statistics;
        }

        public bool Solved { get; private set; }
        public IAction Action { get; private set; }
        public AlgorithmStatistics Statistics { get; private set; }
    }
}
