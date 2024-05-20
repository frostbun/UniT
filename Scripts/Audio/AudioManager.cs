namespace UniT.Audio
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.ResourcesManager;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class AudioManager : IAudioManager
    {
        #region Constructor

        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly GameObject  audioSourcesContainer;
        private readonly AudioSource musicSource;

        private readonly HashSet<AudioSource>            registeredSoundSources = new HashSet<AudioSource>();
        private readonly Queue<AudioSource>              pooledSoundSources     = new Queue<AudioSource>();
        private readonly Dictionary<string, AudioSource> loadedSoundSources     = new Dictionary<string, AudioSource>();

        [Preserve]
        public AudioManager(IAssetsManager assetsManager, ILoggerManager loggerManager)
        {
            this.assetsManager = assetsManager;
            this.logger        = loggerManager.GetLogger(this);

            this.audioSourcesContainer = new GameObject(nameof(AudioManager)).DontDestroyOnLoad();
            this.musicSource           = this.audioSourcesContainer.AddComponent<AudioSource>();
            this.musicSource.loop      = true;

            this.logger.Debug("Constructed");
        }

        #endregion

        #region Configs

        float IAudioManager.SoundVolume
        {
            get => this.soundVolume;
            set
            {
                this.soundVolume = value;
                this.ConfigureAllSoundSources();
                this.logger.Debug($"Sound volume set to {value}");
            }
        }

        float IAudioManager.MusicVolume
        {
            get => this.musicVolume;
            set
            {
                this.musicVolume = value;
                this.ConfigureMusicSource();
                this.logger.Debug($"Music volume set to {value}");
            }
        }

        float IAudioManager.MasterVolume
        {
            get => this.masterVolume;
            set
            {
                this.masterVolume = value;
                this.ConfigureAllSoundSources();
                this.ConfigureMusicSource();
                this.logger.Debug($"Master volume set to {value}");
            }
        }

        bool IAudioManager.MuteSound
        {
            get => this.muteSound;
            set
            {
                this.muteSound = value;
                this.ConfigureAllSoundSources();
                this.logger.Debug(value ? "Sound volume muted" : "Sound volume unmuted");
            }
        }

        bool IAudioManager.MuteMusic
        {
            get => this.muteMusic;
            set
            {
                this.muteMusic = value;
                this.ConfigureMusicSource();
                this.logger.Debug(value ? "Music volume muted" : "Music volume unmuted");
            }
        }

        bool IAudioManager.MuteMaster
        {
            get => this.muteMaster;
            set
            {
                this.muteMaster = value;
                this.ConfigureAllSoundSources();
                this.ConfigureMusicSource();
                this.logger.Debug(value ? "Master volume muted" : "Master volume unmuted");
            }
        }

        private float soundVolume;

        private float musicVolume;

        private float masterVolume;

        private bool muteSound;

        private bool muteMusic;

        private bool muteMaster;

        private void ConfigureAllSoundSources()
        {
            this.registeredSoundSources.ForEach(this.ConfigureSoundSource);
            this.loadedSoundSources.ForEach(this.ConfigureSoundSource);
        }

        private void ConfigureSoundSource(AudioSource soundSource)
        {
            soundSource.volume = this.soundVolume * this.masterVolume;
            soundSource.mute   = this.muteSound || this.muteMaster;
        }

        private void ConfigureMusicSource()
        {
            this.musicSource.volume = this.musicVolume * this.masterVolume;
            this.musicSource.mute   = this.muteMusic || this.muteSound;
        }

        #endregion

        #region Sound

        void IAudioManager.RegisterSound(AudioSource soundSource)
        {
            this.ConfigureSoundSource(soundSource);
            this.registeredSoundSources.Add(soundSource);
        }

        void IAudioManager.UnregisterSound(AudioSource soundSource)
        {
            this.registeredSoundSources.Remove(soundSource);
        }

        void IAudioManager.LoadSound(string name) => this.LoadSound(name);

        void IAudioManager.PlaySoundOneShot(string name)
        {
            var soundSource = this.LoadSound(name);
            soundSource.PlayOneShot(soundSource.clip);
            this.logger.Debug($"Playing sound one shot {name}");
        }

        void IAudioManager.PlaySound(string name, bool loop, bool force)
        {
            var soundSource = this.LoadSound(name);
            soundSource.loop = loop;
            if (!force && soundSource.isPlaying) return;
            soundSource.Play();
            this.logger.Debug($"Playing sound {name}, loop: {loop}");
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

        void IAudioManager.StopAllSounds()
        {
            this.loadedSoundSources.ForEach(soundSource => soundSource.Stop());
            this.logger.Debug("Stopped all sounds");
        }

        void IAudioManager.UnloadSound(string name)
        {
            if (!this.loadedSoundSources.TryRemove(name, out var soundSource))
            {
                this.logger.Warning($"Trying to unload sound {name} that was not loaded");
                return;
            }
            this.UnloadSound(name, soundSource);
        }

        void IAudioManager.UnloadAllSounds() => this.UnloadAllSounds();

        private AudioSource LoadSound(string name)
        {
            return this.loadedSoundSources.GetOrAdd(name, () =>
                this.SpawnSoundSource(this.assetsManager.Load<AudioClip>(name))
            );
        }

        private void UnloadSound(string name, AudioSource soundSource)
        {
            soundSource.Stop();
            soundSource.clip = null;
            this.assetsManager.Unload(name);
            this.pooledSoundSources.Enqueue(soundSource);
        }

        private void UnloadAllSounds()
        {
            this.loadedSoundSources.Clear(this.UnloadSound);
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
            return soundSource;
        }

        #endregion

        #region Music

        string IAudioManager.CurrentMusic => this.currentMusic;

        float IAudioManager.MusicTime { get => this.musicSource.time; set => this.musicSource.time = value; }

        private string currentMusic;

        void IAudioManager.LoadMusic(string name) => this.LoadMusic(name);

        void IAudioManager.PlayMusic(string name, bool force)
        {
            this.LoadMusic(name);
            if (!force && this.musicSource.isPlaying) return;
            this.musicSource.Play();
            this.logger.Debug($"Playing music {name}");
        }

        void IAudioManager.PauseMusic() => this.musicSource.Pause();

        void IAudioManager.ResumeMusic() => this.musicSource.UnPause();

        void IAudioManager.StopMusic() => this.musicSource.Stop();

        void IAudioManager.UnloadMusic() => this.UnloadMusic();

        private void LoadMusic(string name)
        {
            if (this.currentMusic == name) return;
            this.musicSource.clip = this.assetsManager.Load<AudioClip>(name);
            if (this.currentMusic != null) this.assetsManager.Unload(this.currentMusic);
            this.currentMusic = name;
        }

        private void UnloadMusic()
        {
            if (this.currentMusic == null) return;
            this.musicSource.Stop();
            this.musicSource.clip = null;
            this.assetsManager.Unload(this.currentMusic);
            this.currentMusic = null;
        }

        #endregion

        #region Async

        private bool isMusicLoading;

        #if UNIT_UNITASK
        UniTask IAudioManager.LoadSoundAsync(string name, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.loadedSoundSources.TryAddAsync(name, () =>
                this.assetsManager.LoadAsync<AudioClip>(name, progress, cancellationToken)
                    .ContinueWith(this.SpawnSoundSource)
            );
        }

        UniTask IAudioManager.LoadMusicAsync(string name, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(() => !this.isMusicLoading, cancellationToken: cancellationToken)
                .ContinueWith(() =>
                {
                    if (this.currentMusic == name) return UniTask.CompletedTask;
                    this.isMusicLoading = true;
                    return this.assetsManager.LoadAsync<AudioClip>(name, progress, cancellationToken)
                        .ContinueWith(audioClip =>
                        {
                            this.musicSource.clip = audioClip;
                            if (this.currentMusic != null) this.assetsManager.Unload(this.currentMusic);
                            this.currentMusic   = name;
                            this.isMusicLoading = false;
                        });
                });
        }
        #else
        IEnumerator IAudioManager.LoadSoundAsync(string name, Action callback, IProgress<float> progress)
        {
            return this.loadedSoundSources.TryAddAsync(
                name,
                callback => this.assetsManager.LoadAsync<AudioClip>(
                    name,
                    audioClip => callback(this.SpawnSoundSource(audioClip)),
                    progress
                ),
                _ => callback?.Invoke()
            );
        }

        IEnumerator IAudioManager.LoadMusicAsync(string name, Action callback, IProgress<float> progress)
        {
            yield return new WaitUntil(() => !this.isMusicLoading);
            if (this.currentMusic == name)
            {
                callback?.Invoke();
                yield break;
            }
            this.isMusicLoading = true;
            yield return this.assetsManager.LoadAsync<AudioClip>(
                name,
                audioClip =>
                {
                    this.musicSource.clip = audioClip;
                    if (this.currentMusic != null) this.assetsManager.Unload(this.currentMusic);
                    this.currentMusic = name;
                    this.isMusicLoading = false;
                    callback?.Invoke();
                },
                progress
            );
        }
        #endif

        #endregion
    }
}