using UnityEngine;
using UnityEngine.UI;

namespace TextureProcessing
{
    public class TextRenderer : MonoBehaviour
    {
        [SerializeField]
        private Camera _renderCamera;
        [SerializeField]
        private Text _uiText;
        
        public Texture2D RenderTextAsTexture(string text, int width)
        {
            _uiText.text = text;

            RenderTexture cameraTexture = _renderCamera.targetTexture;
            //Save reference to currently active render texture
            RenderTexture currentActiveRenderTexture = RenderTexture.active;
            cameraTexture.Release();
            int height = (int)(cameraTexture.height * width / (float)cameraTexture.width);
            //Change size of camera render texture
            cameraTexture.width = width;
            cameraTexture.height = height;
            //Change currently active render texture
            RenderTexture.active = cameraTexture;
            _renderCamera.Render();

            Texture2D texture = new Texture2D(width, height);
            //Read from RenderTexture.active
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

            //Restore currently active render texture
            RenderTexture.active = currentActiveRenderTexture;

            return texture;
        }
    }
}
