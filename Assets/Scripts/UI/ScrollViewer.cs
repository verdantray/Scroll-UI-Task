using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

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

        [SerializeField] private ScrollItem<DataContainer> itemRef;

        private RectTransform Viewport => scrollRect.viewport;
        private RectTransform Content => scrollRect.content;

        protected readonly List<DataContainer> dataList = new();
        protected readonly List<ScrollItem<DataContainer>> instanceList = new();
        
        // 'spaceElement' will be always the first child of 'Content'
        // 'spaceElement' changes height depending on scroll direction
        private RectTransform spaceElement = null;
        
        private ObjectPool<ScrollItem<DataContainer>> instancePool = null;

        public void DisplayScroll(List<DataContainer> list)
        {
            Initialize();
            
            dataList.AddRange(list);

            for (int i = 0; i < 5; i++)
            {
                ScrollItem<DataContainer> instance = instancePool.Get();
                instance.SetData(dataList[i]);
                
                instanceList.Add(instance);
            }
            
            Debug.Log($"Pool Active : {instancePool.CountActive} / Pool Inactive : {instancePool.CountInactive}");
        }

        private void Initialize()
        {
            InitializePool();

            for (int i = instanceList.Count; i > 0; i--)
            {
                instancePool.Release(instanceList[i - 1]);
            }
            
            dataList.Clear();
            instanceList.Clear();
            
            spaceElement ??= new GameObject("Space Element", typeof(RectTransform), typeof(LayoutElement)).GetComponent<RectTransform>();
            spaceElement.sizeDelta = new Vector2(Viewport.sizeDelta.x, layoutGroup.spacing * -1.0f);
            
            spaceElement.SetParent(Content);
        }

        public void OnScroll(Vector2 dir)
        {
            Debug.Log(dir);
        }

        #region Functions that need to be separate to new class

        private void InitializePool()
        {
            if (instancePool != null) return;
            instancePool = new ObjectPool<ScrollItem<DataContainer>>(CreatePooledItem);
        }

        private ScrollItem<DataContainer> CreatePooledItem()
        {
            return Instantiate(itemRef, Content);
        }

        private void OnElementInstanceGet(ScrollItem<DataContainer> instance)
        {
            instance.gameObject.SetActive(true);
        }

        private void OnElementInstanceRelease(ScrollItem<DataContainer> instance)
        {
            instance.gameObject.SetActive(false);
        }

        #endregion
    }
}
