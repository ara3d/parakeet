using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Demos
{
    public class CsvGrammar : CommonGrammar
    {
        /*
        public Rule TextData => Node(AnyCharRule.);
        public Rule Csv = Node()
        m.notChar('\n\r"' + delimiter);    
        this.quoted     = m.doubleQuoted(m.notChar('"').or('""').zeroOrMore);
        this.field      = this.textdata.or(this.quoted).zeroOrMore.ast;
        this.record     = this.field.delimited(delimiter).ast;
        this.file       = this.record.delimited(m.newLine).ast;        
         */
    }
}
