using FIVE.Robot;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.AWSL
{
    using FuncT = Func<RuntimeContext, List<object>, object>;
    using ParamList = List<object>;

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
            { "==", FuncOp((a,b)=>a==b) },
            { "<", FuncOp((a,b)=>a<b) },
            { ">", FuncOp((a,b)=>a>b) },
            { "<=", FuncOp((a,b)=>a<=b) },
            { ">=", FuncOp((a,b)=>a>=b) },
            { "print", FuncPrint },
            { "goto", FuncGoto },
            { "if", FuncIf },
            { "do", FuncDo },
            { "v", FuncVar },
            { "var", FuncVar },
            { "defun", FuncDefun },
            { "call", FuncCall },
        };

        public string Name;
        public ParamList Params = new ParamList();

        public object Execute(RuntimeContext rc)
        {
            return Funcs.ContainsKey(Name) ? Funcs[Name](rc, Params) : null;
        }

        private static FuncT FuncMove(Movable.Move dir)
        {
            return (rc, args) =>
            {
                rc.Robot.GetComponent<RobotSphere>().Move(dir, EvalInt(rc, args[0]), true);
                return null;
            };
        }

        private static FuncT FuncOp<T>(Func<float, float, T> op)
        {
            return (rc, args) =>
            {
                return op(EvalFloat(rc, args[0]), EvalFloat(rc, args[1]));
            };
        }

        private static object FuncGoto(RuntimeContext rc, ParamList args)
        {
            rc.ExprP = EvalInt(rc, args[0]);
            return null;
        }

        private static object FuncIf(RuntimeContext rc, ParamList args)
        {
            return EvalBool(rc, args[0]) ? Eval(rc, args[1]) : Eval(rc, args[2]);
        }

        private static object FuncDo(RuntimeContext rc, ParamList args)
        {
            if (args.Count > 0)
            {
                for (int i = 0; i < args.Count - 1; i++)
                {
                    Eval(rc, args[i]);
                }
                return Eval(rc, args[args.Count - 1]);
            }
            else
            {
                return null;
            }
        }

        private static object FuncVar(RuntimeContext rc, ParamList args)
        {
            if (args.Count >= 2)
            {
                string k = EvalString(rc, args[0]);
                object v = Eval(rc, args[1]);
                rc.Vars[k] = v;
                return v;
            }
            else
            {
                return rc.Vars[EvalString(rc, args[0])];
            }
        }

        private static object FuncDefun(RuntimeContext rc, ParamList args)
        {
            if (args[1] is Expr a)
            {
                rc.Funcs[EvalString(rc, args[0])] = a;
            }
            return null;
        }

        private static object FuncCall(RuntimeContext rc, ParamList args)
        {
            return rc.Funcs[EvalString(rc, args[0])].Execute(rc);
        }

        private static object FuncPrint(RuntimeContext rc, ParamList args)
        {
            string s = "AWSL Script: ";
            foreach (object a in args)
            {
                s += EvalString(rc, a) + " ";
            }
            Debug.Log(s);
            return null;
        }

        private static object Eval(RuntimeContext rc, object arg)
        {
            return arg is Expr a ? a.Execute(rc) : arg;
        }

        private static float EvalFloat(RuntimeContext rc, object arg)
        {
            arg = Eval(rc, arg);
            switch (arg)
            {
                case string a:
                    return float.Parse(a);

                case float a:
                    return a;

                case int a:
                    return a;

                case null:
                    return 0;

                default:
                    return 0;
            }
        }

        private static int EvalInt(RuntimeContext rc, object arg)
        {
            return (int)EvalFloat(rc, arg);
        }

        private static bool EvalBool(RuntimeContext rc, object arg)
        {
            arg = Eval(rc, arg);
            if (arg is bool a)
            {
                return a;
            }
            else
            {
                throw new Exception("Cannot cast to bool");
            }
        }

        private static string EvalString(RuntimeContext rc, object arg)
        {
            arg = Eval(rc, arg);
            return arg is string a ? a : arg.ToString();
        }
    }
}