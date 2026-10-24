using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using EnhancedStreamChat.Chat;
using EnhancedStreamChat.ChatCoreWrapper;
using EnhancedStreamChat.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zenject;

namespace EnhancedStreamChat.UI
{
    /// <summary>
    /// メインメニューの "Enhanced Stream Chat" ボタンから開く設定画面。
    /// 表示レイヤーはチャット画面の設定 (<see cref="ChatDisplay.Layer"/>) と同じ値を読み書きし、双方の表示を同期する。
    /// </summary>
    [HotReload]
    internal class ESCSettingsViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [Inject]
        private readonly ChatDisplay _chatDisplay;
        [Inject]
        private readonly ChatCoreManager _chatCoreManager;
        private bool _disposedValue;

        [UIValue("layer-types")]
        private readonly List<object> _layerTypes = new List<object>() { $"{PluginConfig.LayerType.UI}", $"{PluginConfig.LayerType.HMDOnly}", $"{PluginConfig.LayerType.Manual}" };

        [UIValue("layer-type")]
        public string Layer
        {
            get => this._chatDisplay.Layer;
            set => this._chatDisplay.Layer = value;
        }

        [UIValue("launch-web-app-on-startup")]
        public bool LaunchWebAppOnStartup
        {
            get => this._chatCoreManager.LaunchWebAppOnStartup;
            set
            {
                this._chatCoreManager.LaunchWebAppOnStartup = value;
                this.NotifyPropertyChanged();
            }
        }

        [UIValue("is-main-settings-available")]
        public bool IsMainSettingsAvailable => this._chatCoreManager.IsMainSettingsAvailable;

        [UIAction("launch-web-app")]
        protected void LaunchWebApp()
        {
            this._chatCoreManager.LaunchWebApp();
        }

        public void Initialize()
        {
            this._chatDisplay.PropertyChanged += this.OnChatDisplayPropertyChanged;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            // Web アプリ側で設定が変更されている可能性があるため、表示のたびに再読込する。
            this.NotifyPropertyChanged(nameof(this.LaunchWebAppOnStartup));
            this.NotifyPropertyChanged(nameof(this.Layer));
        }

        private void OnChatDisplayPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatDisplay.Layer)) {
                this.NotifyPropertyChanged(nameof(this.Layer));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue) {
                if (disposing && this._chatDisplay != null) {
                    this._chatDisplay.PropertyChanged -= this.OnChatDisplayPropertyChanged;
                }
                this._disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
