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
        [SerializeField] private TextAsset dataSheet;
        [SerializeField] private Toggle shuffleToggle;

        private bool UseShuffle => shuffleToggle.isOn;
        
        private readonly List<DataContainer> _containers = new();

        private void Start()
        {
            _containers.Clear();
            _containers.AddRange(JsonConvert.DeserializeObject<List<DataContainer>>(dataSheet.text));

            LogMessage();
        }

        public List<DataContainer> Provide()
        {
            IEnumerable<DataContainer> toProvide = UseShuffle
                ? _containers.OrderBy(_ => Guid.NewGuid())
                : _containers;

            return toProvide.ToList();
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
