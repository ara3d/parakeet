using System.Collections.Generic;

namespace Ara3D.Parakeet
{
    public interface IGrammar
    {
        Rule StartRule { get; }
        IEnumerable<Rule> GetRules();
    }
}