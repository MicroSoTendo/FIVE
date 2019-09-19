using System;
using UnityEngine;

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
                if (word == "forward")
                {
                    this.steps = Convert.ToInt32(steps);
                }
                else
                {
                    this.steps = -Convert.ToInt32(steps);
                }
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

        internal void execute(GameObject gameObject)
        {
            if (stmtType == StmtType.MOVE)
            {
                CharacterController cc = gameObject.GetComponent<CharacterController>();
                cc.SimpleMove(gameObject.transform.forward * steps);
            }
        }
    }
}
