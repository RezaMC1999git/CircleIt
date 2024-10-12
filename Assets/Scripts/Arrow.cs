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
    Vector3 touchPosition;

    [Space]
    [Header("Classes")]
    public LevelManager levelManager;

    private void Start()
    {
        touchPosition = levelManager.checkPoints[0];
        CanMove = true;
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
            //RotateArrow();
        }

        // Stops Dragging
        if (Input.GetMouseButtonUp(0)) 
        {
            if (sfxPlayer.isPlaying)
                sfxPlayer.Stop();
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
            if (hit.collider.CompareTag("Player"))
            {
                foundFirstTag = true;
            }
            else if (hit.collider.CompareTag("Finish"))
            {
                foundSecondTag = true;
            }
            if (foundFirstTag && foundSecondTag)
            {
                if (!sfxPlayer.isPlaying)
                    sfxPlayer.Play();
                transform.position = hit.point;
                levelManager.lineRenderer.DrawLineOfMasks(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                levelManager.lineRenderer.CheckIfFinished(transform.position);
            }
        }
    }

    void RotateArrow() 
    {
        // Check if we reached next checkPoint
        if (Vector3.Distance(transform.position, levelManager.checkPoints[levelManager.checkPointIndex]) <= 0.5f)
        {
            if ((levelManager.checkPointIndex + 1) < levelManager.checkPoints.Length)
            {
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
}
