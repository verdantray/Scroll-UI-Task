using UnityEngine;

namespace UI
{
    /// <summary>
    /// Abstract Class for Scroll-UI-Element.
    /// inheritance of ScrollItem will instantiate as element of ScrollViewer
    /// And show information with T object
    /// </summary>
    /// <typeparam name="T">An Object have information</typeparam>
    public abstract class ScrollItem<T> : MonoBehaviour
    {
        // Show Information with T Object
        public abstract void SetData(T data);
        
        // Show Fallback Content
        // inheritance of ScrollItem will invoke when get invalid data
        protected abstract void Fallback();
    }
}
