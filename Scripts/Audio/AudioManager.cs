namespace UniT.Audio
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Initializables;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public sealed class AudioManager : IAudioManager, IInitializable
    {
        #region Constructor

        private readonly IAssetsManager                  assetsManager;
        private readonly GameObject                      audioSourcesContainer;
        private readonly AudioSource                     musicSource;
        private readonly Queue<AudioSource>              pooledSoundSources;
        private readonly Dictionary<string, AudioSource> loadedSoundSources;
        private readonly ILogger                         logger;

        [Preserve]
        public AudioManager(IAudioConfig config, IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this.Config        = config;
            this.assetsManager = assetsManager ?? IAssetsManager.Default();

            this.audioSourcesContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad();

            this.musicSource      = this.audioSourcesContainer.AddComponent<AudioSource>();
            this.musicSource.loop = true;

            this.pooledSoundSources = new();
            this.loadedSoundSources = new();

            this.logger = logger ?? ILogger.Default(this);
            this.logger.Debug("Constructed");
        }

        void IInitializable.Initialize()
        {
            this.Config.SoundVolume.Subscribe(this.OnSoundVolumeChanged);
            this.Config.MuteSound.Subscribe(this.OnMuteSoundChanged);
            this.Config.MusicVolume.Subscribe(this.OnMusicVolumeChanged);
            this.Config.MuteMusic.Subscribe(this.OnMuteMusicChanged);
            this.Config.MasterVolume.Subscribe(this.OnMasterVolumeChanged);
            this.Config.MuteMaster.Subscribe(this.OnMuteMasterChanged);
            this.logger.Debug("Initialized");
        }

        #endregion

        #region Finalizer

        ~AudioManager()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        public void Dispose()
        {
            this.Config.SoundVolume.Unsubscribe(this.OnSoundVolumeChanged);
            this.Config.MuteSound.Unsubscribe(this.OnMuteSoundChanged);
            this.Config.MusicVolume.Unsubscribe(this.OnMusicVolumeChanged);
            this.Config.MuteMusic.Unsubscribe(this.OnMuteMusicChanged);
            this.Config.MasterVolume.Unsubscribe(this.OnMasterVolumeChanged);
            this.Config.MuteMaster.Unsubscribe(this.OnMuteMasterChanged);
            this.UnloadAllSounds();
            this.UnloadMusic();
            Object.Destroy(this.audioSourcesContainer);
            this.logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig    LogConfig    => this.logger.Config;
        public IAudioConfig Config       { get; }
        public string       CurrentMusic { get; private set; }

        public UniTask LoadSounds(params string[] names)
        {
            return UniTask.WhenAll(names.Select(this.GetSoundSource));
        }

        public void UnloadSounds(params string[] names)
        {
            names.ForEach(name =>
            {
                if (!this.loadedSoundSources.Remove(name, out var soundSource))
                {
                    this.logger.Warning($"Trying to unload sound {name} that was not loaded");
                    return;
                }
                soundSource.Stop();
                soundSource.clip = null;
                this.assetsManager.Unload(name);
                this.pooledSoundSources.Enqueue(soundSource);
                this.logger.Debug($"Unloaded sound {name}");
            });
        }

        public void UnloadAllSounds()
        {
            this.UnloadSounds(this.loadedSoundSources.Keys.ToArray());
        }

        public void PlaySoundOneShot(string name)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                soundSource.PlayOneShot(soundSource.clip);
                this.logger.Debug($"Playing sound one shot {name}");
            }).Forget();
        }

        public void PlaySound(string name, bool loop = false, bool force = false)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                soundSource.loop = loop;
                if (!force && soundSource.isPlaying) return;
                soundSource.Play();
                this.logger.Debug($"Playing sound {name}, loop: {loop}");
            }).Forget();
        }

        public void StopSounds(params string[] names)
        {
            names.ForEach(name =>
            {
                if (!this.loadedSoundSources.TryGetValue(name, out var soundSource))
                {
                    this.logger.Warning($"Trying to stop sound {name} that was not loaded");
                    return;
                }
                soundSource.Stop();
                this.logger.Debug($"Stopped sound {name}");
            });
        }

        public void StopAllSounds()
        {
            this.StopSounds(this.loadedSoundSources.Keys.ToArray());
        }

        public UniTask LoadMusic(string name)
        {
            if (this.CurrentMusic == name) return UniTask.CompletedTask;
            return this.assetsManager.Load<AudioClip>(name).ContinueWith(audioClip =>
            {
                if (this.CurrentMusic == name) return; // Another load request was made while this one was loading
                if (this.CurrentMusic != null) this.assetsManager.Unload(this.CurrentMusic);
                this.CurrentMusic     = name;
                this.musicSource.clip = audioClip;
            });
        }

        public void UnloadMusic()
        {
            if (this.CurrentMusic == null) return;
            this.StopMusic();
            this.musicSource.clip = null;
            this.assetsManager.Unload(this.CurrentMusic);
            this.CurrentMusic = null;
        }

        public void PlayMusic(string name, bool force = false)
        {
            this.LoadMusic(name).ContinueWith(() =>
            {
                if (!force && this.musicSource.isPlaying) return;
                this.musicSource.Play();
                this.logger.Debug($"Playing music {name}");
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

        #endregion

        #region Private

        private UniTask<AudioSource> GetSoundSource(string name)
        {
            return this.loadedSoundSources.GetOrAddAsync(name, () =>
            {
                return this.assetsManager.Load<AudioClip>(name).ContinueWith(audioClip =>
                {
                    var soundSource = this.pooledSoundSources.DequeueOrDefault(() =>
                    {
                        var soundSource = this.audioSourcesContainer.AddComponent<AudioSource>();
                        this.ConfigureSoundSource(soundSource);
                        return soundSource;
                    });
                    soundSource.clip = audioClip;
                    this.logger.Debug($"Loaded sound {name}");
                    return soundSource;
                });
            });
        }

        private void ConfigureSoundSource(AudioSource soundSource)
        {
            soundSource.volume = this.Config.SoundVolume.Value * this.Config.MasterVolume.Value;
            soundSource.mute   = this.Config.MuteSound.Value || this.Config.MuteMaster.Value;
        }

        private void ConfigureAllSoundSources()
        {
            this.loadedSoundSources.Values.ForEach(this.ConfigureSoundSource);
        }

        private void ConfigureMusicSource()
        {
            this.musicSource.volume = this.Config.MusicVolume.Value * this.Config.MasterVolume.Value;
            this.musicSource.mute   = this.Config.MuteMusic.Value || this.Config.MuteMaster.Value;
        }

        #endregion

        #region Events

        private void OnSoundVolumeChanged(float value)
        {
            this.ConfigureAllSoundSources();
            this.logger.Debug($"Sound volume set to {value}");
        }

        private void OnMuteSoundChanged(bool value)
        {
            this.ConfigureAllSoundSources();
            this.logger.Debug(value ? "Sound volume muted" : "Sound volume unmuted");
        }

        private void OnMusicVolumeChanged(float value)
        {
            this.ConfigureMusicSource();
            this.logger.Debug($"Music volume set to {value}");
        }

        private void OnMuteMusicChanged(bool value)
        {
            this.ConfigureMusicSource();
            this.logger.Debug(value ? "Music volume muted" : "Music volume unmuted");
        }

        private void OnMasterVolumeChanged(float value)
        {
            this.ConfigureAllSoundSources();
            this.ConfigureMusicSource();
            this.logger.Debug($"Master volume set to {value}");
        }

        private void OnMuteMasterChanged(bool value)
        {
            this.ConfigureAllSoundSources();
            this.ConfigureMusicSource();
            this.logger.Debug(value ? "Master volume muted" : "Master volume unmuted");
        }

        #endregion
    }
}