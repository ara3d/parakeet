
namespace Ara3D.Parakeet.Tests
{
    public static class Helpers
    {
        public static void OutputTree(this ParserTreeNode treeNode, string indent = "+-+")
        {
            Console.WriteLine($"{indent} {treeNode.Node.Descriptor()}");
            foreach (var child in treeNode.Children)
            {
                OutputTree(child, "| " + indent);
            }
        }

        
        public static void OutputNodes(this ParserNode last)
        {
        }

        public static void OutputState(this ParserState state)
        {
            if (state == null)
                Console.WriteLine("Failed!");
            else
            {
                Console.WriteLine($"State position {state.Position} of {state.Input.Length}");
                OutputNodes(state.Node);
            }
        }

        public static string Descriptor(this ParserNode node)
            => $"Node {node.Name}: {node.Start} to {node.End} = {node.EllidedContents}";

    }
}