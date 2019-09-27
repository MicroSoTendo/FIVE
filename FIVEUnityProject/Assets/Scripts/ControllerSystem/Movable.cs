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

        public delegate void MoveOnce();
        public MoveOnce[] MoveOnces
        {
            get;
            private set;
        }

        private Vector3 rotate = Vector3.zero;

        private Queue<Move> moves;
        private CharacterController cc;

        private void Start()
        {
            cc = GetComponent<CharacterController>();
            MoveSpeed = 15.0f;
            RotateSpeed = 30.0f;

            moves = new Queue<Move>();
            MoveOnces = new MoveOnce[4] { Forward, Backward, TurnLeft, TurnRight, };
        }

        private void Update()
        {
            if (moves.Count > 0)
            {
                Move move = moves.Dequeue();
                MoveOnces[(int)move]();
            }
            //if (MoveTarget != null && Vector3.Distance(transform.position, MoveTarget) > 0.5f)
            //{
            //    cc.SimpleMove(transform.forward * MoveSpeed * Time.deltaTime);
            //}
        }

        public void ScheduleMove(Move move, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                moves.Enqueue(move);
            }
        }

        public void Forward()
        {
            cc.SimpleMove(gameObject.transform.forward * MoveSpeed);
        }

        public void Backward()
        {
            cc.SimpleMove(-gameObject.transform.forward * MoveSpeed);
        }

        public void TurnLeft()
        {
            Debug.Log("Turn Left");
            gameObject.transform.Rotate(0, -5, 0);
        }

        public void TurnRight()
        {
            Debug.Log("Turn Right");
            gameObject.transform.Rotate(0, 5, 0);
        }
    }
}