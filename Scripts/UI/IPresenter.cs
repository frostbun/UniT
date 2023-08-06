namespace UniT.UI
{
    using System;
    using UniT.Extensions;

    public interface IPresenter
    {
        public class Factory : DelegateFactory<IPresenter, Type>
        {
            public static Func<Factory> Default { get; set; } = () => new(type => (IPresenter)Activator.CreateInstance(type));

            public Factory(Func<Type, IPresenter> factory) : base(factory)
            {
            }
        }

        protected internal IView View { set; }
    }
}