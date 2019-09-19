using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(RobotSphere))]
    internal class AwslScript
    {
        private enum Instruction
        { Forward, Backward, Left, Right, Goto }

        private readonly List<Tuple<Instruction, int>> program;
        private int ip; // instruction pointer

        public string content;
        public int index;
        internal Stmts rootStmts;

        internal AwslScript(string script)
        {
            content = script.Trim();
            index = 0;

            program = new List<Tuple<Instruction, int>>();
            ip = 0;

            Parse();
        }

        internal void Execute(GameObject gameObject)
        {
            CharacterController cc = gameObject.GetComponent<CharacterController>();
            RobotSphere robotSphere = gameObject.GetComponent<RobotSphere>();
            if (ip < program.Count)
            {
                Tuple<Instruction, int> instruction = program[ip];

                robotSphere.currState = RobotSphere.RobotState.Idle;

                if (instruction.Item1 == Instruction.Forward)
                {
                    Debug.Log("Forward");
                    cc.SimpleMove(gameObject.transform.forward * 5.0f);
                    robotSphere.currState = RobotSphere.RobotState.Walk;
                }
                else if (instruction.Item1 == Instruction.Backward)
                {
                    cc.SimpleMove(-gameObject.transform.forward * 5.0f);
                    robotSphere.currState = RobotSphere.RobotState.Walk;
                }
                else if (instruction.Item1 == Instruction.Left)
                {
                    gameObject.transform.Rotate(new Vector3(0, 360 - instruction.Item2, 0));
                }
                else if (instruction.Item1 == Instruction.Right)
                {
                    gameObject.transform.Rotate(new Vector3(0, instruction.Item2, 0));
                }
                ip++;
            }
            else
            {
                robotSphere.scriptActive = false;
            }
        }

        internal void Parse()
        {
            while (index < content.Length)
            {
                ParseStmt();
            }
        }

        private void ParseStmt()
        {
            SkipSpace();
            if (content[index] != '(')
            {
                // ERROR
            }
            index++;

            string word = GetWord();
            index += word.Length;
            SkipSpace();
            string data = GetWord();
            index += data.Length;
            if (word == "forward")
            {
                int steps = Convert.ToInt32(data) * 50;
                for (int i = 0; i < steps; i++)
                {
                    program.Add(new Tuple<Instruction, int>(Instruction.Forward, 1));
                }
            }
            else if (word == "backward")
            {
                int steps = Convert.ToInt32(data) * 50;
                for (int i = 0; i < steps; i++)
                {
                    program.Add(new Tuple<Instruction, int>(Instruction.Backward, 1));
                }
            }
            else if (word == "left")
            {
                int steps = Convert.ToInt32(data);
                for (int i = 0; i < steps / 3; i++)
                {
                    program.Add(new Tuple<Instruction, int>(Instruction.Left, 3));
                }
                program.Add(new Tuple<Instruction, int>(Instruction.Left, steps % 3));
            }
            else if (word == "right")
            {
                int steps = Convert.ToInt32(data);
                for (int i = 0; i < steps / 3; i++)
                {
                    program.Add(new Tuple<Instruction, int>(Instruction.Right, 3));
                }
                program.Add(new Tuple<Instruction, int>(Instruction.Right, steps % 3));
            }

            SkipSpace();
            if (content[index] != ')')
            {
                // ERROR
            }
            index++;
        }

        internal void SkipSpace()
        {
            while (index < content.Length && (content[index] == ' ' || content[index] == '\n' || content[index] == '\t'))
            {
                ++index;
            }
        }

        // Return the first word ending in space
        internal string GetWord()
        {
            int endIndex = index;
            while (endIndex < content.Length && !Constants.AwslDelimiter.Contains(content[endIndex]))
            {
                endIndex++;
            }
            return content.Substring(index, endIndex - index);
        }
    }
}