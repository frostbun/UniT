namespace UniT.Audio
{
    using System;
    using UniT.Addressables;
    using UniT.Logging;
    using UniT.ObjectPool;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class AudioManager : IAudioManager
    {
        private readonly IAddressableManager addressableManager;
        private readonly IObjectPoolManager  objectPoolManager;
        private readonly ILogger             logger;

        public AudioManager(IAddressableManager addressableManager, IObjectPoolManager objectPoolManager)
        {
            this.addressableManager = addressableManager;
            this.objectPoolManager  = objectPoolManager;
            this.logger             = LoggerManager.Instance.Get<IAudioManager>();
            this.logger.Info($"{nameof(AudioManager)} instantiated", Color.green);
        }

        #region Sound

        public void PlaySound(string name)
        {
            throw new NotImplementedException();
        }

        public void SetSoundVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void MuteSound()
        {
            throw new NotImplementedException();
        }

        public void UnmuteSound()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Music

        public void PlayMusic(string name)
        {
            throw new NotImplementedException();
        }

        public void SetMusicVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void MuteMusic()
        {
            throw new NotImplementedException();
        }

        public void UnmuteMusic()
        {
            throw new NotImplementedException();
        }

        public void PauseMusic()
        {
            throw new NotImplementedException();
        }

        public void ResumeMusic()
        {
            throw new NotImplementedException();
        }

        public void StopMusic()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Master

        public void SetMasterVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public void MuteMaster()
        {
            throw new NotImplementedException();
        }

        public void UnmuteMaster()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}