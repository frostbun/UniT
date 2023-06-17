namespace Entities
{
    using ABelieveT;
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    public class TankShooterProjectile : Trigger
    {
        public override void Init(Entity caster, Ability ability)
        {
            base.Init(caster, ability);
            this.transform.position = this.caster.transform.position;
        }

        private void Update()
        {
            this.transform.position += Vector3.up * (10 * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var target = other.GetComponent<Entity>();
            if (target == null) return;
            this.ability.OnTrigger(target, this);
        }
    }
}