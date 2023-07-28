namespace UniT.Audio
{
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IAudioManager
    {
        public LogConfig LogConfig { get; }

        public IAudioConfig Config { get; }

        public string CurrentMusic { get; }

        public UniTask LoadSounds(params string[] names);

        public void UnloadSounds(params string[] names);

        public void PlaySoundOneShot(string name);

        public void PlaySound(string name, bool loop = false, bool force = false);

        public void StopSounds(params string[] names);

        public void StopAllSounds();

        public UniTask LoadMusic(string name);

        public void PlayMusic(string name, bool force = false);

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();
    }
}