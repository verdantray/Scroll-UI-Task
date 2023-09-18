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
            Content.anchoredPosition = Vector2.zero;
        }

        private void OnMoveScroll(Vector2 _)
        {
            // get or release previous elements from first element of 'ActiveInstances'
            // depending on 'Content.anchoredPosition'
            AdjustElementFromForward();
            
            // get or release next elements from last element of 'ActiveInstances'
            // depending on 'Content.anchoredPosition'
            AdjustElementFromBackward();
        }

        private void AdjustElementFromForward()
        {
            // get count of elements outside the viewport range
            int outsideElementCount = ActiveInstances.Count(CheckElementOutsideFromTop);

            // if count less than 1, get element
            if (outsideElementCount < 1)
            {
                
            }
            // if count more than 1 including the first element, release remains
            else if (outsideElementCount > 1)
            {
                // for (int i = outsideElementCount; i > 1; i--)
                // {
                //     ScrollElement<DataContainer> lastElement = ActiveInstances.Last();
                //
                //     AddSpaceSize(lastElement.RectTransform.rect.height * -1.0f);
                //     ActiveInstances.Remove(lastElement);
                //     
                //     ReleaseElement(lastElement);
                // }
            }

            return;
            
            bool CheckElementOutsideFromTop(ScrollElement<DataContainer> element)
            {
                float elementTop = GetElementTopPosition(element.RectTransform);
                float contentTop = 0.0f;

                return elementTop < contentTop;
            }

            float GetElementTopPosition(RectTransform rectTransform)
            {
                return rectTransform.anchoredPosition.y
                       + (rectTransform.rect.yMin * -1.0f);
            }
        }

        private void AdjustElementFromBackward()
        {
            // get count of elements outside the viewport range
            int outsideElementCount = ActiveInstances.Count(CheckElementOutsideFromBottom);

            // if count less than 1, get element
            if (outsideElementCount < 1)
            {
                float contentHeight = Content.rect.height - Content.anchoredPosition.y;
                
                while (contentHeight < ViewportRect.height)
                {
                    int orderToGet = ActiveInstances.Any()
                        ? ActiveInstances.Last().Order + 1
                        : 0;

                    // Stop getting element, shown last one already
                    if (orderToGet >= FetchedList.Count) break;

                    ScrollElement<DataContainer> instance = GetElement(orderToGet);
                    contentHeight += instance.RectTransform.rect.height;
                    
                    ActiveInstances.Add(instance);
                }
            }
            // if count more than 1 including the last element, release remains
            else if (outsideElementCount > 1)
            {
                // Debug.Log("More than 1");
                
                for (int i = outsideElementCount; i > 1; i--)
                {
                    ScrollElement<DataContainer> lastElement = ActiveInstances.Last();

                    ActiveInstances.Remove(lastElement);
                    ReleaseElement(lastElement);
                }
            }

            return;

            bool CheckElementOutsideFromBottom(ScrollElement<DataContainer> element)
            {
                float elementBottom = GetElementBottomPosition(element.RectTransform);
                float contentBottom = ViewportRect.height
                                      + Content.anchoredPosition.y
                                      - spaceElement.rect.height;
                
                return elementBottom > contentBottom;
            }
            
            float GetElementBottomPosition(RectTransform rectTransform)
            {
                // value start from 0 to minus, because content pivot.
                return Mathf.Abs(rectTransform.anchoredPosition.y
                                 + (rectTransform.rect.yMax * -1.0f) 
                                 + Content.anchoredPosition.y 
                                 - spaceElement.rect.height
                );
            }
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
            
            Debug.Log($"{order} / {container}");
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
