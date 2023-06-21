namespace Controllers
{
    using UniT.Extensions;
    using UnityEngine;

    public class GameController : MonoBehaviour, IInitializable
    {
        public void Initialize()
        {
            this.gameObject.SetActive(true);
        }
    }
}