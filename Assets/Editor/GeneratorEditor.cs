using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        

        GridGenerator myScript = (GridGenerator)target;
        if (GUILayout.Button("Generate Grid"))
        {
            myScript.GenerateGrid();
        }
        if (GUILayout.Button("Grow Rooms"))
        {
            myScript.GrowRooms();
        }

        

    }
}
