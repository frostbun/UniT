#nullable enable
namespace UniT.Entities.Controller
{
    using System;

    public interface IHasController
    {
        public Type ControllerType { get; }

        public IController Controller { set; }
    }
}