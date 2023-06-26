namespace UniT.Audio
{
    using UniT.Logging;

    public interface IAudioManager
    {
        public ILogger Logger { get; }

        public AudioConfig Config { get; }

        public void PlaySound(string name, bool allowDuplicates = true);

        public void PlayMusic(string name);

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();
    }
}