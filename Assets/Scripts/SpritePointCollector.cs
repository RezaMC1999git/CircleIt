using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Linq;

public class SpritePointCollector : MonoBehaviour
{
    // Summary : This Class is Used For Adding or Removing Shapes Points 

    public LevelManager levelManager;
    public Vector3 startPosition, endPosition;
    public List<Vector3> checkPointClicks = new List<Vector3>();
    public List<Vector3> waitPointClicks = new List<Vector3>();
    [HideInInspector] public bool startPositionToggle, endPositionToggle, removeCheckPointsEnabled, removeWaitPointsEnabled;
    public int pointType, lastCheckPointRemovedIndex = -1, lastWaitPointRemovedIndex = -1;

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
                    if (checkPointClicks.Count != 0) 
                    {
                        GeneratePointsBasedOnDistance(checkPointClicks[checkPointClicks.Count - 1], position, 0.35f);
                        // if You Don't Want to Use Automatic Adding Point, Simply Comment Top Line and Use Bellow Line Instead 
                        //checkPointClicks.Add(position);
                    }
                        
                    else
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

    void GeneratePointsBasedOnDistance(Vector3 start, Vector3 end, float gap)
    {
        float totalDistance = Vector3.Distance(start, end); // Calculate the distance
        int numberOfPoints = Mathf.FloorToInt(totalDistance / gap); // Calculate how many points fit
        List<Vector3> points = new List<Vector3>();

        for (int i = 1; i <= numberOfPoints; i++)
        {
            float t = (float)i / (numberOfPoints + 1);
            Vector3 point = Vector3.Lerp(start, end, t); // Interpolate the point
            points.Add(point);
        }
        var sortedPoints = points.OrderBy(p => Vector3.Distance(start, p)).ToList();
        for (int i = 0; i < sortedPoints.Count; i++)
        {
            checkPointClicks.Add(sortedPoints[i]);
        }
        checkPointClicks.Add(end);
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
