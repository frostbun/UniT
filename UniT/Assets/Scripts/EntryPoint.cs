namespace UniT.Example
{
    using UniT.Assets;
    using UniT.Audio;
    using UniT.Data;
    using UniT.Data.Csv;
    using UniT.Data.Json;
    using UniT.DependencyInjection;
    using UniT.Extensions;
    using UniT.ObjectPool;
    using UniT.UI;
    using UnityEngine;

    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;

        private void Awake()
        {
            #region ServiceProvider

            typeof(IPlayerData).GetDerivedTypes().ForEach(ServiceProvider.AddInterfacesAndSelf);
            typeof(IBlueprintData).GetDerivedTypes().ForEach(ServiceProvider.AddInterfacesAndSelf);

            ServiceProvider.AddInterfaces<AddressablesManager>();
            ServiceProvider.AddInterfaces<ObjectPoolManager>();
            ServiceProvider.AddInterfaces<AudioManager>();

            ServiceProvider.Add(new IPresenter.Factory(type => (IPresenter)ServiceProvider.Instantiate(type)));
            ServiceProvider.AddInterfaces(ServiceProvider.Invoke(this._uiManager, nameof(this._uiManager.Construct)));

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