using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(RobotSphere))]
    internal class AwslScript
    {
        enum Instruction { Forward, Backward, Left, Right }

        List<Tuple<Instruction, int>> program;
        int ip; // instruction pointer

        public string content;
        public int index;
        internal Stmts rootStmts;

        internal AwslScript(string script)
        {
            content = script;
            index = 0;

            program = new List<Tuple<Instruction, int>>();
            ip = 0;

            parse();
        }

        internal void execute(GameObject gameObject)
        {
            CharacterController cc = gameObject.GetComponent<CharacterController>();
            RobotSphere robotSphere = gameObject.GetComponent<RobotSphere>(); 
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
                gameObject.transform.Rotate(new Vector3(0, instruction.Item2, 0));
            }
            else if (instruction.Item1 == Instruction.Right)
            {
                gameObject.transform.Rotate(new Vector3(0, 360 - instruction.Item2, 0));
            }
            ip++;
            if (ip == program.Count)
            {
                ip = 0;
            }
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
            skipSpace();
            if (content[index] != '(')
            {
                // ERROR
            }
            index++;

            string word = getWord();
            index += word.Length;
            skipSpace();
            string data = getWord();
            index += data.Length;
            if (word == "forward")
            {
                int steps = Convert.ToInt32(data);
                for (int i = 0; i < steps; i++)
                {
                    program.Add(new Tuple<Instruction, int>(Instruction.Forward, 1));
                }
            }
            else if (word == "backward")
            {
                int steps = Convert.ToInt32(data);
                for (int i = 0; i < steps; i++)
                {
                    program.Add(new Tuple<Instruction, int>(Instruction.Backward, 1));
                }
            }
            else if (word == "left")
            {
                program.Add(new Tuple<Instruction, int>(Instruction.Left, Convert.ToInt32(data)));
            }
            else if (word == "right")
            {
                program.Add(new Tuple<Instruction, int>(Instruction.Right, Convert.ToInt32(data)));
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
            while (index < content.Length && (content[index] == ' ' || content[index] == '\n' || content[index] == '\t'))
            {
                ++index;
            }
        }

        // Return the first word ending in space
        internal string getWord()
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
