namespace UniT.ResourcesManager
{
    using System;
    using UnityEngine.SceneManagement;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IScenesManager
    {
        public void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single);

        #if UNIT_UNITASK
        public UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, Action callback = null, IProgress<float> progress = null);
        #endif
    }
}