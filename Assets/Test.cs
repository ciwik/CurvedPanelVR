using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour {

    public float AngleRange = 120f;
    public float DistanceToItems = 5f;
    public int ItemsCountMin = 2;
    public int ItemsCountMax = 7;
    public Vector3 ObserverPosition;

    private bool isWorking;
    
    void Start ()
	{
	    ItemsLoader loader = gameObject.AddComponent<ItemsLoader>();
	    int itemsCount = Random.Range(ItemsCountMin, ItemsCountMax);
        ItemsLoaderRequest request = new ItemsLoaderRequest(Values.ItemsUrl, itemsCount);
	    isWorking = true;
	    loader.LoadItems(request, OnItemsLoaded);
	}

    private void OnItemsLoaded(ItemsLoaderResponse itemsLoaderResponse)
    {
        isWorking = false;
    }

    void Update()
    {
        if (isWorking)
        {
            Debug.Log("Loading...");
        }
    }
}
