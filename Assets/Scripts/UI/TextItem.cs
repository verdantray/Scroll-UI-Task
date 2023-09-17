using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TextItem : ScrollItem<DataContainer>
    {
        [SerializeField] private TextMeshProUGUI contentText;
        
        public override void SetData(DataContainer data)
        {
            bool isValid = data.type == DataType.Text
                           && !string.IsNullOrEmpty(data.content);

            if (!isValid)
            {
                Fallback();
                return;
            }
            
            SetText(data.content);
        }

        protected override void Fallback()
        {
            SetText("Fallback Text");
        }

        private void SetText(string content) => contentText.text = content;
    }
}
