using System;

namespace FIVE.Network
{
    /// <summary>
    /// Used for communicating with list server.
    /// </summary>
    [Flags]
    public enum ListServerCode
    {
        AliveTick = 1,
        GetRoomInfos = 1 << 1,
        CreateRoom = 1 << 2,
        RemoveRoom = 1 << 3,
        UpdateRoom = 1 << 4,
        UpdateName = 1 << 5,
        UpdateCurrentPlayer = 1 << 6,
        UpdateMaxPlayer = 1 << 7,
        UpdatePassword = 1 << 8
    }

    /// <summary>
    /// Used for commnuicating between Host and Client.
    /// </summary>
    [Flags]
    public enum GameSyncCode
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
        /// Indicating whether creating or removeing is global or local.
        /// </summary>
        GlobalOperation = 1 << 4,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Indicating whether creating or removeing is single or multiple.
        /// </summary>
        MultipleObjects = 1 << 5,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Indicating whether components need syncing.
        /// </summary>
        ComponentSync = 1 << 6,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Indicating whether remote call(s) exist.
        /// </summary>
        RemoteCall = 1 << 7,
    }
    
    public enum ComponentType : byte
    {
        Transform = 0,
        Animator = 1,
    }

}
