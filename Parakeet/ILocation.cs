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
}