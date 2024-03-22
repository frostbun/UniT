namespace UniT.Audio
{
    using System;
    using UniT.Initializables;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IAudioManager : IInitializable
    {
        public IAudioConfig Config { get; }

        #region Sound

        public void LoadSound(string name);

        public void PlaySoundOneShot(string name);

        public void PlaySound(string name, bool loop = false, bool force = false);

        public void StopSound(string name);

        public void UnloadSound(string name);

        #endregion

        #region Music

        public string CurrentMusic { get; }

        public void LoadMusic(string name);

        public void PlayMusic(string name, bool force = false);

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();

        public void UnloadMusic();

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask LoadSoundAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask LoadMusicAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif

        #endregion
    }
}