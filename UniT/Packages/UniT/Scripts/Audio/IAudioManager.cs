namespace UniT.Audio
{
    public interface IAudioManager
    {
        public void PlaySound(string name);
        public void SetSoundVolume(float volume);
        public void MuteSound();
        public void UnmuteSound();

        public void PlayMusic(string name);
        public void SetMusicVolume(float volume);
        public void MuteMusic();
        public void UnmuteMusic();
        public void PauseMusic();
        public void ResumeMusic();
        public void StopMusic();

        public void SetMasterVolume(float volume);
    }
}