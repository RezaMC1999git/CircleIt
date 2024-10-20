using UnityEngine;
using System.Collections.Generic;

public class SpritePointCollector : MonoBehaviour
{
    // Summary : This Class is Used For Adding or Removing Shapes Points 

    public LevelManager levelManager;
    public Vector3 startPosition, endPosition;
    public List<Vector3> checkPointClicks = new List<Vector3>();
    public List<Vector3> waitPointClicks = new List<Vector3>();
    [HideInInspector] public bool startPositionToggle, endPositionToggle, removeCheckPointsEnabled, removeWaitPointsEnabled;
    [HideInInspector] public int pointType, lastCheckPointRemovedIndex = -1, lastWaitPointRemovedIndex = -1;

    public void AddPosition(Vector3 position)
    {
        switch (pointType)
        {
            case (int)pointNames.start:
                startPosition = position;
                break;

            case (int)pointNames.checkPoint:
                if (lastCheckPointRemovedIndex != -1) // means we removed one or more checkPoints in Editor mode
                {
                    if (lastCheckPointRemovedIndex <= checkPointClicks.Count)
                        checkPointClicks.Insert(lastCheckPointRemovedIndex, position);
                    lastCheckPointRemovedIndex = -1;
                }
                else
                {
                    checkPointClicks.Add(position);
                }
                break;

            case (int)pointNames.waitPoint:
                if (lastWaitPointRemovedIndex != -1) // means we removed one or more waitPoints in Editor mode
                {
                    waitPointClicks.Insert(lastWaitPointRemovedIndex, position);
                    lastWaitPointRemovedIndex = -1;
                }
                else
                {
                    waitPointClicks.Add(position);
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

    public void RemovePosition(int index)
    {
        switch (pointType)
        {
            case (int)pointNames.checkPoint:
                lastCheckPointRemovedIndex = index;
                checkPointClicks.RemoveAt(index); // Remove point from list
                break;

            case (int)pointNames.waitPoint:
                lastWaitPointRemovedIndex = index;
                waitPointClicks.RemoveAt(index); // Remove point from list
                break;
        }
    }

    public bool AddOrRemove()
    {
        bool boolToReturn = false;
        switch (pointType)
        {
            case (int)pointNames.start:
                boolToReturn = true;
                break;

            case (int)pointNames.end:
                boolToReturn = true;
                break;

            case (int)pointNames.checkPoint:
                boolToReturn = !removeCheckPointsEnabled;
                break;

            case (int)pointNames.waitPoint:
                boolToReturn = !removeWaitPointsEnabled;
                break;
        }
        return boolToReturn;
    }

    private void OnDrawGizmos()
    {
        switch (pointType)
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

            case (int)pointNames.waitPoint:
                Gizmos.color = Color.magenta;
                foreach (var point in waitPointClicks)
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
    waitPoint,
    end
}
