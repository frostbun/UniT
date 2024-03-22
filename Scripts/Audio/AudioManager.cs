namespace UniT.Audio
{
    using System.Collections.Generic;
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

    public sealed class AudioManager : IAudioManager, IHasLogger
    {
        #region Constructor

        private readonly IAudioConfig   config;
        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly GameObject  audioSourcesContainer;
        private readonly AudioSource musicSource;

        private readonly Queue<AudioSource>              pooledSoundSources = new Queue<AudioSource>();
        private readonly Dictionary<string, AudioSource> loadedSoundSources = new Dictionary<string, AudioSource>();

        [Preserve]
        public AudioManager(IAudioConfig config, IAssetsManager assetsManager, ILogger.IFactory loggerFactory)
        {
            this.config        = config;
            this.assetsManager = assetsManager;
            this.logger        = loggerFactory.Create(this);

            this.audioSourcesContainer = new GameObject(nameof(AudioManager)).DontDestroyOnLoad();
            this.musicSource           = this.audioSourcesContainer.AddComponent<AudioSource>();
            this.musicSource.loop      = true;

            this.logger.Debug("Constructed");
        }

        void IInitializable.Initialize()
        {
            this.config.SoundVolume.Subscribe(this.OnSoundVolumeChanged);
            this.config.MuteSound.Subscribe(this.OnMuteSoundChanged);
            this.config.MusicVolume.Subscribe(this.OnMusicVolumeChanged);
            this.config.MuteMusic.Subscribe(this.OnMuteMusicChanged);
            this.config.MasterVolume.Subscribe(this.OnMasterVolumeChanged);
            this.config.MuteMaster.Subscribe(this.OnMuteMasterChanged);
            this.logger.Debug("Initialized");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        IAudioConfig IAudioManager.Config => this.config;

        #region Sound

        void IAudioManager.LoadSound(string name) => this.LoadSound(name);

        void IAudioManager.PlaySoundOneShot(string name)
        {
            #if UNIT_UNITASK
            this.LoadSoundAsync(name).ContinueWith(Play).Forget();
            #else
            Play(this.LoadSound(name));
            #endif

            void Play(AudioSource soundSource)
            {
                soundSource.PlayOneShot(soundSource.clip);
                this.logger.Debug($"Playing sound one shot {name}");
            }
        }

        void IAudioManager.PlaySound(string name, bool loop, bool force)
        {
            #if UNIT_UNITASK
            this.LoadSoundAsync(name).ContinueWith(Play).Forget();
            #else
            Play(this.LoadSound(name));
            #endif

            void Play(AudioSource soundSource)
            {
                soundSource.loop = loop;
                if (!force && soundSource.isPlaying) return;
                soundSource.Play();
                this.logger.Debug($"Playing sound {name}, loop: {loop}");
            }
        }

        void IAudioManager.StopSound(string name)
        {
            if (!this.loadedSoundSources.TryGetValue(name, out var soundSource))
            {
                this.logger.Warning($"Trying to stop sound {name} that was not loaded");
                return;
            }
            soundSource.Stop();
            this.logger.Debug($"Stopped sound {name}");
        }

        void IAudioManager.UnloadSound(string name)
        {
            if (!this.loadedSoundSources.TryRemove(name, out var soundSource))
            {
                this.logger.Warning($"Trying to unload sound {name} that was not loaded");
                return;
            }
            soundSource.Stop();
            soundSource.clip = null;
            this.assetsManager.Unload(name);
            this.pooledSoundSources.Enqueue(soundSource);
            this.logger.Debug($"Unloaded sound {name}");
        }

        private AudioSource LoadSound(string name)
        {
            return this.loadedSoundSources.GetOrAdd(name, () =>
                this.SpawnSoundSource(this.assetsManager.Load<AudioClip>(name))
            );
        }

        private AudioSource SpawnSoundSource(AudioClip audioClip)
        {
            var soundSource = this.pooledSoundSources.DequeueOrDefault(() =>
            {
                var soundSource = this.audioSourcesContainer.AddComponent<AudioSource>();
                this.ConfigureSoundSource(soundSource);
                return soundSource;
            });
            soundSource.clip = audioClip;
            this.logger.Debug($"Loaded sound {audioClip.name}");
            return soundSource;
        }

        #endregion

        #region Music

        string IAudioManager.CurrentMusic => this.currentMusic;

        private string currentMusic;

        void IAudioManager.LoadMusic(string name) => this.LoadMusic(name);

        void IAudioManager.PlayMusic(string name, bool force)
        {
            #if UNIT_UNITASK
            this.LoadMusicAsync(name).ContinueWith(Play).Forget();
            #else
            Play(this.LoadMusic(name));
            #endif

            void Play(AudioSource musicSource)
            {
                if (!force && musicSource.isPlaying) return;
                musicSource.Play();
                this.logger.Debug($"Playing music {name}");
            }
        }

        void IAudioManager.PauseMusic() => this.musicSource.Pause();

        void IAudioManager.ResumeMusic() => this.musicSource.UnPause();

        void IAudioManager.StopMusic() => this.musicSource.Stop();

        void IAudioManager.UnloadMusic()
        {
            if (this.currentMusic == null) return;
            this.musicSource.Stop();
            this.musicSource.clip = null;
            this.assetsManager.Unload(this.currentMusic);
            this.currentMusic = null;
        }

        private AudioSource LoadMusic(string name)
        {
            if (this.currentMusic != name)
            {
                this.musicSource.clip = this.assetsManager.Load<AudioClip>(name);
                if (this.currentMusic != null) this.assetsManager.Unload(this.currentMusic);
                this.currentMusic = name;
            }
            return this.musicSource;
        }

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask IAudioManager.LoadSoundAsync(string name, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadSoundAsync(name, progress, cancellationToken);

        private async UniTask<AudioSource> LoadSoundAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            return await this.loadedSoundSources.GetOrAddAsync(name, async () =>
                this.SpawnSoundSource(await this.assetsManager.LoadAsync<AudioClip>(name, progress, cancellationToken))
            );
        }

        private bool isMusicLoading;

        UniTask IAudioManager.LoadMusicAsync(string name, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadMusicAsync(name, progress, cancellationToken);

        private async UniTask<AudioSource> LoadMusicAsync(string name, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            await UniTask.WaitUntil(() => !this.isMusicLoading, cancellationToken: cancellationToken);
            if (this.currentMusic != name)
            {
                this.isMusicLoading   = true;
                this.musicSource.clip = await this.assetsManager.LoadAsync<AudioClip>(name, progress, cancellationToken);
                if (this.currentMusic != null) this.assetsManager.Unload(this.currentMusic);
                this.currentMusic   = name;
                this.isMusicLoading = false;
            }
            return this.musicSource;
        }
        #endif

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

        #region Configure

        private void ConfigureAllSoundSources()
        {
            this.loadedSoundSources.Values.ForEach(this.ConfigureSoundSource);
        }

        private void ConfigureSoundSource(AudioSource soundSource)
        {
            soundSource.volume = this.config.SoundVolume.Value * this.config.MasterVolume.Value;
            soundSource.mute   = this.config.MuteSound.Value || this.config.MuteMaster.Value;
        }

        private void ConfigureMusicSource()
        {
            this.musicSource.volume = this.config.MusicVolume.Value * this.config.MasterVolume.Value;
            this.musicSource.mute   = this.config.MuteMusic.Value || this.config.MuteMaster.Value;
        }

        #endregion
    }
}