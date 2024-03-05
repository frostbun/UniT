namespace UniT.ECC.Controller
{
    using System;

    public interface IHasController
    {
        public Type ControllerType { get; }

        public IController Controller { set; }
    }
}