namespace UniT.UI
{
    #if UNIT_ZENJECT
    using Zenject;
    #elif UNIT_DI
    using UniT.DI;
    #else
    using System;
    #endif

    public class PresenterFactory : IPresenter.IFactory
    {
        public IPresenter Create(IHasPresenter owner)
        {
            #if UNIT_ZENJECT
            var controller = (IPresenter)CurrentContext.Container.Instantiate(owner.PresenterType);
            #elif UNIT_DI
            var controller = (IPresenter)DependencyContainer.Instantiate(owner.PresenterType);
            #else
            var controller = (IPresenter)Activator.CreateInstance(owner.PresenterType);
            #endif
            controller.Owner = owner;
            return controller;
        }
    }
}