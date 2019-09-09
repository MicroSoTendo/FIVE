using System;

namespace FIVE.EventSystem.EventTypes
{
    public delegate void LauncherEventHandler(Launcher launcher, EventArgs args);
    public class OnLauncherAwake : IEventType { }
    public class OnLauncherStart : IEventType { }
    public class OnLauncherUpdate : IEventType { }
}