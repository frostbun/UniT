namespace UniT.Audio
{
    using UniT.Data.Json.Player;
    using UniT.Utils;

    public class AudioConfig : IPlayerJsonData
    {
        public ReactiveProperty<float> SoundVolume { get; } = new(1f);

        public ReactiveProperty<float> MusicVolume { get; } = new(1f);

        public ReactiveProperty<float> MasterVolume { get; } = new(1f);

        public ReactiveProperty<bool> MuteSound { get; } = new(false);

        public ReactiveProperty<bool> MuteMusic { get; } = new(false);

        public ReactiveProperty<bool> MuteMaster { get; } = new(false);
    }
}