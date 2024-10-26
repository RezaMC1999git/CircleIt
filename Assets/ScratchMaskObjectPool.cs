using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchMaskObjectPool : MonoBehaviour
{
    // Summary : Generates Masks PreRendered GameObjects, to Use Whenever Needed

    public static ScratchMaskObjectPool instance;

    public LevelManager levelManager;
    public GameObject maskPrefab;

    [HideInInspector] public List<GameObject> pooledObjects = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        // Generate Masks as much as We Need
        PoolMasks();
    }

    private void PoolMasks()
    {
        for (int i = 0; i < levelManager.requiredMasksForFinishingLevel; i++)
        {
            GameObject newMask = Instantiate(maskPrefab, this.transform);
            newMask.transform.SetParent(this.transform);
            newMask.SetActive(false);
            newMask.name = "mask" + i;
            pooledObjects.Add(newMask);
        }
    }

    public GameObject getMaskPooledObject()
    {
        // returns first diactive mask
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];
        }

        return null;
    }
}
