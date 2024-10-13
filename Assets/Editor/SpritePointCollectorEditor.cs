using UnityEngine;
using UnityEditor;
using static SpritePointCollector;
using System.Collections.Generic;

[CustomEditor(typeof(SpritePointCollector))]
public class SpritePointCollectorEditor : Editor
{
    private void OnSceneGUI()
    {
        SpritePointCollector collector = (SpritePointCollector)target;

        // Detect mouse clicks in the Scene view
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (!collector.removeCheckPointsEnabled) // Add A Point
            {
                if (hit.collider != null && hit.collider.gameObject == collector.gameObject)
                {
                    // Store the clicked position
                    Vector3 clickedPosition = hit.point;
                    collector.AddPosition(clickedPosition);
                    //Debug.Log($"Point stored: {clickedPosition}");

                    // Mark the object as changed to save in Editor
                    EditorUtility.SetDirty(collector);
                }
            }
            else // Remove A Point
            {
                float threshold = 0.2f; // Click detection threshold

                if (hit.collider != null && hit.collider.gameObject == collector.gameObject) 
                {
                    // Find the point closest to the click
                    for (int i = collector.checkPointClicks.Count - 1; i >= 0; i--)
                    {
                        if (Vector3.Distance(collector.checkPointClicks[i], hit.point) < threshold)
                        {
                            collector.lastPointremovedIndex = i;
                            collector.checkPointClicks.RemoveAt(i); // Remove point from list
                            //Debug.Log("Point removed!");
                            break;
                        }
                    }
                }
                // Mark scene as dirty to update changes
                EditorUtility.SetDirty(collector);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();  // Draws the default inspector layout

        SpritePointCollector collector = (SpritePointCollector)target;

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Choose Start Position", GUILayout.Width(400), GUILayout.Height(40)))
        {
            Debug.Log(" Start Point Saved ");
            collector.pointIndex = (int)pointNames.start;
            collector.levelManager.startPosition = collector.startPosition;
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Choose CheckPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            Debug.Log(" Choose Arrow Path ");
            collector.pointIndex = (int)pointNames.checkPoint;
        }

        if (GUILayout.Button("Remove All CheckPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            Debug.Log(" Arrow Path Removed ");
            collector.checkPointClicks = new List<Vector3>();
        }
        
        string buttonText = collector.removeCheckPointsEnabled ? "Removing A Point" : "Adding A Point";
        GUI.backgroundColor = collector.removeCheckPointsEnabled ? Color.black : Color.yellow;
        if (GUILayout.Button(buttonText, GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.removeCheckPointsEnabled = !collector.removeCheckPointsEnabled;
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Subtract CheckPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            if (collector.checkPointClicks.Count != 0)
                collector.checkPointClicks.RemoveAt(collector.checkPointClicks.Count - 1);
            Debug.Log(" CheckPoint Subtracted ");
        }
        
        if (GUILayout.Button("Save CheckPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.levelManager.checkPoints = collector.checkPointClicks;
            Debug.Log(" CheckPoints Data Saved In LevelManager");
        }
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Choose End Position", GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.pointIndex = (int)pointNames.end;
            collector.levelManager.endPosition = collector.endPosition;
            Debug.Log(" End Point Saved ");
        }
    }
}
