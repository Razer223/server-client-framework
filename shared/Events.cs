#define SERVER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerFramework;

/// <summary>Events for the Network system</summary>
public class NetworkEvents {
    /// <summary>Create new OnHandShakeEndEvent event</summary>
    public class BaseEventClass {
        /// <summary>Event name. Same as class name</summary>
        public string? EventName { get; internal set; }
        /// <summary></summary>
        public BaseEventClass() {
            EventName = this.GetType().UnderlyingSystemType.Name;
        }
    }
    /// <summary>Create new OnHandShakeEndEvent event</summary>
    public class OnClientConnectEvent : BaseEventClass {
        /// <summary>ID Of the client that connected</summary>
        public int? ClientID { get; set; }
        /// <summary>User name of the client that connected</summary>
        public string? UserName { get; set; }
        /// <summary>Check if the connection was success</summary>
        public bool? Success { get; set; } = true;
        /// <summary></summary>
        public OnClientConnectEvent (int? id, string? username, bool? success = false) {
            ClientID = id;
            UserName = username;
            Success = success;
        }
    }
    /// <summary>Create new OnHandShakeEndEvent event</summary>
    public class OnClientDisconnectEvent : OnClientConnectEvent {
        /// <summary></summary>
        public OnClientDisconnectEvent (int? id, string? username, bool? success = false) : base(id,username,success) {
            ClientID = id;
            UserName = username;
            Success = success;
        }
    }

    #if SERVER
    /// <summary>Gets never executed on client</summary>
    /// 
    public class OnServerStartEvent : OnServerShutdownEvent {
        /// <summary></summary>
        public OnServerStartEvent (bool? success = false) : base (success) {
            Success = success;
        }
    }
    #endif

    /// <summary>Create new OnHandShakeEndEvent event</summary>
    public class OnServerShutdownEvent : BaseEventClass {
        /// <summary>status if the server was closed succesfully</summary>
        public bool? Success { get; set; } = false;
        /// <summary>Server version</summary>
        public string? Version { get; set; } = Network.ServerVersion;
        /// <summary></summary>
        public OnServerShutdownEvent (bool? success = false) {
            Success = success;
        }
    }
    /// <summary>Create new OnHandShakeEndEvent event</summary>
    public class OnMessageSentEvent : BaseEventClass {
        /// <summary>Network message that was sent</summary>
        public Network.NetworkMessage? Message;
        /// <summary></summary>
        public OnMessageSentEvent (Network.NetworkMessage? message) {
            Message = message;
        }
    }
    ///
    public class OnMessageReceivedEvent : OnMessageSentEvent {
        /// <summary></summary>
        public OnMessageReceivedEvent (Network.NetworkMessage? message) : base (message) {
            Message = message;
        }
    }
    /// <summary>Create new OnHandShakeEndEvent event</summary>
    public class OnHandShakeStartEvent : BaseEventClass {
        /// <summary>Version of the client</summary>
        public string? ClientVersion { get; set; }
        /// <summary>Version of the server</summary>
        public string? ServerVersion { get; set; } = Network.ServerVersion;
        /// <summary>Username of the client</summary>
        public string? UserName { get; set; }
        /// <summary>ID of the client</summary>
        public int? ClientID { get; set; }
        /// <summary></summary>
        public OnHandShakeStartEvent (string? clientVersion, string? username, int? id) {
            ClientID = id;
            ClientVersion = clientVersion;
            UserName = username;
        }
    }
    /// <summary>Create new OnHandShakeEndEvent event</summary>
    public class OnHandShakeEndEvent : OnHandShakeStartEvent {
        /// <summary>0 = unknown, 1 = unknown SERVER error, 2 = version mismatch, 3 = username already in use</summary>
        public int? ErrorCode { get; set;  }
        /// <summary> Check if the HandsShake was success</summary>
        public bool? Success { get; set; }
        /// <summary>
        /// Create new event of OnHandShakeEndEvent
        /// </summary>
        /// <param name="clientVersion"></param>
        /// <param name="username"></param>
        /// <param name="id"></param>
        /// <param name="success"></param>
        /// <param name="code"></param>
        public OnHandShakeEndEvent (string? clientVersion, string? username, int? id, bool? success = false, int? code = 0) : base(clientVersion,username,id) {
            Success = success;
            ClientVersion = clientVersion;
            UserName = username;
            ErrorCode = code;
            ClientID = id;
        }
    }


