namespace UniT.EMC.Controller
{
    using System;

    public interface IHasController
    {
        public Type ControllerType { get; }

        public IController Controller { set; }
    }
}