using ChatCore.Interfaces;
using EnhancedStreamChat.Interfaces;

namespace EnhancedStreamChat.Models
{
    public class ESCChatChannel : IESCChatChannel
    {
        public string ID   => this._channel.Id;
        public string Name => this._channel.Name;

        private readonly IChatChannel _channel;

        public ESCChatChannel(IChatChannel channel)
        {
            this._channel = channel;
        }
    }
}