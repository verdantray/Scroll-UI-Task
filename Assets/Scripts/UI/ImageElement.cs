using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageElement : ScrollElement<DataContainer>
    {
        private const string FallbackSpritePath = "fallback";
        
        [SerializeField] private Image contentImage;
        
        protected override void ShowOnUI(DataContainer data)
        {
            SetImage(FallbackSpritePath);
        }

        private void SetImage(string content)
        {
            var request = Resources.LoadAsync<Sprite>(content);
            
            request.completed += _ =>
            {
                contentImage.sprite = (Sprite)request.asset;
            };
        }
    }
}
