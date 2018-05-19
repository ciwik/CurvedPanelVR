using UnityEngine;
using UnityEngine.UI;

public class TextToImageConverter : MonoBehaviour
{
    public Camera RenderCamera;
    public Text UiText;

    public Texture2D RenderTextAsTexture(string text, int width)
    {
        UiText.text = text;
        RenderTexture currentActiveRenderTexture = RenderTexture.active;
        RenderTexture.active = RenderCamera.targetTexture;
        RenderCamera.Render();
        Texture2D texture = new Texture2D(width, RenderCamera.targetTexture.height);
        int diff = Mathf.Max(0, RenderCamera.targetTexture.width - width);
        width = Mathf.Min(width, RenderCamera.targetTexture.width);
        texture.ReadPixels(new Rect(diff / 2, 0, width, RenderCamera.targetTexture.height), 0, 0);
        RenderTexture.active = currentActiveRenderTexture;
        return texture;
    }
}
