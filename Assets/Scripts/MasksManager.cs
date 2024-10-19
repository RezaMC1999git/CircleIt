using UnityEngine;

public class MasksManager : MonoBehaviour
{
    // Summary : Instantiates a Mask With Draw
    // Note : There Should Be ( Definately There is ! ) a Better Way To Track Player's Drag System
    // Other Than Using Some PreDefined Masks, so Later On This Code Can Be Very Much Optimized
    // Depend On Project Scale, or Even We Can Choose Another Method To Track The Drag .

    public void ResetMasks()
    {
        for (int i = 0; i < MaskObjectPool.instance.transform.childCount; i++)
        {
            MaskObjectPool.instance.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
