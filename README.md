# Parakeet

Parakeet is a simple parsing library written in C#. Parakeet is the parsing library being used by the 
[Plato programming language project](https://github.com/cdiggins/plato) to Parse both Plato 
and C# source code. 

## Overview 

Parakeet is a recursive descent (RD) parsing library based on the parsing expression grammar (PEG) formalization
introduced by Bryan Ford. Parakeet parsers are defined directly in C# using operator overloading. 
Parakeet combines both lexical analysis (aka tokenization) and syntactic analysis in a single pass. 

## Parse Trees 

Parse trees generated from Parakeet are untyped. A parse node is created whenever a `NodeRule` rule 
successfully matches its child rule against the input. After parsing the list of parse nodes 
is converted into a tree structure. 

## Typed Parse Tree 

A set of classes representing a strongly typed parse tree can be created automatically from a Parakeet grammar. 
For an example of the output see [the C# AST](https://github.com/cdiggins/parakeet/blob/master/Parakeet.Demos/CSharpAst.cs).

## History 

Parakeet evolved from the [Jigsaw parser](https://www.codeproject.com/Articles/272494/Implementing-Programming-Languages-using-Csharp) 
and applies lessons learned when writing the [Myna parsing library in TypeScript](https://cdiggins.github.io/myna-parser/) 
as well as my first parsing library [](https://www.codeproject.com/Articles/9121/Parsing-XML-in-C-using-the-YARD-Parser
Parakeet is designed to be as fast as possible while retaining a clean and elegant grammar description. 

## Why not Roslyn?

Roslyn was indeed my first choice for parsing C# and Plato (which is a syntactic subset of C#), however I could not easily extract the different 
modules into stand-alone libraries. Parakeet is being designed so that I can bootstrap Plato. Parakeet will be used to convert itself into Plato, 
and then to convert Plato into JavaScript. The goal is to have the whole Plato tool-chain written in Plato, and running on all platforms 
that Plato targets. 

If you are interested it is apparently possible  to [Run roslyn in a web-page via Blazor](https://github.com/Suchiman/Runny/tree/master/Runny). 

## Related Work

### C# and F# Parsing Libraries 

* https://github.com/benjamin-hodgson/Pidgin
* https://github.com/sprache/Sprache
* https://github.com/plioi/parsley
* https://github.com/datalust/superpower
* https://github.com/IronyProject/Irony
* https://github.com/teo-tsirpanis/Farkle
* https://github.com/takahisa/parseq

### Parser Generator Tools

* https://github.com/dbremner/peg-sharp  
* https://github.com/SickheadGames/TinyPG 
* https://github.com/otac0n/Pegasus
* https://github.com/qwertie/LLLPG-Samples

## References

* https://en.wikipedia.org/wiki/Parser_combinator
* https://en.wikipedia.org/wiki/Parsing_expression_grammar
* https://pdos.csail.mit.edu/~baford/packrat/icfp02/packrat-icfp02.pdf
