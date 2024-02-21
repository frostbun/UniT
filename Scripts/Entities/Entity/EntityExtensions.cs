namespace UniT.Entities
{
    public static class EntityExtensions
    {
        public static void Recycle(this IEntity entity)
        {
            entity.Manager.Recycle(entity);
        }
    }
}