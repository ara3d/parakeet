using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests;

public static class ExpressTests
{
    public static ParserInput GetIfc4ExpressFileAsParserInput()
        => ParserInput.FromFile(InputFolder.RelativeFile("IFC4.exp"));

    public static ParserInput GetIfc2x3ExpressFileAsParserInput()
        => ParserInput.FromFile(InputFolder.RelativeFile("IFC2x3.exp"));

    public static ParserInput GetIfc4x3ExpressFileAsParserInput()
        => ParserInput.FromFile(InputFolder.RelativeFile("IFC4x3.exp"));

    public static bool BasicTest(string input, Rule rule)
    {
        try
        {
            var ps = rule.Parse(input);
            if (ps == null)
            {
                Console.WriteLine("Parsing failed with null result");
                return false;
            }

            if (!ps.AtEnd())
            {
                Console.WriteLine($"Partial passed: {ps.Position}/{ps.Input.Length}");
                return false;
            }
        }
        catch (ParserException pe)
        {
            Console.WriteLine($"Parsing exception {pe.Message} occured at {pe.LastValidState} ");
            return false;
        }

        return true;
    }

    [Test]
    public static void TestEntities()
    {
        var entityRule = ExpressGrammar.Instance.Entity;
        var cnt = 0;
        var pass = 0;
        foreach (var t in GetExpressEntities())
        {
            var result = BasicTest(t, entityRule);
            if (!result)
            {
                Console.WriteLine($"Failed entity test #{cnt}:");
                Console.WriteLine($"Input was: {t}");
            }
            else
            {
                pass++;
            }
            cnt++;
        }
        Console.WriteLine($"Entity test results = {pass} of {cnt}");
    }

    public static ParserTreeNode GetEntityBody(ParserTreeNode node)
    {
        Assert.AreEqual("Entity", node.Node.Name);
        return node.Children.FirstOrDefault(c => c.Node.Name == "EntityBody");
    }

    public static string GetEntityName(ParserTreeNode node)
    {
        Assert.AreEqual("Entity", node.Node.Name);
        return node.Children[0].Children[0].Contents;
    }

    public static string GetTypeString(ParserTreeNode node)
    {
        Assert.AreEqual("TypeExpr", node.Node.Name);
        var partTwo = node.Children[0];
        if (partTwo.Node.Name == "AggregationType")
        {
            Assert.AreEqual(1, partTwo.Children.Count);
            var innerType = GetTypeString(partTwo.Children[0]);
            return $"List<{innerType}>";
        }
        else if (partTwo.Node.Name == "Identifier")
        {
            return partTwo.Node.Contents;
        }

        throw new Exception($"Unexpected type: {node}");
    }

    public static void OutputAttribute(ParserTreeNode node)
    {
        Assert.AreEqual("AttributeDecl", node.Node.Name);
        var name = node.Children[0].Contents;
        var type = GetTypeString(node.Children[1]);
        Console.WriteLine($"  {type} {name};");
    }

    public static string GetEntitySubType(ParserTreeNode node)
    {
        Assert.AreEqual("Entity", node.Node.Name);
        var header = node.Children[0];
        var subTypeHeader = header.Children.FirstOrDefault(c => c.Node.Name == "SubtypeHeader");
        if (subTypeHeader == null) return null;
        var identList = subTypeHeader.Children[0];
        return identList.Children[0].Contents;
    }

    public static void OutputAttributes(ParserTreeNode node)
    {
        var body = GetEntityBody(node);
        var attrs = body.Children;
        foreach (var attr in attrs)
            OutputAttribute(attr);
    }

    [Test]
    public static void TestEntityAttributes()
    {
        var entityRule = ExpressGrammar.Instance.Entity;
        var cnt = 0;
        var pass = 0;
        foreach (var t in GetExpressEntities())
        {
            var ps = entityRule.Parse(t);
            if (ps == null) {
                Console.WriteLine($"Test {cnt} failed");
            }
            else
            {
                var tree = ps.Node.ToParseTree();
                //Console.WriteLine(tree.ToString());
                var name = GetEntityName(tree);
                var subType = GetEntitySubType(tree);
                Console.WriteLine($"class {name}");
                if (!subType.IsNullOrWhiteSpace()) Console.WriteLine($" : {subType}");
                Console.WriteLine("{");
                OutputAttributes(tree);
                Console.WriteLine("}");
            }

            cnt++;
        }
        Console.WriteLine($"Entity test results = {pass} of {cnt}");
    }

