using System.Linq;
using UniT.Assets;
using UniT.Audio;
using UniT.Data.Base;
using UniT.Extensions;
using UniT.ObjectPool;
using UniT.UI;
using UniT.UI.Interfaces;
using UniT.UI.Item.Interfaces;
using UniT.Utilities;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager;

    private void Awake()
    {
        #region ServiceProvider

        ServiceProvider.Add(new IPresenter.Factory(type => (IPresenter)ServiceProvider.Instantiate(type)));
        ServiceProvider.Add(new IItemPresenter.Factory(type => (IItemPresenter)ServiceProvider.Instantiate(type)));

        typeof(IPlayerData).GetDerivedTypes().ForEach(ServiceProvider.AddInterfacesAndSelf);
        typeof(IBlueprintData).GetDerivedTypes().ForEach(ServiceProvider.AddInterfacesAndSelf);

        ServiceProvider.AddInterfaces<AddressablesManager>();
        ServiceProvider.AddInterfaces<ObjectPoolManager>();
        ServiceProvider.AddInterfaces<AudioManager>();
        ServiceProvider.AddInterfaces(ServiceProvider.Invoke(this.uiManager, nameof(this.uiManager.Construct)));

        typeof(IDataHandler).GetDerivedTypes().ForEach(ServiceProvider.AddInterfaces);
        ServiceProvider.AddInterfaces<DataManager>();

        FindObjectsOfType<MonoBehaviour>().OfType<IInitializable>().ForEach(ServiceProvider.Add);
        FindObjectsOfType<MonoBehaviour>().OfType<IAsyncInitializable>().ForEach(ServiceProvider.Add);

        #endregion
    }

    private void Start()
    {
        Destroy(this.gameObject);
    }
}