using UnityEngine;

public class ClickCircleClass : MonoBehaviour
{
    // Summary : Attaches To Click Animation's Last Frame, To Destoy Itself

    public void DestroyInstance()
    {
        Destroy(this.gameObject);
    }
}
