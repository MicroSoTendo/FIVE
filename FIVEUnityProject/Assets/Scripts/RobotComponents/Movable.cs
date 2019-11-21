using FIVE.RobotComponents;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(CharacterController))]
    public class Movable : MonoBehaviour
    {
        public delegate void MoveOnce(int steps);

        public enum Move { Front = 0, Back = 1, Left = 2, Right = 3, };

        public readonly Queue<Move> Moves = new Queue<Move>();

        private CharacterController cc;

        public float MoveSpeed;
        public Vector3 MoveTarget;
        public float RotateSpeed;
        public float RotateTarget;

        public MoveOnce[] MoveOnces { get; private set; }

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
            MoveOnces = new MoveOnce[4] { Forward, Backward, TurnLeft, TurnRight };
            MoveSpeed = 0.15f;
            RotateSpeed = 30.0f;
        }

        private void Update()
        {
            if (Moves.Count > 0)
            {
                Move move = Moves.Dequeue();
                MoveOnces[(int)move](1);
            }
        }

        public void ClearSchedule()
        {
            Moves.Clear();
        }

        public void ScheduleMove(Move move, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                Moves.Enqueue(move);
            }
        }

        public void Forward(int steps)
        {
            if (ConsumePower())
            {
                cc.Move(gameObject.transform.forward * MoveSpeed);
            }
        }

        public void Backward(int steps)
        {
            if (ConsumePower())
            {
                cc.Move(-gameObject.transform.forward * MoveSpeed);
            }
        }

        public void TurnLeft(int steps)
        {
            if (ConsumePower())
            {
                gameObject.transform.Rotate(0, -steps, 0);
            }
        }

        public void TurnRight(int steps)
        {
            if (ConsumePower())
            {
                gameObject.transform.Rotate(0, steps, 0);
            }
        }

        private bool ConsumePower(float n = 0.5f)
        {
            Battery battery = GetComponent<Battery>();
            if (battery.CurrentEnergy > 0f)
            {
                battery.CurrentEnergy -= n * Time.deltaTime;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}