using UnityEngine;

public class Scratch : MonoBehaviour
{
    // Summary : An Attempt to Attaching Mask to LineRenderer, which has Failed !

    public SpriteMask mask;
    public Camera cam;

    public void AssignCameraAsMask()
    {
        int height = Screen.height;
        int width = Screen.width;
        int depth = 1;

        RenderTexture renderTexture = new RenderTexture(width, height, depth);
        Rect rect = new Rect(0, 0, width, height);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        cam.targetTexture = renderTexture;
        cam.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        cam.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        Destroy(renderTexture);

        Sprite sprite = Sprite.Create(texture, rect, new Vector2(-5f, -5f), Screen.height / 10);

        mask.sprite = sprite;
    }
}
