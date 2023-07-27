namespace UniT.Audio
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.Extensions;
    using UniT.Extensions.UniTask;
    using UniT.Utilities;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public class AudioManager : IAudioManager, IInitializable
    {
        public IAudioConfig Config { get; }

        public ILogger Logger { get; }

        public string CurrentMusic { get; private set; }

        private readonly IAssetsManager                  assetsManager;
        private readonly GameObject                      audioSourcesContainer;
        private readonly AudioSource                     musicSource;
        private readonly Queue<AudioSource>              pooledSoundSources;
        private readonly Dictionary<string, AudioSource> loadedSoundSources;

        public AudioManager(IAudioConfig config, IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this.Config                = config;
            this.assetsManager         = assetsManager ?? IAssetsManager.Default();
            this.audioSourcesContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad();

            this.musicSource      = this.audioSourcesContainer.AddComponent<AudioSource>();
            this.musicSource.loop = true;

            this.pooledSoundSources = new();
            this.loadedSoundSources = new();

            this.Logger = logger ?? ILogger.Default(this.GetType().Name);
        }

        public void Initialize()
        {
            void ConfigureAllSoundSources()
            {
                this.loadedSoundSources.Values.ForEach(this.ConfigureSoundSource);
            }

            void ConfigureMusicSource()
            {
                this.musicSource.volume = this.Config.MusicVolume.Value * this.Config.MasterVolume.Value;
                this.musicSource.mute   = this.Config.MuteMusic.Value || this.Config.MuteMaster.Value;
            }

            this.Config.SoundVolume.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                this.Logger.Debug($"Sound volume set to {value}");
            });

            this.Config.MuteSound.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                this.Logger.Debug(value ? "Sound volume muted" : "Sound volume unmuted");
            });

            this.Config.MusicVolume.Subscribe(value =>
            {
                ConfigureMusicSource();
                this.Logger.Debug($"Music volume set to {value}");
            });

            this.Config.MuteMusic.Subscribe(value =>
            {
                ConfigureMusicSource();
                this.Logger.Debug(value ? "Music volume muted" : "Music volume unmuted");
            });

            this.Config.MasterVolume.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                ConfigureMusicSource();
                this.Logger.Debug($"Master volume set to {value}");
            });

            this.Config.MuteMaster.Subscribe(value =>
            {
                ConfigureAllSoundSources();
                ConfigureMusicSource();
                this.Logger.Debug(value ? "Master volume muted" : "Master volume unmuted");
            });

            ConfigureAllSoundSources();
            ConfigureMusicSource();
            this.Logger.Debug($"Audio config: {this.Config.ToJson()}");
        }

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
                    this.Logger.Warning($"Trying to unload sound {name} that was not loaded");
                    return;
                }
                soundSource.Stop();
                soundSource.clip = null;
                this.assetsManager.Unload(name);
                this.pooledSoundSources.Enqueue(soundSource);
                this.Logger.Debug($"Unloaded sound {name}");
            });
        }

        public void PlaySoundOneShot(string name)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                soundSource.PlayOneShot(soundSource.clip);
                this.Logger.Debug($"Playing sound one shot {name}");
            }).Forget();
        }

        public void PlaySound(string name, bool loop = false, bool force = false)
        {
            this.GetSoundSource(name).ContinueWith(soundSource =>
            {
                soundSource.loop = loop;
                if (!force && soundSource.isPlaying) return;
                soundSource.Play();
                this.Logger.Debug($"Playing sound {name}, loop: {loop}");
            }).Forget();
        }

        public void StopSounds(params string[] names)
        {
            names.ForEach(name =>
            {
                if (!this.loadedSoundSources.TryGetValue(name, out var soundSource))
                {
                    this.Logger.Warning($"Trying to stop sound {name} that was not loaded");
                    return;
                }
                soundSource.Stop();
                this.Logger.Debug($"Stopped sound {name}");
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

        public void PlayMusic(string name, bool force = false)
        {
            this.LoadMusic(name).ContinueWith(() =>
            {
                if (!force && this.musicSource.isPlaying) return;
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
            return this.loadedSoundSources.GetOrAdd(name, () =>
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
                    this.Logger.Debug($"Loaded sound {name}");
                    return soundSource;
                });
            });
        }

        private void ConfigureSoundSource(AudioSource soundSource)
        {
            soundSource.volume = this.Config.SoundVolume.Value * this.Config.MasterVolume.Value;
            soundSource.mute   = this.Config.MuteSound.Value || this.Config.MuteMaster.Value;
        }
    }
}