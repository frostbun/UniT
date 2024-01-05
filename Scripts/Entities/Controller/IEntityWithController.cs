namespace UniT.Entities.Controller
{
    using System;

    public interface IEntityWithController : IEntity
    {
        public Type ControllerType { get; }

        public IController Controller { set; }
    }
}