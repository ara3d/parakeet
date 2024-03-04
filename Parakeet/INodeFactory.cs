namespace Ara3D.Parakeet
{
    public interface INodeFactory
    {
        IGrammar Grammar { get; }
        CstNode Create(ParserTreeNode node);
    }
}