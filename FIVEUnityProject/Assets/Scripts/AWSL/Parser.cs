using System.Collections.Generic;
using UnityEngine.Assertions;

namespace FIVE.AWSL
{
    internal class Parser
    {
        private enum TokenType
        {
            LP, RP, ID, END
        }

        private string code;

        public Parser(string code)
        {
            this.code = code;
        }

        public RuntimeContext Parse()
        {
            var exprs = new List<Expr>();

            while (true)
            {
                Expr expr = ParseExpr();
                if (expr != null)
                {
                    exprs.Add(expr);
                }
                else
                {
                    break;
                }
            }

            return new RuntimeContext { Exprs = exprs };
        }

        private Expr ParseExpr()
        {
            var expr = new Expr();

            (TokenType, int) token = PeekToken();
            if (token.Item1 == TokenType.END)
            {
                return null;
            }
            Assert.AreEqual(token.Item1, TokenType.LP);
            SkipToken(token.Item2);

            token = PeekToken();
            Assert.AreEqual(token.Item1, TokenType.ID);
            expr.Name = code.Substring(0, token.Item2);
            SkipToken(token.Item2);

            while (true)
            {
                token = PeekToken();
                switch (token.Item1)
                {
                    case TokenType.LP:
                        expr.Params.Add(ParseExpr());
                        break;

                    case TokenType.ID:
                        expr.Params.Add(code.Substring(0, token.Item2));
                        SkipToken(token.Item2);
                        break;

                    case TokenType.RP:
                        SkipToken(token.Item2);
                        return expr;
                }
            }
        }

        private (TokenType, int) PeekToken()
        {
            code = code.Trim();
            if (code.Length == 0)
            {
                return (TokenType.END, 0);
            }
            if (code[0] == '(')
            {
                return (TokenType.LP, 1);
            }
            if (code[0] == ')')
            {
                return (TokenType.RP, 1);
            }
            int i = 0;
            for (; i < code.Length; i++)
            {
                if (char.IsWhiteSpace(code[i]) || code[i] == '(' || code[i] == ')')
                {
                    break;
                }
            }
            return (TokenType.ID, i);
        }

        private void SkipToken(int n)
        {
            code = code.Substring(n);
        }
    }
}