using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Games.AI.Search;

namespace Games.AI.UninformedSearch
{
    /// <summary>
    /// Class which represents a solution the board game. Contains the intial state of
    /// the board and all the assignments made to bring the board to the final state.
    /// Is immutable.
    /// </summary>
    public class Solution
    {
        public Solution(IState initialState, IEnumerable<Successor> successors)
        {
            if (initialState == null)
                throw new ArgumentNullException("initialState", "initialState is a required argument.");
            if (successors == null)
                throw new ArgumentNullException("successors", "successors is a required argument.");

            InitialState = initialState;
            Successors = new ReadOnlyCollection<Successor>(successors.ToList());
            FinalState = Successors.Last().ResultingState;
            Depth = Successors.Count;
        }

        public IState InitialState { get; private set; }
        public ReadOnlyCollection<Successor> Successors { get; private set; }
        public IState FinalState { get; private set; }
        public int Depth { get; private set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(InitialState.ToString());
            foreach (var successor in Successors)
            {
                builder.AppendLine(successor.Action.ToString());
                builder.AppendLine();
                builder.AppendLine(successor.ResultingState.ToString());
            }

            return builder.ToString();
        }
    }
}
