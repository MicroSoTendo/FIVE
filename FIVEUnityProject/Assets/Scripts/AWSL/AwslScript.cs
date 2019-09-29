using FIVE.Robot;
using System;
using UnityEngine;

namespace FIVE.AWSL
{
    [RequireComponent(typeof(RobotSphere))]
    internal class AWSLScript
    {
        private readonly RuntimeContext rc;

        internal AWSLScript(RobotSphere robot, string code)
        {
            try
            {
                rc = new Parser(code).Parse();
                rc.Robot = robot;
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
                return rc.Execute();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return true;
            }
        }
    }
}