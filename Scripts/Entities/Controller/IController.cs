#nullable enable
namespace UniT.Entities.Controller
{
    public interface IController
    {
        public IHasController Owner { set; }
    }
}