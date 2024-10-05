using UnityEngine;

public class Shapes : MonoBehaviour
{
    // Summary : Attaches To Intro Animation's Last Frame, To Start Level
    
    [Header("Classes")]
    public LevelManager levelManager;

    public void EndAnimationEvent() 
    {
        levelManager.StartGame();
    }
}
