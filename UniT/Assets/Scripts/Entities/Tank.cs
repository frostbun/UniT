namespace Entities
{
    using ABelieveT;
    using UniT.ObjectPool;
    using UniT.Utils;
    using UnityEngine;

    public class Tank : Entity
    {
        protected override void Awake()
        {
            base.Awake();
            this.AddAbility(new TankShooter(ServiceProvider<IObjectPoolManager>.Get()));
        }

        protected override void Update()
        {
            base.Update();
            var horizontal = Input.GetAxis("Horizontal");
            var vertical   = Input.GetAxis("Vertical");
            var direction  = new Vector3(horizontal, vertical);
            var speed      = this.GetStat("Speed").Value.Value * Time.deltaTime;
            this.transform.position += direction * speed;
        }
    }
}