namespace Views
{
    using UniT.UI.Bases;
    using UnityEngine;

    public class TestView : BaseView
    {
        [field: SerializeField]
        public TestItemAdapter AdapterTestItem { get; private set; }

        protected override void OnInitialize()
        {
            this.AdapterTestItem.Construct();
        }

        protected override void OnShow()
        {
            this.AdapterTestItem.Show(new[]
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
            });
        }

        protected override void OnHide()
        {
            this.AdapterTestItem.Hide();
        }

        protected override void OnDispose()
        {
            this.AdapterTestItem.Dispose();
        }
    }
}