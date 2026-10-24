using EnhancedStreamChat.Chat;
using EnhancedStreamChat.UI;
using Zenject;

namespace EnhancedStreamChat.Installers
{
    internal class ESCMenuInstaller : Zenject.Installer
    {
        public override void InstallBindings()
        {
            _ = this.Container.BindInterfacesAndSelfTo<ChatDisplay>().FromNewComponentAsViewController().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<ESCSettingsViewController>().FromNewComponentAsViewController().AsSingle();
            _ = this.Container.BindInterfacesAndSelfTo<ESCSettingsFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
        }
    }
}
