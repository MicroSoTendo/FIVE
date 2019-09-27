using FIVE.Robot;
using System;
using System.Collections.Generic;

namespace FIVE.AWSL
{
    using FuncT = Func<RuntimeContext, List<string>, string>;
    using ParamList = List<string>;

    internal class Expr
    {
        private static readonly Dictionary<string, FuncT> Funcs = new Dictionary<string, FuncT>
        {
            { "forward", FuncForward },
            { "backward", FuncBackward },
            { "left", FuncLeft },
            { "right", FuncRight },
            { "goto", FuncGoto },
            { "+", FuncAdd },
            { "-", FuncSub },
            { "*", FuncMul },
            { "/", FuncDiv },
        };

        public string Name;
        public List<object> Params = new List<object>();

        public string Execute(RuntimeContext rc)
        {
            ParamList actualParams = new List<string>();
            for (int ParamP = 0; ParamP < Params.Count; ParamP++)
            {
                if (Params[ParamP] is string)
                {
                    actualParams.Add(Params[ParamP] as string);
                }
                else if (Params[ParamP] is Expr)
                {
                    actualParams.Add((Params[ParamP] as Expr).Execute(rc));
                }
            }

            if (Funcs.ContainsKey(Name))
            {
                return Funcs[Name](rc, actualParams);
            }
            else
            {
                return null;
            }
        }

        private static string FuncForward(RuntimeContext rc, ParamList args)
        {
            rc.Robot.GetComponent<RobotSphere>().Move(Movable.Move.Front, (int)float.Parse(args[0]), true);
            return null;
        }

        private static string FuncBackward(RuntimeContext rc, ParamList args)
        {
            rc.Robot.GetComponent<RobotSphere>().Move(Movable.Move.Back, (int)float.Parse(args[0]), true);
            return null;
        }

        private static string FuncLeft(RuntimeContext rc, ParamList args)
        {
            rc.Robot.GetComponent<RobotSphere>().Move(Movable.Move.Left, (int)float.Parse(args[0]), true);
            return null;
        }

        private static string FuncRight(RuntimeContext rc, ParamList args)
        {
            rc.Robot.GetComponent<RobotSphere>().Move(Movable.Move.Right, (int)float.Parse(args[0]), true);
            return null;
        }

        private static string FuncGoto(RuntimeContext rc, ParamList args)
        {
            rc.ExprP = int.Parse(args[0]);
            return null;
        }

        private static string FuncAdd(RuntimeContext _, ParamList args)
        {
            float a = float.Parse(args[0]);
            float b = float.Parse(args[1]);
            return (a + b).ToString();
        }

        private static string FuncSub(RuntimeContext _, ParamList args)
        {
            float a = float.Parse(args[0]);
            float b = float.Parse(args[1]);
            return (a - b).ToString();
        }

        private static string FuncMul(RuntimeContext _, ParamList args)
        {
            float a = float.Parse(args[0]);
            float b = float.Parse(args[1]);
            return (a * b).ToString();
        }

        private static string FuncDiv(RuntimeContext _, ParamList args)
        {
            float a = float.Parse(args[0]);
            float b = float.Parse(args[1]);
            return (a / b).ToString();
        }
    }
}