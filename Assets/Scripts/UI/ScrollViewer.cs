using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;
        [SerializeField] private RectTransform spaceElement;

        [SerializeField] private TextElement textElementRef;
        [SerializeField] private ImageElement imageElementRef;
        [SerializeField] private Transform releaseElementStock;

        protected Rect ViewportRect => scrollRect.viewport.rect;
        protected RectTransform Content => scrollRect.content;
        protected float Spacing => layoutGroup.spacing;
        
        protected readonly List<DataContainer> FetchedList = new();
        protected readonly List<ScrollElement<DataContainer>> ActiveInstances = new();

        private ScrollElementPool scrollElementPool = null;

        public void FetchData(List<DataContainer> data)
        {
            Initialize();
            
            FetchedList.AddRange(data);
            scrollRect.onValueChanged.AddListener(OnMoveScroll);
            
            OnMoveScroll(Vector2.zero);
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
            spaceElement.sizeDelta = new Vector2(ViewportRect.x, Spacing * -1.0f);
        }

        private void OnMoveScroll(Vector2 _)
        {
            Debug.Log(Content.anchoredPosition.y);
            
            // get or release previous elements from first element of 'ActiveInstances'
            // depending on 'Content.anchoredPosition'
            AdjustElementFromForward();
            
            // get or release next elements from last element of 'ActiveInstances'
            // depending on 'Content.anchoredPosition'
            AdjustElementFromBackward();
        }

        private void AdjustElementFromForward()
        {
            // if (!ActiveInstances.Any()) return;
            //
            // float contentPosition = Content.anchoredPosition.y;
            // float spaceSize = spaceElement.rect.height;
            // float topElementPosition = ActiveInstances.First().RectTransform.anchoredPosition.y;
            //
            // while()
        }

        private void AdjustElementFromBackward()
        {
            // get count of elements outside the viewport range
            int outsideElementCount = ActiveInstances.Count(CheckElementOutsideFromBottom);

            // if less than 1, get element
            if (outsideElementCount < 1)
            {
                float contentHeight = Content.rect.height - Content.anchoredPosition.y;

                while (contentHeight < ViewportRect.height)
                {
                    int orderToGet = ActiveInstances.Any()
                        ? ActiveInstances.Last().Order + 1
                        : 0;

                    if (orderToGet >= FetchedList.Count) break;

                    ScrollElement<DataContainer> instance = GetElement(orderToGet);
                    ActiveInstances.Add(instance);

                    contentHeight += instance.RectTransform.rect.height;
                }
            }
            // if more than 1 including the last element, release remains
            else if (outsideElementCount > 1)
            {
                for (int i = outsideElementCount; i > 1; i--)
                {
                    ScrollElement<DataContainer> lastElement = ActiveInstances.Last();

                    ActiveInstances.Remove(lastElement);
                    ReleaseElement(lastElement);
                }
            }

            float GetElementBottomPosition(RectTransform rectTransform)
            {
                return rectTransform.anchoredPosition.y
                       + (rectTransform.sizeDelta.y * 0.5f);
            }
            
            bool CheckElementOutsideFromBottom(ScrollElement<DataContainer> element)
            {
                float elementBottom = GetElementBottomPosition(element.RectTransform);
                float contentBottom = Content.anchoredPosition.y + ViewportRect.height;
                
                return elementBottom > contentBottom;
            }
        }

        private bool CheckElementOutsideFromTop(ScrollElement<DataContainer> element)
        {
            return false;
        }

        private void AddSpaceSize(float toAdd)
        {
            float curSize = spaceElement.rect.height;
            spaceElement.sizeDelta = new Vector2(ViewportRect.width, curSize + toAdd);
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
