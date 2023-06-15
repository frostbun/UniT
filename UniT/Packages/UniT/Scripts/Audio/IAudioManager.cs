namespace UniT.Audio
{
    public interface IAudioManager
    {
        #region Sound

        public void PlaySound(string name);

        public void SetSoundVolume(float volume);

        public void MuteSound();

        public void UnmuteSound();

        #endregion

        #region Music

        public void PlayMusic(string name);

        public void SetMusicVolume(float volume);

        public void MuteMusic();

        public void UnmuteMusic();

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();

        #endregion

        #region Master

        public void SetMasterVolume(float volume);

        public void MuteMaster();

        public void UnmuteMaster();

        #endregion
    }
}