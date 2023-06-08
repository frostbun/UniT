namespace UniT.Core.UI
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Core.Addressables;
    using UniT.Core.Extensions;
    using UnityEngine;
    using ILogger = UniT.Core.Logging.ILogger;

    public class ViewManager : IViewManager
    {
        public class ViewInstance : IViewManager.IViewInstance
        {
            private readonly ViewManager viewManager;
            private readonly IView       view;

            internal ViewInstance(ViewManager viewManager, IView view)
            {
                this.viewManager = viewManager;
                this.view        = view;
                this.view.Open();
            }

            public void Stack()
            {
                this.view.Show();
                this.viewManager.stackingViews.Add(this);
            }

            public void Float()
            {
                this.view.Show();
                this.viewManager.floatingViews.Add(this);
            }

            public void Detach()
            {
                this.view.Show();
                this.viewManager.detachedViews.Add(this);
            }

            public void Close(bool cache = true)
            {
            }
        }

        private readonly IAddressableManager              addressableManager;
        private readonly ILogger                          logger;
        private readonly List<IViewManager.IViewInstance> stackingViews;
        private readonly List<IViewManager.IViewInstance> floatingViews;
        private readonly List<IViewManager.IViewInstance> detachedViews;

        public ViewManager(IAddressableManager addressableManager, ILogger logger)
        {
            this.addressableManager = addressableManager;
            this.logger             = logger;
        }

        public UniTask<IViewManager.IViewInstance> GetView<T>() where T : IView
        {
            return this.addressableManager.LoadOnce<GameObject>(typeof(T).GetKeyAttribute()).ContinueWith(go =>
            {
                var view = Object.Instantiate(go).GetComponent<T>();
                return (IViewManager.IViewInstance)new ViewInstance(this, view);
            });
        }
    }
}