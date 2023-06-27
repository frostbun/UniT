namespace UniT.Audio
{
    using Cysharp.Threading.Tasks;
    using UniT.Logging;

    public interface IAudioManager
    {
        public ILogger Logger { get; }

        public AudioConfig Config { get; }

        public string CurrentMusic { get; }

        public UniTask LoadSounds(params string[] names);

        public void UnloadSounds(params string[] names);

        public void PlaySoundOneShot(string name);

        public void PlaySound(string name, bool force = false);

        public void PlayMusic(string name, bool force = false);

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();
    }
}