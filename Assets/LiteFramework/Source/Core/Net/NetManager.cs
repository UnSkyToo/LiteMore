using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using LiteFramework.Core.Log;
using LiteFramework.Core.Security;

namespace LiteFramework.Core.Net
{
    public static class NetManager
    {
        public enum ConnectedCode
        {
            None = 0,
            Succeeded = 1,
            Failed = 2,
        }

        public static BaseNetMsgCodec Codec { get; set; } = null;
        public static float SendInterval { get; set; } = 0.2f;

        public static IPAddress Ip { get; private set; }
        public static int Port { get; private set; }
        public static bool Connected => TcpClient_ != null && TcpClient_.Connected;

        private static TcpClient TcpClient_ = null;

        public delegate void ServerConnectSucceeded(IPAddress Ip, int Port);
        public delegate void ServerConnectFailed(IPAddress Ip, int Port);

        public static event ServerConnectSucceeded OnServerConnectSucceeded = null;
        public static event ServerConnectFailed OnServerConnectFailed = null;

        private static readonly MemoryStream WriteStream_ = new MemoryStream();
        private static readonly MemoryStream ReadStream_ = new MemoryStream();

        private static readonly Queue<NetMsgBuffer> RecvQueue_ = new Queue<NetMsgBuffer>();
        private static readonly object RecvQueueLock_ = new object();
        private static readonly Queue<NetMsgBuffer> SendQueue_ = new Queue<NetMsgBuffer>();

        private static readonly Random Rand_ = new Random();
        private static readonly Dictionary<uint, string> MsgId2Name_ = new Dictionary<uint, string>();
        private static readonly Dictionary<uint, MsgListener> MsgHandlerDic_ = new Dictionary<uint, MsgListener>();
        private static float LastSendTime_ = 0.0f;

        private static int ConnectedStateCode_ = (int) ConnectedCode.None;

        public static bool Startup()
        {
            return true;
        }

        public static void Shutdown()
        {
            foreach (var MsgEntity in MsgHandlerDic_)
            {
                MsgEntity.Value.Dispose();
#if UNITY_EDITOR
                MsgEntity.Value.Check();
#endif
            }

            WriteStream_.Dispose();
            ReadStream_.Dispose();
            DisConnect();
            MsgId2Name_.Clear();
            MsgHandlerDic_.Clear();

            OnServerConnectSucceeded = null;
            OnServerConnectFailed = null;
        }

        public static bool GetMsgNameById(uint MsgId, out string MsgName)
        {
            return MsgId2Name_.TryGetValue(MsgId, out MsgName);
        }

        public static void RegisterMsgHandler<T>(Action<T> Callback) where T : BaseNetMsg
        {
            string MsgName = typeof(T).FullName;
            uint MsgId = Crc32.Calculate(MsgName);
            MsgId2Name_[MsgId] = MsgName;

            if (!MsgHandlerDic_.ContainsKey(MsgId))
            {
                MsgHandlerDic_.Add(MsgId, new MsgListenerImpl<T>());
            }

            ((MsgListenerImpl<T>) MsgHandlerDic_[MsgId]).OnEvent += Callback;
        }

        public static void UnRegisterMsgHandler<T>(Action<T> Callback) where T : BaseNetMsg
        {
            string MsgName = typeof(T).FullName;
            uint MsgId = Crc32.Calculate(MsgName);
            if (MsgHandlerDic_.ContainsKey(MsgId))
            {
                ((MsgListenerImpl<T>) MsgHandlerDic_[MsgId]).OnEvent -= Callback;
            }
        }

        private static void DispatchMsg(object Msg)
        {
            var MsgName = Msg.GetType().FullName;
            var MsgId = Crc32.Calculate(MsgName);
            if (MsgHandlerDic_.TryGetValue(MsgId, out var MsgListener))
            {
                MsgListener.Trigger(Msg);
            }
        }


        public static void Connect(IPAddress Ip, int Port)
        {
            if (Codec == null)
            {
                LLogger.LError("NetManager.Codec is null");
                return;
            }

            NetManager.Ip = Ip;
            NetManager.Port = Port;
            NetManager.TcpClient_ = new TcpClient();
            NetManager.TcpClient_.BeginConnect(Ip, Port, OnTcpConnectResult, TcpClient_);
        }

        public static void DisConnect()
        {
            if (TcpClient_ != null)
            {
                if (TcpClient_.Connected)
                {
                    TcpClient_.GetStream()?.Close();
                }

                TcpClient_.Close();
                TcpClient_ = null;
            }
        }

        private static void OnTcpConnectResult(IAsyncResult Ar)
        {
            try
            {
                TcpClient_.EndConnect(Ar);
                Interlocked.Exchange(ref ConnectedStateCode_, (int) ConnectedCode.Succeeded);

                ReadStateObject StateObj = new ReadStateObject(TcpClient_, 4);
                TcpClient_.GetStream().BeginRead(StateObj.Buffer_, 0, StateObj.BytesNeed_, OnTcpReadResult, StateObj);
            }
            catch
            {
                Interlocked.Exchange(ref ConnectedStateCode_, (int) ConnectedCode.Failed);
            }
        }

