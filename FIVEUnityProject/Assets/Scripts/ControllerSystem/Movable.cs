using System.Collections;
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

        private Queue<Move> moves;
        private CharacterController cc;

        void Start()
        {
            cc = GetComponent<CharacterController>();
            MoveSpeed = 5.0f;
            RotateSpeed = 30.0f;

            moves = new Queue<Move>();
        }

        void FixedUpdate()
        {
            if (moves.Count > 0)
            {
                Move move = moves.Dequeue();
            }
            //if (MoveTarget != null && Vector3.Distance(transform.position, MoveTarget) > 0.5f)
            //{
            //    cc.SimpleMove(transform.forward * MoveSpeed * Time.deltaTime);
            //}
        }

        public void MoveStep(Move move, int steps)
        {
            //if (move == Move.Front || move == Move.Back)
            //{
            for (int i = 0; i < steps; i++)
            {
                moves.Enqueue(move);
            }
            //}
        }

        private void forward()
        {
            cc.SimpleMove(gameObject.transform.forward * MoveSpeed);
        }

        private void backward()
        {
            cc.SimpleMove(gameObject.transform.forward * MoveSpeed);
        }

        private void turnLeft()
        {
            Quaternion target = Quaternion.Euler(0, -1, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5.0f);
        }

        private void turnRight()
        {
            Quaternion target = Quaternion.Euler(0, 1, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5.0f);
        }
    }
}