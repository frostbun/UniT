namespace UniT.Audio
{
    using System;
    using UniT.Logging;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IAudioManager
    {
        public LogConfig LogConfig { get; }

        public IAudioConfig Config { get; }

        public string CurrentMusic { get; }

        #region Sound

        public void LoadSounds(params string[] names);

        #if UNIT_UNITASK
        public UniTask LoadSoundsAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        public UniTask LoadSoundsAsync(string[] names, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif

        public void UnloadSounds(params string[] names);

        public void UnloadAllSounds();

        public void PlaySoundOneShot(string name);

        public void PlaySound(string name, bool loop = false, bool force = false);

        public void StopSounds(params string[] names);

        public void StopAllSounds();

        #endregion

        #region Music

        public void LoadMusic(string name);

        #if UNIT_UNITASK
        public UniTask LoadMusicAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif

        public void UnloadMusic();

        public void PlayMusic(string name, bool force = false);

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();

        #endregion
    }
}