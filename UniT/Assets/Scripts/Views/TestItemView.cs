namespace Views
{
    using TMPro;
    using UniT.UI.Adapter.ABC;
    using UnityEngine;

    public class TestItemView : BaseItemView<string, TestItemPresenter>
    {
        [field: SerializeField]
        public TMP_Text Txt { get; private set; }

        protected override void Show()
        {
            this.Txt.text = this.Contract.Item;
        }
    }

    public class TestItemPresenter : BaseItemPresenter<string, TestItemView>
    {
    }
}