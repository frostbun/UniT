namespace UniT.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using ILogger = UniT.Logging.ILogger;

    public class ViewManager : MonoBehaviour, IViewManager
    {
        private class ViewInstance : IViewManager.IViewInstance
        {
            private ViewStatus _currentStatus = ViewStatus.Hidden;

            public ViewStatus CurrentStatus
            {
                get => this._currentStatus;
                private set
                {
                    switch (value)
                    {
                        case ViewStatus.Closed:
                            if (this._currentStatus is not ViewStatus.Hidden)
                            {
                                this.view.OnHide();
                                this.presenter.OnHide();
                            }
                            this.view.OnClose();
                            this.presenter.OnClose();
                            break;
                        case ViewStatus.Hidden:
                            if (this._currentStatus is ViewStatus.Hidden) break;
                            this.view.gameObject.SetActive(false);
                            this.view.OnHide();
                            this.presenter.OnHide();
                            break;
                        case ViewStatus.Stacking or ViewStatus.Floating or ViewStatus.Detached:
                            if (this._currentStatus is ViewStatus.Stacking or ViewStatus.Floating or ViewStatus.Detached) break;
                            this.view.OnShow();
                            this.presenter.OnShow();
                            this.view.gameObject.SetActive(true);
                            break;
                    }

                    this._currentStatus = value;
                    this.manager.logger?.Debug($"{this.view.GetType().Name} status: {value}");
                }
            }

            private readonly IView       view;
            private readonly IPresenter  presenter;
            private readonly ViewManager manager;

            public ViewInstance(IView view, IPresenter presenter, ViewManager manager)
            {
                this.view      = view;
                this.presenter = presenter;
                this.manager   = manager;

                this.view.Presenter = this.presenter;
                this.presenter.View = this.view;

                this.view.OnInitialize();
                this.presenter.OnInitialize();

                this.view.transform.SetParent(this.manager.canvas);
                this.manager.logger?.Debug($"Instantiated {this.view.GetType().Name}");
            }

            public IViewManager.IViewInstance BindModel(object model)
            {
                this.EnsureViewIsNotClosed();
                this.presenter.Model = model;
                return this;
            }

            public void Stack()
            {
                this.EnsureViewIsNotClosed();
                this.CurrentStatus = ViewStatus.Stacking;
                this.manager.instances.Values.Where(instance => !ReferenceEquals(instance, this) && instance.CurrentStatus is not ViewStatus.Detached).ForEach(instance =>
                {
                    instance.Hide();
                });
            }

            public void Float()
            {
                this.EnsureViewIsNotClosed();
                this.CurrentStatus = ViewStatus.Floating;
            }

            public void Detach()
            {
                this.EnsureViewIsNotClosed();
                this.CurrentStatus = ViewStatus.Detached;
            }

            public void Hide()
            {
                this.EnsureViewIsNotClosed();
                this.CurrentStatus = ViewStatus.Hidden;
            }

            public void Close()
            {
                this.EnsureViewIsNotClosed();
                this.CurrentStatus = ViewStatus.Closed;
                this.manager.instances.Remove(this.view.GetType());
                Destroy(this.view.gameObject);
                if (this.manager.instanceToKey.Remove(this.view.GetType(), out var key))
                {
                    this.manager.addressableManager.Unload(key);
                }
            }

            private void EnsureViewIsNotClosed()
            {
                if (this.CurrentStatus is ViewStatus.Closed)
                    throw new InvalidOperationException($"{this.view.GetType().Name} is already closed");
            }
        }

        [SerializeField]
        private Transform canvas;

        public static ViewManager Instantiate(
            Action<Canvas> canvasConfigurator = null,
            Action<CanvasScaler> canvasScalerConfigurator = null,
            Action<GraphicRaycaster> graphicRaycasterConfigurator = null,
            Action<EventSystem> eventSystemConfigurator = null,
            Action<StandaloneInputModule> standaloneInputModuleConfigurator = null
        )
        {
            canvasConfigurator ??= canvas => canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var manager = new GameObject(nameof(ViewManager)).AddComponent<ViewManager>();
            DontDestroyOnLoad(manager);

            var canvasGo = new GameObject(nameof(Canvas));
            canvasGo.transform.SetParent(manager.transform);

            canvasConfigurator(canvasGo.AddComponent<Canvas>());

            var canvasScaler = canvasGo.AddComponent<CanvasScaler>();
            canvasScalerConfigurator?.Invoke(canvasScaler);

            var graphicRaycaster = canvasGo.AddComponent<GraphicRaycaster>();
            graphicRaycasterConfigurator?.Invoke(graphicRaycaster);

            var eventSystemGo = new GameObject(nameof(EventSystem));
            eventSystemGo.transform.SetParent(manager.transform);

            var eventSystem = eventSystemGo.AddComponent<EventSystem>();
            eventSystemConfigurator?.Invoke(eventSystem);

            var inputModule = eventSystemGo.AddComponent<StandaloneInputModule>();
            standaloneInputModuleConfigurator?.Invoke(inputModule);

            manager.canvas = canvasGo.transform;

            return manager;
        }

        private IAddressableManager                          addressableManager;
        private ILogger                                      logger;
        private Dictionary<Type, IViewManager.IViewInstance> instances;
        private Dictionary<Type, string>                     instanceToKey;

        public void Inject(IAddressableManager addressableManager, ILogger logger = null)
        {
            this.addressableManager = addressableManager;
            this.logger             = logger;
            this.instances          = new();
        }

        public IViewManager.IViewInstance CurrentView => this.instances.Values.FirstOrDefault(instance => instance.CurrentStatus is ViewStatus.Stacking);

        public IViewManager.IViewInstance GetView<TView, TPresenter>(TView view)
            where TView : Component, IView
            where TPresenter : IPresenter, new()
        {
            return this.instances.GetOrAdd(typeof(TView), () => new ViewInstance(view, new TPresenter(), this));
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(string key)
            where TView : Component, IView
            where TPresenter : IPresenter, new()
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => this.addressableManager.Load<GameObject>(key)
                          .ContinueWith(
                              go =>
                              {
                                  var instance = (IViewManager.IViewInstance)new ViewInstance(
                                      Instantiate(go).GetComponent<TView>(),
                                      new TPresenter(),
                                      this
                                  );
                                  this.instanceToKey.Add(typeof(TView), key);
                                  return instance;
                              })
            );
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>()
            where TView : Component, IView
            where TPresenter : IPresenter, new()
        {
            return this.GetView<TView, TPresenter>(typeof(TView).GetKeyAttribute());
        }

        public IViewManager.IViewInstance GetView<TView, TPresenter>(TView view, Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(typeof(TView), () => new ViewInstance(view, presenterFactory(), this));
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(string key, Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.instances.GetOrAdd(
                typeof(TView),
                () => this.addressableManager.Load<GameObject>(key)
                          .ContinueWith(
                              go =>
                              {
                                  var instance = (IViewManager.IViewInstance)new ViewInstance(
                                      Instantiate(go).GetComponent<TView>(),
                                      presenterFactory(),
                                      this
                                  );
                                  this.instanceToKey.Add(typeof(TView), key);
                                  return instance;
                              })
            );
        }

        public UniTask<IViewManager.IViewInstance> GetView<TView, TPresenter>(Func<TPresenter> presenterFactory)
            where TView : Component, IView
            where TPresenter : IPresenter
        {
            return this.GetView<TView, TPresenter>(typeof(TView).GetKeyAttribute(), presenterFactory);
        }
    }
}