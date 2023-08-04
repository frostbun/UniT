namespace UniT.UI
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Logging;
    using UniT.UI.Screen;
    using UnityEngine;

    public interface IUIManager
    {
        public LogConfig LogConfig { get; }

        public TView Initialize<TView>(TView view) where TView : IView;

        public IScreen StackingScreen { get; }

        public IScreen NextScreenInStack { get; }

        public IEnumerable<IScreen> FloatingScreens { get; }

        public IEnumerable<IScreen> DockedScreens { get; }

        public IScreen GetScreen(IScreen screen);

        public UniTask<IScreen> GetScreen<TScreen>(string key) where TScreen : Component, IScreen;

        public UniTask<IScreen> GetScreen<TScreen>() where TScreen : Component, IScreen;

        public void Stack(IScreen screen, bool force = false);

        public void Float(IScreen screen, bool force = false);

        public void Dock(IScreen screen, bool force = false);

        public void Hide(IScreen screen, bool removeFromStack = true, bool autoStack = true);

        public void Dispose(IScreen screen, bool autoStack = true);
    }
}