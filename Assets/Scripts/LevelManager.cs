using RTLTMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Summary : All Relative Variables To Controll A Level 

    public static LevelManager instance;

    [Header("UI")]
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Vector3 endPosition;
    public SpriteRenderer levelShapeBase, levelShapeDotted, levelShapeTouch, levelShapeComplete, levelShapeBlackAndWhite;
    public Animator shapeBWAnimator;
    [SerializeField] GameObject clickCirclePrefab;
    public GameObject quranIcon, dragGuideHand, dragCircle, endPanel;
    List<GameObject> quranIconsCreated;
    public Transform quranIconsParent;
    [HideInInspector] public List<Vector3> checkPoints, waitPoints;
    [HideInInspector] public int checkPointIndex, waitPointIndex, checkTest;
    [HideInInspector] public bool levelStarted, timeToScratch, levelFinished;

    [SerializeField] RTLTextMeshPro ayeText;
    [SerializeField] BoxCollider2D levelShapeBaseBoxCollider;
    [SerializeField] List<string> surasTexts;
    [SerializeField]
    [Range(10, 1000)]
    public int requiredMasksForFinishingLevel = 175;

    [Space]
    [Header("Classes")]
    public Arrow arrow;
    public MasksManager masksManager;

    [Space]
    [Header("SFX")]
    [SerializeField] AudioSource levelCompleteAudioSource;
    [SerializeField] AudioSource suraAudioSource;
    [SerializeField] List<AudioClip> suraSFX;

    [SerializeField] Queue<AudioClip> fetchedSuras;
    Coroutine suraCoroutine;

    private void Awake()
    {
        if (instance == null)
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
        dragCircle.SetActive(false);
        dragGuideHand.SetActive(false);
        quranIconsCreated = new List<GameObject>();
        for (int i = 0; i < waitPoints.Count; i++)
        {
            GameObject newQuranPrefab = Instantiate(quranIcon, quranIconsParent);
            newQuranPrefab.transform.position = waitPoints[i];
            quranIconsCreated.Add(newQuranPrefab);
        }
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
        checkPointIndex = 0;
        levelShapeTouch.gameObject.SetActive(true);
        masksManager.ResetMasks();
        arrow.ResetArrow();
        levelStarted = true;
    }

    public void TimeToScratch()
    {
        StartCoroutine(TimeToScratchIE());
    }

    public IEnumerator TimeToScratchIE()
    {
        arrow.CanMove = false;

        timeToScratch = true;

        Color nearTransparentColor = new Color(levelShapeBase.color.r, levelShapeBase.color.g, levelShapeBase.color.b,0.3f);
        levelShapeBase.color = nearTransparentColor;
        levelShapeDotted.gameObject.SetActive(false);
        levelShapeTouch.gameObject.SetActive(false);
        levelShapeComplete.enabled = true;
        levelShapeBase.maskInteraction = SpriteMaskInteraction.None;
        shapeBWAnimator.enabled = true;
        levelShapeBaseBoxCollider.enabled = false;

        arrow.sfxPlayer.Stop();
        arrow.gameObject.SetActive(false);
        dragGuideHand.SetActive(true);
        yield return new WaitForSeconds(2);
        dragGuideHand.SetActive(false);
    }

    public void FinishGame() 
    {
        StartCoroutine(FinishGameIE());
    } 

    IEnumerator FinishGameIE()
    {
        timeToScratch = false;
        levelFinished = true;
        dragCircle.SetActive(false);
        levelShapeBlackAndWhite.gameObject.SetActive(false);
        Color notTransparentColor = new Color(levelShapeBase.color.r, levelShapeBase.color.g, levelShapeBase.color.b, 1f);
        levelShapeBase.color = notTransparentColor;
        levelShapeComplete.maskInteraction = SpriteMaskInteraction.None;
        levelCompleteAudioSource.Play();

        yield return new WaitForSeconds(2f);
        endPanel.SetActive(true);
    }

    public void RepeatLevelOnClick() 
    {
        Application.LoadLevel(SceneManager.GetActiveScene().name);
    }

    public void NextLevelOnClick()
    {
        string levelName = SceneManager.GetActiveScene().name;
        Match match = Regex.Match(levelName, @"\d+"); // Find digits in the string
        if (match.Success)
        {
            int number = int.Parse(match.Value);
            if (number < 2)  // Load Next Level
                SceneManager.LoadScene("Level " + (number + 1).ToString());
            else // Back To Main Menu
            {
                SceneManager.LoadScene("Main Menu");
            }
        }
    }

    public void PlaySura()
    {
        waitPointIndex++;
        fetchedSuras.Enqueue(suraSFX[0]);
        suraSFX.RemoveAt(0);
        if (suraCoroutine == null)
        {
            suraCoroutine = StartCoroutine(PlaySuraIE());
        }
    }

    public IEnumerator PlaySuraIE()
    {
        ayeText.gameObject.SetActive(true);
        ayeText.text = surasTexts[0];
        surasTexts.RemoveAt(0);
        suraAudioSource.clip = fetchedSuras.Dequeue();
        suraAudioSource.Play();
        yield return new WaitForSeconds(suraAudioSource.clip.length);
        GameObject toDestroy = quranIconsCreated[0];
        Destroy(toDestroy);
        quranIconsCreated.RemoveAt(0);
        if (fetchedSuras.Count != 0)
        {
            suraCoroutine = StartCoroutine(PlaySuraIE());
        }
        else
            suraCoroutine = null;
    }

    #region Find Red Pixels in SpriteRenderer - Not Used

    [HideInInspector] public Vector2 WorldUnitsInCamera;
    [HideInInspector] public Vector2 WorldToPixelAmount;

    Vector2 FindRedPixelPosition()
    {
        // Get the texture from the sprite
        //Texture2D texture = test.sprite.texture;
        Texture2D texture = null;

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
                    //Vector2 worldPosition = test.transform.TransformPoint(localPosition);
                    Vector2 worldPosition = new Vector2(0f, 0f);
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