    [Test]
    public static void TestTypes()
    {
        var typeRule = ExpressGrammar.Instance.TypeDecl;
        var cnt = 0;
        var pass = 0;
        foreach (var t in GetExpressTypes())
        {
            var result = BasicTest(t, typeRule);
            if (!result)
            {
                Console.WriteLine($"Failed type test #{cnt}:");
                Console.WriteLine($"Input was: {t}");
            }
            else
            {
                pass++;
            }
            cnt++;
        }
        Console.WriteLine($"Types test results = {pass} of {cnt}");
    }

    public static void GenerateCodeForSelect(ParserTreeNode node, string name)
    {
        Assert.AreEqual("SelectType", node.Node.Name);
        var options = node.Children[0].Children.Select(c => c.Contents);
        Console.WriteLine($"interface {name} {{ }}");
        Console.WriteLine("// " + options.JoinStringsWithComma());
    }

    public static void GenerateCodeForEnum(ParserTreeNode node, string name)
    {
        Assert.AreEqual("EnumerationType", node.Node.Name);
        var options = node.Children[0].Children.Select(c => c.Contents);
        Console.WriteLine($"enum {name} {{");
        foreach (var option in options)
            Console.WriteLine($"  {option},");
        Console.WriteLine("}");
    }

    [Test]
    public static void TestTypeCodeGeneration()
    {
        var typeRule = ExpressGrammar.Instance.TypeDecl;
        var cnt = 0;

        foreach (var t in GetExpressTypes())
        {
            var ps = typeRule.Parse(t);
            if (ps == null)
            {
                Console.WriteLine($"Test {cnt} failed");
            }
            else
            {

                var tree = ps.Node.ToParseTree();
                var name = tree.Children[0].Node.Contents;
                var typeDef = tree.Children[1].Children[0];

                //Console.WriteLine($"Node {name} is {typeDef.Node.Name}");
                
                if (typeDef.Node.Name == "EnumerationType")
                    GenerateCodeForEnum(typeDef, name);
                else if (typeDef.Node.Name == "SelectType")
                    GenerateCodeForSelect(typeDef, name);
                else if (typeDef.Node.Name == "TypeExpr")
                {
                    var typeExpr = GetTypeString(typeDef);
                    Console.WriteLine($"type {name} => {typeExpr}");
                }
                else
                    throw new Exception($"Unrecognized type {typeDef}");
            }

            cnt++;
        }
    }

    public static IEnumerable<string> GetExpressTypes() {
        var pi = GetIfc4ExpressFileAsParserInput();
        var rule = ExpressGrammar.Instance.TypeBlocks;
        var ps = rule.Parse(pi);
        Assert.IsTrue(ps.AtEnd());
        return ps.AllEndNodes().Select(n => n.Contents);
    }

    public static IEnumerable<string> GetExpressEntities()
    {
        var pi = GetIfc4ExpressFileAsParserInput();
        var rule = ExpressGrammar.Instance.EntityBlocks;
        var ps = rule.Parse(pi);
        Assert.IsTrue(ps.AtEnd());
        return ps.AllEndNodes().Select(n => n.Contents);
    }


    public static ExpressGrammar Grammar = ExpressGrammar.Instance;
    public static DirectoryPath InputFolder =
        PathUtil.GetCallerSourceFolder().RelativeFolder("..", "input", "exp");

    // ----------------------------------------
    // Core: WS + comment + skipping
    // ----------------------------------------
    public static string[] Comments =
    {
        "(* hello *)",
        "(* multi\nline\ncomment *)",
        "(**)",
        "(***)",
        "(* * *)",
    };

    public static string[] WsCases =
    {
        "",
        " ",
        "\n",
        "\r\n",
        "\t  \r\n",
        "(* c *)",
        " (* c *) \n (* d *) \r\n",
    };

