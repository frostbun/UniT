using Data.Player;
using UniT.Addressables;
using UniT.Audio;
using UniT.Data.Base;
using UniT.Data.Csv.Blueprint;
using UniT.Data.Json.Player;
using UniT.ObjectPool;
using UniT.UI;
using UniT.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

public class ServiceProvider : MonoBehaviour
{
    [FormerlySerializedAs("viewManager")]
    [SerializeField]
    private UIManager uiManager;

    private void Awake()
    {
        #region ServiceProvider

        var addressableManager = new AddressableManager();
        ServiceProvider<IAddressableManager>.Add(addressableManager);

        var objectPoolManager = new ObjectPoolManager(addressableManager);
        ServiceProvider<IObjectPoolManager>.Add(objectPoolManager);

        var audioConfig = new AudioConfig();

        var dataManager = new DataManager(
            new IData[] { audioConfig },
            new IDataHandler[]
            {
                new PlayerPrefsJsonDataHandler(),
                new BlueprintAddressableCsvDataHandler(addressableManager),
            }
        );
        ServiceProvider<IDataManager>.Add(dataManager);

        var audioManager = new AudioManager(audioConfig, addressableManager);
        ServiceProvider<IAudioManager>.Add(audioManager);
        ServiceProvider<IInitializable>.Add(audioManager);

        this.uiManager.Construct(addressableManager);
        ServiceProvider<IUIManager>.Add(this.uiManager);

        #endregion
    }

    private void Start()
    {
        Destroy(this.gameObject);
    }
}