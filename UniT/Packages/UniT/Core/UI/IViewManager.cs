namespace UniT.Core.UI
{
    using Cysharp.Threading.Tasks;

    /// <summary>
    ///     A view manager.
    /// </summary>
    public interface IViewManager
    {
        /// <summary>
        ///     A managed view instance.
        /// </summary>
        public interface IViewInstance
        {
            /// <summary>
            ///     Open a view to top of the stack.
            ///     Hide the last stacking view and all floating views.
            /// </summary>
            /// <seealso cref="Float"/>
            /// <seealso cref="Detach"/>
            public void Stack();

            /// <summary>
            ///     Open a floating view.
            /// </summary>
            /// <seealso cref="Stack"/>
            /// <seealso cref="Detach"/>
            public void Float();

            /// <summary>
            ///     Open a detached, always on top view.
            ///     Must be hidden manually.
            /// </summary>
            /// <seealso cref="Stack"/>
            /// <seealso cref="Float"/>
            public void Detach();

            /// <summary>
            ///     Hide, close the view and remove from the stack.
            /// </summary>
            /// <param name="cache">
            ///     True mean the view will be disabled.
            ///     False mean the view will be destroyed.
            /// </param>
            public void Close(bool cache = true);
        }

        /// <summary>
        ///     Get a managed view instance for <typeparamref name="T"/>.
        /// </summary>
        /// <see cref="IViewInstance"/>
        public UniTask<IViewInstance> GetView<T>() where T : IView;
    }
}