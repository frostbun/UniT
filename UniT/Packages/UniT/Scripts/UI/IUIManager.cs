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

        public IScreenView StackingScreen { get; }

        public IScreenView NextScreenInStack { get; }

        public IEnumerable<IScreenView> FloatingScreens { get; }

        public IEnumerable<IScreenView> DockedScreens { get; }

        public UniTask<IScreenView> GetScreen<TScreenView>(string key) where TScreenView : Component, IScreenView;

        public UniTask<IScreenView> GetScreen<TScreenView>() where TScreenView : Component, IScreenView;

        public void Stack(IScreenView screen, bool force = false);

        public void Float(IScreenView screen, bool force = false);

        public void Dock(IScreenView screen, bool force = false);

        public void Hide(IScreenView screen, bool removeFromStack = true, bool autoStack = true);

        public void Dispose(IScreenView screen, bool autoStack = true);
    }
}