using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests;

/// <summary>
/// EXPRESS constructs the STEP long forms (AP203, AP214, AP242) use beyond the IFC schemas. Each block must be
/// read in full: the grammar recovers from a failure by jumping to the end, so AtEnd alone proves nothing and
/// every test checks that no error was recorded.
/// (Written without raw strings or collection expressions so that CSharpTests.TestFileParser can parse it.)
/// </summary>
public static class ExpressGrammarTests
{
    public static ExpressGrammar Grammar => ExpressGrammar.Instance;

    public static ParserTreeNode ReadInFull(Rule rule, string text)
    {
        var ps = rule.Parse(text);
        Assert.That(ps, Is.Not.Null);
        Assert.That(ps.LastError, Is.Null, () => ps.LastError.ToString());
        Assert.That(ps.AtEnd(), Is.True);
        return ps.Node.ToParseTree();
    }

    public static List<ParserTreeNode> Descendants(this ParserTreeNode node, string name)
    {
        var r = new List<ParserTreeNode>();
        foreach (var c in node.Children)
        {
            if (c.Type == name)
                r.Add(c);
            r.AddRange(c.Descendants(name));
        }
        return r;
    }

    public static List<string> Contents(this IEnumerable<ParserTreeNode> nodes)
        => nodes.Select(n => n.Contents.Trim()).ToList();

    public static List<string> FirstChildContents(this IEnumerable<ParserTreeNode> nodes)
        => nodes.Select(n => n.Children[0]).Contents();

    public static List<string> FirstChildTypes(this IEnumerable<ParserTreeNode> nodes)
        => nodes.Select(n => n.Children[0].Type).ToList();

