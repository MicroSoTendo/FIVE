namespace FIVE.Network.Core
{

    /// <summary>
    /// Used for commnuicating between Host and Client.
    /// </summary>
    public enum GameSyncHeader : ushort
    {   
        /// <summary>
        /// 0 is never allowed.
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Used by <b>Client</b> only.<br/>
        /// Sent to host for requesting joining room.
        /// </summary>
        JoinRequest = 1,
        /// <summary>
        /// Used by <b>Host</b> only.<br/>
        /// Sent to client for accept join, otherwise refuse join.
        /// </summary>
        AcceptJoin = 2,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Make sure connection is still alive.
        /// </summary>
        AliveTick = 3,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Start a time stamp.
        /// </summary>
        StampBegin = 4,        
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// End a time stamp.
        /// </summary>
        StampEnd = 5,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Request creating object(s).
        /// </summary>
        CreateObject = 6,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Request removing object(s).
        /// </summary>
        RemoveObject = 7,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Indicating whether components need syncing.
        /// </summary>
        ComponentSync = 8,
        /// <summary>
        /// Can be used by both <b>Client</b> and <b>Host</b>.<br/>
        /// Indicating whether remote call(s) exist.
        /// </summary>
        RemoteCall = 9
    }
}
