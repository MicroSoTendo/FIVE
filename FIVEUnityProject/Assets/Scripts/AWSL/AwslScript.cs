using FIVE.Robot;
using MoonSharp.Interpreter;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace FIVE.AWSL
{
    internal class AWSLScript
    {
        private readonly DynValue coroutine;
        private readonly RobotSphere robot;

        private readonly Script script;

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
                script.Globals["nearestEnemy"] = FuncNearestEnemy();

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
                Physics.SphereCast(robot.transform.position + Vector3.up * 0.005f, 0.05f, robot.transform.forward,
                    out RaycastHit hitinfo);
                script.Globals["DISTANCE"] = hitinfo.collider ? hitinfo.distance : 1e7f;

                DynValue result = coroutine.Coroutine.Resume();
                return result.Type != DataType.YieldRequest;
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

        private Func<GameObject> FuncNearestEnemy()
        {
            return () =>
            {
                GameObject nearestEnemy = EnemyManager.Instance().Enemies[0];
                float nearestDistance = Vector3.Distance(nearestEnemy.transform.position, robot.gameObject.transform.position);

                foreach (GameObject enemy in EnemyManager.Instance().Enemies)
                {
                    float distance = Vector3.Distance(enemy.transform.position, robot.gameObject.transform.position);
                    if (Vector3.Distance(enemy.transform.position, robot.gameObject.transform.position) < distance)
                    {
                        nearestEnemy = enemy;
                        nearestDistance = distance;
                    }
                }

                return nearestEnemy;
            };
        }

        private Func<GameObject> FuncNearestBattery()
        {
            return () =>
            {
                GameObject nearestBattery = BatteryManager.Instance().Batteries[0];
                float nearestDistance = Vector3.Distance(nearestBattery.transform.position, robot.gameObject.transform.position);

                foreach (GameObject battery in BatteryManager.Instance().Batteries)
                {
                    float distance = Vector3.Distance(battery.transform.position, robot.gameObject.transform.position);
                    if (Vector3.Distance(battery.transform.position, robot.gameObject.transform.position) < distance)
                    {
                        nearestBattery = battery;
                        nearestDistance = distance;
                    }
                }

                return nearestBattery;
            };
        }
    }
}