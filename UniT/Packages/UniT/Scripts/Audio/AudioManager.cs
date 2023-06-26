namespace UniT.Audio
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    // TODO: auto release audio clips after a while
    public class AudioManager : IAudioManager
    {
        public ILogger     Logger { get; }
        public AudioConfig Config { get; }

        private readonly IAddressableManager             addressableManager;
        private readonly GameObject                      audioSourceContainer;
        private readonly AudioSource                     musicSource;
        private readonly Queue<AudioSource>              pooledSoundSource;
        private readonly Dictionary<string, AudioSource> spawnedSoundSource;

        public AudioManager(AudioConfig config, IAddressableManager addressableManager, ILogger logger)
        {
            this.Config = config;
            this.ObserveConfig();

            this.addressableManager = addressableManager;

            this.audioSourceContainer = new(nameof(AudioManager));
            this.musicSource          = this.audioSourceContainer.AddComponent<AudioSource>();
            this.musicSource.loop     = true;
            Object.DontDestroyOnLoad(this.audioSourceContainer);

            this.pooledSoundSource  = new();
            this.spawnedSoundSource = new();

            this.Logger = logger;
            this.Logger.Info($"{nameof(AudioManager)} instantiated", Color.green);
        }

        private void ObserveConfig()
        {
            this.Config.SoundVolume.Subscribe(_ => this.ConfigureAllSoundSources());
            this.Config.MuteSound.Subscribe(_ => this.ConfigureAllSoundSources());
            this.Config.MusicVolume.Subscribe(_ => this.ConfigureMusicSource());
            this.Config.MuteMusic.Subscribe(_ => this.ConfigureMusicSource());
            this.Config.MasterVolume.Subscribe(_ =>
            {
                this.ConfigureAllSoundSources();
                this.ConfigureMusicSource();
            });
            this.Config.MuteMaster.Subscribe(_ =>
            {
                this.ConfigureAllSoundSources();
                this.ConfigureMusicSource();
            });
        }

        public void PlaySound(string name, bool allowDuplicates = true)
        {
            this.addressableManager.Load<AudioClip>(name).ContinueWith(audioClip =>
            {
                var soundSource = this.GetSoundSource(name);
                if (!allowDuplicates && soundSource.isPlaying) return;
                soundSource.PlayOneShot(audioClip);
            }).Forget();
        }


        public void PlayMusic(string name)
        {
            this.addressableManager.Load<AudioClip>(name).ContinueWith(audioClip =>
            {
                this.musicSource.clip = audioClip;
                this.musicSource.Play();
            }).Forget();
        }

        public void PauseMusic()
        {
            this.musicSource.Pause();
        }

        public void ResumeMusic()
        {
            this.musicSource.UnPause();
        }

        public void StopMusic()
        {
            this.musicSource.Stop();
        }

        private AudioSource GetSoundSource(string name)
        {
            var soundSource = this.spawnedSoundSource.GetOrAdd(name, () => this.pooledSoundSource.DequeueOrDefault(() => this.audioSourceContainer.AddComponent<AudioSource>()));
            this.ConfigureSoundSource(soundSource);
            return soundSource;
        }

        private void ConfigureAllSoundSources()
        {
            this.spawnedSoundSource.Values.ForEach(this.ConfigureSoundSource);
        }

        private void ConfigureSoundSource(AudioSource soundSource)
        {
            soundSource.volume = this.Config.SoundVolume.Value * this.Config.MasterVolume.Value;
            soundSource.mute   = this.Config.MuteSound.Value || this.Config.MuteMaster.Value;
        }

        private void ConfigureMusicSource()
        {
            this.musicSource.volume = this.Config.MusicVolume.Value * this.Config.MasterVolume.Value;
            this.musicSource.mute   = this.Config.MuteMusic.Value || this.Config.MuteMaster.Value;
        }
    }
}