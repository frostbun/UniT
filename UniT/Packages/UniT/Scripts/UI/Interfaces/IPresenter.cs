namespace UniT.UI.Interfaces
{
    using System;
    using UniT.Utilities;

    public interface IPresenter
    {
        public class Factory : DelegateFactory<IPresenter, Type>
        {
            public static Func<Factory> Default { get; set; } = () => new(type => (IPresenter)Activator.CreateInstance(type));

            public Factory(Func<Type, IPresenter> factory) : base(factory)
            {
            }
        }

        protected internal IContract Contract { set; }

        protected internal IView View { set; }
    }
}