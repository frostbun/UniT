namespace UniT.UI
{
    using System;

    public interface IPresenter
    {
        public sealed class Factory : DelegateFactory<IPresenter, Type>
        {
            public static Func<Factory> Default { get; set; } = () => new(type => (IPresenter)Activator.CreateInstance(type));

            public Factory(Func<Type, IPresenter> factory) : base(factory)
            {
            }
        }

        public IViewWithPresenter View { set; }
    }
}