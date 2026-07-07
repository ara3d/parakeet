    using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Logging;
using Ara3D.Parakeet;
using Ara3D.Utils;

namespace Ara3D.Parsing
{
    /// <summary>
    /// Combines common parsing tasks.
    /// Provides robust error handling, logging, and optional CST generation.
    /// Can be used for tokenization: you can provide a tokenization grammar and look at generated ParseNode list 
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// The main parse rule. 
        /// </summary>
        public Rule ParseRule { get; }
        
        /// <summary>
        /// Created from the input file or input text.
        /// </summary>
        public ParserInput Input { get; }

        /// <summary>
        /// The result of applying the parse rule to the input. 
        /// </summary>
        public ParserState ParseResult { get; }

        /// <summary>
        /// If not provided, a default logger that outputs to the std out and debug console
        /// will be created. 
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Whether the parse and generation of CST, if present is successful
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// The root of the raw parse tree. 
        /// </summary>
        public ParserTreeNode ParseTree { get; } 

        /// <summary>
        /// The root of the concrete syntax tree.
        /// </summary>
        public CstNode Cst { get; }

        /// <summary>
        /// Any caught exceptions will go here. 
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// List of all error messages generated during all phases. 
        /// </summary>
        public List<string> ErrorMessages { get; } = new List<string>();

        /// <summary>
        /// All error strings concatenated together 
        /// </summary>
        public string ParserErrorsString
            => ErrorMessages.JoinStringsWithNewLine();

        /// <summary>
        /// List of all of the caught error during parsing. 
        /// </summary>
        public List<ParserError> ParserErrors { get; } = new List<ParserError>();

        /// <summary>
        /// Flat list of all generated parser nodes. Useful when using parser for tokenization. 
        /// </summary>
        public List<ParserNode> ParserNodes { get; } = new List<ParserNode>();

        /// <summary>
        /// Each node converted to a string and concatenated together 
        /// </summary>
        public string ParserNodesString
            => ParserNodes.JoinStringsWithNewLine();

        /// <summary>
        /// Generates an XML representation of the CST if present. 
        /// </summary>
        public string CstXml =>
            Cst?.ToXml()?.ToString() ?? "";

        /// <summary>
        /// Generates an XML representation of the CST if present. 
        /// </summary>
        public string ParseXml
            => ParseTree?.ToXml() ?? "";

        //==

        private void LogInfo(string message)
            => Logger.Log(message);

        private void LogError(string message)
        {
            Logger.Log(message, LogLevel.Error);
            ErrorMessages.Add(message);
        }

        private void LogError(ParserError error)
        {
            // TODO: convert to a proper error message. 
            ParserErrors.Add(error);
            var msg = error.ToString();
            Logger.Log(msg, LogLevel.Error);
            ErrorMessages.Add(msg);
        }

        //---
        // The main constructor 

        public Parser(
            ParserInput input,
            Rule parseRule,
            Func<ParserTreeNode, CstNode> cstFunc = null,
            ILogger logger = null
            )
        {
            try
            {
                Logger = logger.SelfOrDefault();
                
                Verifier.AssertNotNull(parseRule, nameof(parseRule));
                Verifier.AssertNotNull(input, nameof(input));

                ParseRule = parseRule;
                Input = input;

                LogInfo($"Input has {Input.LineToChar.Count} lines with {Input.Text.Length} characters");

                LogInfo($"Starting parsing");
                ParseResult = ParseRule.Parse(Input);

                var succeeded = true;
                if (ParseResult == null)
                {
                    LogError("Parsing failed with no info");
                    succeeded = false;
                }
                else
                {
                    if (!ParseResult.AtEnd())
                    {
                        LogError($"Parsing failed to reach end of input, stopped at: {ParseResult}");
                        succeeded = false;
                    }
                    else
                    {
                        LogInfo($"Parser reach end of input");
                    }

                    var errors = ParseResult.AllErrors().ToList();
                    LogInfo($"{errors.Count} parsing errors");
                    foreach (var e in errors)
                    {
                        LogError(e);
                        succeeded = false;
                    }

                    if (succeeded)
                    {
                        LogInfo($"Successfully completed parsing");
                    }
                    else
                    {
                        LogInfo($"Unsuccessfully completed parsing");
                    }

                    ParserNodes.AddRange(ParseResult.AllEndNodes());
                    LogInfo($"{ParserNodes.Count} parse nodes");
                }

                Succeeded = succeeded;

                if (Succeeded)
                {
                    LogInfo($"Creating parse tree");
                    ParseTree = ParseResult.Node?.ToParseTree();
                    if (ParseTree == null)
                    {
                        // Not necessarily an error, because maybe there are no node rules
                        LogInfo($"No parse tree created");
                    }
                    else
                    {
                        LogInfo($"Successfully created a parse tree");
                    }
                }
                else
                {
                    LogInfo($"Parsing was not successful, skipping parse tree creation");
                }

                if (cstFunc == null)
                {
                    LogInfo($"No CST factory function provided, skipping CST creation");
                }
                else
                {
                    if (ParseTree != null)
                    {
                        LogInfo($"Starting creating CST tree");
                        Cst = cstFunc(ParseTree);
                        LogInfo($"Finished creating CST");
                    }
                    else
                    {
                        LogInfo($"No parse tree created, so skipping CST creation");
                    }
                }
            }
            catch (Exception e)
            {
                Exception = e;
                LogError($"Unhandled exception occurred {e.Message}");
            }
        }
    }
}
    