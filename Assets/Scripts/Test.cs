using System.Collections;
using System.Collections.Generic;
using Core;
using UI;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private DataProvider dataProvider;
    [SerializeField] private ScrollViewer scrollViewer;
    
    public void ShowScroll()
    {
        scrollViewer.DisplayScroll(dataProvider.Provide());
    }
}
