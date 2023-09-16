using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Core
{
    public class DataProvider : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<TextAsset> dataSheet;
        [SerializeField] private Toggle shuffleToggle;

        private bool UseShuffle => shuffleToggle.isOn;
        
        private readonly List<DataContainer> _containers = new();

        private async void Start()
        {
            _containers.Clear();

            TextAsset sheet = await Addressables.LoadAssetAsync<TextAsset>(dataSheet).Task;
            _containers.AddRange(JsonConvert.DeserializeObject<List<DataContainer>>(sheet.text));

            LogMessage();
        }

        public IEnumerable<DataContainer> Provide()
        {
            return UseShuffle
                ? _containers.OrderBy(_ => Guid.NewGuid())
                : _containers;
        }

        #if UNITY_EDITOR
        // Test Functions
        
        public void LogMessage()
        {
            foreach (DataContainer container in Provide())
            {
                Debug.Log(container);
            }
        }
        
        #endif
    }
}
