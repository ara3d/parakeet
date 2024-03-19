# The Basics of Using the Parakeet Text Parser in C#

[Parakeet](https://github.com/ara3d/parakeet) is a C# recursive descent library parser that uses operator overloading to make it easy to declare grammars 
in a readable format. It is an attempt to combine the advantages of parser generator tools and hand-rolled parsers.  

Parakeet allows parsers to be defined using collections of parsing rules called grammars
which support operator overloading, thus parsers can be defined using an embedded domain specific language (DSL). 

## Points of Interest 

This is my fourth parsing library. My previous work can be found here; 

1. YARD Parser - C++ Parsing Library - https://www.codeproject.com/Articles/9121/Parsing-XML-in-C-using-the-YARD-Parser
which inspired a popular C++ library PEGTL (https://github.com/taocpp/PEGTL/blob/main/doc/Changelog.md).
2. Jigsaw Parser - C# Parsing Library https://www.codeproject.com/Articles/272494/Implementing-Programming-Languages-using-Csharp
3. Myna Parser - TypeScript Parsing Library https://github.com/cdiggins/myna-parser

A very popular C++ parsing library called "PEGTL" was based on https://github.com/taocpp/PEGTL

The [Plato language implementation](https://github.com/cdiggins/plato) is being built using Parakeet.

This article was authored in Markdown and converted into HTML using Parakeet. 
See the code here at <https://github.com/ara3d/ara3d/tree/main/src/Ara3D.Parsing.Markdown>. 

## Parsing Rules

A Parsing rule, also colliqually called a "parser", is an instance of a class derived from `Rule` which provides 
a `ParserState Match(ParserState state)` member function. 

If the rule matches the input at the current position a new `ParserState` instance will be returned, otherwise
the function returns `null`. 

## Rule Combinator

A rule combinator creates a rule from other rules, similar to the core operations of a PEG grammar.
For example: `Choice`, `Sequence`, `ZeroOrMore`, `OneOrMore`, `NotAt`, etc. 

There are several ways to create rules from other rules:

1. Creating instances of the combinator classes (i.e., using `new`)
2. Calling one of the extension methods on `Rule` (e.g., `Optional()`, `NotAt()`, etc.)
3. Using operator overloading
    * `+` => `Sequence`
    * `|` => `Choice`
    * `!` => `NotAt`

For more information see the [Wikipedia Article on Parsing Expression Grammars](https://en.wikipedia.org/wiki/Parsing_expression_grammar).

## ParserState - The Input and Output of Rule Matching 

A ParserState is an immutable class that contains

1. A pointer to the input - a string combined with look-up tables for quickly determining line and columns from indexes)
1. An offset from the beginning of the input string
1. A pointer to a linked list of  parse nodes  
1. A pointer to a linked list of error.   

The fields of a `ParserState` instance are. 

```csharp 
public class ParserState 
{
    public readonly ParserInput Input;
    public readonly int Position;
    public readonly ParserNode Node;
    public readonly ParserError LastError;
    ...
}
```

## Definining Grammars

A Parakeet grammar is a class derived from the `Grammar` class. 
A class deriving from `Grammar` is required to provide an
overidden definition of the starting rule, and an optional whitespace rule.

1. `abstract Rule StartRule {get;}`
2. `virtual Rule WS {get;}` 

Most rules are defined as computed properties, but can also be
functions or fields. It is up to the programmer.

Top-level rules, those that are directly associated with properties in the grammar
are usually either `Node` rules or `Named` rules. 

Named rules simply return the result of matching a child rule. They help
with grammar debugging and diagnostics. 

## Node Rules

A `Node` rule is liked a `NamedRule` in that it has a name but if matched successfully 
will return a `ParserState` that has at least two new `ParserNode` instances 
added to the linked list of nodes. One node points the beginning of the match, and the
other node points to the end.   

A `ParserNode` looks like this:

```csharp
public class ParserNode
{
    public readonly ParserRange Range;
    public readonly string Name;
    public readonly ParserNode Previous;
    ...
}
```

A `ParserNode` linked list can be converted into a parse tree using the `ParseTreeNode ToParseTree()` method.

## Generating Parsing Errors with OnFail Rules  

There are several ways that a parsing rule might not match successfully:

1. Returning `null` - a normal failure to match 
2. Not reaching the end of the input `ParserState.AtEnd == false`
3. Accumulating one or more `ParserError` in a linked list.

A special rule called `OnFail` is used to generate `ParserError`. The `OnFail` rule is expected to appear as a child
of a `SequenceRule`. 

It indicates that if the preceding child rules succeed, then any failure in the following rule will generate a `ParserError`.
The `OnFail` has an optional recovery rule as a parameter, allowing the parser to advance to the next location, such
as just past the end of statement, and attempt to continue. 
 
The following is a snippet from the [Plato grammar](https://github.com/ara3d/parakeet/blob/master/Parakeet.Grammars/PlatoGrammar.cs)
which demonstrates how the error handling and recovery occurs.  

```csharp
public Rule AdvanceOnFail => 
    OnFail(Token.RepeatUntilPast(EOS | "}"));

public Rule IfStatement =>
    Node(Keyword("if") + AdvanceOnFail + ParenthesizedExpression + Statement + ElseClause.Optional());
```

The `IfStatement` rule indicates that if the keyword `if` is matched then it must be followed 
by an expression enclosed in parenthesis, then a valid statement, anad then optionally an else clause. 

If a failure occurs after the keyword `if` then we know there was a parsing error. The parser will consume tokens until 
it passes the end of statement (EOS) marker (`;`) or a closing brace (`}`). The IfStatement rule will return a 
valid ParserState, but it will have a new `ParserError` prepended to the linked list of errors.  



