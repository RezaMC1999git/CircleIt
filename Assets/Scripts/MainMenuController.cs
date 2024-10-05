using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Summary : Controlls Every Action In MainMenu

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void LoadScene() 
    {
        SceneManager.LoadScene("Level 1");        
    }
}