    public static string[] SkippedBlocks =
    {
        @"FUNCTION IfcCorrectUnitAssignment
(Units : SET [1:?] OF IfcUnit)
   : LOGICAL;
  LOCAL
    NamedUnitNumber : INTEGER := 0;
  END_LOCAL;
  RETURN(TRUE);
END_FUNCTION;",
        @"RULE Foo;
  WHERE
    WR1 : TRUE;
END_RULE;"
    };

    [Test]
    [TestCaseSource(nameof(Comments))]
    public static void TestCommentParses(string input)
        => ParserTests.SingleParseTest(input, Grammar.Comment);

    [Test]
    [TestCaseSource(nameof(WsCases))]
    public static void TestWsParses(string input)
        => ParserTests.SingleParseTest(input, Grammar.WS);

    [Test]
    [TestCaseSource(nameof(SkippedBlocks))]
    public static void TestSkippedSectionParses(string input)
        => ParserTests.SingleParseTest(input, Grammar.SkippedSection);

    // ----------------------------------------
    // Header tests
    // ----------------------------------------

    public static string[] EntityHeaders =
    {
        "ENTITY IfcDerivedUnit;",
        @"ENTITY IfcRectangleHollowProfileDef
      SUBTYPE OF (IfcRectangleProfileDef);",

        @"ENTITY IfcRectangleProfileDef
      SUPERTYPE OF(ONEOF
        (IfcRectangleHollowProfileDef
        , IfcRoundedRectangleProfileDef))
      SUBTYPE OF(IfcParameterizedProfileDef);",

        @"ENTITY IfcRectangularPyramid
      SUBTYPE OF(IfcCsgPrimitive3D);",
    };

    [Test]
    [TestCaseSource(nameof(EntityHeaders))]
    public static void TestEntityHeader(string input)
        => ParserTests.SingleParseTest(input, Grammar.EntityHeader);

    // ----------------------------------------
    // TypeExpr / Attribute parsing
    // ----------------------------------------
    public static string[] TypeExprs =
    {
        "INTEGER",
        "IfcLabel",
        "OPTIONAL IfcLabel",
        "SET OF IfcUnit",
        "SET [1:?] OF IfcDerivedUnitElement",
        "LIST [0:?] OF IfcLabel",
        "ARRAY [1:3] OF IfcCartesianPoint",
        "SET [1:?] OF UNIQUE IfcUnit",
    };

    [Test]
    [TestCaseSource(nameof(TypeExprs))]
    public static void TestTypeExpr(string input)
        => ParserTests.SingleParseTest(input, Grammar.TypeExpr);

    public static string[] AttributeDecls =
    {
        "Elements : SET [1:?] OF IfcDerivedUnitElement;",
        "UnitType : IfcDerivedUnitEnum;",
        "UserDefinedType : OPTIONAL IfcLabel;",
        "Exponent : INTEGER;",
        "PredefinedType : IfcDistributionChamberElementTypeEnum;",
    };

    [Test]
    [TestCaseSource(nameof(AttributeDecls))]
    public static void TestAttributeDecl(string input)
        => ParserTests.SingleParseTest(input, Grammar.AttributeDecl);
    
    public static string[] Attributes =
    {
        "WallThickness : IfcPositiveLengthMeasure;",
        "InnerFilletRadius : OPTIONAL IfcNonNegativeLengthMeasure;",
        "Elements : SET [1:?] OF IfcDerivedUnitElement;",
        "Usense : IfcBoolean;",
        "BasisSurface : IfcSurface;",
    };

    [Test]
    [TestCaseSource(nameof(Attributes))]
    public static void TestAttribute(string input)
        => ParserTests.SingleParseTest(input, Grammar.AttributeDecl);

    public static string[] WhereSections =
    {
        @"WHERE
       ValidWallThickness : (WallThickness<(SELF\IfcRectangleProfileDef.XDim/2.)) AND
                            (WallThickness<(SELF\IfcRectangleProfileDef.YDim/2.));",

        @"WHERE
       UsenseCompatible : (('IFC4.IFCELEMENTARYSURFACE' IN TYPEOF(BasisSurface)) AND
                           (NOT ('IFC4.IFCPLANE' IN TYPEOF(BasisSurface)))) OR
                          ('IFC4.IFCSURFACEOFREVOLUTION' IN TYPEOF(BasisSurface)) OR
                          (Usense = (U2 > U1));
       VsenseCompatible : Vsense = (V2 > V1);",
    };

