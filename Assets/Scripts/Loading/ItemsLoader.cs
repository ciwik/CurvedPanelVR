using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class ItemsLoader : MonoBehaviour
{

    public Coroutine LoadItems(ItemsLoaderRequest loaderRequest, Action<ItemsLoaderResponse> callback)
    {
        return StartCoroutine(LoadItemsRoutine(loaderRequest, callback));
    }

    private IEnumerator LoadItemsRoutine(ItemsLoaderRequest loaderRequest, Action<ItemsLoaderResponse> callback)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        using (var request = UnityWebRequest.Get(loaderRequest.Url))
        {
            yield return request.SendWebRequest();
            bool isError = request.isHttpError || request.isNetworkError;
            
            if (isError)
            {
                callback(new ItemsLoaderResponse(isError, request.error));
                yield break;
            }            
            
            string json = Encoding.UTF8.GetString(request.downloadHandler.data);
            
            Item[] items;
            try
            {
                items = JsonConvert.DeserializeObject<Item[]>(json)
                    .Take(loaderRequest.RequestedItemsCount)
                    .ToArray();
            }
            catch (JsonException)
            {
                callback(new ItemsLoaderResponse(true, "JSON data has wrong format"));
                yield break;
            }
            var icons = new Dictionary<string, Texture2D>();
            var iconLoadingErrors = new Dictionary<string, string>();
            foreach (var item in items)
            {
                yield return LoadIconRoutine(
                    item.IconUrl,
                    texture =>
                    {
                        icons.Add(item.Id, texture);
                    },
                    errorMessage =>
                    {
                        iconLoadingErrors.Add(item.Id, errorMessage);
                    }
                );
            }
            callback(new ItemsLoaderResponse(isError, items, icons, iconLoadingErrors));                        
        }

        stopwatch.Stop();
        Debug.Log(stopwatch.ElapsedMilliseconds);
    }

    private IEnumerator LoadIconRoutine(string url, Action<Texture2D> successCallback, Action<string> errorCallback)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            bool isError = request.isHttpError || request.isNetworkError;
            if (isError)
            {
                errorCallback($"Can't load icon: {request.error}");
                yield break;
            }
           
            var texture = DownloadHandlerTexture.GetContent(request);
            successCallback(texture);
        }
    }
}
