namespace UniT.UI
{
    using System;
    using UniT.Factories;

    public interface IPresenter
    {
        public sealed class Factory : DelegateFactory<Type, IPresenter>
        {
            public static Func<Factory> Default { get; set; } = () => new(type => (IPresenter)Activator.CreateInstance(type));

            public Factory(Func<Type, IPresenter> factory) : base(factory)
            {
            }
        }

        public IViewWithPresenter View { set; }
    }
}