namespace ABelieveT
{
    using System;
    using UniT.Utils;
    using UnityEngine;

    [Serializable]
    public class Stat
    {
        [field: SerializeField] public string Name      { get; private set; }
        [field: SerializeField] public float  BaseValue { get; private set; }

        private ReactiveProperty<float> _value;
        public  ReactiveProperty<float> Value => this._value ??= new(this.BaseValue);

        public Stat(string name, int value)
        {
            this.Name      = name;
            this.BaseValue = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Stat stat && this.Name == stat.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}