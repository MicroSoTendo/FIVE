using System;

namespace FIVE.Network
{

    [Flags]
    public enum SyncHeader
    {
        NetworkCall = 1 << 1,
        ComponentSync = 1 << 2,
    }

    public enum NetworkCall
    {
        CreateObject = 0,
        RemoveObject = 1,
    }

    public enum ComponentType
    {
        Transform = 0,
        Animator = 1,
    }

}
