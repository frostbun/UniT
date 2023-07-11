namespace UniT.UI.ScrollView
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Adapter<TItem, TView, TPresenter> : MonoBehaviour
    {
        [SerializeField]
        private RectTransform content;

        private List<TItem>        items            = new();
        private Queue<IContract>   pooledContracts  = new();
        private HashSet<IContract> spawnedContracts = new();

        public void Show(int index)
        {

        }
    }
}