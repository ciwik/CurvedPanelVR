using Common;
using Data.Loading;
using UnityEngine;
using View;
using Random = UnityEngine.Random;

namespace Player
{
    [RequireComponent(typeof(Camera))]
    public class PlayerManager : MonoBehaviour {

        [SerializeField]
        private int _itemsCountMin = 2;
        [SerializeField]
        private int _itemsCountMax = 7;
        [SerializeField]
        private Texture2D _errorIcon;

        private ItemViewInitializer _itemViewInitializer;

        private bool isWorking;

        void Awake()
        {
            Input.backButtonLeavesApp = true;

            _itemViewInitializer = GetComponent<ItemViewInitializer>();
            _itemViewInitializer.ItemsCount = _itemsCountMax;
            _itemViewInitializer.Center = GetComponent<Camera>().transform.position;
        }

        void Start ()
        {
            ItemsLoader loader = gameObject.AddComponent<ItemsLoader>();
            int itemsCount = Random.Range(_itemsCountMin, _itemsCountMax);
            ItemsLoaderRequest request = new ItemsLoaderRequest(Values.ItemsUrl, itemsCount);
            isWorking = true;
            loader.LoadItemsAsync(request, OnItemsLoaded);
        }

        private void OnItemsLoaded(ItemsLoaderResponse itemsLoaderResponse)
        {
            isWorking = false;

            var itemViews = _itemViewInitializer.CreateItemViews(_itemsCountMax);

            int i = 0;
            foreach (var item in itemsLoaderResponse.Items)
            {
                string errorMessage;
                Texture2D texture;
                if (itemsLoaderResponse.IconLoadingErrors.TryGetValue(item.Id, out errorMessage))       //If icon wasn't loaded then error message will be shown
                {
                    _itemViewInitializer.DecorateItemView(itemViews[i], $"{item.Title}\n{errorMessage}", _errorIcon);
                }
                else if (itemsLoaderResponse.Icons.TryGetValue(item.Id, out texture))
                {
                    _itemViewInitializer.DecorateItemView(itemViews[i], item.Title, texture);
                }
                i++;
            }
        }

        void Update()
        {
            if (isWorking)
            {
                //TODO: loading spinner
                //Debug.Log("Loading...");
            }
        }
    }
}
