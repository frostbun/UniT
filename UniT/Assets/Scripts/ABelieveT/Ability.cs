namespace ABelieveT
{
    public abstract class Ability
    {
        public abstract void OnCreate(Entity caster);

        public abstract void OnUpdate();

        public abstract void OnTrigger(Entity target, Trigger trigger);
    }
}