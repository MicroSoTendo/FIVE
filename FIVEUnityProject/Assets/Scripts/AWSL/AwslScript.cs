using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    internal class AwslScript
    {
        public string content;
        public int index;
        internal Stmts rootStmts;

        internal AwslScript(string script)
        {
            content = script;
            index = 0;
            rootStmts = new Stmts(this);
        }

        internal void parse()
        {
            rootStmts.parse();
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
            return content.Substring(index, content.IndexOf(' ', index));
        }
    }
}
