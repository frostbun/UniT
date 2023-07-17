namespace Views
{
    using UniT.UI.ABC;
    using UnityEngine;

    public class TestView : BaseView<TestPresenter>
    {
        [field: SerializeField]
        public TestItemAdapter AdapterTestItem { get; private set; }

        protected override void Initialize()
        {
            this.AdapterTestItem.Construct(new[]
            {
                "Test 1",
                "Test 2",
                "Test 3",
                "Test 4",
                "Test 5",
                "Test 6",
                "Test 7",
                "Test 8",
                "Test 9",
                "Test 10",
            }).Show();
        }
    }

    public class TestPresenter : BasePresenter<TestView>
    {
    }
}