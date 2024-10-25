using System.Collections.Generic;
using UnityEngine;

public class Scratch : MonoBehaviour
{
    // Summary : An Attempt to Attaching Mask to LineRenderer, which has Failed !

    [SerializeField] SpriteRenderer bwSpriteRenderer;
    [SerializeField] LevelManager levelManager;
    [SerializeField] GameObject scratchMaskPrefab;
    [SerializeField] Transform scratchMasksParent;

    List<GameObject> scratckMasksCreated;
    Vector2 mousePosition;

    private void Start()
    {
        scratckMasksCreated = new List<GameObject>();
    }

    private void OnMouseDrag()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (scratckMasksCreated.Count >= levelManager.requiredMasksForFinishingLevel)
        {
            Destroy(scratchMasksParent.gameObject);
            StartCoroutine(levelManager.FinishGame());
            return;
        }
        if (levelManager.timeToScratch)
        {
            if (!levelManager.dragCircle.activeInHierarchy)
            {
                levelManager.dragCircle.SetActive(true);
            }
            levelManager.dragCircle.transform.position = mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);
            // Iterate through all pixels
            //foreach (Color pixel in pixels)
            //{
            //    // Check if the pixel is revealed (e.g., non-transparent)
            //    if (pixel.a > 0.0f)
            //    { // Alpha greater than 0 means it's visible
            //        revealedPixelCount++;
            //    }
            //}
            //Debug.LogError((revealedPixelCount / bwTexture.GetPixels().Length) * 100f);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("BlackAndWhite"))
                {
                    if (scratckMasksCreated.Count != 0)
                    {
                        if (Vector3.Distance(hit.point, scratckMasksCreated[scratckMasksCreated.Count - 1].transform.position) >= 0.5f)
                        {
                            AddMask();
                        }
                    }
                    else
                    {
                        AddMask();
                    }
                }
            }
        }
    }

    private void AddMask()
    {
        GameObject newScratchMask = Instantiate(scratchMaskPrefab, mousePosition, Quaternion.identity);
        newScratchMask.transform.SetParent(scratchMasksParent);
        scratckMasksCreated.Add(newScratchMask);
    }
}
