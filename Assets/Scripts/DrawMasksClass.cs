using System.Collections.Generic;
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
    [SerializeField] List<GameObject> allMasks;

    [Space]
    [Header("Classes")]
    public LevelManager levelManager;

    private void Start()
    {
        previousPosition = transform.position;
        allMasks = new List<GameObject>();
    }

    public void DrawLineOfMasks(Vector3 currentPosition)
    {
        if (!MaskObjectPool.instance)
            return;
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
                    if(allMasks.Count > 1 && levelManager.checkPointIndex > 3)
                        allMasks[allMasks.Count-1].transform.position = levelManager.checkPoints[levelManager.checkPointIndex - 3];
                    allMasks.Add(newMask);
                }
            }
            previousPosition = currentPosition;
        }
    }

    public void CheckIfFinished(Vector3 currentPosition)
    {
        if (!MaskObjectPool.instance)
            return;
        // reached endPoint and doesn't reach it backward ! ( at least touched one of checkPoints )
        if (Vector2.Distance(currentPosition, levelManager.endPosition) <= 0.25f && levelManager.checkPointIndex >= 1f)
        {
            StartCoroutine(levelManager.FinishGame());
            Destroy(MaskObjectPool.instance.gameObject);
        }
    }
}
