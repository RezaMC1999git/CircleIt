using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class SpritePointCollector : MonoBehaviour
{
    public LevelManager levelManager;
    public Vector3 startPosition, endPosition;
    public List<Vector3> checkPointClicks = new List<Vector3>();
    public List<Vector3> waitPointClicks = new List<Vector3>();
    [HideInInspector] public bool removeCheckPointsEnabled, removeWaitPointsEnabled;
    [HideInInspector] public int pointType, lastCheckPointremovedIndex = -1, lastWaitPointremovedIndex = -1;

    // Optional: Draw gizmos to visualize stored points
    public void AddPosition(Vector3 position)
    {
        switch (pointType)
        {
            case (int)pointNames.start:
                startPosition = position;
                break;

            case (int)pointNames.checkPoint:
                if (lastCheckPointremovedIndex != -1) // means we removed one or more points in Editor mode
                {
                    checkPointClicks.Insert(lastCheckPointremovedIndex, position);
                    lastCheckPointremovedIndex = -1;
                }
                else
                {
                    checkPointClicks.Add(position);
                }
                break;

            case (int)pointNames.waitPoint:
                if (lastWaitPointremovedIndex != -1) // means we removed one or more points in Editor mode
                {
                    waitPointClicks.Insert(lastWaitPointremovedIndex, position);
                    lastWaitPointremovedIndex = -1;
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
                lastCheckPointremovedIndex = index;
                checkPointClicks.RemoveAt(index); // Remove point from list
                break;

            case (int)pointNames.waitPoint:
                lastWaitPointremovedIndex = index;
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
