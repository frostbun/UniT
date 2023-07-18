namespace UniT.UI
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.UI.Interfaces;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public interface IUIManager
    {
        public ILogger Logger { get; }

        public IContract StackingContract { get; }

        public IContract NextStackingContract { get; }

        public IEnumerable<IContract> FloatingContracts { get; }

        public IEnumerable<IContract> DockedContracts { get; }

        public IContract GetContract<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter;

        public UniTask<IContract> GetContract<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter;

        public UniTask<IContract> GetContract<TView, TPresenter>()
            where TView : Component, IView
            where TPresenter : IPresenter;
    }
}