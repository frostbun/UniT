namespace UniT.Example.Data.Player
{
    using UniT.Audio;
    using UniT.Data.Json;
    using UniT.Reactive;

    public class AudioConfig : IAudioConfig, IPlayerJsonData
    {
        public ReactiveProperty<float> SoundVolume  { get; } = new(.75f);
        public ReactiveProperty<float> MusicVolume  { get; } = new(.75f);
        public ReactiveProperty<float> MasterVolume { get; } = new(.75f);
        public ReactiveProperty<bool>  MuteSound    { get; } = new();
        public ReactiveProperty<bool>  MuteMusic    { get; } = new();
        public ReactiveProperty<bool>  MuteMaster   { get; } = new();
    }
}