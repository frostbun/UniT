namespace UniT.Example
{
    using UniT.Assets;
    using UniT.Audio;
    using UniT.Data.Base;
    using UniT.Data.Csv.Blueprint;
    using UniT.Data.Json.Player;
    using UniT.DependencyInjection;
    using UniT.Extensions;
    using UniT.ObjectPool;
    using UniT.UI;
    using UniT.UI.Interfaces;
    using UniT.UI.Item.Interfaces;
    using UnityEngine;

    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private UIManager uiManager;

        private void Awake()
        {
            #region ServiceProvider

            typeof(IPlayerData).GetDerivedTypes().ForEach(ServiceProvider.AddInterfacesAndSelf);
            typeof(IBlueprintData).GetDerivedTypes().ForEach(ServiceProvider.AddInterfacesAndSelf);

            ServiceProvider.AddInterfaces<AddressablesManager>();
            ServiceProvider.AddInterfaces<ObjectPoolManager>();
            ServiceProvider.AddInterfaces<AudioManager>();

            ServiceProvider.Add(new IPresenter.Factory(type => (IPresenter)ServiceProvider.Instantiate(type)));
            ServiceProvider.Add(new IItemPresenter.Factory(type => (IItemPresenter)ServiceProvider.Instantiate(type)));
            ServiceProvider.AddInterfaces(ServiceProvider.Invoke(this.uiManager, nameof(this.uiManager.Construct)));

            ServiceProvider.AddInterfaces<PlayerPrefsJsonDataHandler>();
            ServiceProvider.AddInterfaces<BlueprintAssetCsvDataHandler>();

            ServiceProvider.AddInterfaces<DataManager>();

            #endregion
        }

        private void Start()
        {
            Destroy(this.gameObject);
        }
    }
}