    [Test]
    [TestCaseSource(nameof(WhereSections))]
    public static void TestNoiseSectionDoesNotRequireParsing(string input)
    {
        // Wrap in an entity so the "skip" rule is exercised.
        var full = "ENTITY X; A : INTEGER; " + input + " END_ENTITY;";
        ParserTests.SingleParseTest(full, Grammar.Entity);
    }

    public static string[] EntityHeaderVariants =
    {
        "ENTITY A;",

        // no space before '('
        @"ENTITY B
      SUBTYPE OF(Parent);",

        // space before '('
        @"ENTITY C
      SUBTYPE OF (Parent);",

        // ABSTRACT + SUPERTYPE OF + ONEOF formatting
        @"ENTITY D
      ABSTRACT SUPERTYPE OF(ONEOF
        (D1, D2))
      SUBTYPE OF(Parent);",
    };

    [Test]
    [TestCaseSource(nameof(EntityHeaderVariants))]
    public static void TestEntityHeaderVariants(string input)
        => ParserTests.SingleParseTest(input, Grammar.EntityHeader);

    // ----------------------------------------
    // Type parsing 
    // ----------------------------------------
    public static string[] TypeDecls =
    {
        @"TYPE IfcDoorTypeEnum = ENUMERATION OF
  ( DOOR, GATE, USERDEFINED );
END_TYPE;",

        @"TYPE IfcFillAreaStyle = SELECT
  ( IfcFillAreaStyleHatching, IfcFillAreaStyleTiles );
END_TYPE;",

        @"TYPE IfcLabel = STRING;
END_TYPE;",
        
        @"TYPE IfcAbsorbedDoseMeasure = REAL;
        END_TYPE;",

        @"TYPE IfcUnitAssignment = SET [1:?] OF IfcUnit;
END_TYPE;",
    };

    [Test]
    [TestCaseSource(nameof(TypeDecls))]
    public static void TestTypeDecl(string input)
        => ParserTests.SingleParseTest(input, Grammar.TypeDecl);

