namespace Views
{
    using UniT.UI.Bases;
    using UnityEngine;

    public class TestView : BaseView
    {
        [SerializeField]
        private TestItemAdapter testItemAdapter;

        protected override void OnInitialize()
        {
            this.Manager.Initialize(this.testItemAdapter);
        }

        protected override void OnShow()
        {
            this.testItemAdapter.Show(new[]
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
            this.testItemAdapter.Hide();
        }

        protected override void OnDispose()
        {
            this.testItemAdapter.Dispose();
        }
    }
}