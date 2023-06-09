namespace UniT.DI.Extensions
{
    using UnityEngine;
    using Zenject;

    public static class ZenjectUtils
    {
        private static SceneContext currentSceneContext;

        public static DiContainer CurrentContainer
        {
            get
            {
                if (!currentSceneContext) currentSceneContext = Object.FindObjectOfType<SceneContext>();
                return currentSceneContext ? currentSceneContext.Container : ProjectContext.Instance.Container;
            }
        }

        public static void Inject(this object obj)
        {
            CurrentContainer.Inject(obj);
        }
    }
}