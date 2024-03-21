using Ara3D.Utils;

namespace Ara3D.Parakeet
{
    /// <summary>
    /// An abstract notion of location.
    /// This could be ParserState, ParserRange, ParseNode, CstNode, or AstNode. 
    /// </summary>
    public interface ILocation
    {
        ParserRange GetRange();
    }

    public class Location : ILocation
    {
        public ParserRange GetRange() => null;
    }

    public static class LocationExtensions
    {
        public static FileAndRange ToFileAndRange(this ILocation location)
            => location.GetRange().ToFileAndRange();

        public static FileAndRange ToFileAndRange(this ParserRange range)
            => new FileAndRange(range.FilePath, range.BeginPosition, range.EndPosition);
    }
}