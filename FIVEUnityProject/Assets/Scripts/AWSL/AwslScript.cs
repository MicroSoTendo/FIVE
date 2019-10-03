using FIVE.Robot;
using MoonSharp.Interpreter;
using System;
using UnityEngine;

namespace FIVE.AWSL
{
    [RequireComponent(typeof(RobotSphere))]
    internal class AWSLScript
    {
        private readonly RobotSphere robot;

        private readonly Script script;
        private readonly DynValue coroutine;

        internal AWSLScript(RobotSphere robot, string code)
        {
            this.robot = robot;

            try
            {
                script = new Script(CoreModules.None);
                script.DoString(code);
                coroutine = script.CreateCoroutine(script.Globals.Get("main"));

                script.Globals["print"] = (Action<DynValue>)(x => Debug.Log(x));
                script.Globals["forward"] = FuncMove(Movable.Move.Front);
                script.Globals["backward"] = FuncMove(Movable.Move.Back);
                script.Globals["left"] = FuncMove(Movable.Move.Left);
                script.Globals["right"] = FuncMove(Movable.Move.Right);

                coroutine.Coroutine.AutoYieldCounter = 4 * robot.CPU.Speed;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal bool Execute()
        {
            try
            {
                Physics.SphereCast(robot.transform.position + Vector3.up * 0.005f, 0.05f, robot.transform.forward, out RaycastHit hitinfo);
                script.Globals["DISTANCE"] = hitinfo.collider ? hitinfo.distance : 1e7f;

                DynValue result = coroutine.Coroutine.Resume();
                if (result.Type == DataType.Void)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return true;
        }

        private Action<float> FuncMove(Movable.Move dir)
        {
            return x => robot.GetComponent<RobotSphere>().Move(dir, (int)x, true);
        }
    }
}