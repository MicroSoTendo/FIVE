using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIVE
{
    internal class Stmt
    {
        internal enum StmtType { IF, WHILE, MOVE, TURN, };

        internal StmtType stmtType;

        internal Cond cond;

        internal Stmt stmt;

        internal int steps;

        internal int degree;
    }
}
