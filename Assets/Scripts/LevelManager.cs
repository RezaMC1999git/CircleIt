using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Summary : All Relative Variables To Controll A Level 

    public static LevelManager instance;

    [Header("UI")]
    public Vector3 startPosition;
    public Vector3 endPosition;
    public SpriteRenderer levelShapeBase ,levelShapeDotted, levelShapeTouch;
    public Animator shapeCompleteAnimator, shapesAnimator;
    [SerializeField] GameObject  clickCirclePrefab, fakeArrowGO, fakeMasksGO;
    public GameObject quranIcon;
    [SerializeField] List<GameObject> quranIconsCreated;
    public Transform quranIconsParent;
    [Range(50,500)]
    public int amountOfMasksToPool;
    public List<Vector3> checkPoints, waitPoints;
    public int checkPointIndex, waitPointIndex;
    [HideInInspector] public bool levelStarted, levelFinished;
    public SpriteRenderer test;

    [Space]
    [Header("Classes")]
    public Arrow arrow;
    public DrawMasksClass lineRenderer;
    
    [Space]
    [Header("SFX")]
    [SerializeField] AudioSource levelCompleteAudioSource;
    [SerializeField] AudioSource suraAudioSource;
    [SerializeField] List<AudioClip> suraSFX;

    [SerializeField] Queue<AudioClip> fetchedSuras;
    Coroutine suraCoroutine;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        #region Find Red Pixels in SpriteRenderer - Not Used
        //WorldUnitsInCamera.y = Camera.main.orthographicSize * 2;
        //WorldUnitsInCamera.x = WorldUnitsInCamera.y * Screen.width / Screen.height;

        //WorldToPixelAmount.x = Screen.width / WorldUnitsInCamera.x;
        //WorldToPixelAmount.y = Screen.height / WorldUnitsInCamera.y;
        #endregion
        for (int i = 0; i < waitPoints.Count; i++)
        {
            GameObject newQuranPrefab = Instantiate(quranIcon, quranIconsParent);
            newQuranPrefab.transform.position = waitPoints[i];
            quranIconsCreated.Add(newQuranPrefab);
        }
        arrow.gameObject.transform.position = startPosition;
        fetchedSuras = new Queue<AudioClip>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateWave();

            // To Skip Level's Intro Animation
            if (!levelStarted)
                StartGame();
        }
    }

    void CreateWave()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // to match camera distance
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Instantiate(clickCirclePrefab, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
    }

    public void StartGame() 
    {
        // Make All Shapes Full Color If Skipped Intro Animation
        Color fullAlphaColor = new Color(1f, 1f, 1f,1f);
        levelShapeBase.color = new Color(levelShapeBase.color.r, levelShapeBase.color.g, levelShapeBase.color.b, fullAlphaColor.a);
        levelShapeDotted.color = new Color(levelShapeDotted.color.r, levelShapeDotted.color.g, levelShapeDotted.color.b, fullAlphaColor.a);
        levelShapeTouch.color = new Color(levelShapeTouch.color.r, levelShapeTouch.color.g, levelShapeTouch.color.b, fullAlphaColor.a);

        levelShapeTouch.gameObject.SetActive(true);
        fakeMasksGO.SetActive(false);
        fakeArrowGO.SetActive(false);
        shapesAnimator.enabled = false;
        arrow.gameObject.SetActive(true);
        lineRenderer.gameObject.SetActive(true);
    }

    public IEnumerator FinishGame()
    {
        arrow.CanMove = false;
        levelFinished = true;
        levelShapeDotted.gameObject.SetActive(false);
        levelShapeTouch.gameObject.SetActive(false);
        shapeCompleteAnimator.enabled = true;

        arrow.sfxPlayer.Stop();
        levelCompleteAudioSource.Play();
        yield return new WaitForSeconds(4f);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlaySura() 
    {
        waitPointIndex++;
        fetchedSuras.Enqueue(suraSFX[0]);
        if (suraCoroutine == null) 
        {
            suraCoroutine = StartCoroutine(PlaySuraIE());
        }
    }

    public IEnumerator PlaySuraIE() 
    {
        suraAudioSource.clip = fetchedSuras.Dequeue();
        suraAudioSource.Play();
        yield return new WaitForSeconds(suraAudioSource.clip.length);
        GameObject toDestroy = quranIconsCreated[0];
        Destroy(toDestroy);
        quranIconsCreated.RemoveAt(0);
        if (fetchedSuras.Count != 0)
            suraCoroutine = StartCoroutine(PlaySuraIE());
        else
            suraCoroutine = null;
    }

    #region Find Red Pixels in SpriteRenderer - Not Used

    [HideInInspector] public Vector2 WorldUnitsInCamera;
    [HideInInspector] public Vector2 WorldToPixelAmount;
    
    Vector2 FindRedPixelPosition()
    {
        // Get the texture from the sprite
        Texture2D texture = test.sprite.texture;

        // Loop through pixels in the texture
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color pixelColor = texture.GetPixel(x, y);

                // Check if the pixel is red (with tolerance)
                if (IsRed(pixelColor))
                {
                    // Convert pixel coordinates to local space
                    Vector2 localPosition = new Vector2(
                        (float)x / texture.width + 0.5f,
                        (float)y / texture.height + 0.5f
                    );

                    // Convert to world space
                    Debug.LogError("Lo:" + localPosition);
                    Vector2 worldPosition = test.transform.TransformPoint(localPosition);
                    return worldPosition;
                }
            }
        }

        return Vector2.zero; // No red pixel found
    }

    bool IsRed(Color color)
    {
        // Simple red color detection with a tolerance
        return color.r == 1 && color.g == 0f && color.b == 0f;
    }

    Vector2 FindRedDot(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        Color[] pixels = texture.GetPixels();

        // Loop through pixels to find the red dot (assuming it's pure red)
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color pixelColor = pixels[y * texture.width + x];

                // Check if pixel is pure red
                if (pixelColor == Color.red)
                {
                    // Convert texture coordinates to sprite space (pivot-adjusted)
                    Vector2 spritePos = new Vector2(
                        (float)x / texture.width - sprite.pivot.x / sprite.rect.width,
                        (float)y / texture.height - sprite.pivot.y / sprite.rect.height
                    );
                    return spritePos;
                }
            }
        }
        return Vector2.zero; // Default if not found
    }

    public Vector2 ConvertToWorldUnits(Vector2 TouchLocation)
    {
        Vector2 returnVec2;

        returnVec2.x = ((TouchLocation.x / WorldToPixelAmount.x) - (WorldUnitsInCamera.x / 2));
        returnVec2.y = ((TouchLocation.y / WorldToPixelAmount.y) - (WorldUnitsInCamera.y / 2));

        return returnVec2;
    }
    #endregion
}
