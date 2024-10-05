using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Summary : All Relative Variables To Controll A Level 

    public static LevelManager instance;

    [Header("UI")]
    public Vector3 startPosition;
    public Vector3 finishPosition;
    public SpriteRenderer levelShapeBase ,levelShapeDotted, levelShapeTouch;
    public Animator shapeCompleteAnimator, shapesAnimator;
    [SerializeField] GameObject clickCirclePrefab, fakeArrowGO, fakeMasksGO;
    [Range(50,200)]
    public int amountOfMasksToPool;
    public Vector3[] checkPoints;
    [HideInInspector] public int checkPointIndex;
    [HideInInspector] public bool levelStarted, levelFinished;

    [Space]
    [Header("Classes")]
    public Arrow arrow;
    public DrawMasksClass lineRenderer;
    
    [Space]
    [Header("SFX")]
    [SerializeField] AudioSource levelCompleteSFX;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        arrow.gameObject.transform.position = startPosition;
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
        levelCompleteSFX.Play();
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
