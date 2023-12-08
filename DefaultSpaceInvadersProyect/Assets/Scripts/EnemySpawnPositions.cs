using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemySpawnPositions : MonoBehaviour
{

    public static EnemySpawnPositions Instance { get; private set; }
    public List<Vector3> SpawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetRandomSP()
    {
        return SpawnPoints[Random.Range(0,SpawnPoints.Count)];
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(EnemySpawnPositions))]
public class MyEnemySpawnPositionsEditor : Editor
{
    void OnSceneGUI()
    {
        EnemySpawnPositions myEnemySpawnScriptRef = target as EnemySpawnPositions;
        var tr = myEnemySpawnScriptRef.transform;

        var colorOrange = new Color(1, 0.8f, 0.4f, 1);
        Handles.color = colorOrange;

        for (int i = 0; i < myEnemySpawnScriptRef.SpawnPoints.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 newVector = Handles.PositionHandle(myEnemySpawnScriptRef.SpawnPoints[i], Quaternion.identity);
            Handles.DrawWireDisc(newVector, tr.up, 1f);
            Handles.Label(newVector, i.ToString());

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(myEnemySpawnScriptRef, $"Change Vector {i}");
                myEnemySpawnScriptRef.SpawnPoints[i] = newVector;
            }
        }
    }
}

#endif