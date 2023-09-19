using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum ScrollDirection
    {
        Forward,
        Backward
    }
    
    /// <summary>
    /// A Component for make up Scroll-UI with data collection.
    /// Scroll-UI only operate one-axis.
    /// </summary>
    /// <typeparam name="T">Type of element in data collection</typeparam>
    public abstract class OneAxisScrollViewer<T> : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private HorizontalOrVerticalLayoutGroup layoutGroup;
        
        protected Rect ViewportRect => scrollRect.viewport.rect;
        protected RectTransform Content => scrollRect.content;
        protected float Spacing => layoutGroup.spacing;
        
        protected readonly List<T> FetchedList = new();
        protected readonly List<ScrollElement<T>> ActiveInstances = new();
        protected float PrevScrollPos = 0.0f;
        
        public void FetchData(List<T> data)
        {
            scrollRect.onValueChanged.RemoveListener(OnMoveScroll);
            FetchedList.Clear();
            
            FetchedList.AddRange(data);
            scrollRect.onValueChanged.AddListener(OnMoveScroll);
            
            Initialize();
        }
        
        protected virtual void Initialize()
        {
            scrollRect.velocity = Vector2.zero;
            
            for (int i = ActiveInstances.Count; i > 0; i--)
            {
                ReleaseElement(ActiveInstances[i - 1]);
            }
            
            ActiveInstances.Clear();
            Content.anchoredPosition = Vector2.zero;
            
            PrevScrollPos = 0.0f;
            
            OnMoveScroll(Vector2.zero);
        }

        private void OnMoveScroll(Vector2 _)
        {
            ScrollDirection direction = GetScrollDirection(Content.anchoredPosition.y);
            float curScrollPos = GetClampedScrollPosition();
            
            AdjustElement(direction, curScrollPos);
            PrevScrollPos = curScrollPos;
        }

        /// <summary>
        /// decide and return ScrollDirection after compare 'PrevScrollPos' and curContentPos
        /// </summary>
        /// <param name="curContentPos">Current AnchoredPosition of 'Content'</param>
        /// <returns></returns>
        protected abstract ScrollDirection GetScrollDirection(float curContentPos);
        
        /// <summary>
        /// Get clamped PosX or PosY from Content AnchoredPosition 
        /// </summary>
        /// <returns></returns>
        protected abstract float GetClampedScrollPosition();
        
        /// <summary>
        /// get or release UI-Elements of ScrollViewer, depending on direction and scrollPosition
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="scrollPos"></param>
        protected abstract void AdjustElement(ScrollDirection direction, float scrollPos);
        
        protected abstract ScrollElement<T> GetElement(int order);
        protected abstract void ReleaseElement(ScrollElement<T> element);
    }
}
