using System.Collections.Generic;
using Common;
using TextureProcessing;
using UnityEngine;

namespace View
{
    public class ItemViewInitializer : MonoBehaviour {

        [SerializeField]
        private float _angleRange = 120f;           //Central angle
        [SerializeField]
        private float _distanceToItems = 5f;        //Raduis of drawing circle
        [SerializeField]
        private int _quadPolygonsPerSegment = 25;   //Count of quad polygons for each item
        [SerializeField]
        private GameObject _itemPrefab;

        public Vector3 Center { get; set; }

        private float _anglePerItem;
        private TextRenderer _textRenderer;

        void Awake()
        {
            _textRenderer = FindObjectOfType<TextRenderer>();
        }

        public ItemView[] CreateItemViews(int itemsCount, int itemsCountMax)
        {
            _anglePerItem = _angleRange / itemsCountMax;

            List<ItemView> result = new List<ItemView>();
            
            float angle = -_anglePerItem * (itemsCount - 1) / 2f;
            for (int i = 0; i < itemsCount; i++)
            {
                result.Add(InitItemView(angle));
                angle += _anglePerItem;
            }

            return result.ToArray();
        }

        private ItemView InitItemView(float angle)
        {
            Vector2 rotatedRadius = GetRotatedRadiusByAngle(angle);
            Vector3 direction = new Vector3(rotatedRadius.x, 0, rotatedRadius.y);
            Vector3 itemPosition = Center + direction;
            GameObject instance = Instantiate(_itemPrefab);
            instance.transform.position = Center;
            instance.transform.forward = itemPosition - Center;
            return instance.AddComponent<ItemView>();        
        }

        public void DecorateItemView(ItemView itemView, string text, Texture2D texture)
        {
            Texture2D textTexture = CreateTextureByText(text, texture.width);
            texture = textTexture.Merge(texture);

            float textureRatio = texture.height / (float) texture.width;

            Mesh mesh = CreateMesh(_anglePerItem, _distanceToItems, textureRatio);        
            MeshRenderer meshRenderer = itemView.transform.GetComponent<MeshRenderer>();
            meshRenderer.materials[0].SetTexture(Values.TextureMaterialPropertyName, texture);
            MeshFilter meshFilter = itemView.transform.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;        
        }

        private Texture2D CreateTextureByText(string text, int width)
        {
            return _textRenderer.RenderTextAsTexture(text, width);
        }

        private Vector2 GetRotatedRadiusByLerpParameter(float t)
        {
            //Use local angles of slice
            float leftAngle = -_anglePerItem / 2f;
            float rightAngle = _anglePerItem / 2f;
            float angle = Mathf.LerpAngle(leftAngle, rightAngle, t);

            return GetRotatedRadiusByAngle(angle);
        }

        private Vector2 GetRotatedRadiusByAngle(float angle)
        {           
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 up = new Vector2(0, _distanceToItems);
            //Rotate vector by angle
            return new Vector2(up.x * cos - up.y * sin, up.x * sin + up.y * cos);
        }

        private Mesh CreateMesh(float centralAngle, float radius, float ratio)
        {
            int verticesCount = 2 * _quadPolygonsPerSegment + 2;
            int trianglesCount = 3 * 2 * _quadPolygonsPerSegment;

            float D = 2 * radius;                                   //Diameter
            float alpha = centralAngle * Mathf.Deg2Rad / 2f;        //Angle
            float L = D * alpha;                                    //Arc length
            float X = D * Mathf.Sin(alpha);                         //Chord
            float dX = X / _quadPolygonsPerSegment;                 //Chord step
            float Y = L * ratio;                                    //Height

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[verticesCount];
            Vector2[] uv = new Vector2[verticesCount];
            int[] triangles = new int[trianglesCount];

            float x = 0;
            for (int i = 0; i < _quadPolygonsPerSegment; i++, x += dX)
            {
                Vector2 rotatedRadius = GetRotatedRadiusByLerpParameter(x / X);
                vertices[2 * i] = new Vector3(rotatedRadius.x, 0, rotatedRadius.y);
                vertices[2 * i + 1] = new Vector3(rotatedRadius.x, Y, rotatedRadius.y);

                uv[2 * i] = new Vector2((X - x) / X, 0);
                uv[2 * i + 1] = new Vector2((X - x) / X, 1);
 
                triangles[3 * 2 * i] = 2 * i;
                triangles[3 * 2 * i + 1] = 2 * i + 3;
                triangles[3 * 2 * i + 2] = 2 * i + 1;

                triangles[3 * 2 * i + 3] = 2 * i + 3;
                triangles[3 * 2 * i + 4] = 2 * i;
                triangles[3 * 2 * i + 5] = 2 * i + 2;
            }

            //Right border should be added outside of cycle
            Vector2 rotatedRadiusForRightBorder = GetRotatedRadiusByLerpParameter(1);
            vertices[verticesCount - 2] = new Vector3(rotatedRadiusForRightBorder.x, 0, rotatedRadiusForRightBorder.y);
            vertices[verticesCount - 1] = new Vector3(rotatedRadiusForRightBorder.x, Y, rotatedRadiusForRightBorder.y);

            uv[verticesCount - 2] = new Vector2(0, 0);
            uv[verticesCount - 1] = new Vector2(0, 1);

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }
    }
}
