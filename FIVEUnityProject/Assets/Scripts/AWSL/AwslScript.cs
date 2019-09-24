using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FIVE.Robot;

namespace FIVE.AWSL
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
        internal Stmts rootStmts = null;

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

                if (instruction.Item1 == Instruction.Forward)
                {
                    robotSphere.Move(Movable.Move.Front, 1, true);
                    //robotSphere.currState = RobotSphere.RobotState.Walk;
                    //cc.SimpleMove(gameObject.transform.forward * 5.0f);
                }
                else if (instruction.Item1 == Instruction.Backward)
                {
                    robotSphere.Move(Movable.Move.Front, 1, true);
                    //robotSphere.currState = RobotSphere.RobotState.Walk;
                    //cc.SimpleMove(-gameObject.transform.forward * 5.0f);
                }
                else if (instruction.Item1 == Instruction.Left)
                {
                    robotSphere.Move(Movable.Move.Left, 1, true);
                    //gameObject.transform.Rotate(new Vector3(0, 360 - instruction.Item2, 0));
                }
                else if (instruction.Item1 == Instruction.Right)
                {
                    robotSphere.Move(Movable.Move.Right, 1, true);
                    //gameObject.transform.Rotate(new Vector3(0, instruction.Item2, 0));
                }
                else if (instruction.Item1 == Instruction.Goto)
                {
                    ip = instruction.Item2;
                    ip--;
                }
                ip++;
            }
            else
            {
                robotSphere.currState = RobotSphere.RobotState.Idle;
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
            else if (word == "goto")
            {
                program.Add(new Tuple<Instruction, int>(Instruction.Goto, Convert.ToInt32(data)));
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