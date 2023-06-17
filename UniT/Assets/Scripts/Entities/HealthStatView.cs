namespace Entities
{
    using System;
    using ABelieveT;
    using UnityEngine;

    public class HealthStatView : StatView, IDisposable
    {
        [SerializeField] private TextMesh    txt;
        private                  Stat        stat;
        private                  IDisposable statObserver;

        public override string StatName => "Health";

        public override void Init(Stat stat)
        {
            this.stat = stat;
            this.UpdateView(stat.Value.Value);
            this.statObserver = this.stat.Value.Subscribe(this.UpdateView);
        }

        public void UpdateView(float value)
        {
            this.txt.text = $"{value}/{this.stat.BaseValue}";
        }

        public void Dispose()
        {
            this.statObserver?.Dispose();
        }
    }
}