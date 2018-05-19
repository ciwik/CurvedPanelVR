using UnityEngine;
using UnityEngine.UI;

namespace TextureProcessing
{
    public class TextToImageConverter : MonoBehaviour
    {
        [SerializeField]
        private Camera _renderCamera;
        [SerializeField]
        private Text _uiText;
        
        public Texture2D RenderTextAsTexture(string text, int width)
        {
            _uiText.text = text;
            RenderTexture currentActiveRenderTexture = RenderTexture.active;
            RenderTexture.active = _renderCamera.targetTexture;
            _renderCamera.Render();
            Texture2D texture = new Texture2D(width, _renderCamera.targetTexture.height);
            int diff = Mathf.Max(0, _renderCamera.targetTexture.width - width);
            width = Mathf.Min(width, _renderCamera.targetTexture.width);
            texture.ReadPixels(new Rect(diff / 2, 0, width, _renderCamera.targetTexture.height), 0, 0);     //If requested width is less than renderTexture width, then texture is cutted
            RenderTexture.active = currentActiveRenderTexture;
            return texture;
        }
    }
}
