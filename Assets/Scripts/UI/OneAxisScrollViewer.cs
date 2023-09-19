using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
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
        protected float CurScrollPos = 0.0f;
        
        public void FetchData(List<T> data)
        {
            Initialize();
            
            FetchedList.AddRange(data);
            scrollRect.onValueChanged.AddListener(OnMoveScroll);
            
            OnMoveScroll(Vector2.zero);
        }
        
        protected virtual void Initialize()
        {
            scrollRect.onValueChanged.RemoveListener(OnMoveScroll);
            FetchedList.Clear();

            scrollRect.velocity = Vector2.zero;

            
            for (int i = ActiveInstances.Count; i > 0; i--)
            {
                ReleaseElement(ActiveInstances[i - 1]);
            }
            
            ActiveInstances.Clear();
            Content.anchoredPosition = Vector2.zero;
            
            CurScrollPos = 0.0f;
        }

        private void OnMoveScroll(Vector2 _)
        {
            ScrollDirection direction = GetScrollDirection(Content.anchoredPosition.y);
            float curScrollPos = GetScrollPosition();
            
            AdjustElement(direction, curScrollPos);
            CurScrollPos = curScrollPos;
        }

        protected abstract ScrollDirection GetScrollDirection(float curContentPos);
        protected abstract float GetScrollPosition();
        protected abstract void AdjustElement(ScrollDirection direction, float scrollPos);
        protected abstract ScrollElement<T> GetElement(int order);
        protected abstract void ReleaseElement(ScrollElement<T> element);
    }
}
