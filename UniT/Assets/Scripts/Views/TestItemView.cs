namespace Views
{
    using TMPro;
    using UniT.UI.Item.Bases;
    using UnityEngine;

    public class TestItemView : BaseItemView<string>
    {
        [field: SerializeField]
        public TMP_Text Txt { get; private set; }

        protected override void Show()
        {
            this.Txt.text = this.Item;
        }
    }
}