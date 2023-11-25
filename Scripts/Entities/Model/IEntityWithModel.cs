namespace UniT.Entities.Model
{
    public interface IEntityWithModel<in TModel> : IEntity
    {
        public TModel Model { set; }
    }
}