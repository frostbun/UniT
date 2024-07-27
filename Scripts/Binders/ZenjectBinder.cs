#if UNIT_ZENJECT
#nullable enable
namespace UniT
{
    using UniT.Audio;
    using UniT.Data;
    using UniT.DI;
    using UniT.Entities;
    using UniT.Extensions;
    using UniT.Initializables;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourceManagement;
    using UniT.Services;
    using UniT.UI;
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindUniT(this DiContainer container)
        {
            container.BindInterfacesTo<ZenjectContainer>().AsSingle();
            container.BindLoggerManager();
            container.BindAssetsManager();
            container.BindDataManager();
            container.BindUIManager();
            container.BindObjectPoolManager();
            container.BindAudioManager();
            container.BindEntityManager();
            typeof(IService).GetDerivedTypes().ForEach(type => container.BindInterfacesAndSelfTo(type).AsSingle());
            container.BindInitializableManager();
        }
    }
}
#endif