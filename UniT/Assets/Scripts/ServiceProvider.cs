using UniT.Assets;
using UniT.Audio;
using UniT.Data.Base;
using UniT.Data.Csv.Blueprint;
using UniT.Data.Json.Player;
using UniT.ObjectPool;
using UniT.UI;
using UniT.Utilities;
using UnityEngine;

public class ServiceProvider : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager;

    private void Awake()
    {
        #region ServiceProvider

        ServiceProvider<IAssetsManager>.Add(new AddressablesManager());

        ServiceProvider<IObjectPoolManager>.Add(new ObjectPoolManager());

        this.uiManager.Construct();
        ServiceProvider<IUIManager>.Add(this.uiManager);

        var audioManager = new AudioManager();
        ServiceProvider<IAudioManager>.Add(audioManager);
        ServiceProvider<IInitializable>.Add(audioManager);

        ServiceProvider<IDataManager>.Add(
            new DataManager(
                new IData[]
                {
                    audioManager.Config,
                },
                new IDataHandler[]
                {
                    new PlayerPrefsJsonDataHandler(),
                    new BlueprintAddressableCsvDataHandler(),
                }
            )
        );

        #endregion
    }

    private void Start()
    {
        Destroy(this.gameObject);
    }
}