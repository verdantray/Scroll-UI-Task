using UnityEngine;

namespace UI
{
    public struct ScrollElementParam<T>
    {
        public int OrderInCollection;
        public T Data;
        
        public ScrollElementParam(int order, T data)
        {
            OrderInCollection = order;
            Data = data;
        }
    }
    
    /// <summary>
    /// Abstract Class for Scroll-UI-Element.
    /// inheritance of ScrollItem will instantiate as element of ScrollViewer
    /// And show information with T object
    /// </summary>
    /// <typeparam name="T">An Object have information</typeparam>
    public abstract class ScrollElement<T> : MonoBehaviour
    {
        public int Order { get; private set; } = -1;
        
        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private RectTransform _rectTransform;

        public void SetData(ScrollElementParam<T> param)
        {
            Order = param.OrderInCollection;
            ShowOnUI(param.Data);
        }
        
        // Show Information with T Object
        protected abstract void ShowOnUI(T data);
        
        // Show Fallback Content
        // inheritance of ScrollItem will invoke when get invalid data
        protected virtual void Fallback()
        { }
    }
}
