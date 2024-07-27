#if UNIT_DI
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

    public static class DIBinder
    {
        public static void AddUniT(this DependencyContainer container)
        {
            container.AddLoggerManager();
            container.AddAssetsManager();
            container.AddDataManager();
            container.AddUIManager();
            container.AddObjectPoolManager();
            container.AddAudioManager();
            container.AddEntityManager();
            typeof(IService).GetDerivedTypes().ForEach(container.AddInterfacesAndSelf);
            container.AddInitializableManager();
        }
    }
}
#endif