    // ----------------------------------------
    // ENTITY parsing 
    // ----------------------------------------
    public static string[] Entities =
    {
        @"ENTITY IfcDerivedUnit;
    Elements : SET [1:?] OF IfcDerivedUnitElement;
    UnitType : IfcDerivedUnitEnum;
    UserDefinedType : OPTIONAL IfcLabel;
 DERIVE
    Dimensions : IfcDimensionalExponents := IfcDeriveDimensionalExponents(Elements);
 WHERE
    WR1 : (SIZEOF (Elements) > 1);
END_ENTITY;",

        @"ENTITY IfcDerivedUnitElement;
    Unit : IfcNamedUnit;
    Exponent : INTEGER;
END_ENTITY;",

        @"ENTITY IfcDimensionalExponents;
    LengthExponent : INTEGER;
    MassExponent : INTEGER;
    TimeExponent : INTEGER;
    ElectricCurrentExponent : INTEGER;
    ThermodynamicTemperatureExponent : INTEGER;
    AmountOfSubstanceExponent : INTEGER;
    LuminousIntensityExponent : INTEGER;
END_ENTITY;",

        // subtype clause example
        @"ENTITY IfcDistributionChamberElementType
 SUBTYPE OF (IfcDistributionFlowElementType);
    PredefinedType : IfcDistributionChamberElementTypeEnum;
 WHERE
    CorrectPredefinedType : (PredefinedType <> IfcDistributionChamberElementTypeEnum.USERDEFINED);
END_ENTITY;",

        @"ENTITY IfcRectangleHollowProfileDef
 SUBTYPE OF (IfcRectangleProfileDef);
    WallThickness : IfcPositiveLengthMeasure;
	InnerFilletRadius : OPTIONAL IfcNonNegativeLengthMeasure;
    OuterFilletRadius : OPTIONAL IfcNonNegativeLengthMeasure;
    WHERE
       ValidWallThickness : (WallThickness<(SELF\IfcRectangleProfileDef.XDim/2.)) AND
(WallThickness<(SELF\IfcRectangleProfileDef.YDim/2.));
	ValidInnerRadius : NOT(EXISTS(InnerFilletRadius)) OR
((InnerFilletRadius <= (SELF\IfcRectangleProfileDef.XDim/2. - WallThickness)) AND
 (InnerFilletRadius <= (SELF\IfcRectangleProfileDef.YDim/2. - WallThickness)));
	ValidOuterRadius : NOT(EXISTS(OuterFilletRadius)) OR
((OuterFilletRadius <= (SELF\IfcRectangleProfileDef.XDim/2.)) AND
 (OuterFilletRadius <= (SELF\IfcRectangleProfileDef.YDim/2.)));
END_ENTITY;",

        @"ENTITY IfcRectangleProfileDef
 SUPERTYPE OF(ONEOF
    (IfcRectangleHollowProfileDef
    , IfcRoundedRectangleProfileDef))
 SUBTYPE OF(IfcParameterizedProfileDef);
    XDim : IfcPositiveLengthMeasure;
	YDim : IfcPositiveLengthMeasure;
END_ENTITY;",

        @"ENTITY IfcRectangularPyramid
 SUBTYPE OF(IfcCsgPrimitive3D);
    XLength : IfcPositiveLengthMeasure;
	YLength : IfcPositiveLengthMeasure;
	Height : IfcPositiveLengthMeasure;
END_ENTITY;",

        @"ENTITY IfcRectangularTrimmedSurface
 SUBTYPE OF(IfcBoundedSurface);
    BasisSurface : IfcSurface;
	U1 : IfcParameterValue;
	V1 : IfcParameterValue;
	U2 : IfcParameterValue;
	V2 : IfcParameterValue;
	Usense : IfcBoolean;
	Vsense : IfcBoolean;
 WHERE
    U1AndU2Different : U1<> U2;
    V1AndV2Different : V1<> V2;
    UsenseCompatible : (('IFC4.IFCELEMENTARYSURFACE' IN TYPEOF(BasisSurface)) AND
         (NOT ('IFC4.IFCPLANE' IN TYPEOF(BasisSurface)))) OR
         ('IFC4.IFCSURFACEOFREVOLUTION' IN TYPEOF(BasisSurface)) OR
         (Usense = (U2 > U1));
    VsenseCompatible : Vsense = (V2 > V1);
END_ENTITY;",

        @"ENTITY IfcRecurrencePattern;
    RecurrenceType : IfcRecurrenceTypeEnum;
	DayComponent : OPTIONAL SET[1:?] OF IfcDayInMonthNumber;
    WeekdayComponent : OPTIONAL SET[1:?] OF IfcDayInWeekNumber;
    MonthComponent : OPTIONAL SET[1:?] OF IfcMonthInYearNumber;
    Position : OPTIONAL IfcInteger;
    Interval : OPTIONAL IfcInteger;
    Occurrences : OPTIONAL IfcInteger;
    TimePeriods : OPTIONAL LIST[1:?] OF IfcTimePeriod;
    END_ENTITY;",

        @"ENTITY IfcReference;
    TypeIdentifier : OPTIONAL IfcIdentifier;
    AttributeIdentifier : OPTIONAL IfcIdentifier;
    InstanceName : OPTIONAL IfcLabel;
    ListPositions : OPTIONAL LIST[1:?] OF IfcInteger;
    InnerReference : OPTIONAL IfcReference;
    END_ENTITY;",

        @"ENTITY IfcRegularTimeSeries
 SUBTYPE OF(IfcTimeSeries);
    TimeStep : IfcTimeMeasure;
	Values : LIST[1:?] OF IfcTimeSeriesValue;
END_ENTITY;",

        @"ENTITY IfcReinforcementBarProperties
 SUBTYPE OF(IfcPreDefinedProperties);
    TotalCrossSectionArea : IfcAreaMeasure;
	SteelGrade : IfcLabel;
	BarSurface : OPTIONAL IfcReinforcingBarSurfaceEnum;
    EffectiveDepth : OPTIONAL IfcLengthMeasure;
    NominalBarDiameter : OPTIONAL IfcPositiveLengthMeasure;
    BarCount : OPTIONAL IfcCountMeasure;
    END_ENTITY;",

        @"ENTITY IfcReinforcingBar
 SUBTYPE OF(IfcReinforcingElement);
    NominalDiameter : OPTIONAL IfcPositiveLengthMeasure;
    CrossSectionArea : OPTIONAL IfcAreaMeasure;
    BarLength : OPTIONAL IfcPositiveLengthMeasure;
    PredefinedType : OPTIONAL IfcReinforcingBarTypeEnum;
    BarSurface : OPTIONAL IfcReinforcingBarSurfaceEnum;
    WHERE
       CorrectPredefinedType : NOT EXISTS(PredefinedType) OR
   (PredefinedType<> IfcReinforcingBarTypeEnum.USERDEFINED) OR
   ((PredefinedType = IfcReinforcingBarTypeEnum.USERDEFINED) AND EXISTS(SELF\IfcObject.ObjectType));
	CorrectTypeAssigned : (SIZEOF(IsTypedBy) = 0) OR
('IFC4.IFCREINFORCINGBARTYPE' IN TYPEOF(SELF\IfcObject.IsTypedBy[1].RelatingType));
END_ENTITY;",

        @"ENTITY IfcRelAssigns
 ABSTRACT SUPERTYPE OF(ONEOF
    (IfcRelAssignsToActor
    , IfcRelAssignsToControl
    , IfcRelAssignsToGroup
    , IfcRelAssignsToProcess
    , IfcRelAssignsToProduct
    , IfcRelAssignsToResource))
 SUBTYPE OF(IfcRelationship);
    RelatedObjects : SET[1:?] OF IfcObjectDefinition;
	RelatedObjectsType : OPTIONAL IfcObjectTypeEnum;
    WHERE
       WR1 : IfcCorrectObjectAssignment(RelatedObjectsType, RelatedObjects);
    END_ENTITY;",

        @"ENTITY IfcRelAssignsToActor
 SUBTYPE OF(IfcRelAssigns);
    RelatingActor : IfcActor;
	ActingRole : OPTIONAL IfcActorRole;
    WHERE
       NoSelfReference : SIZEOF(QUERY(Temp<* SELF\IfcRelAssigns.RelatedObjects | RelatingActor :=: Temp)) = 0;
END_ENTITY;",

        @"ENTITY IfcRelAssignsToControl
 SUBTYPE OF(IfcRelAssigns);
    RelatingControl : IfcControl;
 WHERE
    NoSelfReference : SIZEOF(QUERY(Temp<* SELF\IfcRelAssigns.RelatedObjects | RelatingControl :=: Temp)) = 0;
END_ENTITY;",
    };



