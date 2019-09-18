using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIVE
{
    internal class Stmts
    {
        AwslScript script;
        internal Stmt stmt;
        internal Stmts stmts;

        internal Stmts(AwslScript script)
        {
            this.script = script;
            stmt = new Stmt(script);
        }

        internal void parse()
        {
            stmt.parse();
            if (script.content[script.index] == '(')
            {
                stmts = new Stmts(script);       
            }
        }
    }
}
