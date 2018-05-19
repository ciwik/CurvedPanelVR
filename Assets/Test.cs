using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour {

    public int ItemsCountMin = 2;
    public int ItemsCountMax = 7;

    public ItemViewFactory ItemViewFactory;
    public Texture2D ErrorIcon;

    private bool isWorking;

    void Awake()
    {
        ItemViewFactory.ItemsCount = ItemsCountMax;
    }

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

        var itemViews = ItemViewFactory.CreateItemViews(ItemsCountMax);

        int i = 0;
        foreach (var item in itemsLoaderResponse.Items)
        {
            string errorMessage;
            Texture2D texture;
            if (itemsLoaderResponse.IconLoadingErrors.TryGetValue(item.Id, out errorMessage))
            {
                ItemViewFactory.DecorateItemView(itemViews[i], $"{item.Title}\n{errorMessage}", ErrorIcon);
            }
            else if (itemsLoaderResponse.Icons.TryGetValue(item.Id, out texture))
            {
                ItemViewFactory.DecorateItemView(itemViews[i], item.Title, texture);
            }
            i++;
        }
    }

    void Update()
    {
        if (isWorking)
        {
            Debug.Log("Loading...");
        }
    }
}
