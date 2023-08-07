#if UNIT_FBINSTANT
namespace UniT.Advertisements
{
    public interface IFbInstantAdvertisementConfig
    {
        public string[] BannerAdIds { get; }

        public string[] InterstitialAdIds { get; }

        public string[] RewardedAdIds { get; }
    }
}
#endif