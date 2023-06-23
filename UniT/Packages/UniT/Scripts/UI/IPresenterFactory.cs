namespace UniT.UI
{
    using System;
    using UniT.Utils;

    public interface IPresenterFactory : IFactory<IPresenter, Type>
    {
    }
}