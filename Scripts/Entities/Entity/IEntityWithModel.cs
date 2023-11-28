namespace UniT.Entities
{
    public interface IEntityWithModel<in TModel> : IEntity
    {
        public TModel Model { set; }
    }
}