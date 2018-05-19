using System.Collections.Generic;
using UnityEngine;

public class ItemViewFactory : MonoBehaviour {

    public float AngleRange = 120f;
    public float DistanceToItems = 5f;
    public int ItemsCount;
    public int QuadPolygonsPerSegment = 50;
    public Vector3 ObserverPosition;
    public GameObject ItemPrefab;

    private float _anglePerItem;

    public ItemView[] CreateItemViews(int itemsCount)
    {
        ItemsCount = itemsCount;
        _anglePerItem = AngleRange / ItemsCount;

        List<ItemView> result = new List<ItemView>();

        float diffAngle = Mathf.Deg2Rad * AngleRange / ItemsCount;
        float angle = 0;

        if (ItemsCount % 2 == 0)
        {
            angle = diffAngle / 2;
        }

        int itemIndex = 0;
        int iterationsCount = ItemsCount / 2 + (ItemsCount % 2 == 0 ? 0 : 1);
        for (int i = 0; i < iterationsCount; i++, angle += diffAngle)
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
        Vector3 direction = DistanceToItems * new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(Mathf.Abs(angle)));
        Vector3 itemPosition = ObserverPosition + direction;
        GameObject instance = Instantiate(ItemPrefab);        
        instance.transform.position = itemPosition;
        instance.transform.forward = -(ObserverPosition - itemPosition);    //move to DecorateItemView
        return instance.AddComponent<ItemView>();        
    }

    public void DecorateItemView(ItemView itemView, string text, Texture2D texture)
    {
        float textureRatio = texture.height / (float) texture.width;

        Mesh mesh = CreateMesh(_anglePerItem, DistanceToItems, textureRatio);
        MeshRenderer meshRenderer = itemView.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.materials[0].SetTexture(Values.TextureMaterialPropertyName, texture);              
        itemView.gameObject.GetComponent<MeshFilter>().mesh = mesh;        
    }

    private Vector2 GetRotatedRadius(float angleRange, float radius, float t)
    {
        float leftAngle = -angleRange / 2f;
        float rightAngle = angleRange / 2f;
        float angle = Mathf.LerpAngle(leftAngle, rightAngle, t);

        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);

        Vector2 up = new Vector2(0, radius);
        return new Vector2(up.x * cos - up.y * sin, up.x * sin + up.y * cos);   //rotate vector by angle
    }

    private Mesh CreateMesh(float centerAngle, float radius, float ratio)
    {
        int verticesCount = 2 * QuadPolygonsPerSegment + 2;
        int trianglesCount = 3 * 2 * QuadPolygonsPerSegment;

        float D = 2 * radius;
        float alpha = centerAngle * Mathf.PI / 360;
        float L = D * alpha;
        float X = D * Mathf.Sin(alpha);
        float dX = X / QuadPolygonsPerSegment;
        float Y = L * ratio;
        float dAngle = AngleRange / (ItemsCount - 1);

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[verticesCount];
        Vector2[] uv = new Vector2[verticesCount];
        int[] triangles = new int[trianglesCount];
        Vector3[] normals = new Vector3[verticesCount];

        float x = 0;
        for (int i = 0; i < QuadPolygonsPerSegment; i++, x += dX)
        {
            Vector2 xz = GetRotatedRadius(dAngle, radius, x / X);
            vertices[2 * i] = new Vector3(xz.x, 0, xz.y);
            vertices[2 * i + 1] = new Vector3(xz.x, Y, xz.y);
            xz = GetRotatedRadius(dAngle, radius, (x + dX) / X);
            vertices[2 * i + 2] = new Vector3(xz.x, 0, xz.y);
            vertices[2 * i + 3] = new Vector3(xz.x, Y, xz.y);

            uv[2 * i] = new Vector2((X - x) / X, 0);
            uv[2 * i + 1] = new Vector2((X - x) / X, 1);
            uv[2 * i + 2] = new Vector2((X - (x + dX)) / X, 0);
            uv[2 * i + 3] = new Vector2((X - (x + dX)) / X, 1);

            triangles[3 * 2 * i] = 2 * i;
            triangles[3 * 2 * i + 1] = 2 * i + 3;
            triangles[3 * 2 * i + 2] = 2 * i + 1;

            triangles[3 * 2 * i + 3] = 2 * i + 3;
            triangles[3 * 2 * i + 4] = 2 * i;
            triangles[3 * 2 * i + 5] = 2 * i + 2;

            Vector3 normal = new Vector3(1, 0, 0);
            normals[2 * i] = normal;
            normals[2 * i + 1] = normal;
            normals[2 * i + 2] = normal;
            normals[2 * i + 3] = normal;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.normals = normals;
        return mesh;
    }
}
