
using Games.AI.Search;

namespace Games.AI.AdversarialSearch
{
    /// <summary>
    /// Represents a successor to a given state. Includes the resulting state
    /// and the action taken to get to that state.
    /// </summary>
    public class Successor
    {
        public Successor(IAction action, IState resultingState)
        {
            Action = action;
            ResultingState = resultingState;
        }

        public IAction Action { get; private set; }
        public IState ResultingState { get; private set; }
    }
}
