using ChatCore.Interfaces;
using ChatCore.Models;
using ChatCore.Utilities;
using EnhancedStreamChat.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EnhancedStreamChat.Models
{
    public class ESCChatMessage : IESCChatMessage
    {
        public string Id                              { get; internal set; }
        public bool   IsSystemMessage                 { get; internal set; }
        public bool   IsActionMessage                 { get; internal set; }
        public bool   IsMentioned                     { get; internal set; }
        public bool   IsHighlighted                   { get; internal set; }
        public string Message                         { get; internal set; } = "";
        public string SubMessage                      { get; internal set; } = "";
        public IChatUser Sender                       { get; internal set; }
        public IESCChatChannel Channel                { get; internal set; }
        public ReadOnlyCollection<IChatEmote> Emotes  { get; internal set; }
            = new ReadOnlyCollection<IChatEmote>(Array.Empty<IChatEmote>());
        public ReadOnlyDictionary<string, string> Metadata { get; internal set; }
            = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        /// <summary>ChatCore の IChatMessage から構築するコンストラクタ。</summary>
        public ESCChatMessage(IChatMessage chatMessage)
        {
            this.Id               = chatMessage.Id;
            this.IsSystemMessage  = chatMessage.IsSystemMessage;
            this.IsActionMessage  = chatMessage.IsActionMessage;
            this.IsMentioned      = chatMessage.IsPing;
            this.Sender           = chatMessage.Sender;

            if (chatMessage.Emotes != null) {
                this.Emotes = new ReadOnlyCollection<IChatEmote>(chatMessage.Emotes);
            }
            if (chatMessage.Metadata != null) {
                this.Metadata = new ReadOnlyDictionary<string, string>(chatMessage.Metadata);
            }
            if (chatMessage.Channel != null) {
                this.Channel = new ESCChatChannel(chatMessage.Channel);
            }

            if (this.SystemMessageSetup()) {
                // システムメッセージは本文を SubMessage に退避
                this.SubMessage = chatMessage.Message;
            }
            else {
                this.Message    = chatMessage.Message;
                this.SubMessage = "";
            }
        }

        /// <summary>ID と文字列から直接生成するコンストラクタ（接続通知などに利用）。</summary>
        public ESCChatMessage(string id, string message)
        {
            this.Id      = id;
            this.Message = message;
        }

        // -------------------------------------------------------
        // msg-id メタデータに基づいてシステムメッセージを解析する
        // -------------------------------------------------------
        private bool SystemMessageSetup()
        {
            var updateMessage = false;

            if (!this.Metadata.TryGetValue("msg-id", out var msgIdValue)) {
                this.IsSystemMessage = false;
                return false;
            }

            switch (msgIdValue) {
                case "skip-subs-mode-message":
                    this.Message         = "Redeemed Send a Message In Sub-Only Mode";
                    this.IsHighlighted   = false;
                    this.IsSystemMessage = true;
                    updateMessage        = true;
                    break;

                case "highlighted-message":
                    this.Message         = "Redeemed Highlight My Message";
                    this.IsHighlighted   = true;
                    this.IsSystemMessage = true;
                    updateMessage        = true;
                    break;

                default:
                    if (this.Metadata.TryGetValue("system-msg", out var systemMsgText)) {
                        systemMsgText        = systemMsgText.Replace(@"\s", " ");
                        this.IsHighlighted   = true;
                        this.IsSystemMessage = true;

                        if (this.Metadata.TryGetValue("msg-param-sub-plan", out var subPlanName)) {
                            this.Message  = subPlanName == "Prime"
                                ? $"👑  {systemMsgText}"
                                : $"⭐  {systemMsgText}";
                            updateMessage = true;
                        }
                        else if (this.Metadata.TryGetValue("msg-param-profileImageURL", out var profileImage)
                              && this.Metadata.TryGetValue("msg-param-login",            out var loginUser)) {
                            var emoteId = $"ProfileImage_{loginUser}";
                            var url     = new StringBuilder(profileImage)
                                              .Replace("_image-%s.png", "_image-300x300.png")
                                              .ToString();

                            this.Emotes = new ReadOnlyCollection<IChatEmote>(new IChatEmote[] {
                                new SimpleEmote(emoteId, $"[{emoteId}]", url)
                            });
                            this.Message  = $"{this.Emotes[0].Name}  {systemMsgText}";
                            updateMessage = true;
                        }
                    }
                    else {
                        this.IsHighlighted   = true;
                        this.IsSystemMessage = true;
                    }
                    break;
            }
            return updateMessage;
        }

        // -------------------------------------------------------
        // ChatCore v2 の IChatEmote 完全実装（内部用途のみ）
        // -------------------------------------------------------
        private class SimpleEmote : IChatEmote
        {
            public string    Id         { get; }
            public string    Name       { get; }
            public string    Uri        { get; }
            public int       StartIndex { get; } = 0;
            public int       EndIndex   { get; }
            public bool      IsAnimated { get; } = false;
            public EmoteType Type       { get; } = EmoteType.SingleImage;
            public ImageRect UVs        { get; } = ImageRect.None;

            public SimpleEmote(string id, string name, string uri)
            {
                this.Id       = id;
                this.Name     = name;
                this.Uri      = uri;
                this.EndIndex = name.Length - 1;
            }

            public JSONObject ToJson()
            {
                var obj = new JSONObject();
                obj.Add(nameof(Id),         new JSONString(Id));
                obj.Add(nameof(Name),       new JSONString(Name));
                obj.Add(nameof(Uri),        new JSONString(Uri));
                obj.Add(nameof(StartIndex), new JSONNumber(StartIndex));
                obj.Add(nameof(EndIndex),   new JSONNumber(EndIndex));
                obj.Add(nameof(IsAnimated), new JSONBool(IsAnimated));
                obj.Add(nameof(Type),       new JSONString(Type.ToString()));
                obj.Add(nameof(UVs),        UVs.ToJson());
                return obj;
            }
        }
    }
}