public class ItemsLoaderRequest
{
    public string Url { get; }
    public int RequestedItemsCount { get; }

    public ItemsLoaderRequest(string url, int requestedItemsCount)
    {
        Url = url;
        RequestedItemsCount = requestedItemsCount;
    }
}