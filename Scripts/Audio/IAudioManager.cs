#nullable enable
namespace UniT.Audio
{
    using System;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IAudioManager
    {
        #region Configs

        public float SoundVolume { get; set; }

        public float MusicVolume { get; set; }

        public float MasterVolume { get; set; }

        public bool MuteSound { get; set; }

        public bool MuteMusic { get; set; }

        public bool MuteMaster { get; set; }

        #endregion

        #region Sound

        public void RegisterSound(AudioSource soundSource);

        public void UnregisterSound(AudioSource soundSource);

        public void LoadSound(string name);

        public void PlaySoundOneShot(string name);

        public void PlaySound(string name, bool loop = false, bool force = false);

        public void StopSound(string name);

        public void StopAllSounds();

        public void UnloadSound(string name);

        public void UnloadAllSounds();

        #endregion

        #region Music

        public string? CurrentMusic { get; }

        public float MusicTime { get; set; }

        public void LoadMusic(string name);

        public void PlayMusic(string name, bool force = false);

        public void PauseMusic();

        public void ResumeMusic();

        public void StopMusic();

        public void UnloadMusic();

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask LoadSoundAsync(string name, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask LoadMusicAsync(string name, IProgress<float>? progress = null, CancellationToken cancellationToken = default);
        #else
        public IEnumerator LoadSoundAsync(string name, Action? callback = null, IProgress<float>? progress = null);

        public IEnumerator LoadMusicAsync(string name, Action? callback = null, IProgress<float>? progress = null);
        #endif

        #endregion
    }
}