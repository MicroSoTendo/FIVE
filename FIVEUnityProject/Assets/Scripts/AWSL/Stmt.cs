using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIVE
{
    internal class Stmt
    {
        AwslScript script;

        internal enum StmtType { IF, WHILE, MOVE, TURN, };

        internal StmtType stmtType;

        internal int steps;

        internal int degree;

        internal Stmt(AwslScript script)
        {
            this.script = script;
        }

        internal void parse()
        {
            if (script.content[script.index] != '(')
            {

            }

            script.index++;
            string word = script.getWord();
            script.index += word.Length;
            if (word == "forward" || word == "backward")
            {
                stmtType = StmtType.MOVE;
                script.skipSpace();
                string steps = script.getWord();
                script.index += steps.Length;
                steps = steps.Substring(0, steps.Length - 1); // remove trailing ')'
                this.steps = Convert.ToInt32(steps);
            }
            else if (word == "left" || word == "right")
            {
                stmtType = StmtType.TURN;
                script.skipSpace();
                string degree = script.getWord();
                script.index += degree.Length;
                degree = degree.Substring(0, degree.Length - 1); // remove trailing ')'
                this.degree = Convert.ToInt32(degree);
            }
        }
    }
}
