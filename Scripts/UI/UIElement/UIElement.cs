﻿namespace UniT.UI.UIElement
{
    using UniT.Utilities;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseUIElement : BetterMonoBehavior, IUIElement
    {
        IUIManager IUIElement.Manager { get => this.Manager; set => this.Manager = value; }

        RectTransform IUIElement.Transform => this.Transform;

        void IUIElement.OnInitialize()
        {
            this.Transform = (RectTransform)this.transform;
            this.OnInitialize();
        }

        void IUIElement.OnShow()
        {
            this.OnShow();
        }

        void IUIElement.OnHide()
        {
            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif
            this.OnHide();
        }

        void IUIElement.OnDispose()
        {
            this.OnDispose();
        }

        #if UNIT_UNITASK
        protected internal CancellationTokenSource hideCts;

        public CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new CancellationTokenSource()).Token;
        }
        #endif

        protected IUIManager Manager { get; private set; }

        protected RectTransform Transform { get; private set; }

        protected virtual void OnInitialize() { }

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }

        protected virtual void OnDispose() { }
    }

    public abstract class UIElement : BaseUIElement, IUIElementWithoutParams
    {
    }

    public abstract class UIElement<TParams> : BaseUIElement, IUIElementWithParams<TParams>
    {
        TParams IUIElementWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        protected TParams Params { get; private set; }
    }
}