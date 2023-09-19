using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Core;
using UnityEngine.AddressableAssets;

namespace UI
{
    public enum ScrollDirection
    {
        Forward,
        Backward
    }
    
    
    public class TestScrollViewer : OneAxisScrollViewer<DataContainer>
    {
        [SerializeField] private RectTransform spaceElement;
        [SerializeField] private AssetReferenceT<TextElement> textElementAsset;
        [SerializeField] private AssetReferenceT<ImageElement> imageElementAsset;
        [SerializeField] private Transform releaseElementStock;
        
        private ScrollElementPool _scrollElementPool = null;
        
        private void AddSpaceSize(float toAdd)
        {
            float curSize = spaceElement.rect.height;
            spaceElement.sizeDelta = new Vector2(ViewportRect.width, curSize + toAdd);
        }

        private async void InitializePoolAsync()
        {
            if (_scrollElementPool != null) return;
            
            TextElement textElementRef = await textElementAsset.LoadAssetAsync<TextElement>().Task;
            ImageElement imageElementRef = await imageElementAsset.LoadAssetAsync<ImageElement>().Task;
            
            _scrollElementPool = new ScrollElementPool(textElementRef, imageElementRef, Content, releaseElementStock);
        }

        #region Inherits of OneAxisScrollViewer

        protected override void Initialize()
        {
            InitializePoolAsync();
            spaceElement.sizeDelta = new Vector2(ViewportRect.x, Spacing * -1.0f);
            
            base.Initialize();
        }
        
        protected override void AdjustElement(ScrollDirection direction, float scrollPos)
        {
            bool isForward = direction == ScrollDirection.Forward;

            int outsideOfTopCount = ActiveInstances.Count(element => GetElementPosition(element.RectTransform) < scrollPos);
            int outSideOfBottomCount = ActiveInstances.Count(element => GetElementPosition(element.RectTransform) > scrollPos + ViewportRect.height);
            
            // if scroll forward, get element to bottom, release element from top
            if (direction == ScrollDirection.Forward)
            {
                // release element from top
                if (outsideOfTopCount > 0)
                {
                    for (int i = outsideOfTopCount; i > 0; i--)
                    {
                        ScrollElement<DataContainer> instance = ActiveInstances.First();
                        
                        // if instance is last one of ActiveInstances, stop loop
                        if (instance.Order >= FetchedList.Count - 1) break;

                        ActiveInstances.Remove(instance);
                        AddSpaceSize(instance.RectTransform.rect.height + Spacing);
                        
                        ReleaseElement(instance);
                    }
                }
                
                // get element to bottom
                if (outSideOfBottomCount < 1)
                {
                    float contentHeight = Content.rect.height - scrollPos;

                    while (contentHeight < ViewportRect.height)
                    {
                        int orderToGet = ActiveInstances.Any()
                            ? ActiveInstances.Last().Order + 1
                            : 0;
                        
                        // if instance is last one of ActiveInstances, stop loop
                        if (orderToGet > FetchedList.Count - 1) break;

                        ScrollElement<DataContainer> instance = GetElement(orderToGet);
                        contentHeight += instance.RectTransform.rect.height + Spacing;
                        
                        ActiveInstances.Add(instance);
                    }
                }
            }
            // if scroll backward, release element from bottom, get element to top
            else
            {
                // get element to top
                if (outsideOfTopCount < 1)
                {
                    float viewPortTop = ViewportRect.yMin * -1.0f;
                    float contentHeight = spaceElement.rect.height - scrollPos;
                    
                    Debug.Log(contentHeight);
                    
                    // set baseline lower to prevent malfunction when scroll velocity is too high
                    while (contentHeight - viewPortTop < viewPortTop)
                    {
                        int orderToGet = ActiveInstances.Any()
                            ? ActiveInstances.First().Order - 1
                            : 0;
                    
                        if (orderToGet < 0) break;
                    
                        ScrollElement<DataContainer> instance = GetElement(orderToGet);
                        instance.transform.SetSiblingIndex(1);
                        
                        float height = instance.RectTransform.rect.height + Spacing;
                        
                        AddSpaceSize(height * -1.0f);
                        ActiveInstances.Insert(0, instance);
                        
                        contentHeight += height;
                    }
                }

                // release element from bottom
                if (outSideOfBottomCount > 0)
                {
                    for (int i = outSideOfBottomCount; i > 0; i--)
                    {
                        ScrollElement<DataContainer> instance = ActiveInstances.Last();

                        if (instance.Order <= 0) break;

                        ActiveInstances.Remove(instance);
                        ReleaseElement(instance);
                    }
                }
            }

            return;
            
            // closure, get 'direction' out of scope
            // get element position of top or bottom, depending on direction
            float GetElementPosition(RectTransform rectTransform)
            {
                // value start from 0 to minus, because content pivot.
                return Mathf.Abs(
                    rectTransform.anchoredPosition.y
                    + (isForward ? rectTransform.rect.yMax : rectTransform.rect.yMin) * -1.0f
                );
            }
        }

        protected override ScrollDirection GetScrollDirection(float curContentPos)
        {
            return CurScrollPos <= curContentPos
                ? ScrollDirection.Forward
                : ScrollDirection.Backward;
        }

        protected override float GetScrollPosition()
        {
            // content move range 0 ~ float.Max
            return Mathf.Clamp(Content.anchoredPosition.y, 0.0f, float.MaxValue);
        }

        protected override ScrollElement<DataContainer> GetElement(int order)
        {
            DataContainer container = FetchedList[order];
            ScrollElement<DataContainer> ret = null;

            switch (container.type)
            {
                case DataType.Text:
                default:
                    _scrollElementPool.Get(out TextElement textElement);
                    ret = textElement;
                    break;
                
                case DataType.Image:
                    _scrollElementPool.Get(out ImageElement imageElement);
                    ret = imageElement;
                    break;
            }
            
            // Debug.Log($"{order} / {container}");
            ret.SetData(new ScrollElementParam<DataContainer>(order, container));

            return ret;
        }

        protected override void ReleaseElement(ScrollElement<DataContainer> element)
        {
            switch (element)
            {
                case TextElement textElement :
                    _scrollElementPool.Release(textElement);
                    break;
                case ImageElement imageElement :
                    _scrollElementPool.Release(imageElement);
                    break;
                
                case null :
                    Debug.Log("Try remove null Element....");
                    break;
            }
        }

        #endregion
    }
}
