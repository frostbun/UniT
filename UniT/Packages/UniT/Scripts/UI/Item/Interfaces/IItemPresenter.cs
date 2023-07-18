namespace UniT.UI.Item.Interfaces
{
    using System;
    using UniT.Utilities;

    public interface IItemPresenter
    {
        public class Factory : DelegateFactory<IItemPresenter, Type>
        {
            public static Func<Factory> Default { get; set; } = () => new(type => (IItemPresenter)Activator.CreateInstance(type));

            public Factory(Func<Type, IItemPresenter> factory) : base(factory)
            {
            }
        }

        protected internal IItemView View { set; }
    }
}