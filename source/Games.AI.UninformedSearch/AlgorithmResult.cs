using System.Collections.Generic;
using System.Linq;
using Games.AI.UninformedSearch;

namespace Games.AI.UninformedSearch
{
    public class AlgorithmResult
    {
        public AlgorithmResult(IEnumerable<Solution> solutions, AlgorithmStatistics statistics)
        {
            solutions = solutions.ToList();

            Solutions = solutions;
            Statistics = statistics;
            Solved = solutions.Any();
        }

        public bool Solved { get; private set; }
        public IEnumerable<Solution> Solutions { get; private set; } 
        public AlgorithmStatistics Statistics { get; private set; }        
    }
}
