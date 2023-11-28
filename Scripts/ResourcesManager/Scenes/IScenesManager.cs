namespace UniT.ResourcesManager
{
    using UniT.Logging;
    using UnityEngine.SceneManagement;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IScenesManager : IHasLogger
    {
        public void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single);

        #if UNIT_UNITASK
        public UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif
    }
}