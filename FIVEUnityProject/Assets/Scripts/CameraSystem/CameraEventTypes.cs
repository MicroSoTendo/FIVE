using System;
using FIVE.EventSystem;
using UnityEngine;

namespace FIVE.CameraSystem
{
    public abstract class OnCameraCreated : IEventType<CameraCreatedEventArgs> { }

    public sealed class CameraCreatedEventArgs : EventArgs
    {
        public string Id { get; }
        public Camera Camera { get; }

        public CameraCreatedEventArgs(string id, Camera camera)
        {
            Id = id;
            Camera = camera;
        }
    }

    public abstract class OnCameraSwitched : IEventType<CameraSwitchedEventArgs>
    {
    }

    public sealed class CameraSwitchedEventArgs : EventArgs
    {
        public Camera OldCamera { get; }
        public Camera NewCamera { get; }

        public CameraSwitchedEventArgs(Camera newCamera, Camera oldCamera = null)
        {
            OldCamera = oldCamera;
            NewCamera = newCamera;
        }
    }

    public abstract class OnMultiCameraModeRequested : IEventType { }
}