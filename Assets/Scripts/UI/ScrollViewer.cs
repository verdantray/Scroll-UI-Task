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

        private RectTransform Viewport => scrollRect.viewport;
        private RectTransform Content => scrollRect.content;

        protected readonly List<DataContainer> FetchedList = new();
        protected readonly List<ScrollElement<DataContainer>> ActiveInstances = new();
        
        

        public void FetchData(List<DataContainer> data)
        {
            Initialize();
            
            FetchedList.AddRange(data);
            scrollRect.onValueChanged.AddListener(OnMoveScroll); 
        }

        protected virtual void Initialize()
        {
            scrollRect.onValueChanged.RemoveListener(OnMoveScroll);
            FetchedList.Clear();
        }

        private void OnMoveScroll(Vector2 _)
        {
            
        }
    }
}
