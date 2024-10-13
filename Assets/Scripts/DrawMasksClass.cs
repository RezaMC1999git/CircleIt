using UnityEngine;

public class DrawMasksClass : MonoBehaviour
{
    // Summary : Instantiates a Mask With Draw
    // Note : There Should Be ( Definately There is ! ) a Better Way To Track Player's Drag System
    // Other Than Using Some PreDefined Masks, so Later On This Code Can Be Very Much Optimized
    // Depend On Project Scale, or Even We Can Choose Another Method To Track The Drag .

    [Header("UI")]
    [Range(0.1f, 1)]
    [SerializeField] float minDistanceBetweenLines = 0.1f;
    Vector3 previousPosition;

    [Space]
    [Header("Classes")]
    public LevelManager levelManager;

    private void Start()
    {
        previousPosition = transform.position;
    }

    public void DrawLineOfMasks(Vector3 currentPosition)
    {
        if (Vector2.Distance(currentPosition, previousPosition) > minDistanceBetweenLines)
        {
            if (previousPosition != transform.position)
            {
                // Obtain a Mask From Pooled GameObjects
                GameObject newMask = MaskObjectPool.instance.getMaskPooledObject();

                if (newMask != null)
                {
                    currentPosition.z = 0;
                    newMask.transform.position = currentPosition;
                    newMask.SetActive(true);
                }
            }
            previousPosition = currentPosition;
        }
    }

    public void CheckIfFinished(Vector3 currentPosition)
    {
        // reached endPoint and doesn't reach it backward ! ( at least touched one of checkPoints )
        if (Vector2.Distance(currentPosition, levelManager.endPosition) <= 0.25f && levelManager.checkPointIndex >= 1)
        {
            StartCoroutine(levelManager.FinishGame());
            Destroy(MaskObjectPool.instance.gameObject);
        }
    }
}
