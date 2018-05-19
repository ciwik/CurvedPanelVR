using System.Collections.Generic;
using UnityEngine;

public class ItemViewFactory : MonoBehaviour {

    public float AngleRange = 120f;
    public float DistanceToItems = 5f;
    public int ItemsCount;
    public int QuadPolygonsPerSegment = 25;
    public Vector3 ObserverPosition;
    public GameObject ItemPrefab;

    private float _anglePerItem;
    private TextToImageConverter _textToImageConverter;

    void Awake()
    {
        _textToImageConverter = FindObjectOfType<TextToImageConverter>();
    }

    public ItemView[] CreateItemViews(int itemsCount)
    {
        ItemsCount = itemsCount;
        _anglePerItem = AngleRange / ItemsCount;

        List<ItemView> result = new List<ItemView>();

        float angle = 0;

        if (ItemsCount % 2 == 0)
        {
            angle = _anglePerItem / 2;
        }

        int iterationsCount = ItemsCount / 2 + (ItemsCount % 2 == 0 ? 0 : 1);
        for (int i = 0; i < iterationsCount; i++, angle += _anglePerItem)
        {
            result.Add(InitItemView(angle));
            if (angle > 0f)
            {
                result.Add(InitItemView(-angle));
            }
        }

        return result.ToArray();
    }

    private ItemView InitItemView(float angle)
    {
        Vector3 direction = DistanceToItems * 
            new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 
            0, 
            Mathf.Cos(Mathf.Abs(angle * Mathf.Deg2Rad)))
            .normalized;
        Vector3 itemPosition = ObserverPosition + direction;
        GameObject instance = Instantiate(ItemPrefab);
        instance.transform.position = ObserverPosition;
        instance.transform.forward = itemPosition - ObserverPosition;
        return instance.AddComponent<ItemView>();        
    }

    public void DecorateItemView(ItemView itemView, string text, Texture2D texture)
    {
        Texture2D textTexture = CreateTextureByText(text);
        texture = textTexture.Merge(texture);

        float textureRatio = texture.height / (float) texture.width;

        Mesh mesh = CreateMesh(_anglePerItem, DistanceToItems, textureRatio);        
        MeshRenderer meshRenderer = itemView.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.materials[0].SetTexture(Values.TextureMaterialPropertyName, texture);
        itemView.gameObject.GetComponent<MeshFilter>().mesh = mesh;        
    }

    private Texture2D CreateTextureByText(string text)
    {
        return _textToImageConverter.RenderTextAsTexture(text);
    }

    private Vector2 GetRotatedRadius(float t)
    {
        float leftAngle = -_anglePerItem / 2f;
        float rightAngle = _anglePerItem / 2f;
        float angle = Mathf.LerpAngle(leftAngle, rightAngle, t);

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);

        Vector2 up = new Vector2(0, DistanceToItems);
        return new Vector2(up.x * cos - up.y * sin, up.x * sin + up.y * cos);   //rotate vector by angle
    }

    private Mesh CreateMesh(float centerAngle, float radius, float ratio)
    {
        int verticesCount = 2 * QuadPolygonsPerSegment + 2;
        int trianglesCount = 3 * 2 * QuadPolygonsPerSegment;

        float D = 2 * radius;
        float alpha = centerAngle * Mathf.Deg2Rad / 2f;
        float L = D * alpha;
        float X = D * Mathf.Sin(alpha);
        float dX = X / QuadPolygonsPerSegment;
        float Y = L * ratio;

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[verticesCount];
        Vector2[] uv = new Vector2[verticesCount];
        int[] triangles = new int[trianglesCount];

        float x = 0;
        for (int i = 0; i < QuadPolygonsPerSegment; i++, x += dX)
        {
            Vector2 rotatedRadius = GetRotatedRadius(x / X);
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

        Vector2 rotatedRadius1 = GetRotatedRadius(1);
        vertices[verticesCount - 2] = new Vector3(rotatedRadius1.x, 0, rotatedRadius1.y);
        vertices[verticesCount - 1] = new Vector3(rotatedRadius1.x, Y, rotatedRadius1.y);

        uv[verticesCount - 2] = new Vector2(0, 0);
        uv[verticesCount - 1] = new Vector2(0, 1);

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        return mesh;
    }
}
