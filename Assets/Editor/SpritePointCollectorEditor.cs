using UnityEngine;
using UnityEditor;
using static SpritePointCollector;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(SpritePointCollector))]
public class SpritePointCollectorEditor : Editor
{
    private void OnSceneGUI()
    {
        SpritePointCollector collector = (SpritePointCollector)target;
        bool addingPoint = collector.AddOrRemove();
        // Detect mouse clicks in the Scene view
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (addingPoint) // Add A Point
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
                    switch (collector.pointType)
                    {
                        case (int)pointNames.checkPoint:
                            // Find the point closest to the click
                            for (int i = collector.checkPointClicks.Count - 1; i >= 0; i--)
                            {
                                if (Vector3.Distance(collector.checkPointClicks[i], hit.point) < threshold)
                                {
                                    collector.RemovePosition(i);
                                    //Debug.Log("Point removed!");
                                    break;
                                }
                            }
                            break;

                        case (int)pointNames.waitPoint:
                            // Find the point closest to the click
                            for (int i = collector.waitPointClicks.Count - 1; i >= 0; i--)
                            {
                                if (Vector3.Distance(collector.waitPointClicks[i], hit.point) < threshold)
                                {
                                    collector.RemovePosition(i);
                                    //Debug.Log("Point removed!");
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
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

        #region Start Position Button
        GUI.backgroundColor = Color.green;
        string startButtonText = collector.startPositionToggle ? "Tap Again To Save" : "Choose Start Position";
        if (GUILayout.Button(startButtonText, GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.pointType = (int)pointNames.start;
            collector.startPositionToggle = !collector.startPositionToggle;
            collector.endPositionToggle = false;

            if (!collector.startPositionToggle) 
            {
                Debug.Log(" Start Point Saved ");
                collector.levelManager.startPosition = collector.startPosition;
            }
        }
        #endregion
        #region CheckPoints Position Button
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Choose CheckPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            Debug.Log(" Choose Arrow Path ");
            collector.pointType = (int)pointNames.checkPoint;
        }

        if (GUILayout.Button("Remove All CheckPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            Debug.Log(" Arrow Path Removed ");
            collector.checkPointClicks = new List<Vector3>();
        }
        
        string checkPointButtonText = collector.removeCheckPointsEnabled ? "Removing A Point" : "Adding A Point";
        GUI.backgroundColor = collector.removeCheckPointsEnabled ? Color.black : Color.yellow;
        if (GUILayout.Button(checkPointButtonText, GUILayout.Width(400), GUILayout.Height(40)))
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
        #endregion
        #region waitPoints Position Button
        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("Choose WaitPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            Debug.Log(" Choose Suras Path ");
            collector.pointType = (int)pointNames.waitPoint;
        }

        if (GUILayout.Button("Remove All WaitPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            Debug.Log(" Suras Path Removed ");
            collector.waitPointClicks = new List<Vector3>();
        }

        string suraButtonText = collector.removeWaitPointsEnabled ? "Removing A Point" : "Adding A Point";
        GUI.backgroundColor = collector.removeWaitPointsEnabled ? Color.black : Color.magenta;
        if (GUILayout.Button(suraButtonText, GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.removeWaitPointsEnabled = !collector.removeWaitPointsEnabled;
        }

        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("Subtract WaitPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            if (collector.waitPointClicks.Count != 0)
                collector.waitPointClicks.RemoveAt(collector.waitPointClicks.Count - 1);
            Debug.Log(" WaitPoint Subtracted ");
        }

        if (GUILayout.Button("Save WaitPoints", GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.levelManager.waitPoints = collector.waitPointClicks;
            Debug.Log(" WaitPoints Data Saved In LevelManager");
        }
        #endregion
        #region End Position Button
        GUI.backgroundColor = Color.red;
        string endButtonText = collector.endPositionToggle ? "Tap Again To Save" : "Choose End Position";
        if (GUILayout.Button(endButtonText, GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.pointType = (int)pointNames.end;
            collector.endPositionToggle = !collector.endPositionToggle;
            collector.startPositionToggle = false;

            if (!collector.endPositionToggle) 
            {
                collector.levelManager.endPosition = collector.endPosition;
                Debug.Log(" End Point Saved ");
            }
        }
        #endregion
        #region Disable Editor Button
        GUI.backgroundColor = Color.black;
        if (GUILayout.Button("Disable Editor Button", GUILayout.Width(400), GUILayout.Height(40)))
        {
            collector.pointType = -1;
            collector.endPositionToggle = false;
            collector.startPositionToggle = false;
            Debug.Log(" Editor Disabled ");
        }
        #endregion
    }
}
