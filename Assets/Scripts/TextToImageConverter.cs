using UnityEngine;
using UnityEngine.UI;

public class TextToImageConverter : MonoBehaviour
{
    public Camera RenderCamera;
    public Text UiText;

    public Texture2D RenderTextAsTexture(string text)
    {
        UiText.text = text;
        RenderTexture currentActiveRenderTexture = RenderTexture.active;
        RenderTexture.active = RenderCamera.targetTexture;
        RenderCamera.Render();
        Texture2D texture = new Texture2D(RenderCamera.targetTexture.width, RenderCamera.targetTexture.height);
        texture.ReadPixels(RenderCamera.pixelRect, 0, 0);
        RenderTexture.active = currentActiveRenderTexture;
        return texture;
    }
}
