using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Games.AI.Search;

namespace Games.AI.AdversarialSearch
{
    public class MinimaxWithAlphaBetaPruningAlgorithm : IAlgorithm
    {        
        private readonly Dictionary<IState, double> transpositionTable = new Dictionary<IState, double>();

        private IProblem Problem { get; set; }
        private AlgorithmStatistics Statistics { get; set; }
        private StringBuilder DebugOutput { get; set; }

        public bool UseTranspositionTable { get; set; }
        public bool Debug { get; set; }

        public AlgorithmResult SolveForBestAction(IProblem problem, IState state)
        {
            Problem = problem;
            Statistics = new AlgorithmStatistics();
            DebugOutput = new StringBuilder();
            transpositionTable.Clear();
            var stopwach = Stopwatch.StartNew();

            var v = double.NegativeInfinity;
            var alpha = double.NegativeInfinity;
            var beta = double.PositiveInfinity;
            IAction action = null;

            // perform the MAX-VALUE portion of minimax except we'll
            // record the action associated with the successor so we 
            // can return.
            var successors = Problem.GetSuccessors(state);
            PrintSuccessors(state, successors);
            foreach (var successor in successors)
            {
                RecordStatistics(successor.ResultingState);

                var vTemp = Math.Max(v, MinValue(successor.ResultingState, alpha, beta));
                if (vTemp > v)
                {
                    v = vTemp;
                    action = successor.Action;
                }
                if (v >= beta)
                    break;
                alpha = Math.Max(alpha, v);
            }

            stopwach.Stop();
            Statistics.ElapsedTimeInSeconds = new TimeSpan(stopwach.ElapsedTicks).TotalSeconds;

            return action == null 
                ? new AlgorithmResult(false, null, Statistics, DebugOutput.ToString()) 
                : new AlgorithmResult(true, action, Statistics, DebugOutput.ToString());
        }

        /// <summary>
        /// Max portion of the minimax algorithm.
        /// </summary>
        /// <param name="state">The current state in gam.</param>
        /// <param name="alpha">The value of the best alternative for MAX along the path to state.</param>
        /// <param name="beta">The value of the best alternative for MIN along the path to state.</param>
        /// <returns>A utility value.</returns>
        private double MaxValue(IState state, double alpha, double beta)
        {
            if (Problem.IsTerminalState(state))
                return Problem.GetUtilityValue(state);

            if (UseTranspositionTable && transpositionTable.ContainsKey(state))
                return transpositionTable[state];

            var v = double.NegativeInfinity;
            var successors = Problem.GetSuccessors(state);
            PrintSuccessors(state, successors);
            foreach (var successor in successors)
            {
                RecordStatistics(successor.ResultingState);

                if (UseTranspositionTable && transpositionTable.ContainsKey(successor.ResultingState))
                {
                    v = transpositionTable[successor.ResultingState];
                }
                else if (successor.ResultingState.Level > Problem.MaxLevel)
                {
                    // don't explore, leave v as is and check transposition table
                    if (UseTranspositionTable && !transpositionTable.ContainsKey(state))
                        transpositionTable[state] = v;
                }
                else
                {
                    v = Math.Max(v, MinValue(successor.ResultingState, alpha, beta));
                    if (UseTranspositionTable && !transpositionTable.ContainsKey(state))
                        transpositionTable[state] = v;
                }

                if (v >= beta)
                    return v;
                alpha = Math.Max(alpha, v);
            }            

            return v;
        }

        /// <summary>
        /// Min portion of the minimax algorithm.
        /// </summary>
        /// <param name="state">The current state in gam.</param>
        /// <param name="alpha">The value of the best alternative for MAX along the path to state.</param>
        /// <param name="beta">The value of the best alternative for MIN along the path to state.</param>
        /// <returns>A utility value.</returns>
        private double MinValue(IState state, double alpha, double beta)
        {
            if (Problem.IsTerminalState(state))
                return Problem.GetUtilityValue(state);

            if (UseTranspositionTable && transpositionTable.ContainsKey(state))
                return transpositionTable[state];

            var v = double.PositiveInfinity;
            var successors = Problem.GetSuccessors(state);
            PrintSuccessors(state, successors);
            foreach (var successor in successors)
            {                
                RecordStatistics(successor.ResultingState);

                if (UseTranspositionTable && transpositionTable.ContainsKey(successor.ResultingState))
                {
                    v = transpositionTable[successor.ResultingState];
                }
                else if (successor.ResultingState.Level > Problem.MaxLevel)
                {
                    // don't explore, leave v as is and set transposition table
                    if (UseTranspositionTable && !transpositionTable.ContainsKey(state))
                        transpositionTable[state] = v;
                }
                else
                {
                    v = Math.Min(v, MaxValue(successor.ResultingState, alpha, beta));
                    if (UseTranspositionTable && !transpositionTable.ContainsKey(state))
                        transpositionTable[state] = v;
                }

                if (v <= alpha)
                    return v;
                beta = Math.Min(beta, v);
            }            

            return v;
        }

        private void PrintSuccessors(IState state, List<Successor> successors)
        {
            if (!Debug)
                return;

            DebugOutput.AppendLine("--------------------------");
            DebugOutput.AppendLine(string.Format("For state:\n{0}", state));
            DebugOutput.AppendLine(string.Format("Found {0} successors.\n", successors.Count));
            for (int i = 0; i < successors.Count; i++)
            {
                var successor = successors[i];

                DebugOutput.AppendLine(string.Format("Successor {0}", i));
                DebugOutput.AppendLine(string.Format("Action: {0}", successor.Action));
                DebugOutput.AppendLine(string.Format("Resulting State:\n{0}\n", successor.ResultingState));
            }
            DebugOutput.AppendLine("--------------------------");
        }

        private void RecordStatistics(IState state)
        {
            Statistics.VisitedStateCount++;

            if (state.Level > Statistics.MaxLevel)
                Statistics.MaxLevel = state.Level;
        }
    }
}
