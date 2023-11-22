namespace UniT.EMC.Controller
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
        public IController Create(IHasController component)
        {
            #if UNIT_ZENJECT
            var controller = (IController)CurrentContext.Container.Instantiate(component.ControllerType);
            #elif UNIT_DI
            var controller = (IController)DependencyContainer.Instantiate(component.ControllerType);
            #else
            var controller = (IController)Activator.CreateInstance(component.ControllerType);
            #endif
            controller.Component = component;
            return controller;
        }
    }
}