namespace UniT.EMC.Controller
{
    using System;
    using UniT.Factories;

    public interface IController
    {
        public interface IFactory : IFactory<IHasController, IController>
        {
            public static Func<IFactory> Default { get; set; } = () => new ControllerFactory();
        }

        public IHasController Component { set; }
    }
}