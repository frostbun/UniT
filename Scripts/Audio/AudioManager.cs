namespace UniT.Audio
{
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Initializables;
    using UniT.Logging;
    using UniT.ResourcesManager;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    #if UNIT_UNITASK
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

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
        public AudioManager(IAudioConfig config, IAssetsManager assetsManager, ILogger logger)
        {
            this.Config        = config;
            this.assetsManager = assetsManager;

            this.audioSourcesContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad();

            this.musicSource      = this.audioSourcesContainer.AddComponent<AudioSource>();
            this.musicSource.loop = true;

            this.pooledSoundSources = new();
            this.loadedSoundSources = new();

            this.logger = logger;
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

        #region Public

        public LogConfig    LogConfig    => this.logger.Config;
        public IAudioConfig Config       { get; }
        public string       CurrentMusic { get; private set; }

        public void LoadSounds(params string[] names)
        {
            names.ForEach(name => this.GetSoundSource(name));
        }

        #if UNIT_UNITASK
        public UniTask LoadSoundsAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return this.GetSoundSourceAsync(name, progress, cancellationToken);
        }

        public UniTask LoadSoundsAsync(string[] names, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return names.ForEachAsync(
                (name, progress, cancellationToken) => this.GetSoundSourceAsync(name, progress, cancellationToken),
                progress,
                cancellationToken
            );
        }
        #endif

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
            #if UNIT_UNITASK
            this.GetSoundSourceAsync(name).ContinueWith(Play).Forget();
            #else
            Play(this.GetSoundSource(name));
            #endif

            void Play(AudioSource soundSource)
            {
                soundSource.PlayOneShot(soundSource.clip);
                this.logger.Debug($"Playing sound one shot {name}");
            }
        }

        public void PlaySound(string name, bool loop = false, bool force = false)
        {
            #if UNIT_UNITASK
            this.GetSoundSourceAsync(name).ContinueWith(Play).Forget();
            #else
            Play(this.GetSoundSource(name));
            #endif

            void Play(AudioSource soundSource)
            {
                soundSource.loop = loop;
                if (!force && soundSource.isPlaying) return;
                soundSource.Play();
                this.logger.Debug($"Playing sound {name}, loop: {loop}");
            }
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

        public void LoadMusic(string name)
        {
            if (this.CurrentMusic == name) return;
            this.musicSource.clip = this.assetsManager.Load<AudioClip>(name);
            if (this.CurrentMusic != null) this.assetsManager.Unload(this.CurrentMusic);
            this.CurrentMusic = name;
        }

        #if UNIT_UNITASK
        public UniTask LoadMusicAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            if (this.CurrentMusic == name) return UniTask.CompletedTask;
            return this.assetsManager
                .LoadAsync<AudioClip>(name, progress, cancellationToken)
                .ContinueWith(audioClip =>
                {
                    if (this.CurrentMusic == name) return; // Another load request was made while this one was loading
                    if (this.CurrentMusic != null) this.assetsManager.Unload(this.CurrentMusic);
                    this.CurrentMusic     = name;
                    this.musicSource.clip = audioClip;
                });
        }
        #endif

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
            #if UNIT_UNITASK
            this.LoadMusicAsync(name).ContinueWith(Play).Forget();
            #else
            this.LoadMusic(name);
            Play();
            #endif

            void Play()
            {
                if (!force && this.musicSource.isPlaying) return;
                this.musicSource.Play();
                this.logger.Debug($"Playing music {name}");
            }
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

        private AudioSource GetSoundSource(string name)
        {
            return this.loadedSoundSources.GetOrAdd(name, () =>
            {
                var soundSource = this.pooledSoundSources.DequeueOrDefault(() =>
                {
                    var soundSource = this.audioSourcesContainer.AddComponent<AudioSource>();
                    this.ConfigureSoundSource(soundSource);
                    return soundSource;
                });
                soundSource.clip = this.assetsManager.Load<AudioClip>(name);
                this.logger.Debug($"Loaded sound {name}");
                return soundSource;
            });
        }

        #if UNIT_UNITASK
        private UniTask<AudioSource> GetSoundSourceAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return this.loadedSoundSources.GetOrAddAsync(name, () =>
                this.assetsManager
                    .LoadAsync<AudioClip>(name, progress, cancellationToken)
                    .ContinueWith(audioClip =>
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
                    })
            );
        }
        #endif

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