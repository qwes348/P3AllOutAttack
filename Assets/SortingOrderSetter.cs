using System;
using UnityEngine;

public class SortingOrderSetter : MonoBehaviour
{
    public int sortingOrder;
    
    private Renderer myRenderer;

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
        if (myRenderer == null)
            return;
        myRenderer.sortingOrder = sortingOrder;
    }
}