    [Test]
    public static void AndOrSupertypeKeepsSubtypeAndAttributes()
    {
        var tree = ReadInFull(Grammar.Entity, @"ENTITY b_spline_curve
  SUPERTYPE OF (ONEOF(uniform_curve, b_spline_curve_with_knots, quasi_uniform_curve, bezier_curve) ANDOR
                rational_b_spline_curve)
  SUBTYPE OF (bounded_curve);
  degree              : INTEGER;
  control_points_list : LIST [2 : ?] OF cartesian_point;
  curve_form          : b_spline_curve_form;
  closed_curve        : LOGICAL;
  self_intersect      : LOGICAL;
DERIVE
  upper_index_on_control_points : INTEGER := (SIZEOF(control_points_list) - 1);
WHERE
  WR1: ('SCHEMA.UNIFORM_CURVE' IN TYPEOF(SELF));
END_ENTITY;");
        Assert.That(tree.Descendants("SubtypeHeader").Single().Descendants("Identifier").Contents(), Is.EqualTo(new[] { "bounded_curve" }));
        Assert.That(tree.Descendants("AttributeDecl").FirstChildContents(), Is.EqualTo(new[] { "degree", "control_points_list", "curve_form", "closed_curve", "self_intersect" }));
        Assert.That(tree.Descendants("OneOfSupertype"), Has.Count.EqualTo(1));
    }

    [TestCase("ENTITY a SUPERTYPE OF ((ONEOF(b, c)) AND (ONEOF(d, e))) SUBTYPE OF (f); END_ENTITY;")]
    [TestCase("ENTITY a SUPERTYPE OF (ONEOF((b AND c), (d AND c), b, d)); END_ENTITY;")]
    [TestCase("ENTITY a ABSTRACT SUPERTYPE; x : REAL; END_ENTITY;")]
    [TestCase("ENTITY a ABSTRACT SUPERTYPE SUBTYPE OF (b); END_ENTITY;")]
    [TestCase("ENTITY a SUPERTYPE OF (b ANDOR c ANDOR d); END_ENTITY;")]
    public static void SupertypeExpressions(string text)
        => ReadInFull(Grammar.Entity, text);

    [Test]
    public static void ExplicitRedeclarationDoesNotHideLaterAttributes()
    {
        var tree = ReadInFull(Grammar.Entity, @"ENTITY annotation_symbol
  SUBTYPE OF (mapped_item);
  SELF\mapped_item.mapping_source : symbol_representation_map;
  SELF\mapped_item.mapping_target RENAMED target : symbol_target;
  extra : REAL;
END_ENTITY;");
        var decls = tree.Descendants("AttributeDecl");
        Assert.That(decls.FirstChildTypes(), Is.EqualTo(new[] { "RedeclaredAttribute", "RedeclaredAttribute", "Identifier" }));
        Assert.That(decls[1].Children[0].Children.Contents(), Is.EqualTo(new[] { "mapped_item", "mapping_target", "target" }));
    }

    [Test]
    public static void DeriveSectionNamesRedeclaredAttributes()
    {
        var tree = ReadInFull(Grammar.Entity, @"ENTITY oriented_edge
  SUBTYPE OF (edge);
  edge_element : edge;
  orientation  : BOOLEAN;
DERIVE
  SELF\edge.edge_start : vertex := boolean_choose(SELF.orientation, SELF.edge_element.edge_start,
                                                  SELF.edge_element.edge_end);
  SELF\edge.edge_end   : vertex := boolean_choose(SELF.orientation, 'a;b', SELF.edge_element.edge_start);
  local_value          : ARRAY [0 : upper] OF REAL := f(x);
WHERE
  WR1: NOT ('SCHEMA.ORIENTED_EDGE' IN TYPEOF(SELF.edge_element));
END_ENTITY;");
        var derived = tree.Descendants("DerivedAttribute");
        Assert.That(derived.FirstChildTypes(), Is.EqualTo(new[] { "RedeclaredAttribute", "RedeclaredAttribute", "Identifier" }));
        Assert.That(derived[0].Children[0].Children.Contents(), Is.EqualTo(new[] { "edge", "edge_start" }));
        Assert.That(tree.Descendants("AttributeDecl").FirstChildContents(), Is.EqualTo(new[] { "edge_element", "orientation" }));
    }

    [TestCase("ENTITY a; segment_radii : LIST [1 : segments] OF positive_length_measure; END_ENTITY;")]
    [TestCase("ENTITY a; pixel : LIST [1:?] OF BINARY(32); END_ENTITY;")]
    [TestCase("ENTITY a; code : STRING(8) FIXED; END_ENTITY;")]
    [TestCase("ENTITY a; v : ARRAY [1 : 3] OF OPTIONAL UNIQUE REAL; END_ENTITY;")]
    public static void AttributeTypes(string text)
        => Assert.That(ReadInFull(Grammar.Entity, text).Descendants("AttributeDecl"), Has.Count.EqualTo(1));

    [TestCase("TYPE dimension_count = INTEGER;\nWHERE\n  WR1: SELF > 0;\nEND_TYPE;")]
    [TestCase("TYPE ypr_rotation = ARRAY [ypr_index(yaw) : ypr_index(roll)] OF plane_angle_measure;\nEND_TYPE;")]
    [TestCase("TYPE s = SELECT\n  (a,\n   b);\nEND_TYPE;")]
    public static void TypeDeclarations(string text)
        => ReadInFull(Grammar.TypeDecl, text);

    [Test]
    public static void BlockScanStartsOnlyAtWholeKeywords()
    {
        var text = "-- Santa Barbara d'Oeste\n"
            + "(* a comment with ENTITY and TYPE in it *)\n"
            + "RULE r FOR (a); WHERE WR1: SIZEOF(TYPEOF(x) * ['S.TYPE']) = 0; END_RULE;\n"
            + "TYPE t = REAL;\nEND_TYPE;\n"
            + "ENTITY e SUBTYPE OF (f); END_ENTITY;\n";
        var input = new ParserInput(text);
        var types = Grammar.TypeBlocks.Parse(input).AllEndNodes().Select(n => n.Contents).ToList();
        var entities = Grammar.EntityBlocks.Parse(input).AllEndNodes().Select(n => n.Contents).ToList();
        Assert.That(types, Is.EqualTo(new[] { "TYPE t = REAL;\nEND_TYPE;" }));
        Assert.That(entities, Is.EqualTo(new[] { "ENTITY e SUBTYPE OF (f); END_ENTITY;" }));
    }

    [TestCase("IFC2X3")]
    [TestCase("IFC4")]
    [TestCase("IFC4X3")]
    public static void EveryIfcBlockIsReadInFull(string schema)
    {
        var input = ParserInput.FromFile(Folders.InputFolder("exp").RelativeFile(schema + ".exp"));
        foreach (var block in Grammar.EntityBlocks.Parse(input).AllEndNodes())
            ReadInFull(Grammar.Entity, block.Contents);
        foreach (var block in Grammar.TypeBlocks.Parse(input).AllEndNodes())
            ReadInFull(Grammar.TypeDecl, block.Contents);
    }
}
