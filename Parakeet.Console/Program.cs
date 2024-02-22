using System.Text;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            foreach (var g in AllGrammars.Grammars)
            {
                var sb = new StringBuilder();
                foreach (var r in g.GetRules())
                {
                    sb.AppendLine(r.GetName() + " := " + r.Body().ToDefinition());
                }
                var s = sb.ToString();
                Console.WriteLine(s);
            }
        }
    }
}
