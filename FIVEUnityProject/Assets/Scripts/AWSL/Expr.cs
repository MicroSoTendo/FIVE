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
            { "forward", FuncMove(Movable.Move.Front) },
            { "backward", FuncMove(Movable.Move.Back) },
            { "left", FuncMove(Movable.Move.Left) },
            { "right", FuncMove(Movable.Move.Right) },
            { "+", FuncOp((a,b)=>a+b) },
            { "-", FuncOp((a,b)=>a-b) },
            { "*", FuncOp((a,b)=>a*b) },
            { "/", FuncOp((a,b)=>a/b) },
            { "goto", FuncGoto },
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

        private static FuncT FuncMove(Movable.Move dir)
        {
            return (rc, args) =>
            {
                rc.Robot.GetComponent<RobotSphere>().Move(dir, (int)float.Parse(args[0]), true);
                return null;
            };
        }

        private static FuncT FuncOp(Func<float, float, float> op)
        {
            return (_, args) =>
            {
                float a = float.Parse(args[0]);
                float b = float.Parse(args[1]);
                return op(a, b).ToString();
            };
        }

        private static string FuncGoto(RuntimeContext rc, ParamList args)
        {
            rc.ExprP = int.Parse(args[0]);
            return null;
        }
    }
}