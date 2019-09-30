using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(CharacterController))]
    public class Movable : MonoBehaviour
    {
        public enum Move { Front = 0, Back = 1, Left = 2, Right = 3, };

        public Vector3 MoveTarget;

        public float MoveSpeed;

        public float RotateTarget;

        public float RotateSpeed;

        public readonly Queue<Move> Moves = new Queue<Move>();

        public delegate void MoveOnce(int steps);

        public MoveOnce[] MoveOnces
        {
            get;
            private set;
        }

        private CharacterController cc;

        private void Start()
        {
            cc = GetComponent<CharacterController>();
            MoveSpeed = 15.0f;
            RotateSpeed = 30.0f;

            MoveOnces = new MoveOnce[4] { Forward, Backward, TurnLeft, TurnRight, };
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
            cc.SimpleMove(gameObject.transform.forward * MoveSpeed);
        }

        public void Backward(int steps)
        {
            cc.SimpleMove(-gameObject.transform.forward * MoveSpeed);
        }

        public void TurnLeft(int steps)
        {
            gameObject.transform.Rotate(0, -steps, 0);
        }

        public void TurnRight(int steps)
        {
            gameObject.transform.Rotate(0, steps, 0);
        }
    }
}