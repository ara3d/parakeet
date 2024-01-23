# Ara3D.Parsing 

The **Ara3D.Parsing** library is a [recursive descent](https://en.wikipedia.org/wiki/Recursive_descent_parser) (RD) parsing library based on the [parsing expression grammar](https://en.wikipedia.org/wiki/Parsing_expression_grammar) (PEG) formalization
introduced by [Bryan Ford](https://bford.info/pub/lang/peg.pdf). Parsers are defined directly in C# using operator overloading. 
The parsing library can be used to do both lexical analysis (aka tokenization) and syntactic analysis in a single pass, or multiple passes. 

## Features 

* Parsing error recovery  
* Run-time detection of stuck parsers 
* Line number and column number reporting 
* Immutable data structures 
* Operator overloading
* Fluent API syntax (aka method chaining)
* Automated creation of untyped parse trees

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

A parser is defined by a class deriving from [`Rule`](https://github.com/cdiggins/parakeet/blob/master/Parakeet/Rule.cs). 
Some rules are defined by combining rules. 

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

* `OnError` - Used only within `Sequence` rules. Contains a child rule called the recovery rule. 
Will always return the `ParserState` when matched. If a sequence encounters an `OnError` node and 
one of the subsequent child rules fails, the parser will then try to match the `OnError` recovery rule to advance 
to a place where it is likely to be able to continue parsing successfully (e.g. the end of a statement).

## Parse Node Lists and Parse Trees

A parse node is created whenever a `NodeRule` rule and stored in a list. The `ParserState`
points to the head of the list. After parsing the list of parse nodes can be converted into a 
an untyped parse tree. 

## History 

Parakeet was previously called [Parakeet](https://github.com/cdiggins/parakeet) which evolved from the [Jigsaw parser](https://www.codeproject.com/Articles/272494/Implementing-Programming-Languages-using-Csharp) 
and applies lessons learned when writing the [Myna parsing library in TypeScript](https://cdiggins.github.io/myna-parser/) 
as well as my first parsing library [YARD](https://www.codeproject.com/Articles/9121/Parsing-XML-in-C-using-the-YARD-Parser).
