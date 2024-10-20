using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Summary : Controlls All Arrow Actions

    [Header("UI")]
    public Animator arrowAnimator;
    [HideInInspector] public bool CanMove;
    public AudioSource sfxPlayer;
    [Range(5, 10)]
    [SerializeField] float rotationSpeed = 5f;
    [Range(2, 10)]
    [SerializeField] float firstAnimationSpeed = 2f;
    Vector3 touchPosition, nextPosition;
    public Quaternion firstRotation;
    [SerializeField] GameObject arrowShapeChild, guideHand;
    bool showTutorial;

    [Space]
    [Header("Classes")]
    public LevelManager levelManager;

    private void Start()
    {
        StartCoroutine(ShowTutorialIE());
    }

    IEnumerator ShowTutorialIE()
    {
        // First Initial Arrow Then Play Hand Animation Then Follow The Path
        transform.position = levelManager.startPosition;
        guideHand.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        //nextPosition = levelManager.startPosition;
        touchPosition = levelManager.checkPoints[0];
        Vector3 position = touchPosition - transform.position;
        float targetAngle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        CanMove = true;
        yield return new WaitForSeconds(1f);
        showTutorial = true;
    }

    private void Update()
    {
        if (showTutorial)
            Tutorial();

        // Arrow Starts To Move
        if (Input.GetMouseButtonDown(0))
        {
            arrowAnimator.Play("IDLE");
        }

        // Dragging Arrow
        else if (Input.GetMouseButton(0) && CanMove)
        {
            ChangeArrowPosition();
            FollowPath();
            ReadSura();
        }

        // Stops Dragging
        if (Input.GetMouseButtonUp(0))
        {
            if (sfxPlayer.isPlaying)
                sfxPlayer.Stop();
            if (arrowShapeChild.transform.localPosition.x != 0)
                arrowShapeChild.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    void Tutorial()
    {
        if (!levelManager.levelStarted)
        {
            guideHand.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            FollowPath();
            if (levelManager.checkPointIndex >= levelManager.checkPoints.Count - 1)
            {
                showTutorial = false;
                levelManager.StartGame();
                return;
            }
            transform.position = Vector2.MoveTowards(transform.position, levelManager.checkPoints[levelManager.checkPointIndex], firstAnimationSpeed * Time.deltaTime);
        }
    }

    public void ResetArrow()
    {
        guideHand.SetActive(false);
        transform.position = levelManager.startPosition;
        touchPosition = levelManager.checkPoints[0];
        Vector3 position = touchPosition - transform.position;
        float targetAngle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    // If Touching Both Shape and Forward Collider, change arrow position
    void ChangeArrowPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        bool foundFirstTag = false;
        bool foundSecondTag = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Shape"))
            {
                foundFirstTag = true;
            }
            else if (hit.collider.CompareTag("Forward"))
            {
                foundSecondTag = true;
            }
            if (foundFirstTag && foundSecondTag)
            {
                if (!sfxPlayer.isPlaying)
                    sfxPlayer.Play();
                Vector3 phonePosition = new Vector3(hit.point.x, hit.point.y + 0.75f, 0);
                nextPosition = hit.point;
#if UNITY_ANDROID
                arrowShapeChild.transform.position = phonePosition;
#endif
                transform.position = nextPosition;
                CheckIfFinished(transform.position);
            }
        }
    }

    // Rotate Arrow and Create A Mask In The Path if Reached Next CheckPoint
    void FollowPath()
    {
        // Check if we reached next checkPoint
        if (Vector3.Distance(transform.position, levelManager.checkPoints[levelManager.checkPointIndex]) <= 0.55f)
        {
            if ((levelManager.checkPointIndex + 1) < levelManager.checkPoints.Count)
            {

                GameObject newMask = MaskObjectPool.instance.getMaskPooledObject();

                if (newMask != null)
                {
                    newMask.transform.rotation = levelManager.arrow.transform.rotation;
                    newMask.SetActive(true);
                }
                levelManager.checkPointIndex++;
                touchPosition = levelManager.checkPoints[levelManager.checkPointIndex];
            }
        }

        // Rotate Arrow Smoothly
        Vector3 position = touchPosition - transform.position;
        float targetAngle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        float currentAngle = transform.rotation.eulerAngles.z;
        float Lastangle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, 0, Lastangle);
    }

    void ReadSura()
    {
        if (levelManager.waitPointIndex >= levelManager.waitPoints.Count)
            return;
        // Check if we reached next waitPoint
        if (Vector3.Distance(transform.position, levelManager.waitPoints[levelManager.waitPointIndex]) <= 0.6f)
        {
            levelManager.PlaySura();
        }
    }

    public void CheckIfFinished(Vector3 currentPosition)
    {
        if (!MaskObjectPool.instance)
            return;
        // reached endPoint and doesn't reach it backward ! ( at last Missed Five of checkPoints )
        if (Vector2.Distance(currentPosition, levelManager.endPosition) <= 0.75f && levelManager.checkPointIndex >= levelManager.checkPoints.Count - 5)
        {
            StartCoroutine(levelManager.FinishGame());
            Destroy(MaskObjectPool.instance.gameObject);
        }
    }
}
