using System.Collections;
using Common;
using Data.Loading;
using UnityEngine;
using View;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerManager : MonoBehaviour {

        [SerializeField]
        private int _itemsCountMin = 2;
        [SerializeField]
        private int _itemsCountMax = 7;
        [SerializeField]
        private Texture2D _errorIcon;        

        private LoadingIndicator _loadingIndicator;
        private ItemViewInitializer _itemViewInitializer;

        void Awake()
        {
            Input.backButtonLeavesApp = true;

            _itemViewInitializer = FindObjectOfType<ItemViewInitializer>();
            _loadingIndicator = GetComponentInChildren<LoadingIndicator>();
            _itemViewInitializer.Center = transform.position;
        }

        void Start ()
        {
           Load();
        }

        private void Load()
        {
            ItemsLoader loader = gameObject.AddComponent<ItemsLoader>();
            int itemsCount = Random.Range(_itemsCountMin, _itemsCountMax);
            ItemsLoaderRequest request = new ItemsLoaderRequest(Values.ItemsUrl, itemsCount);
            _loadingIndicator.enabled = true;
            loader.LoadItemsAsync(request, OnItemsLoaded);
        }

        private void OnItemsLoaded(ItemsLoaderResponse itemsLoaderResponse)
        {
            StartCoroutine(InitItemViewsRoutine(itemsLoaderResponse));
        }

        private IEnumerator InitItemViewsRoutine(ItemsLoaderResponse itemsLoaderResponse)
        {
            var itemViews = _itemViewInitializer.CreateItemViews(itemsLoaderResponse.Items.Length, _itemsCountMax);
            yield return null;

            int i = 0;
            foreach (var item in itemsLoaderResponse.Items)
            {
                string errorMessage;
                Texture2D texture;
                //If icon wasn't loaded then error message will be shown
                if (itemsLoaderResponse.IconLoadingErrors.TryGetValue(item.Id, out errorMessage))
                {
                    _itemViewInitializer.DecorateItemView(itemViews[i], $"{item.Title}\n{errorMessage}", _errorIcon);
                }
                else if (itemsLoaderResponse.Icons.TryGetValue(item.Id, out texture))
                {
                    _itemViewInitializer.DecorateItemView(itemViews[i], item.Title, texture);
                }
                i++;
                yield return null;
            }

            _loadingIndicator.enabled = false;
        }
    }
}
