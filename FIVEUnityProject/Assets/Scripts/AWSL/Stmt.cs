using System;
using UnityEngine;

namespace FIVE
{
    internal class Stmt
    {
        private readonly AwslScript script;

        internal enum StmtType { IF, WHILE, MOVE, TURN, };

        internal StmtType stmtType;

        internal int steps;

        internal int degree;

        internal Stmt(AwslScript script)
        {
            this.script = script;
        }

        internal void Parse()
        {
            if (script.content[script.index] != '(')
            {
            }

            script.index++;
            string word = script.GetWord();
            script.index += word.Length;
            if (word == "forward" || word == "backward")
            {
                stmtType = StmtType.MOVE;
                script.SkipSpace();
                string steps = script.GetWord();
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
                script.SkipSpace();
                string degree = script.GetWord();
                script.index += degree.Length;
                degree = degree.Substring(0, degree.Length - 1); // remove trailing ')'
                this.degree = Convert.ToInt32(degree);
            }
        }

        internal void Execute(GameObject gameObject)
        {
            if (stmtType == StmtType.MOVE)
            {
                CharacterController cc = gameObject.GetComponent<CharacterController>();
                cc.SimpleMove(gameObject.transform.forward * steps);
            }
        }
    }
}