        private static void OnTcpReadResult(IAsyncResult Ar)
        {
            if (Connected == false)
            {
                return;
            }

            try
            {
                ReadStateObject StateObj = (ReadStateObject) Ar.AsyncState;
                int NumberOfBytesRead = StateObj.Client_.GetStream().EndRead(Ar);
                StateObj.BytesRead_ += NumberOfBytesRead;
                StateObj.BytesNeed_ -= NumberOfBytesRead;
                if (NumberOfBytesRead > 0)
                {
                    if (StateObj.BytesNeed_ == 0)
                    {
                        if (StateObj.Step_ == ReadStateObject.ReadStep.ReadLength)
                        {
                            int BytesNeed = BitConverter.ToInt32(StateObj.Buffer_, 0);
                            StateObj.Reset(ReadStateObject.ReadStep.Read, BytesNeed);
                            TcpClient_.GetStream().BeginRead(StateObj.Buffer_, 0, BytesNeed, OnTcpReadResult, StateObj);
                        }
                        else
                        {
                            lock (RecvQueueLock_)
                            {
                                RecvQueue_.Enqueue(new NetMsgBuffer(StateObj.Buffer_));
                            }

                            StateObj.Reset(ReadStateObject.ReadStep.ReadLength, 4);
                            TcpClient_.GetStream()
                                .BeginRead(StateObj.Buffer_, 0, StateObj.BytesNeed_, OnTcpReadResult, StateObj);
                        }
                    }
                    else
                    {
                        StateObj.Client_.GetStream()
                            .BeginRead(StateObj.Buffer_, StateObj.BytesRead_, StateObj.BytesNeed_, OnTcpReadResult,
                                StateObj);
                    }
                }
                else
                {
                    StateObj.Client_.GetStream().Close();
                    StateObj.Client_.Close();

                    Interlocked.Exchange(ref ConnectedStateCode_, (int) ConnectedCode.Failed);
                }
            }
            catch
            {
                Interlocked.Exchange(ref ConnectedStateCode_, (int) ConnectedCode.Failed);
            }
        }

        public static void SendMsg<T>(T Msg) where T : BaseNetMsg
        {
            if (Connected)
            {
                var Buffer = Codec.Encode(Msg);
                SendQueue_.Enqueue(new NetMsgBuffer(Buffer));
            }
            else
            {
                UnityEngine.Debug.Log("Not Connected");
            }
        }

        public static void SendMsg<T>() where T : BaseNetMsg, new()
        {
            var Msg = new T();
            SendMsg(Msg);
        }

        public static void Tick(float DeltaTime)
        {
            if (ConnectedStateCode_ == (int) ConnectedCode.Failed && OnServerConnectFailed != null)
            {
                Interlocked.Exchange(ref ConnectedStateCode_, (int) ConnectedCode.None);
                OnServerConnectFailed(Ip, Port);
            }
            else if (ConnectedStateCode_ == (int) ConnectedCode.Succeeded && OnServerConnectSucceeded != null)
            {
                Interlocked.Exchange(ref ConnectedStateCode_, (int) ConnectedCode.None);
                OnServerConnectSucceeded(Ip, Port);
            }

            while (true)
            {
                try
                {
                    NetMsgBuffer Buffer = null;
                    lock (RecvQueueLock_)
                    {
                        if (RecvQueue_.Count == 0)
                        {
                            break;
                        }

                        Buffer = RecvQueue_.Dequeue();
                    }

                    var Msg = Codec.Decode(Buffer.Data);
                    if (Msg != null)
                    {
                        DispatchMsg(Msg);
                    }
                }
                catch
                {
                    break;
                }
            }

            if (LastSendTime_ >= SendInterval && SendQueue_.Count > 0)
            {
                var Data = SendQueue_.Dequeue().Data;
                /*IAsyncResult result = */
                TcpClient_.GetStream().BeginWrite(Data, 0, (int)Data.Length,
                    Ar => { ((TcpClient) Ar.AsyncState).GetStream().EndWrite(Ar); },
                    TcpClient_);

                LastSendTime_ = 0.0f;
            }
            else
            {
                LastSendTime_ += DeltaTime;
            }
        }

        private class ReadStateObject
        {
            public enum ReadStep
            {
                ReadLength,
                Read
            }

            public readonly TcpClient Client_ = null;
            public ReadStep Step_ = ReadStep.ReadLength;
            public int BytesRead_ = 0;
            public int BytesNeed_ = 0;
            public byte[] Buffer_ = null;

            public ReadStateObject(TcpClient TcpClient, int BytesNeed)
            {
                Client_ = TcpClient;
                Reset(ReadStep.ReadLength, BytesNeed);
            }

            public void Reset(ReadStep Step, int BytesNeed)
            {
                Step_ = Step;
                BytesRead_ = 0;
                BytesNeed_ = BytesNeed;
                Buffer_ = new byte[BytesNeed_];
            }
        }

        private abstract class MsgListener : IDisposable
        {
            public abstract void Trigger(object Msg);

            public abstract void Check();
            public abstract void Dispose();
        }

        private class MsgListenerImpl<T> : MsgListener
        {
            public event Action<T> OnEvent = null;

            public override void Trigger(object Msg)
            {
                OnEvent?.Invoke((T)Msg);
            }

            public override void Check()
            {
                if (OnEvent != null)
                {
                    var CallbackList = OnEvent.GetInvocationList();

                    foreach (var Callback in CallbackList)
                    {
                        LLogger.LWarning($"{Callback.Method.ReflectedType.Name} : {Callback.Method.Name} UnRegister");
                    }
                }
            }

            public override void Dispose()
            {
                OnEvent = null;
            }
        }
    }
}