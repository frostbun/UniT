namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public class ViewManager : IViewManager
    {
        public class ViewInstance : IViewManager.IViewInstance
        {
            public IViewManager.IViewInstance.Status CurrentStatus { get; private set; }

            private readonly ViewManager manager;
            private readonly BaseView    view;

            internal ViewInstance(ViewManager manager, BaseView view)
            {
                this.manager       = manager;
                this.view          = view;
                this.CurrentStatus = IViewManager.IViewInstance.Status.Hidden;
            }

            public void Stack()
            {
                this.view.transform.SetParent(this.manager.stackingCanvas.transform, false);
                this.manager.instances.Values.Where(instance => instance.CurrentStatus is IViewManager.IViewInstance.Status.Floating).ForEach(instance =>
                {
                    instance.Hide();
                });
            }

            public void Float()
            {
                this.view.transform.SetParent(this.manager.floatingCanvas.transform, false);
            }

            public void Detach()
            {
                this.view.transform.SetParent(this.manager.detachedCanvas.transform, false);
            }

            public void Hide()
            {
                this.view.transform.SetParent(this.manager.hiddenCanvas.transform, false);
            }

            public void Close()
            {
                this.manager.instances.Remove(this.view.GetType());
                Object.Destroy(this.view);
            }
        }

        private Canvas hiddenCanvas;
        private Canvas stackingCanvas;
        private Canvas floatingCanvas;
        private Canvas detachedCanvas;

        private readonly IAddressableManager                          addressableManager;
        private readonly ILogger                                      logger;
        private readonly Dictionary<Type, IViewManager.IViewInstance> instances;

        public ViewManager(IAddressableManager addressableManager, ILogger logger)
        {
            this.addressableManager = addressableManager;
            this.logger             = logger;
            this.instances          = new();
        }

        public UniTask<IViewManager.IViewInstance> Open<T>() where T : BaseView
        {
            return this.instances.GetOrAdd(
                typeof(T),
                () => this.addressableManager.Load<GameObject>(typeof(T).GetKeyAttribute())
                          .ContinueWith(
                              go => (IViewManager.IViewInstance)new ViewInstance(
                                  this,
                                  Object.Instantiate(go).GetComponent<T>()
                              )
                          )
            );
        }

        public void Hide<T>() where T : BaseView
        {
        }

        public void Close<T>() where T : BaseView
        {
        }
    }
}