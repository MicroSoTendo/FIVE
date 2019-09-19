using UnityEngine;

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
                stmts.parse();
            }
        }

        internal void execute(GameObject gameObject)
        {
            stmt.execute(gameObject);
            if (stmts != null)
            {
                stmts.execute(gameObject);
            }
        }
    }
}
