# Parakeet

Parakeet is a simple parsing library written in C#. 

Parakeet is the paraing library being used by the 
[Plato programming language project](https://github.com/cdiggins/plato). 

## Overview 

Parakeet is a recursive descent (RD) parsing library based on the parsing expression grammar (PEG) formalization
introduced by Bryan Ford. Parakeet parsers are defined directly in C# using operator overloading. 
Parakeet combines both lexical analysis (aka tokenization) and syntactic analysis in a single pass. 

## Parse Trees 

Parse trees generated from Parakeet are untyped. A node in the parse tree is created whenever a `NodeRule` rules 
successfully matches its child rule against the input. 

## Typed Parse Tree 

A set of classes representing a strongly typed parse tree can be created automatically from a Parakeet grammar. 

## History 

Parakeet evolved from the [Jigsaw parser](https://www.codeproject.com/Articles/272494/Implementing-Programming-Languages-using-Csharp) 
and applies lessons learned when writing the [Myna parsing library in TypeScript](https://cdiggins.github.io/myna-parser/). 
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

### Parser Generator Tools

* https://github.com/dbremner/peg-sharp  
* https://github.com/SickheadGames/TinyPG 
* https://github.com/otac0n/Pegasus
* https://github.com/qwertie/LLLPG-Samples

## References

* https://en.wikipedia.org/wiki/Parser_combinator
* https://en.wikipedia.org/wiki/Parsing_expression_grammar
* https://pdos.csail.mit.edu/~baford/packrat/icfp02/packrat-icfp02.pdf


