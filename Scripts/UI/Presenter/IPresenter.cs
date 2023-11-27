namespace UniT.UI
{
    using UniT.Factories;

    public interface IPresenter
    {
        public interface IFactory : IFactory<IHasPresenter, IPresenter>
        {
        }

        public IHasPresenter Owner { set; }
    }
}