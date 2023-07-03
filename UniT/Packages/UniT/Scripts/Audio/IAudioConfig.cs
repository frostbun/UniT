namespace UniT.Audio
{
    using UniT.Reactive;

    public interface IAudioConfig
    {
        public ReactiveProperty<float> SoundVolume { get; }

        public ReactiveProperty<float> MusicVolume { get; }

        public ReactiveProperty<float> MasterVolume { get; }

        public ReactiveProperty<bool> MuteSound { get; }

        public ReactiveProperty<bool> MuteMusic { get; }

        public ReactiveProperty<bool> MuteMaster { get; }
    }
}