namespace Ara3D.Parakeet;

internal sealed class ParserFailure
{
    private int _position = -1;

    internal int Position => _position;

    internal void Record(int position)
    {
        if (position > _position)
            _position = position;
    }
}

public static class ParserDiagnostics
{
    public static int FurthestFailurePosition(this Rule rule, ParserInput input)
    {
        var failure = new ParserFailure();
        rule.Match(new ParserState(input, 0, null, null, failure));
        return failure.Position;
    }
}
