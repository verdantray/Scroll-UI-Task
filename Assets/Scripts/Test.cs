using Core;
using UI;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private DataProvider dataProvider;
    [SerializeField] private ScrollViewer scrollViewer;

    private void Start()
    {
        ShowScroll();
    }

    public void ShowScroll()
    {
        scrollViewer.FetchData(dataProvider.Provide());
    }
}
