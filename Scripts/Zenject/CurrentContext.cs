#if UNIT_ZENJECT
namespace Zenject
{
    using UnityEngine;

    public static class CurrentContext
    {
        private static SceneContext SceneContext;

        public static DiContainer Container
        {
            get
            {
                if (!SceneContext) SceneContext = Object.FindObjectOfType<SceneContext>();
                return SceneContext ? SceneContext.Container : ProjectContext.Instance.Container;
            }
        }
    }
}
#endif