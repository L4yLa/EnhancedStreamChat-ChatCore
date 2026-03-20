using ChatCore.Interfaces;
using EnhancedStreamChat.Interfaces;

namespace EnhancedStreamChat.Models
{
    public class ESCChatChannel : IESCChatChannel
    {
        public string ID   => this._channel.Id;
        public string Name => this._channel.Name;

        private readonly IChatChannel _channel;
        private readonly IChatService _service;

        public ESCChatChannel(IChatChannel channel, IChatService service = null)
        {
            this._channel = channel;
            this._service = service;
        }

        public void SendMessage(string message)
        {
            if (this._service != null) {
                this._service.SendTextMessage(message, this._channel);
            }
            else {
                Logger.Warn($"ESCChatChannel.SendMessage called but no IChatService is available. Message: {message}");
            }
        }
    }
}