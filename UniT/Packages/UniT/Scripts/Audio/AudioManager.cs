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
        public AudioConfig Config { get; }

        public ILogger Logger { get; }

        public string CurrentMusic { get; private set; }

        private readonly IAssetsManager                  assetsManager;
        private readonly GameObject                      audioSourcesContainer;
        private readonly AudioSource                     musicSource;
        private readonly Queue<AudioSource>              pooledSoundSources;
        private readonly Dictionary<string, AudioSource> loadedSoundSources;

        public AudioManager(AudioConfig config = null, IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this.Config                = config ?? new();
            this.assetsManager         = assetsManager ?? IAssetsManager.Factory.Default();
            this.audioSourcesContainer = new GameObject(this.GetType().Name).DontDestroyOnLoad();

            this.musicSource      = this.audioSourcesContainer.AddComponent<AudioSource>();
            this.musicSource.loop = true;

            this.pooledSoundSources = new();
            this.loadedSoundSources = new();

            this.Logger = logger ?? ILogger.Factory.Default(this.GetType().Name);
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

        public UniTask LoadSounds(params string[] names)
        {
            return UniTask.WhenAll(names.Select(this.GetSoundSource));
        }

        public void UnloadSounds(params string[] names)
        {
            names.ForEach(this.RecycleSoundSource);
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
            names.ForEach(this.StopSound);
        }

        public void StopAllSounds()
        {
            this.StopSounds(this.loadedSoundSources.Keys.ToArray());
        }

        public UniTask LoadMusic(string name)
        {
            return this.assetsManager.Load<AudioClip>(name);
        }

        public void PlayMusic(string name, bool force = false)
        {
            if (!force && this.CurrentMusic == name) return;
            this.StopMusic();
            if (this.CurrentMusic != name && this.CurrentMusic != null) this.assetsManager.Unload(this.CurrentMusic);
            this.CurrentMusic = name;
            this.assetsManager.Load<AudioClip>(name).ContinueWith(audioClip =>
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
            if (this.CurrentMusic == null) return;
            this.musicSource.Stop();
            this.Logger.Debug($"Stopped music {this.CurrentMusic}");
        }

        private UniTask<AudioSource> GetSoundSource(string name)
        {
            return this.loadedSoundSources.GetOrAdd(name, () =>
            {
                return this.assetsManager.Load<AudioClip>(name).ContinueWith(audioClip =>
                {
                    var soundSource = this.pooledSoundSources.DequeueOrDefault(() => this.audioSourcesContainer.AddComponent<AudioSource>());
                    this.ConfigureSoundSource(soundSource);
                    soundSource.clip = audioClip;
                    return soundSource;
                });
            });
        }

        private void StopSound(string name)
        {
            if (!this.loadedSoundSources.TryGetValue(name, out var soundSource))
            {
                this.Logger.Warning($"Trying to stop sound {name} that was not loaded");
                return;
            }

            soundSource.Stop();
            this.Logger.Debug($"Stopped sound {name}");
        }

        private void RecycleSoundSource(string name)
        {
            this.StopSound(name);

            if (!this.loadedSoundSources.Remove(name, out var soundSource))
            {
                this.Logger.Warning($"Trying to recycle sound {name} that was not loaded");
                return;
            }

            this.pooledSoundSources.Enqueue(soundSource);
            this.assetsManager.Unload(name);
            this.Logger.Debug($"Recycled sound source {name}");
        }

        private void ConfigureAllSoundSources()
        {
            this.loadedSoundSources.Values.ForEach(this.ConfigureSoundSource);
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