using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ScrollModel : MonoBehaviour
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
        
        Show();
    }

    public void Show()
    {
        IEnumerable<DataContainer> toEnumerate = UseShuffle
            ? _containers.OrderBy(item => Guid.NewGuid())
            : _containers;

        foreach (DataContainer container in toEnumerate)
        {
            Debug.Log(container);
        }
    }
}
