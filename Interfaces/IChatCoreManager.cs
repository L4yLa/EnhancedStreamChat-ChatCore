using ChatCore.Interfaces;
using System;
using System.Threading.Tasks;

namespace EnhancedStreamChat.Interfaces
{
    public interface IChatCoreManager
    {
        // 接続・認証
        event Action<IChatService> OnAuthenticatedStateChanged;
        event Action<IChatService> OnChatConnected;

        // チャンネル参加/退出
        event Action<IChatService, IChatChannel> OnJoinChannel;
        event Action<IChatService, IChatChannel> OnLeaveChannel;
        event Action<IChatService, IChatChannel> OnRoomStateUpdated;

        // メッセージ
        event Action<IChatService, IChatMessage> OnTextMessageReceived;
        event Action<IChatService, string> OnMessageDeleted;
        event Action<IChatService, string> OnChatCleared;

        // PubSub (ChatCore v2 では未サポートのため空実装用デリゲートのみ定義)
        event OnFollowHandler OnFollow;
        event OnRewardRedeemedHandler OnRewardRedeemed;

        void RunService();
        void LaunchWebApp(bool shouldLaunchWebApp = true);

        Task IrcStart();
        Task IrcStop();
        Task PubSubStart(object instance);
        Task PubSubStop(object instance);
    }

    public delegate void OnFollowHandler(string channelId, string displayName, string username);
    public delegate void OnRewardRedeemedHandler(string channelId, string userDisplayName, string rewardTitle, int rewardCost);
}
