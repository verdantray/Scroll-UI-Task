using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TextElement : ScrollElement<DataContainer>
    {
        [SerializeField] private TextMeshProUGUI contentText;
        
        protected override void ShowOnUI(DataContainer data)
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
