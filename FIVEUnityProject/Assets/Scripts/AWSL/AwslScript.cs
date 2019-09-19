using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    internal class AwslScript
    {
        enum Instruction { Forward, Backward, Left, Right }

        Dictionary<Instruction, int> program;

        public string content;
        public int index;
        internal Stmts rootStmts;

        internal AwslScript(string script)
        {
            content = script;
            index = 0;
            rootStmts = new Stmts(this);

            parse();
        }

        internal void execute(GameObject gameObject)
        {
            
        }

        internal void parse()
        {
            while (index < content.Length)
            {
                parseStmt();
            }
        }

        private void parseStmt()
        {
            if (content[index] != '(')
            {
                // ERROR
            }
            index++;

            string word = getWord();
            index += word.Length;
            string data = getWord();
            index += data.Length;
            if (word == "forward")
            {
                program.Add(Instruction.Forward, Convert.ToInt32(data));
            }
            else if (word == "backward")
            {
                program.Add(Instruction.Backward, Convert.ToInt32(data));
            }
            else if (word == "left")
            {
                program.Add(Instruction.Left, Convert.ToInt32(data));
            }
            else if (word == "right")
            {
                program.Add(Instruction.Right, Convert.ToInt32(data));
            }


            skipSpace();
            if (content[index] != ')')
            {
                // ERROR
            }
            index++;
        }

        internal void skipSpace()
        {
            while (content[index] == ' ')
            {
                ++index;
            }
        }

        // Return the first word ending in space
        internal string getWord()
        {
            int endIndex = index;
            while (!Constants.AwslDelimiter.Contains(content[endIndex]) || endIndex < content.Length)
            {
                endIndex++;
            }
            return content.Substring(index, endIndex - index);
        }
    }
}
