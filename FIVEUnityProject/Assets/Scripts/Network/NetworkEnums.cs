using System;

namespace FIVE.Network
{

    /// <summary>
    /// Used for commnuicating between Host and Client.
    /// </summary>
    [Flags]
    public enum GameSyncHeader : ushort
    {
        /// <summary>
        /// Used by <b>Client</b> only.<br/>
        /// Sent to host for requesting joining room.
        /// </summary>
        JoinRequest = 1,
        /// <summary>
        /// Used by <b>Host</b> only.<br/>
        /// Sent to client for accept join, otherwise refuse join.
        /// </summary>
        AcceptJoin = 1 << 1,
        /// <summary>
        /// Used by <b>Client</b> only.<br/>
        /// Sent by host to confirm connection is alive.
        /// </summary>
        AliveTick = 1 << 1,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Request creating object(s).
        /// </summary>
        CreateObject = 1 << 2,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Request removing object(s).
        /// </summary>
        RemoveObject = 1 << 3,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Indicating whether components need syncing.
        /// </summary>
        ComponentSync = 1 << 4,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Indicating whether remote call(s) exist.
        /// </summary>
        RemoteCall = 1 << 5
    }

    public enum ComponentType : byte
    {
        Transform = 0,
        Animator = 1,
    }

}
