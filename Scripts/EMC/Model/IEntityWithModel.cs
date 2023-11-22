namespace UniT.EMC.Model
{
    public interface IEntityWithModel<in TModel> : IEntity
    {
        public TModel Model { set; }
    }
}