    /// <summary>Object of the events to be executed to</summary>
    public static NetworkEvents eventsListener { get; set; } = new NetworkEvents();
    internal void ExecuteEvent(dynamic? classData, bool useBlocked = false) {
        Action action = (() => {
            try {
                string? eventName = (classData is JsonElement) ? ((JsonElement)classData).GetProperty("EventName").GetString() : classData?.EventName;
                if (eventName == null) throw new Exception("INVALID EVENT. Not found!");

                switch (eventName.ToLower()) {
                    case "onclientconnectevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<OnClientConnectEvent>();
                        OnClientConnected(classData);
                        break;
                    case "onclientdisconnectevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<OnClientDisconnectEvent>();
                        OnClientDisconnect(classData);
                        break;
                    #if SERVER
                    case "onserverstartevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<OnServerStartEvent>();
                        OnServerStart(classData);
                        break;
                    #endif
                    case "onservershutdownevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<OnServerShutdownEvent>();
                        OnServerShutdown(classData);
                        break;

                    case "onmessagesentevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<Network.NetworkMessage>();
                        OnMessageSent(classData);
                        break;
                    case "onmessagereceivedevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<Network.NetworkMessage>();
                        OnMessageReceived(classData);
                        break;

                    case "onhandshakestartevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<OnHandShakeStartEvent>();
                        OnHandShakeStart(classData);
                        break;
                    case "onhandshakeendevent":
                        if (classData is JsonElement) classData = ((JsonElement)classData).Deserialize<OnHandShakeEndEvent>();
                        OnHandShakeEnd(classData);
                        break;
                        
                    default:
                        Logger.Log(JsonSerializer.Deserialize<object>(classData));
                        throw new NotImplementedException();
                }
            } catch (Exception ex) {
                Logger.Log(ex);
            }
        });
        if (useBlocked) {
            action.Invoke();
        } else {
            new Thread(() => {
                action.Invoke();
            }).Start();
        }
    }


    /// <summary> Uses async.</summary>
    public event EventHandler<OnClientConnectEvent>? ClientConnected;
    protected private virtual void OnClientConnected(OnClientConnectEvent classData) => ClientConnected?.Invoke(this, classData);

    /// <summary> Uses async.</summary>
    public event EventHandler<OnClientDisconnectEvent>? ClientDisconnect;
    protected private virtual void OnClientDisconnect(OnClientDisconnectEvent classData) => ClientDisconnect?.Invoke(this, classData);


    #if SERVER
    /// <summary> Uses blocking. Once events are finished server continues to start</summary>
    public event EventHandler<OnServerStartEvent>? ServerStart;
    protected private virtual void OnServerStart(OnServerStartEvent classData) => ServerStart?.Invoke(this, classData);
    #endif

    /// <summary> Uses blocking. Once events are finished server continues to stop</summary>
    public event EventHandler<OnServerShutdownEvent>? ServerShutdown;
    protected private virtual void OnServerShutdown(OnServerShutdownEvent classData) => ServerShutdown?.Invoke(this, classData);



    /// <summary> Uses async.</summary>
    public event EventHandler<OnMessageSentEvent>? MessageSent;
    protected private virtual void OnMessageSent(OnMessageSentEvent classData) => MessageSent?.Invoke(this, classData);

    /// <summary> Uses async.</summary>
    public event EventHandler<OnMessageReceivedEvent>? MessageReceived;
    protected private virtual void OnMessageReceived(OnMessageReceivedEvent classData) => MessageReceived?.Invoke(this, classData);



    /// <summary> Uses blocking.</summary>
    public event EventHandler<OnHandShakeStartEvent>? HandshakeStart;
    protected private virtual void OnHandShakeStart(OnHandShakeStartEvent classData) => HandshakeStart?.Invoke(this, classData);

    /// <summary> Uses blocking.</summary>
    public event EventHandler<OnHandShakeEndEvent>? HandshakeEnd;
    protected private virtual void OnHandShakeEnd(OnHandShakeEndEvent classData) => HandshakeEnd?.Invoke(this, classData);
}