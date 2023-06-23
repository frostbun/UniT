namespace UniT.UI
{
    using System;

    public class PresenterFactory : IPresenterFactory
    {
        public IPresenter Create(Type type)
        {
            return (IPresenter)Activator.CreateInstance(type);
        }
    }
}