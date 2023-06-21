namespace Controllers
{
    using UniT.Extensions;
    using UnityEngine;

    public class GameController : MonoBehaviour
    {
        public void Initialize()
        {
            this.gameObject.SetActive(true);
        }
    }
}