using FIVE.Interactive;
using FIVE.Robot;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.AWSL
{
    internal class RobotProxy
    {
        private RobotSphere robot;

        [MoonSharpHidden]
        public RobotProxy(RobotSphere o)
        {
            robot = o;
        }

        public int ID => robot.ID;

        public Dictionary<string, int> Items
        {
            get
            {
                var d = new Dictionary<string, int>();
                foreach (GameObject p in InventoryManager.Inventory.Items)
                {
                    if (d.ContainsKey(p.name))
                    {
                        d[p.name] += 1;
                    }
                    else
                    {
                        d[p.name] = 1;
                    }
                }
                return d;
            }
        }
    }
}