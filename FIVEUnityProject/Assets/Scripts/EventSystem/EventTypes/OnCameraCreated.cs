using System;
using UnityEngine;

namespace FIVE.EventSystem
{
    public abstract class OnCameraCreated : IEventType<OnCameraCreatedArgs>
    {
    }

    public sealed class OnCameraCreatedArgs : EventArgs
    {
        public string Id { get; }
        public Camera Camera { get; }

        public OnCameraCreatedArgs(string id, Camera camera)
        {
            Id = id;
            Camera = camera;
        }
    }
}