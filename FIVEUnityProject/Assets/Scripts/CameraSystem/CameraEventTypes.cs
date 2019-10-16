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

        public CameraSwitchedEventArgs(Camera oldCamera = null, Camera activeCamera = null)
        {
            OldCamera = oldCamera;
            NewCamera = NewCamera;
        }
    }

    public abstract class OnSwitchCameraModeRequested : IEventType { }

    public class SwitchCameraModeRequestedEventArgs : EventArgs
    {
        public SwitchCameraModeRequestedEventArgs(CameraMode mode)
        {
            Mode = mode;
        }

        public enum CameraMode
        {
            Single,
            Multiple
        }

        public  CameraMode Mode { get; }
        
    }
}