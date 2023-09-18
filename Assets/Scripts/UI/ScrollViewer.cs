using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

namespace UI
{
    /// <summary>
    /// A Component for make up Scroll-UI with data collection.
    /// </summary>
    /// <typeparam name="T">Type of element in data collection</typeparam>
    public class ScrollViewer : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        [SerializeField] private RectTransform spaceElement;

        [SerializeField] private TextElement textElementRef;
        [SerializeField] private ImageElement imageElementRef;
        [SerializeField] private Transform releaseElementStock;

        private Rect ViewportSize => scrollRect.viewport.rect;
        private RectTransform Content => scrollRect.content;
        private float Spacing => layoutGroup.spacing;

        protected readonly List<DataContainer> FetchedList = new();
        protected readonly List<ScrollElement<DataContainer>> ActiveInstances = new();

        private ScrollElementPool scrollElementPool = null;

        public void FetchData(List<DataContainer> data)
        {
            Initialize();
            
            FetchedList.AddRange(data);
            scrollRect.onValueChanged.AddListener(OnMoveScroll);

            spaceElement.sizeDelta = new Vector2(ViewportSize.x, Spacing * -1.0f);

            float contentHeight = 0.0f;
            int count = 0;
            
            while (contentHeight < ViewportSize.height)
            {
                contentHeight += GetElement(count).RectTransform.rect.height;
                count += 1;
            }
        }

        private void Initialize()
        {
            scrollRect.onValueChanged.RemoveListener(OnMoveScroll);
            FetchedList.Clear();

            scrollElementPool ??= new ScrollElementPool(textElementRef, imageElementRef, Content, releaseElementStock);
            
            for (int i = ActiveInstances.Count; i > 0; i--)
            {
                ReleaseElement(ActiveInstances[i - 1]);
            }
            
            ActiveInstances.Clear();
        }

        private void OnMoveScroll(Vector2 scrollPos)
        {
            
        }

        private ScrollElement<DataContainer> GetElement(int order)
        {
            DataContainer container = FetchedList[order];
            ScrollElement<DataContainer> ret = null;

            switch (container.type)
            {
                case DataType.Text:
                default:
                    scrollElementPool.Get(out TextElement textElement);
                    ret = textElement;
                    break;
                
                case DataType.Image:
                    scrollElementPool.Get(out ImageElement imageElement);
                    ret = imageElement;
                    break;
            }
            
            ret.SetData(new ScrollElementParam<DataContainer>(order, container));

            return ret;
        }

        private void ReleaseElement(ScrollElement<DataContainer> element)
        {
            switch (element)
            {
                case TextElement textElement :
                    scrollElementPool.Release(textElement);
                    break;
                case ImageElement imageElement :
                    scrollElementPool.Release(imageElement);
                    break;
                
                case null :
                    Debug.Log("Try remove null Element....");
                    break;
            }
        }
    }
}
