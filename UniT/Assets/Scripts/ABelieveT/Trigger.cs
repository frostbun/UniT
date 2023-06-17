namespace ABelieveT
{
    using UnityEngine;

    public abstract class Trigger : MonoBehaviour
    {
        protected Entity  caster;
        protected Ability ability;

        public virtual void Init(Entity caster, Ability ability)
        {
            this.caster  = caster;
            this.ability = ability;
        }
    }
}