namespace UniT.Audio
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public class AudioManager : IAudioManager, IInitializable
    {
        #region Constructor

        private readonly IAssetManager                   _assetManager;
        private readonly GameObject                      _audioSourcesContainer;
        private readonly AudioSource                     _musicSource;
        private readonly Queue<AudioSource>              _pooledSoundSources;
        private readonly Dictionary<string, AudioSource> _loadedSoundSources;
        private readonly ILogger                         _logger;

        [Preserve]
        public AudioManager(IAudioConfig config, IAssetManager assetManager = null, ILogger logger = null)
        {
            this.Config        = config;
            this._assetManager = assetManager ?? IAssetManager.Default();

            this._audioSourcesContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad();

            this._musicSource      = this._audioSourcesContainer.AddComponent<AudioSource>();
            this._musicSource.loop = true;

            this._pooledSoundSources = new();
            this._loadedSoundSources = new();

            this._logger = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Info("Constructed");
        }

        void IInitializable.Initialize()
        {
            this.Config.SoundVolume.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                this._logger.Debug($"Sound volume set to {value}");
            });

            this.Config.MuteSound.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                this._logger.Debug(value ? "Sound volume muted" : "Sound volume unmuted");
            });

            this.Config.MusicVolume.Subscribe(value =>
            {
                ConfigureMusicSource();
                this._logger.Debug($"Music volume set to {value}");
            });

            this.Config.MuteMusic.Subscribe(value =>
            {
                ConfigureMusicSource();
                this._logger.Debug(value ? "Music volume muted" : "Music volume unmuted");
            });

            this.Config.MasterVolume.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                ConfigureMusicSource();
                this._logger.Debug($"Master volume set to {value}");
            });

            this.Config.MuteMaster.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                ConfigureMusicSource();
                this._logger.Debug(value ? "Master volume muted" : "Master volume unmuted");
            });

            this._logger.Debug("Initialized");
            return;

            void ConfigureAllSoundSources()
            {
                this._loadedSoundSources.Values.ForEach(this.ConfigureSoundSource);
            }

            void ConfigureMusicSource()
            {
                this._musicSource.volume = this.Config.MusicVolume.Value * this.Config.MasterVolume.Value;
                this._musicSource.mute   = this.Config.MuteMusic.Value || this.Config.MuteMaster.Value;
            }
        }

        #endregion

        #region Public

        public LogConfig    LogConfig    => this._logger.Config;
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
                if (!this._loadedSoundSources.Remove(name, out var soundSource))
                {
                    this._logger.Warning($"Trying to unload sound {name} that was not loaded");
                    return;
                }
                soundSource.Stop();
                soundSource.clip = null;
                this._assetManager.Unload(name);
                this._pooledSoundSources.Enqueue(soundSource);
                this._logger.Debug($"Unloaded sound {name}");
            });
        }

        public void PlaySoundOneShot(string name)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                soundSource.PlayOneShot(soundSource.clip);
                this._logger.Debug($"Playing sound one shot {name}");
            }).Forget();
        }

        public void PlaySound(string name, bool loop = false, bool force = false)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                soundSource.loop = loop;
                if (!force && soundSource.isPlaying) return;
                soundSource.Play();
                this._logger.Debug($"Playing sound {name}, loop: {loop}");
            }).Forget();
        }

        public void StopSounds(params string[] names)
        {
            names.ForEach(name =>
            {
                if (!this._loadedSoundSources.TryGetValue(name, out var soundSource))
                {
                    this._logger.Warning($"Trying to stop sound {name} that was not loaded");
                    return;
                }
                soundSource.Stop();
                this._logger.Debug($"Stopped sound {name}");
            });
        }

        public void StopAllSounds()
        {
            this.StopSounds(this._loadedSoundSources.Keys.ToArray());
        }

        public UniTask LoadMusic(string name)
        {
            if (this.CurrentMusic == name) return UniTask.CompletedTask;
            return this._assetManager.Load<AudioClip>(name).ContinueWith(audioClip =>
            {
                if (this.CurrentMusic == name) return; // Another load request was made while this one was loading
                if (this.CurrentMusic != null) this._assetManager.Unload(this.CurrentMusic);
                this.CurrentMusic      = name;
                this._musicSource.clip = audioClip;
            });
        }

        public void PlayMusic(string name, bool force = false)
        {
            this.LoadMusic(name).ContinueWith(() =>
            {
                if (!force && this._musicSource.isPlaying) return;
                this._musicSource.Play();
                this._logger.Debug($"Playing music {name}");
            }).Forget();
        }

        public void PauseMusic()
        {
            this._musicSource.Pause();
        }

        public void ResumeMusic()
        {
            this._musicSource.UnPause();
        }

        public void StopMusic()
        {
            this._musicSource.Stop();
        }

        #endregion

        #region Private

        private UniTask<AudioSource> GetSoundSource(string name)
        {
            return this._loadedSoundSources.GetOrAdd(name, () =>
            {
                return this._assetManager.Load<AudioClip>(name).ContinueWith(audioClip =>
                {
                    var soundSource = this._pooledSoundSources.DequeueOrDefault(() =>
                    {
                        var soundSource = this._audioSourcesContainer.AddComponent<AudioSource>();
                        this.ConfigureSoundSource(soundSource);
                        return soundSource;
                    });
                    soundSource.clip = audioClip;
                    this._logger.Debug($"Loaded sound {name}");
                    return soundSource;
                });
            });
        }

        private void ConfigureSoundSource(AudioSource soundSource)
        {
            soundSource.volume = this.Config.SoundVolume.Value * this.Config.MasterVolume.Value;
            soundSource.mute   = this.Config.MuteSound.Value || this.Config.MuteMaster.Value;
        }

        #endregion
    }
}