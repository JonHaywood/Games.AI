using System.Collections.Generic;
using Games.AI.Search;

namespace Games.AI.UninformedSearch
{
    /// <summary>
    /// Reperesents an entity that can be transformed from one state into another
    /// equivalent state.
    /// </summary>
    public interface ITransformable
    {
        /// <summary>
        /// Retrieves all the equivalent trasformations that can be applied.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IState> GetTransforms();
    }
}