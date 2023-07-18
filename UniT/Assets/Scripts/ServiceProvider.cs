using UniT.Assets;
using UniT.Audio;
using UniT.Data.Base;
using UniT.Data.Csv.Blueprint;
using UniT.Data.Json.Blueprint;
using UniT.Data.Json.Player;
using UniT.ObjectPool;
using UniT.UI;
using UniT.Utilities;
using UnityEngine;
using Views;

public class ServiceProvider : MonoBehaviour
{
    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private TestView testView;

    private void Awake()
    {
        #region ServiceProvider

        ServiceProvider<IAssetsManager>.Add(IAssetsManager.Default());

        ServiceProvider<IObjectPoolManager>.Add(new ObjectPoolManager());

        ServiceProvider<IUIManager>.Add(this.uiManager.Construct());

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
                    new BlueprintAssetJsonDataHandler(),
                    new BlueprintAssetCsvDataHandler(),
                }
            )
        );

        #endregion
    }

    private void Start()
    {
        this.uiManager.GetView(this.testView).Stack();
        Destroy(this.gameObject);
    }
}