using System.Collections.Generic;
using UnityEngine;

namespace Data.Loading
{
    public class ItemsLoaderResponse
    {
        public bool IsError { get; }
        public Item[] Items { get; }
        public Dictionary<string, Texture2D> Icons { get; }
        public Dictionary<string, string> IconLoadingErrors { get; }
        public string Error { get; }

        public ItemsLoaderResponse(bool isError, Item[] items, Dictionary<string, Texture2D> icons, Dictionary<string, string> iconLoadingErrors)
        {
            IsError = isError;
            Items = items;
            Icons = icons;
            IconLoadingErrors = iconLoadingErrors;
        }

        public ItemsLoaderResponse(bool isError, string error)
        {
            IsError = isError;
            Error = error;
        }
    }
}