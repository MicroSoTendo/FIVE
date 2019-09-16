using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    internal class Tokenizer
    {
        List<Token> tokens;
        Dictionary<State, Dictionary<char, State>> fsa;

        private string program;
        private int index;
        private Token currToken;

        public struct Token
        {
            public string Word;
            public TokenKind TokenKind;
        }

        enum State
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

        internal Tokenizer(string program)
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

        void readToken()
        {
            State state = State.Start;
            string token;

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

                if (fsa.ContainsKey(state))
                {
                    State next;
                }
            }
        }

        void skipToken()
        {
            GetToken();
            currToken = new Token() { Word = "", TokenKind = TokenKind.ERR };
        }

        internal Token GetToken()
        {
            if (currToken.TokenKind == TokenKind.ERR)
            {
                return currToken;
            }
            else
            {
                readToken();
                return currToken;
            }
        }
    }
}
