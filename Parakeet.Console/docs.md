# Parakeet 🦜 Documentation

## A Parsing Library for .NET Standard 2.0

# Overview 

**Parakeet** is a .NET Standard 2.0 compliant text parsing library.
designed specifically for parsing programming languages written in C#

It is used in the [Plato programming language](https://github.com/cdiggins/plato) 
tool chain. 

## Why do you care? 

A parsing library is an extremely powerful utility that make a class of traditionally 
hard programming problems much easier to solve, and can even provide a new way to look at 
problems. Sometimes expressing the solution to a problem in terms of a domain-specific 
language can make it much more tractable. 

## Parakeet compared to Regular Expressions

Recursive-descent parsers like Parakeet can parse more complex patterns than a regular expression. 
Specifically input where there is an implicit tree-like structure like programming languages, and 
arithmetic expressions.

Parakeet can of course also be used as a replacement for regular expression. While parser combinator libraries 
tend to be more verbose, the fact that sub-rules can be easily named and reused 
as variable or functions, makes them easier to build, debug, and understand. 

To illustrate compare the following phone number grammar in Parakeet:

```
public class PhoneNumberGrammar : CommonGrammar
{
    public Rule CountryCode 
        => Named(Optional('+' + Spaces) + Digit.Counted(1, 3));
    public Rule Separators 
        => Named(".- ".ToCharSetRule().ZeroOrMore());
    public Rule AreaCodeDigits 
        => Named(Digit.Counted(3));
    public Rule AreaCode 
        => Named(Parenthesized(AreaCodeDigits) | AreaCodeDigits);
    public Rule Exchange 
        => Named(Digit.Counted(3));
    public Rule Subscriber 
        => Named(Digit.Counted(4));
    public Rule PhoneNumber 
        => Named((CountryCode + Separators).Optional() 
            + AreaCode + Separators 
            + Exchange + Separators 
            + Subscriber);
}
```

To this regular expression from [StackOverflow](https://stackoverflow.com/a/16699507/184528):

```
^(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$
```

## Parsing Expression Grammar (PEG) Parser

Parakeet is closely related to 
[Parsing Expression Grammars (PEG)](https://en.wikipedia.org/wiki/Parsing_expression_grammar). 
A PEG describes a formal language in terms of a set of rules for recognizing strings

    NOTE: parakeet is not a packrat parser. It does not memoize intermediate results.
    This makes it faster for common types of grammars, but slower for grammars 
    with lots of potential backtracking scenarios. 

## A Combinatory Parsing Library 

Parakeet is also an example of a [combinatory parsing library](https://en.wikipedia.org/wiki/Parser_combinator). 
This means that oarsing rules are expressed in terms of primitive rules (called terminals), 
or combinations of rules defined used rule combinators.   

A Rule combinator is a function that takes one or or more rules as inputs and outputs a new rule. 

Technically in the literature a combinator is a higher-order function, but Parakeet rules 
are actually classes, with a single `Match` function. This allows Parakeet rules to contain 
useful meta-information. 

## Design Goals 

The design goals of Parakeet were 

1. easy to use and debug
2. reasonably performant for programming language
3. clear identification of parsing errors
4. ability to recover from, and collect, errors 
5. separate parsing and analysis
6. combine tokenization and parsing 

Unlike Antlr, Flex, Lex, YACC, Bison, and others Parakeet is not a parser code generator library. 
This choice was made to make it easier to iterate on the code, and to more easily leverage
the full abstraction capabilities of a programming language (e.g., variables, functions, 
and constants) rather than a limited domain specific language (DSL). 

Unlike the older toolchains Parakeet allows a single grammar to combine what used used to 
be separated into both tokenization and syntactic analysis. 

## Features

* Parsing error recovery  
* Run-time detection of stuck parsers 
* Line number and column number reporting 
* Immutable data structures 
* Operator overloading
* Fluent API syntax (aka method chaining)
* Automated creation of untyped parse trees
* Code generation for typed concrete syntax trees (CST)

# Using Parakeet

## Rules and Matching 

A Parakeet parser is defined by a single class deriving 
from [`Rule`](https://github.com/cdiggins/parakeet/blob/master/Parakeet/Rule.cs). 

Like regular expressions, a parser's primary purpose is to determinate whether a given input either 
conforms to (i.e., matches) a particular rule, or not. 

Every rule has a single function:

```chsarp
public ParserState Match(ParserState state);
```

The `Match` will return `null` if the Rule failed to match, or a new `ParserState` object if successful.
ParserState is a small lightweight object that is immutable. 

## Defining Grammars 

A grammar is a set of rules that are used for matching strings in a particular "language". 
A grammar derives from the `Grammar` base class. Grammars are not necessary for working with 
Parakeet but are a convenient way to group related rules.  

Most grammars derive from the `CommonGrammar` base class which provides a set of 
commonly used Rules and Rule combinators.

Most members of a grammar are properties representing specific rules. 

    NOTE: The choice to define a grammar using properties (verus fields or methods) is arbitrary. 
    The only thing required to make Parakeet work is an instance of a Rule class. Grammars 
    just make it easier to work with them. 

## Sample Grammar

The following is a basic grammar for CSV (Comma Separated Values) file,
based on an Antlr grammar. 

```
public class CsvGrammar : CommonGrammar
{
    public static readonly CsvGrammar Instance = new CsvGrammar();
    public override Rule StartRule => File;

    public Rule StringChar 
        => Named(AnyChar.Except('"') | "\"\"");
    public Rule String 
        => Node(DoubleQuotedString(StringChar));
    public Rule Text 
        => Node(AnyChar.Except(",\n\r\"").OneOrMore());
    public Rule Field 
        => Node(Text | String);
    public Rule Row 
        => Node(Field.ZeroOrMore() + Optional('\r') + '\n');
    public Rule File 
        => Node(Row.ZeroOrMore());
}
```

Compare to the following [Antlr Grammar](https://github.com/antlr/grammars-v4/blob/master/csv/CSV.g4): 

```
csvFile
    : hdr row+ EOF;
hdr
    : row;
row
    : field (',' field)* '\r'? '\n';
field
    : TEXT
    | STRING
    |;
TEXT
    : ~[,\n\r"]+;
STRING
    : '"' ('""' | ~'"')* '"'; 
```

# Technical Details

## Capturing Successful Matches via Node Rules

Capturing information about a successful match, such as the begin and end points and the name of the rule
is done via a special rule called `Node`. 

The `Node` rule has a child which it attempts to match, and if successful, adds a new `ParserNode` to the 
returned `ParserState` object. 

The resulting `ParserNode` is prepended to a linked-list of `ParserNode`. 

## Creating a Parse Tree from Parse Nodes

Once parsing of the input has completed succesffully the `ParserState` 
object will point to a linked list of `ParseNode` objects, one for each succesfully matched `Node` rule.

An extension function called `ParserTree ToParseTree(this ParserNode self)` will convert 
the flat list of nodes, into a tree structure. 

## Named Rules 

A `Named` rule assigns the name of the generating function or property to a 'NamedRule` which wraps 
the child rule. This makes debugging and outputting definitions of rules easier. 
Usually `NamedRule` are used for tokens. They do not create parse nodes.

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

### Primitive Rules

* `StringMatchRule` - Matches a string
* `AnyCharRule` - Matches any character, but fails at end of the file 
* `CharRangeRule` - Matches any character within a range  
* `CharSetRule` - Matches any character within a set
* `CharMatchRule` - Matches a specific character

### Assertion Rules

Several rules never advance the parser state:

* `At` - Returns the original parser state if the child rule succeeds, or null otherwise 
* `NotAt` - Returns the original parser state if the child rule fails, or null otherwise 
* `EndOfInput` - Returns the parser state if at the end of input, or null otherwise 


## ParserState 

The `ParserState` is a small immutable structure representing the current state of the parser, which 
includes a pointer to the `ParserInput`, the current index, a pointer 
to a list of `ParseNode` objects, and a pointer to a list of `ParseError` objects. 

## The Primitive Rules

* `StringRule` - Matches a sequence of characters. 
* `CaseInvariantStringRule` - Matches a sequence of characters disregarding case. 
* `CharRule` - Matches a single character. 
* `CharRangeRule` - Matches any character within a specified range. 
* `CharSetRule` - Matches any single character in a set. 

## Non-Advancing Rules 

These rules will return a ParserState, if successful but will not advance it's current index. 

* `EndOfInput` - succeeds only if the parser is at the end of the input.
* `BooleanRule` - either always succeeds or always fails, but does not advance input 

## The Basic Combinators 

* `ChoiceRule` - Attempts to match at least one of the child rules, and fails if none pass. 
* `SequenceRule` - Attempts to match every single rule in the a sequence, or fails if they don't all pass. 
* `OptionalRule` - Attempts to match a child rule a single time. Always succeeds. 
* `ZeroOrMore` - Attempts to match a child rule as many times as possible. Will always succeed. The child rule must advance input. 
* `CountedRule` - Succeeds only if the child rule matches a minimum number of times. Will stop trying to match once the upper-bound is reached. 

## Non-Advancing Combinators 

* `AtRule` - Succeeds if the child rule matches, but does not advance input.  
* `NotRule` - Succeeds only if the child rule does not match.  

## Special Combinators

* `RecursiveRule` - used to refer to a rule which has a cyclical reference. 
* `NamedRule` - assigns a name to a rule, which by default is the name of the function or property generating the rule
* `NodeRule` - like a `NamedRule` but also create a `ParseNode` when matched successfully. 
* `OnError` - used only in sequences, it advances the parser if one of the subsequent rules in the sequence fails.
For example, to go to the end of the statement, or the end of a code-block. 

## Fluent Syntax 

For convenience many of the rules can be constructed via extension methods defined on `Rule`
for a list see the file `RuleExtensions.cs`.

Some of the most common fluent syntax rules are:

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

You can of course define new ones as you want. 

## Operator Overloading 

Some combinators are available through operator overloading:

* `rule1 + rule2` => `new SequenceRule(rule1, rule2)`
* `rule1 | rule2` => `new ChoiceRule(rule1, rule2)`
* `!rule` => `new NotAt(rule)`

### Implicit Casts

The following implicit casts are defined for Rules: 

* `(string s)` => `new StringMatchRule(s)`
* `(char c)` => `new CharMatchRule(c)`
* `(char[] cs)` => `new CharSetRule(cs)`
* `(string[] xs)` => `new Choice(xs.Select(x => (Rule)x))`

## Context Free Grammars versus Parsing Expression Grammars 

Parakeet is similar to a [Parsing Expression Grammar](https://en.wikipedia.org/wiki/Parsing_expression_grammar), but has many additional operations. 
Because Parakeet is "just a library" you can quickly and easily define new operators 
to simplify the creation of grammars and tools. 

A Parakeet does not correspond to a CFG (Context Free Grammars), because 
the parsing rules will always yield either exactly one parse tree for each string,
or will reject it. In general this does not provide any practical limitation in recognizing 
Context Free Languages. 

# Advanced Topics 

## Recursive (Cyclic) References in a Grammar

Some rules need to reference a rule that indirectly references itself. This can created a stack-overflow
when creating rules, so a `RecursiveRule` needs to be created that can prevent infinite recursion. 

A `RecursiveRule` rule, retrieves a child rule via function only when required. The function for constructing it
is called `Grammar.Recursive` and takes the name of another rule as a parameter. 

The Following lambda calculus grammar demonstrates the grammar in action.

```
// https://en.wikipedia.org/wiki/Lambda_calculus
public class SimpleLambdaCalculusGrammar : BaseCommonGrammar
{
    public static readonly SimpleLambdaCalculusGrammar Instance 
        = new SimpleLambdaCalculusGrammar();
    public override Rule StartRule
        => Term;
    public Rule Variable 
        => Node(Identifier);
    public Rule Parameter 
        => Node("\\" + Variable);
    public Rule Abstraction 
        => Node("(" + Parameter + "." + Term + ")");
    public Rule InnerTerm 
        => Node(Variable | Abstraction | Application);
    public Rule Term
        => Node(Recursive(nameof(InnerTerm)));
    public Rule Application 
        => Node("(" + Term + WS + Term + ")");
}
```

## Creating a Typed Concrete Syntax Tree (CST)

The `ParserTree` created by Parakeet by default is an untyped parse tree: the rule associated with each node
is identifed via a string. 

This is not very convenient to work with for programming languages because you have to 
implicitly understand the production rules to know what to look for in the children. This can lead to
a lot of error checking boiler-plate code when trying to write robust code for working with the parse tree. 

To avoid the boiler-plate, and make the compiler do more work for you, you can use the `CstNodeBuilder`
to generate code for representing and constructing a type-safe version of the concrete syntax tree.

The `CstNodeBuilder` will generate a separate class for each type of node in the tree, and will 
provide a factory function to convert a `ParserTree` into the appropriate `CST`.  

## What about Abstract Syntax Trees (AST)?

The Abstract Syntax Tree or AST is an intermediate data structure frequently used 
in compilers and language tools for analysis and 
generating output. The precise form of an AST need to be determined on a case per case basis, 
for accomplishing whatever tasks you may have. Parakeet does not offer additional facilities 
for generating or managing your AST, beyond the construction of a typed CST. 

If you want an example of how you might convert your CST into an AST, take a look at the 
[Plato compiler](https://github.com/cdiggins/plato). 

## Optimizing Grammars

Parakeet offers a function for rewriting grammars at run-time called `Optimize`. It works by merging 
rules in a grammar while retaining meaning. The generated grammar is usually faster, and has fewer rules.
This allows people to express grammars in a way that is most conveneient without worry about performance. 

## Forward Only versus Backtracking

Parakeet is fastest when doing forward-only parsing. Parakeet does not memoize intermediate results
to assure that non-backtracking parsing is as fast possible. When a grammar requires 
backtracking, you will reprocess the same input multiple times. 

## Operator Precedence 

A common technique used in traditional language parsers is to account for operator precedence rules 
in the grammar, so that the generated parse tree has the precedence of operations already defined.
In general this slows down parsing, but is especially true for Parakeet, because the same
input can be potentially matched dozens of extra times to find the best production rule. 

The best practice for using Parakeet is to not incorporate precedence rules in the grammar, but rather 
account for them when constructing an AST from the CST. It is a simple and quick operation during that 
phase and makes the grammar simpler and the overall parsing much faster.

For an example of how to do this see the [Plato compiler](https://github.com/cdiggins/plato).  

## History 

Parakeet evolved from the [Jigsaw parser](https://www.codeproject.com/Articles/272494/Implementing-Programming-Languages-using-Csharp) 
and applies lessons learned when writing the 
[Myna parsing library in TypeScript](https://cdiggins.github.io/myna-parser/) 
as well as my first parsing library 
[YARD](https://www.codeproject.com/Articles/9121/Parsing-XML-in-C-using-the-YARD-Parser).

The Quixotic effort of writing parsing libraries was motivated by a long-standing obsession to
create a new language, along with the fact that when
I first started writing compilers, the ubiquitous "Dragon Book" made parsing out to be too darn 
complicated in my opinion. My first efforts at writing a parser was a 
rediscovery of top-down parsing technique invented by others. 
