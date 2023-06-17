namespace Entities
{
    using System.Collections.Generic;
    using System.Threading;
    using ABelieveT;
    using Cysharp.Threading.Tasks;
    using UniT.ObjectPool;
    using UnityEngine;

    public class TankShooter : Ability
    {
        private readonly IObjectPoolManager objectPoolManager;

        public TankShooter(IObjectPoolManager objectPoolManager)
        {
            this.objectPoolManager = objectPoolManager;
        }

        private          Entity                                       caster;
        private          float                                        dt;
        private readonly Dictionary<Trigger, CancellationTokenSource> triggerToCts = new();

        public override void OnCreate(Entity caster)
        {
            this.caster = caster;
            this.dt     = 0;
        }

        public override async void OnUpdate()
        {
            this.dt += Time.deltaTime;
            while (this.dt >= 1)
            {
                this.dt -= 1;
                var projectile = await this.objectPoolManager.Spawn<TankShooterProjectile>();
                projectile.Init(this.caster, this);
                UniTask.Delay(3000, cancellationToken: (this.triggerToCts[projectile] = new()).Token)
                       .SuppressCancellationThrow()
                       .ContinueWith(isCancelled =>
                       {
                           if (isCancelled) return; // Already hit something and recycled
                           this.Recycle_Internal(projectile);
                       }).Forget();
            }
        }

        public override void OnTrigger(Entity target, Trigger trigger)
        {
            if (this.caster.Team == target.Team) return;
            target.GetStat("Health").Value.Value -= this.caster.GetStat("Damage").Value.Value;
            this.Recycle_Internal(trigger);
        }

        private void Recycle_Internal(Trigger trigger)
        {
            this.triggerToCts.Remove(trigger, out var cts);
            cts.Cancel();
            cts.Dispose();
            this.objectPoolManager.Recycle(trigger);
        }
    }
}