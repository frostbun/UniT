namespace UniT.Audio
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.Utils;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    // TODO: auto release audio clips after a while
    public class AudioManager : IAudioManager, IInitializable
    {
        public ILogger     Logger { get; }
        public AudioConfig Config { get; }

        public string CurrentMusic { get; private set; }

        private readonly IAddressableManager             addressableManager;
        private readonly GameObject                      audioSourceContainer;
        private readonly AudioSource                     musicSource;
        private readonly Queue<AudioSource>              pooledSoundSource;
        private readonly Dictionary<string, AudioSource> spawnedSoundSource;

        public AudioManager(AudioConfig config, IAddressableManager addressableManager, ILogger logger)
        {
            this.Config             = config;
            this.addressableManager = addressableManager;

            this.audioSourceContainer = new(this.GetType().Name);
            this.musicSource          = this.audioSourceContainer.AddComponent<AudioSource>();
            this.musicSource.loop     = true;
            Object.DontDestroyOnLoad(this.audioSourceContainer);

            this.pooledSoundSource  = new();
            this.spawnedSoundSource = new();

            this.Logger = logger;
            this.Logger.Info($"{this.GetType().Name} instantiated", Color.green);
        }

        public void Initialize()
        {
            this.Config.SoundVolume.Subscribe(value =>
            {
                this.ConfigureAllSoundSources();
                this.Logger.Debug($"Sound volume set to {value}");
            });
            this.Config.MuteSound.Subscribe(value =>
            {
                this.ConfigureAllSoundSources();
                this.Logger.Debug(value ? "Sound volume muted" : "Sound volume unmuted");
            });
            this.Config.MusicVolume.Subscribe(value =>
            {
                this.ConfigureMusicSource();
                this.Logger.Debug($"Music volume set to {value}");
            });
            this.Config.MuteMusic.Subscribe(value =>
            {
                this.ConfigureMusicSource();
                this.Logger.Debug(value ? "Music volume muted" : "Music volume unmuted");
            });
            this.Config.MasterVolume.Subscribe(value =>
            {
                this.ConfigureAllSoundSources();
                this.ConfigureMusicSource();
                this.Logger.Debug($"Master volume set to {value}");
            });
            this.Config.MuteMaster.Subscribe(value =>
            {
                this.ConfigureAllSoundSources();
                this.ConfigureMusicSource();
                this.Logger.Debug(value ? "Master volume muted" : "Master volume unmuted");
            });
            this.ConfigureAllSoundSources();
            this.ConfigureMusicSource();
            this.Logger.Debug($"Audio config: {this.Config.ToJson()}");
        }

        public void PlaySoundOneShot(string name)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                soundSource.PlayOneShot(soundSource.clip);
                this.Logger.Debug($"Playing sound one shot {name}");
            }).Forget();
        }

        public void PlaySound(string name, bool force = false)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                if (!force && soundSource.isPlaying) return;
                soundSource.Play();
                this.Logger.Debug($"Playing sound {name}");
            }).Forget();
        }

        public void PlayMusic(string name, bool force = false)
        {
            if (!force && this.CurrentMusic == name) return;
            this.CurrentMusic = name;
            this.addressableManager.Load<AudioClip>(name).ContinueWith(audioClip =>
            {
                this.musicSource.clip = audioClip;
                this.musicSource.Play();
                this.Logger.Debug($"Playing music {name}");
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

        private UniTask<AudioSource> GetSoundSource(string name)
        {
            return this.spawnedSoundSource.GetOrAdd(name, () =>
            {
                return this.addressableManager.Load<AudioClip>(name).ContinueWith(audioClip =>
                {
                    var soundSource = this.pooledSoundSource.DequeueOrDefault(() => this.audioSourceContainer.AddComponent<AudioSource>());
                    this.ConfigureSoundSource(soundSource);
                    soundSource.clip = audioClip;
                    return soundSource;
                });
            });
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