    [Test]
    [TestCaseSource(nameof(Entities))]
    public static void TestEntity(string input)
        => ParserTests.SingleParseTest(input, Grammar.Entity, true);

    // ----------------------------------------
    // File parsing test
    // ----------------------------------------
    [Test]
    [TestCase("IFC4X3.exp")]
    public static void TestFile(string file)
    {
        var pi = ParserInput.FromFile(InputFolder.RelativeFile(file));
        ParserTests.ParseTest(pi, Grammar.File);
    }

    // ----------------------------------------
    // Quick targeted "rule by name" helper (like Plato)
    // ----------------------------------------
    [Test]
    [TestCase("(* x *)", "Comment")]
    [TestCase("   (* x *)  \n", "WS")]
    [TestCase("OPTIONAL IfcLabel", "TypeExpr")]
    [TestCase("Elements : SET [1:?] OF IfcDerivedUnitElement;", "AttributeDecl")]
    [TestCase("ENTITY IfcDerivedUnit; Elements : SET [1:?] OF IfcDerivedUnitElement; END_ENTITY;", "Entity")]
    public static void TargetedTest(string input, string name)
    {
        var rule = Grammar.GetRuleFromName(name);
        if (rule == null) throw new Exception($"Could not find rule {name}");
        var result = ParserTests.ParseTest(input, rule);
        Assert.IsTrue(result == 1);
    }
}
