using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(CharacterController))]
    public class Movable : MonoBehaviour
    {
        public enum Move { Front = 0, Back = 1, Left = 2, Right = 3, };

        public Vector3 MoveTarget
        {
            get; set;
        }

        public float MoveSpeed
        {
            get; set;
        }

        public float RotateTarget
        {
            get; set;
        }

        public float RotateSpeed
        {
            get; set;
        }

        public Queue<Move> Moves
        {
            get; private set;
        }

        public delegate void MoveOnce(int steps);
        public MoveOnce[] MoveOnces
        {
            get;
            private set;
        }

        private Vector3 rotate = Vector3.zero;

        private CharacterController cc;

        private void Start()
        {
            cc = GetComponent<CharacterController>();
            MoveSpeed = 15.0f;
            RotateSpeed = 30.0f;

            Moves = new Queue<Move>();
            MoveOnces = new MoveOnce[4] { Forward, Backward, TurnLeft, TurnRight, };
        }

        private void Update()
        {
            if (Moves.Count > 0)
            {
                Move move = Moves.Dequeue();
                MoveOnces[(int)move](1);
            }
            //if (MoveTarget != null && Vector3.Distance(transform.position, MoveTarget) > 0.5f)
            //{
            //    cc.SimpleMove(transform.forward * MoveSpeed * Time.deltaTime);
            //}
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
            Debug.Log("Turn Left");
            gameObject.transform.Rotate(0, -steps, 0);
        }

        public void TurnRight(int steps)
        {
            Debug.Log("Turn Right");
            gameObject.transform.Rotate(0, steps, 0);
        }
    }
}