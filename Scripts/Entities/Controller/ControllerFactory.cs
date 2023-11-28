namespace UniT.Entities.Controller
{
    #if UNIT_ZENJECT
    using Zenject;
    #elif UNIT_DI
    using UniT.DI;
    #else
    using System;
    #endif

    public class ControllerFactory : IController.IFactory
    {
        public IController Create(IHasController owner)
        {
            #if UNIT_ZENJECT
            var controller = (IController)CurrentContext.Container.Instantiate(owner.ControllerType);
            #elif UNIT_DI
            var controller = (IController)DependencyContainer.Instantiate(owner.ControllerType);
            #else
            var controller = (IController)Activator.CreateInstance(owner.ControllerType);
            #endif
            controller.Owner = owner;
            return controller;
        }
    }
}