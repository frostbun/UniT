namespace UniT.Entities.Controller
{
    using UniT.Factories;

    public interface IController
    {
        public interface IFactory : IFactory<IEntityWithController, IController>
        {
        }

        public IEntityWithController Owner { set; }
    }
}