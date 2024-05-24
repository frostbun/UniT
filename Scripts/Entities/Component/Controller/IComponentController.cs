#nullable enable
namespace UniT.Entities.Component.Controller
{
    using UniT.Entities.Controller;

    public interface IComponentController : IController
    {
        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();
    }
}