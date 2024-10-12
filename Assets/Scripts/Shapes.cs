using System.Collections;
using UnityEngine;

public class Shapes : MonoBehaviour
{
    // Summary : Attaches To Intro Animation's Last Frame, To Start Level
    
    [Header("Classes")]
    public LevelManager levelManager;
    [SerializeField] Vector3[] worldVertices;
    public SpriteRenderer spriteRenderer;
    public GameObject arrowTest;

    private void Start()
    {
        worldVertices = GetWorldVertices(spriteRenderer);
        //levelManager.checkPoints = worldVertices;
        //StartCoroutine(TestPosition());
    }

    IEnumerator TestPosition() 
    {
        for (int i = 0; i < worldVertices.Length; i++)
        {
            arrowTest.transform.position = worldVertices[i];
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(TestPosition());
    }

    Vector3[] GetWorldVertices(SpriteRenderer renderer)
    {
        Sprite sprite = renderer.sprite;
        Vector3[] localVertices = new Vector3[sprite.vertices.Length];

        // Convert local sprite vertices to world space
        for (int i = 0; i < sprite.vertices.Length; i++)
        {
            Vector2 localPos = sprite.vertices[i];
            localVertices[i] = renderer.transform.TransformPoint(localPos);
        }

        return localVertices;
    }

    public void EndAnimationEvent() 
    {
        levelManager.StartGame();
    }
}
