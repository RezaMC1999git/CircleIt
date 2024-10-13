using UnityEngine;
using System.Collections.Generic;

public class SpritePointCollector : MonoBehaviour
{
    public LevelManager levelManager;
    public Vector3 startPosition, endPosition;
    public List<Vector3> checkPointClicks = new List<Vector3>();
    [HideInInspector] public bool removeCheckPointsEnabled;

    public int pointIndex, lastPointremovedIndex = -1;

    // Optional: Draw gizmos to visualize stored points
    public void AddPosition(Vector3 position)
    {
        switch (pointIndex)
        {
            case (int)pointNames.start:
                startPosition = position;
                break;

            case (int)pointNames.checkPoint:
                if (lastPointremovedIndex != -1) // means we removed one or more points in Editor mode
                {
                    checkPointClicks.Insert(lastPointremovedIndex, position);
                    lastPointremovedIndex = -1;
                }
                else
                {
                    checkPointClicks.Add(position);
                }
                break;

            case (int)pointNames.end:
                endPosition = position;
                break;
            default:
                startPosition = position;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        switch (pointIndex)
        {
            case (int)pointNames.start:
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(startPosition, 0.1f);
                break;

            case (int)pointNames.checkPoint:
                Gizmos.color = Color.yellow;
                foreach (var point in checkPointClicks)
                {
                    Gizmos.DrawSphere(point, 0.1f);
                }
                break;

            case (int)pointNames.end:
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(endPosition, 0.1f);
                break;

            default:
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(startPosition, 0.1f);
                break;
        }
    }
}

public enum pointNames
{
    start,
    checkPoint,
    end
}
