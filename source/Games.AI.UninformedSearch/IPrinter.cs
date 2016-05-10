namespace Games.AI.UninformedSearch
{
    /// <summary>
    /// Represents a class which can print <typeparam name="T"></typeparam> to
    /// the console as a string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPrinter<in T>
    {
        string Print(T obj);
    }
}
