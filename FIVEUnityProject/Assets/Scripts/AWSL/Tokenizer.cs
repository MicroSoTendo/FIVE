using System;
using System.Collections.Generic;

namespace FIVE
{
    internal class Tokenizer
    {
        private readonly List<Token> tokens;
        private readonly Dictionary<State, Dictionary<char, State>> fsa;

        private readonly string program;
        private int index;
        private Token currToken;

        public struct Token
        {
            public string Word;
            public TokenKind TokenKind;
        }

        private enum State
        {
            Start, Error, End,
            LP, // Left Parenthesis
            RP, // right Parenthesis
            Identifier,
        }

        public enum TokenKind
        {
            LP, RP, ID, ERR, END,
        }

        public Tokenizer(string program)
        {
            fsa = new Dictionary<State, Dictionary<char, State>>();
            foreach (State state in (State[])Enum.GetValues(typeof(State)))
            {
                foreach (char c in Constants.AwslCharset)
                {
                    fsa[state][c] = State.End;
                }
            }

            fsa[State.Start]['('] = State.LP;
            fsa[State.Start][')'] = State.RP;

            for (char c = 'a'; c <= 'z'; c++)
            {
                fsa[State.Start][c] = State.Identifier;
            }

            this.program = program;
            index = 0;
            currToken = new Token() { Word = "", TokenKind = TokenKind.ERR };
        }

        private void ReadToken()
        {
            State state = State.Start;
            string token = "";

            while (program[index] == ' ')
            {
                ++index;
            }

            if (index == program.Length - 1)
            {
                currToken = new Token() { Word = "", TokenKind = TokenKind.ERR };
                return;
            }

            while (true)
            {
                char ch = program[index];
                State next;

                if (fsa.ContainsKey(state) && fsa[state].ContainsKey(ch))
                {
                    next = fsa[state][ch];
                }
                else
                {
                    next = State.Error;
                }

                if (next == State.End)
                {
                    currToken = new Token() { Word = token, TokenKind = TokenKind.ID };
                }
            }
        }

        private void SkipToken()
        {
            GetToken();
            currToken = new Token() { Word = "", TokenKind = TokenKind.ERR };
        }

        private Token GetToken()
        {
            if (currToken.TokenKind != TokenKind.ERR)
            {
                return currToken;
            }
            else
            {
                ReadToken();
                return currToken;
            }
        }
    }
}