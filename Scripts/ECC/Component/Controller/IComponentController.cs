namespace UniT.ECC.Component.Controller
{
    using UniT.ECC.Controller;

    public interface IComponentController : IController
    {
        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();
    }
}