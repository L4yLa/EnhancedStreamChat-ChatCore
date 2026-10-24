using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using System;
using Zenject;

namespace EnhancedStreamChat.UI
{
    /// <summary>
    /// メインメニュー左側の MOD ボタン欄に "Enhanced Stream Chat" ボタンを登録し、設定画面を表示する。
    /// 表示レイヤーを HMDOnly にした場合でも、FPFC (デスクトップ) からチャット設定に到達できるようにするための導線。
    /// </summary>
    internal class ESCSettingsFlowCoordinator : FlowCoordinator, IInitializable, IDisposable
    {
        [Inject]
        private readonly ESCSettingsViewController _settingsViewController;
        [Inject]
        private readonly MenuButtons _menuButtons;
        [Inject]
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private MenuButton _menuButton;
        private bool _disposedValue;

        public void Initialize()
        {
            this._menuButton = new MenuButton("Enhanced Stream Chat", "Enhanced Stream Chat settings", this.OnMenuButtonClicked, true);
            this._menuButtons.RegisterButton(this._menuButton);
        }

        private void OnMenuButtonClicked()
        {
            this._mainFlowCoordinator.PresentFlowCoordinator(this, null, ViewController.AnimationDirection.Horizontal, false, false);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation) {
                this.SetTitle("Enhanced Stream Chat");
                this.showBackButton = true;
            }
            if (addedToHierarchy) {
                this.ProvideInitialViewControllers(this._settingsViewController);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            this._mainFlowCoordinator.DismissFlowCoordinator(this, ViewController.AnimationDirection.Horizontal, null, false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue) {
                if (disposing && this._menuButton != null) {
                    try {
                        this._menuButtons.UnregisterButton(this._menuButton);
                    }
                    catch (Exception e) {
                        Logger.Error(e);
                    }
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
