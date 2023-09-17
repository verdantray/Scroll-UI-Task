using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class DataProvider : MonoBehaviour
    {
        [SerializeField] private TextAsset dataSheet;   // DataProvider will get collections from data sheet
        [SerializeField] private Toggle shuffleToggle;  // Toggle UI to decide shuffle collection or not

        private bool UseShuffle => shuffleToggle.isOn;
        
        private readonly List<DataContainer> _containers = new();

        private void Start()
        {
            _containers.Clear();
            
            // Use 'Newtonsoft.Json.JsonConvert' for Deserialize Collection from JSON Array
            // native JsonUtility is not support deserialize collection
            _containers.AddRange(JsonConvert.DeserializeObject<List<DataContainer>>(dataSheet.text));

            LogMessage();
        }

        // Provide Data collections to caller
        public List<DataContainer> Provide()
        {
            return UseShuffle
                ? _containers.OrderBy(_ => Guid.NewGuid()).ToList()
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
