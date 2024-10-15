using Unity.Burst.CompilerServices;
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
    Vector3 touchPosition, nextPosition;
    [SerializeField] GameObject arrowShapeChild;

    [Space]
    [Header("Classes")]
    public LevelManager levelManager;

    private void Start()
    {
        touchPosition = levelManager.checkPoints[0];
        nextPosition = levelManager.startPosition;
        CanMove = true;
        RotateArrow(); // at first arrow rotation should be toward first checkPoint
    }

    private void Update()
    {
        // Arrow Starts To Move
        if (Input.GetMouseButtonDown(0))
        {
            arrowAnimator.Play("IDLE");
        }

        // Dragging Arrow
        else if (Input.GetMouseButton(0) && CanMove)
        {
            ChangeArrowPosition();
            RotateArrow();
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
                Vector3 phonePosition = new Vector3(hit.point.x, hit.point.y + 0.5f, 0);
                nextPosition = hit.point;
#if UNITY_ANDROID
                arrowShapeChild.transform.position = phonePosition;
#endif
                transform.position = nextPosition;
                levelManager.lineRenderer.DrawLineOfMasks(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                levelManager.lineRenderer.CheckIfFinished(transform.position);
            }
        }
    }

    void RotateArrow()
    {
        // Check if we reached next checkPoint
        if (Vector3.Distance(transform.position, levelManager.checkPoints[levelManager.checkPointIndex]) <= 0.6f)
        {
            if ((levelManager.checkPointIndex + 1) < levelManager.checkPoints.Count)
            {

                levelManager.checkPointIndex++;
                //levelManager.lineRenderer.DrawLineOfMasks(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
        if (Vector3.Distance(transform.position, levelManager.waitPoints[levelManager.waitPointIndex]) <= 0.5f)
        {
            levelManager.PlaySura();
        }
    }
}
