using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet
{
    // TODO: put this in Ptarmigan (maybe)
    public class CodeBuilder<T> where T: CodeBuilder<T>
    {
        public StringBuilder sb { get; } = new StringBuilder();
        public bool AtNewLine { get; private set; } 
        public int IndentLevel { get; private set; }
        
        public T Indent()
        {
            IndentLevel++;
            return this as T;
        }

        public T Dedent()
        {
            IndentLevel--;
            return this as T;
        }
        
        public string Indentation()
        {
            return new string(' ', IndentLevel * 4);
        }

        public T Write(char c)
        {
            return Write(c.ToString());
        }

        public T Write(string s)
        {
            if (string.IsNullOrEmpty(s))
                return this as T;
            if (AtNewLine)
            {
                sb.Append(Indentation());
                AtNewLine = false;
            }
            sb.Append(s);
            return this as T;
        }
        
        public T WriteLine()
        {
            AtNewLine = true;
            sb.AppendLine();
            return this as T;
        }
        
        public T WriteLine(string s)
        {
            return Write(s).WriteLine();
        }
        
        public T Parenthesize(Func<T, T> f) 
        {
            return f(Write("(") as T).Write(")");
        }

        public T WriteCommaList<TElement>(IEnumerable<TElement> elements, Func<T, TElement, T> fElement)
        {
            return WriteList(elements, fElement, w => w.Write(", "));
        }

        public T WriteList<TElement>(IEnumerable<TElement> elements, Func<T, TElement, T> fElement, Func<T, T> fSeparator = null)
        {
            var first = true;
            var r = this as T;
            foreach (var element in elements)
            {
                if (!first)
                {
                    r = fSeparator?.Invoke(r) ?? r;
                }
                else
                {
                    first = false;
                }

                r = fElement(r, element);
            }
            return r;
        }

        public T Brace(Func<T, T> f) 
        {
            return f(Write("{").Indent().WriteLine()).Dedent().WriteLine("}");
        }
        
        public T WriteIf(bool condition, Func<T, T> f) 
        {
            return condition ? f(this as T) : this as T;
        }
        
        public override string ToString()
        {
            return sb.ToString();
        }

        public T Space()
        {
            return Write(" ");
        }

        public T WriteStartBlock()
            => WriteLine("{").Indent();

        public T WriteEndBlock()
            => Dedent().WriteLine("}");
    }

    public class CodeBuilder : CodeBuilder<CodeBuilder>
    { }
}