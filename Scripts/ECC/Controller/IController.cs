namespace UniT.ECC.Controller
{
    using UniT.Factories;

    public interface IController
    {
        public interface IFactory : IFactory<IHasController, IController>
        {
        }

        public IHasController Owner { set; }
    }
}