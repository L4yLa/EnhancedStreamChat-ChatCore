using ChatCore;
using ChatCore.Interfaces;
using ChatCore.Services;
using EnhancedStreamChat.Interfaces;
using IPA.Logging;
using System;
using System.Threading.Tasks;
using Zenject;

namespace EnhancedStreamChat.ChatCoreWrapper
{
    internal class ChatCoreManager : IDisposable, IChatCoreManager, IInitializable
    {
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ
        #region // イベント
        public event Action<IChatService> OnAuthenticatedStateChanged;
        public event Action<IChatService> OnChatConnected;
        public event Action<IChatService, IChatChannel> OnJoinChannel;
        public event Action<IChatService, IChatChannel> OnLeaveChannel;
        public event Action<IChatService, IChatChannel> OnRoomStateUpdated;
        public event Action<IChatService, IChatMessage> OnTextMessageReceived;
        public event Action<IChatService, string> OnMessageDeleted;
        public event Action<IChatService, string> OnChatCleared;

        // ChatCore v2 では PubSub 未サポート。互換維持のため宣言のみ。
        public event OnFollowHandler OnFollow;
        public event OnRewardRedeemedHandler OnRewardRedeemed;
        #endregion

        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ
        #region // パブリックメソッド
        public void Initialize()
        {
            this.RunService();
        }

        public void RunService()
        {
            if (this._instance == null) {
                return;
            }
            this._streamingService = this._instance.RunAllServices();
            this._streamingService.OnLogin                  += this.StreamingService_OnLogin;
            this._streamingService.OnJoinChannel            += this.StreamingService_OnJoinChannel;
            this._streamingService.OnLeaveChannel           += this.StreamingService_OnLeaveChannel;
            this._streamingService.OnRoomStateUpdated       += this.StreamingService_OnRoomStateUpdated;
            this._streamingService.OnTextMessageReceived    += this.StreamingService_OnTextMessageReceived;
            this._streamingService.OnMessageCleared         += this.StreamingService_OnMessageCleared;
            this._streamingService.OnChatCleared            += this.StreamingService_OnChatCleared;
        }

        /// <summary>
        /// ChatCore の設定 Web ポータルを起動します。
        /// </summary>
        public void LaunchWebPortal(bool shouldLaunchPortal = true)
        {
            if (!shouldLaunchPortal) {
                return;
            }
            this._instance?.LaunchWebPortal();
        }

        // ChatCore v2 では IRC の直接起動 API が公開されていないため no-op。
        public Task IrcStart()  => Task.CompletedTask;
        public Task IrcStop()   => Task.CompletedTask;

        // ChatCore v2 では PubSub の直接制御 API が公開されていないため no-op。
        public Task PubSubStart(object instance) => Task.CompletedTask;
        public Task PubSubStop(object instance)  => Task.CompletedTask;
        #endregion

        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ
        #region // プライベートメソッド（イベント中継）
        // OnLogin → IChatCoreManager の OnAuthenticatedStateChanged と OnChatConnected 両方に流す
        private void StreamingService_OnLogin(IChatService service)
        {
            this.OnAuthenticatedStateChanged?.Invoke(service);
            this.OnChatConnected?.Invoke(service);
        }

        private void StreamingService_OnJoinChannel(IChatService service, IChatChannel channel)
            => this.OnJoinChannel?.Invoke(service, channel);

        private void StreamingService_OnLeaveChannel(IChatService service, IChatChannel channel)
            => this.OnLeaveChannel?.Invoke(service, channel);

        private void StreamingService_OnRoomStateUpdated(IChatService service, IChatChannel channel)
            => this.OnRoomStateUpdated?.Invoke(service, channel);

        private void StreamingService_OnTextMessageReceived(IChatService service, IChatMessage message)
            => this.OnTextMessageReceived?.Invoke(service, message);

        // ChatCore v2: OnMessageCleared(IChatService, string messageId)
        private void StreamingService_OnMessageCleared(IChatService service, string messageId)
            => this.OnMessageDeleted?.Invoke(service, messageId);

        // ChatCore v2: OnChatCleared(IChatService, string userId)
        private void StreamingService_OnChatCleared(IChatService service, string userId)
            => this.OnChatCleared?.Invoke(service, userId);
        #endregion

        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ
        #region // メンバ変数
        private readonly ChatCoreInstance _instance;
        private IChatService _streamingService;
        private bool _disposedValue;
        #endregion

        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ
        #region // ���築・破棄
        public ChatCoreManager()
        {
            this._instance = ChatCoreInstance.Create();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue) {
                if (disposing && this._streamingService != null) {
                    this._streamingService.OnLogin               -= this.StreamingService_OnLogin;
                    this._streamingService.OnJoinChannel         -= this.StreamingService_OnJoinChannel;
                    this._streamingService.OnLeaveChannel        -= this.StreamingService_OnLeaveChannel;
                    this._streamingService.OnRoomStateUpdated    -= this.StreamingService_OnRoomStateUpdated;
                    this._streamingService.OnTextMessageReceived -= this.StreamingService_OnTextMessageReceived;
                    this._streamingService.OnMessageCleared      -= this.StreamingService_OnMessageCleared;
                    this._streamingService.OnChatCleared         -= this.StreamingService_OnChatCleared;
                    this._instance?.StopAllServices();
                }
                this._disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}