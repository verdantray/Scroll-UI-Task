using System;
using UI;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class ScrollElementPool : IDisposable
    {
        private ObjectPool<TextElement> textElementPool;
        private ObjectPool<ImageElement> imageElementPool;

        private Transform parentOnGet;
        private Transform parentOnRelease;
        
        public ScrollElementPool(TextElement textElementRef, ImageElement imageElementRef, Transform parentOnGet, Transform parentOnRelease)
        {
            this.parentOnGet = parentOnGet;
            this.parentOnRelease = parentOnRelease;
            
            textElementPool = InitializePool(textElementRef);
            imageElementPool = InitializePool(imageElementRef);
        }

        public void Get(out TextElement element)
        {
            element = textElementPool.Get();
        }

        public void Get(out ImageElement element)
        {
            element = imageElementPool.Get();
        }

        public void Release(TextElement element)
        {
            textElementPool.Release(element);
        }

        public void Release(ImageElement element)
        {
            imageElementPool.Release(element);
        }
        
        public void Dispose()
        {
            textElementPool.Clear();
            textElementPool = null;
            
            imageElementPool.Clear();
            imageElementPool = null;

            parentOnGet = null;
            parentOnRelease = null;
        }

        private ObjectPool<T> InitializePool<T>(T elementRef) where T : ScrollElement<DataContainer>
        {
            return new ObjectPool<T>(
                ()=> UnityEngine.Object.Instantiate(elementRef, parentOnRelease),
                actionOnGet: OnGetElement,
                actionOnRelease: OnReleaseElement
            );
        }

        private void OnGetElement<T>(T element) where T : ScrollElement<DataContainer>
        {
            element.gameObject.SetActive(true);
            element.transform.SetParent(parentOnGet);
        }
        
        private void OnReleaseElement<T>(T element) where T : ScrollElement<DataContainer>
        {
            element.gameObject.SetActive(false);
            element.transform.SetParent(parentOnRelease);
        }
    }
}
