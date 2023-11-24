namespace UniT.Audio
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IAudioManager : IDisposable
    {
        public LogConfig LogConfig { get; }

        public IAudioConfig Config { get; }

        public string CurrentMusic { get; }

        #region Sound

        public void LoadSounds(params string[] names);

        public UniTask LoadSoundsAsync(params string[] names);

        public void UnloadSounds(params string[] names);

        public void UnloadAllSounds();

        public void PlaySoundOneShot(string name);

        public void PlaySound(string name, bool loop = false, bool force = false);

        public void StopSounds(params string[] names);

        public void StopAllSounds();

        #endregion

        #region Music

        public void LoadMusic(string name);

        public UniTask LoadMusicAsync(string name);

        public void UnloadMusic();

        public void PlayMusic(string name, bool force = false);

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();

        #endregion
    }
}