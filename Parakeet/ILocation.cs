namespace Parakeet
{
    /// <summary>
    /// An abstract notion of location. This could be ParserState, ParserRange, or a ParseNode,
    /// but it can also be the result of some generated code, or refactoring, and express a relationship to
    /// something else. 
    /// </summary>
    public interface ILocation
    { }

    public class Location : ILocation
    {
    }

    public static class LocationExtensions
    {
    }
}