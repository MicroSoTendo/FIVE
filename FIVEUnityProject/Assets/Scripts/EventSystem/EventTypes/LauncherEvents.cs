using System;

namespace FIVE.EventSystem.EventTypes
{
    public delegate void LauncherEventHandler(MainLoader launcher, EventArgs args);
    public class OnLauncherAwake : IEventType { }
    public class OnLauncherStart : IEventType { }
    public class OnLauncherDestroyed : IEventType { }
}