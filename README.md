# Parakeet

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.Parakeet)](https://www.nuget.org/packages/Ara3D.Parakeet)

**Parakeet** is a text parsing library written in C#. Parakeet is the parsing library being used by the 
[Plato programming language project](https://github.com/cdiggins/plato) to Parse both Plato and C# source code. 

![Parakeet1](https://user-images.githubusercontent.com/1759994/222930131-4edeb2ce-757f-4471-8905-8c24ecbc67f8.png)

[Image by Hugo Wai](https://unsplash.com/photos/MEborZA-3Ps)

## Overview 

Parakeet is a [recursive descent](https://en.wikipedia.org/wiki/Recursive_descent_parser) (RD) parsing library based on the 
[parsing expression grammar](https://en.wikipedia.org/wiki/Parsing_expression_grammar) (PEG) formalization
introduced by [Bryan Ford](https://bford.info/pub/lang/peg.pdf). Parakeet parsers are defined directly in C# using operator overloading. 
Parakeet combines both lexical analysis (aka tokenization) and syntactic analysis in a single pass. 

See this [CodeProject article](https://www.codeproject.com/Articles/5379232/Introduction-to-Text-Parsing-in-Csharp-using-Parak)
for an introduction to the core concepts of Parakeet. 

## More Details and Features

Parakeet was designed primarily for the challenge of parsing programming languages. It can be used in different contexts of course. 

Parakeet supports:

* Parsing error recovery  
* Run-time detection of stuck parsers 
* Line number and column number reporting 
* Immutable data structures 
* Operator overloading
* Fluent API syntax (aka method chaining)
* Automated creation of untyped parse trees
* Code generation for converting untyped parse trees into a strongly typed concrete syntax tree (CST). 

## Steps 

1. Define a grammar in code: a class with a set of properties that map to rules 
1. Convert the input text into a parser input object 
1. Choose the starting rule of the grammar 
1. Call the match function of the starting rule 
1. Examine the resulting `ParserState` object
1. If the result is `null` the parser failed to match, and failed to recover 
	* Consider adding `OnError` to your grammar
1. Convert  

## Primary Classes

* `ParserInput` - Wraps a string with convenience functions to retrieve row/column, and potentially the source file name. 
Can be implicitly converted from a string 
* `ParserState` - Represents a position in the input and a pointer to the most recently created parse node. 
* `ParserRange` - Contains two `ParserState` objects, one representing the beginning of a range of input and the other the end.
* `ParserNode` - A named node in parse node list.  
* `ParserTree` - A tree structure created from a linked list of `ParserNode` objects
* `ParserCache` - Stores parser errors, and successful parse results to accelerated future lookups. 
* `Rule` - Base class of a parser, provides a match function that accepts a ParserState and a ParserCache.
* `Grammar` - Base class of a collection of parsing rules, usually defined as properties. 
* `CstNode` - Base class of typed parse trees generated from `Grammar` objects. 
* `CstClassBuilder` - Static class with functions for generating `AstNode` classes and factory functions from `ParserTree` objects. 
* `ParserError` - An error created when a `Sequence` fails to match a child rule after an `OnError` rule.
* `ParserException` - This represents an internal parser error which usually results from a grammar mistake.  

## Grammar

A `Grammar` is the base class for collections of parsing rules. Usually each parse rule in a grammar is defined 
as a computed property that either returns a `NodeRule` or a `TokenRule`. By using the functions `Node` and `Token`
to wrap rule definitions, names are automatically assigned to each rule. 

The `GrammarExtensions.cs` file contains a number of helper functions for outputting the definitions of grammars 
or to simplify parsing rules. 

When defining rules it is important that any cyclical reference from a rule to itself uses at least one `RecursiveRule`
in the relationship chain. This prevents stack overflow errors from occuring.
 
## Rules

A Parakeet parser is defined by a class deriving from [`Rule`](https://github.com/cdiggins/parakeet/blob/master/Parakeet/Rule.cs). Some rules are defined by combining rules. 
Those combining rules are called "combinators". 

Ever rule has a single function:

```chsarp
public ParserState Match(ParserState state, ParserCache cache)
```

The `Match` will return `null` if the Rule failed to match, or a `ParserState` object if successful.

### Fluent Syntax for Rules

Rules can be combined using a fluent syntax (aka method chaining). 

* `rule.At()` => `new At(rule)`
* `rule.NotAt()` => `new NotAt(rule)`
* `rule1.Then(rule2)` => `new Sequence(rule1, rule2)`
* `rule1.ThenNot(rule2)` => `new Sequence(rule, rule2.NotAt())`
* `rule.Optional()` => `new Optional(rule)`
* `rule1.Or(rule2)` => `new Choice(rule1, rule2)`
* `rule1.Except(rule2)` => `new Sequence(rule2.NotAt(), rule1)`
* `rule.ZeroOrMore()` => `new ZeroOrMore(rule)`
* `rule.OneOrMore()` => `new Sequence(rule, rule.ZeroOrMoree)`
* `char1.To(char2)` => `new CharRangeRule(char1, char2)`

### Overloaded Operators for Rules

Rules can be combined using the following overloaded operators.

* `rule1 + rule2` => `new SequenceRule(rule1, rule2)`
* `rule1 | rule2` => `new ChoiceRule(rule1, rule2)`
* `!rule` => `new NotAt(rule)`

### Implicit Casts

The following implicit casts are defined for Rules: 

* `Rule(string s)` => `new StringMatchRule(s)`
* `Rule(char c)` => `new CharMatchRule(c)`
* `Rule(char[] cs)` => `new CharSetRule(cs)`
* `Rule(string[] xs)` => `new Choice(xs.Select(x => (Rule)x))`

### Primitive Rules

* `StringMatchRule` - Matches a string
* `AnyCharRule` - Matches any character, but fails at end of the file 
* `CharRangeRule` - Matches any character within a range  
* `CharSetRule` - Matches any character within a set
* `CharMatchRule` - Matches a specific character

### Rule Combinators

Rule combinators combine zero or more rules. 

* `ZeroOrMore` - Tries to match a child rule as many times as possible, returning the original `ParserState` if not successful at least once. 
* `Optional` - Tries to match a child rule exactly once, returning the original `ParserState` if the child rule fails. 
* `Sequence` - Matches a sequence of rules one by one, returning `null` if not successful.
* `Choice` - Matches a collection of rules one by one, until one succeeeds, or `null` if not successful.
* `RecursiveRule` - Matches a child rule defined by a lambda, thus allowing Rule definitions to have cycles.
* `TokenRule` - A rule that just matches it child without creating a node. Used for defining grammars. 
* `NodeRule` - Creates a new `ParserNode` and adds it to the `ParserState`. May also eat whitespace (true by default). 

### Assertion Rules

Several rules never advance the parser state:

* `At` - Returns the original parser state if the child rule succeeds, or null otherwise 
* `NotAt` - Returns the original parser state if the child rule fails, or null otherwise 
* `EndOfInput` - Returns the parser state if at the end of input, or null otherwise 

### Error Handling Rules

* `OnFail` - Used only within `Sequence` rules. Contains a child rule called the recovery rule. 
Will always succeed and return the `ParserState` when matched. If a sequence encounters an `OnFail` error and 
one of the subsequent child rules fails, the parser will then use the recovery rule to try to advance 
to a place where it is likely to be able to continue parsing successfully (e.g. the end of a statement).

## Parse Trees 

Parse trees generated from Parakeet are untyped. A parse node is created whenever a `NodeRule` rule 
successfully matches its child rule against the input. After parsing the list of parse nodes 
is converted into a tree structure. 

## Typed Parse Tree (CST)

A set of classes representing a strongly typed parse tree can be created automatically from a Parakeet grammar. This is called the 
Concrete Syntax Tree. Concrete syntax trees are generated from the `Ara3D.Parakeet.Grammars` project using one of the 
functions in the `Ara3d.Parakeet.Tests` project. 

## Examples of Using Parakeet 

The following projects use Parakeet:

* <https://github.com/ara3d/ara3d/tree/main/src/Ara3D.Parsing.Markdown>
* <https://github.com/cdiggins/Plato>

## History 

Parakeet evolved from the [Jigsaw parser](https://www.codeproject.com/Articles/272494/Implementing-Programming-Languages-using-Csharp) 
and applies lessons learned when writing the [Myna parsing library in TypeScript](https://cdiggins.github.io/myna-parser/) 
as well as my first parsing library [YARD](https://www.codeproject.com/Articles/9121/Parsing-XML-in-C-using-the-YARD-Parser
Parakeet is designed to be as fast as possible while retaining a clean and elegant grammar description. 

## Related Work

### C# and F# Parsing Libraries 

* https://github.com/benjamin-hodgson/Pidgin
* https://github.com/sprache/Sprache
* https://github.com/plioi/parsley
* https://github.com/datalust/superpower
* https://github.com/IronyProject/Irony
* https://github.com/teo-tsirpanis/Farkle
* https://github.com/takahisa/parseq
* https://github.com/picoe/Eto.Parse
* https://github.com/b3b00/CSLY
* https://github.com/stephan-tolksdorf/fparsec

### Parser Generator Tools

* https://github.com/dbremner/peg-sharp  
* https://github.com/SickheadGames/TinyPG 
* https://github.com/otac0n/Pegasus
* https://github.com/qwertie/LLLPG-Samples
* https://github.com/antlr

## References

* https://en.wikipedia.org/wiki/Parser_combinator
* https://en.wikipedia.org/wiki/Parsing_expression_grammar
* https://pdos.csail.mit.edu/~baford/packrat/icfp02/packrat-icfp02.pdf

## FAQ 

Q: Isn't a parse tree and concrete syntax tree (CST) the same thing? 

> Yes. Parakeet uses the term parse tree to refer to untyped parse tree, and CST to refer to the typed parse tree.  

Q: What is the difference between a CST and an AST? 

> The CST is generated from parsing the input text as is. 
> An AST is the result of transforming the CST into a form that has the same semantic meaning but is presumably 
> simpler and easier. 

Q: Why isn't Parakeet a code generator from the beginning 

> I find it easier to learn, understand, use, and debug libraries that aren't generated. 
> Writing an extension to Parakeet that generates parser code would not be very hard.  

Q: So why is the CST code generated? 

> Having a strongly typed CST is very beneficial when writing analysis and transformation tools 
> especially for non-trivial grammars like those of programming languages. 
> I don't know of any other way in C# to generate strongly typed libraries other than generating code. 

Q: Isn't parsing library X faster? 

> Maybe. I designed Parakeet to be fast enough for my needs, but I prioritized making it robust and (hopefully) easy to use. 
> See the related work section for other parsing libraries to consider. 

Q: Can you provide some benchmarks? Or implement grammar X? 

> I'm kind of busy getting work done. 
> If you are willing to fund this project, then we should talk: email me at <cdiggins@gmail.com>.   
