namespace ABelieveT
{
    using UnityEngine;

    public abstract class StatView : MonoBehaviour
    {
        public abstract string StatName { get; }

        public abstract void Init(Stat stat);
    }
}