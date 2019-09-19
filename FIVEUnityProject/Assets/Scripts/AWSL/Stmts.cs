using UnityEngine;

namespace FIVE.AWSL
{
    internal class Stmts
    {
        private readonly AwslScript script;
        internal Stmt stmt;
        internal Stmts stmts;

        internal Stmts(AwslScript script)
        {
            this.script = script;
            stmt = new Stmt(script);
        }

        internal void Parse()
        {
            stmt.Parse();
            if (script.content[script.index] == '(')
            {
                stmts = new Stmts(script);
                stmts.Parse();
            }
        }

        internal void Execute(GameObject gameObject)
        {
            stmt.Execute(gameObject);
            if (stmts != null)
            {
                stmts.Execute(gameObject);
            }
        }
    }
}