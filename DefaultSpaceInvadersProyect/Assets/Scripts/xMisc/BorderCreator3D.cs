using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BorderCreator3D : MonoBehaviour
{
    public List<Vector3> MapBorders;
    public List<Vector3> Centers;
    public List<BoxCollider> Colliders;

    [Header("Offsets")]
    public float TopOffset = 0f;
    public float BotOffset = 0f;
    public float LeftOffset = 0f;
    public float RightOffset = 0f;

    private float ColliderRadius = 0.5f;
    private float ScreenWidth;
    private float ScreenHeight;
    private Vector2 ScreenSizeInUnits;
    private Camera Cam = null;
    private MeshFilter MeshFilter { get; set; }
    private MeshCollider MeshCollider { get; set; }
    private EdgeCollider2D Collider2D { get; set; }
    private Vector3[] Vertices = new Vector3[16];
    private Vector2[] UV = new Vector2[16];
    private int[] Triangles = new int[48];


    private void Start()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        StartBorderConfig();
    }

    public void StartBorderConfig()
    {
        if (Colliders.Count != 0)
        {
            var ToDelete = Colliders.GetRange(0, Colliders.Count);
            foreach (var collider in ToDelete)
            {
                Destroy(collider);
            }
            ToDelete.Clear();
            Colliders.Clear();
        }
        MapBorders.Clear();
        Centers.Clear();
        CreateBorder();
        ScreenSizeInUnits = new Vector2(Mathf.Abs(MapBorders[0].x) + Mathf.Abs(MapBorders[1].x), Mathf.Abs(MapBorders[0].y) + Mathf.Abs(MapBorders[2].y));
        CreateColliders();
        AddOffset();
    }

    private void CreateBorder()
    {
        Cam = Camera.main;//GameSceneController.Instance.Camera.GetComponent<Camera>();
        Vector3 DLP = Cam.ScreenToWorldPoint(Vector3.zero);
        Vector3 DRP = Cam.ScreenToWorldPoint(new Vector3(ScreenWidth, 0, 0));
        Vector3 ULP = Cam.ScreenToWorldPoint(new Vector3(0, ScreenHeight, 0));
        Vector3 URP = Cam.ScreenToWorldPoint(new Vector3(ScreenWidth, ScreenHeight));
        DLP.y = 0;
        DRP.y = 0;
        ULP.y = 0;
        URP.y = 0;
        MapBorders.Add(DLP);
        MapBorders.Add(DRP);
        MapBorders.Add(ULP);
        MapBorders.Add(URP);
    }

    private void CreateColliders()
    {
        //ColliderUp
        var ColUp = gameObject.AddComponent<BoxCollider>();
        ColUp.center = new Vector3(0, 0, MapBorders[2].z + ColliderRadius * 2);
        ColUp.size = new Vector3(ScreenSizeInUnits.x + ColliderRadius * 2, ColliderRadius * 2, ColliderRadius * 2);
        Centers.Add(ColUp.center);
        Colliders.Add(ColUp);
        //ColliderDown
        var ColDown = gameObject.AddComponent<BoxCollider>();
        ColDown.center = new Vector3(0, 0, -MapBorders[2].z - ColliderRadius * 2);
        ColDown.size = new Vector3(ScreenSizeInUnits.x + ColliderRadius * 2, ColliderRadius * 2, ColliderRadius * 2);
        Centers.Add(ColDown.center);
        Colliders.Add(ColDown);
        //ColliderLeft
        var ColLeft = gameObject.AddComponent<BoxCollider>();
        ColLeft.center = new Vector3(MapBorders[2].x - ColliderRadius * 2, 0, 0);
        ColLeft.size = new Vector3(ColliderRadius * 2, ColliderRadius * 2, ScreenSizeInUnits.x + ColliderRadius * 2);
        Centers.Add(ColLeft.center);
        Colliders.Add(ColLeft);
        //ColliderRight
        var ColRight = gameObject.AddComponent<BoxCollider>();
        ColRight.center = new Vector3(MapBorders[3].x + ColliderRadius * 2, 0, 0);
        ColRight.size = new Vector3(ColliderRadius * 2, ColliderRadius * 2, ScreenSizeInUnits.x + ColliderRadius * 2);
        Centers.Add(ColRight.center);
        Colliders.Add(ColRight);
    }

    void AddOffset()
    {
        Colliders[0].center = new Vector3(Colliders[0].center.x, Colliders[0].center.y, Colliders[0].center.z + TopOffset);
        Colliders[1].center = new Vector3(Colliders[1].center.x, Colliders[1].center.y, Colliders[1].center.z + BotOffset);
        Colliders[2].center = new Vector3(Colliders[2].center.x + LeftOffset, Colliders[2].center.y, Colliders[2].center.z);
        Colliders[3].center = new Vector3(Colliders[3].center.x + RightOffset, Colliders[3].center.y, Colliders[3].center.z);
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(BorderCreator3D))]
public class BorderCreator3DEditor : Editor
{
    void OnSceneGUI()
    {
        BorderCreator3D myEnemySpawnScriptRef = target as BorderCreator3D;
        var tr = myEnemySpawnScriptRef.transform;

        var colorOrange = new Color(1, 0.8f, 0.4f, 1);
        Handles.color = colorOrange;

        for (int i = 0; i < myEnemySpawnScriptRef.MapBorders.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 newVector = Handles.PositionHandle(myEnemySpawnScriptRef.MapBorders[i], Quaternion.identity);
            Handles.DrawWireDisc(newVector, tr.up, 1f);
            Handles.Label(newVector, i.ToString());

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(myEnemySpawnScriptRef, $"Change Vector {i}");
                myEnemySpawnScriptRef.MapBorders[i] = newVector;
            }
        } 
    }

    public override void OnInspectorGUI()
    {

        BorderCreator3D myEnemySpawnScriptRef = (BorderCreator3D)target;

        DrawDefaultInspector();
        if (GUILayout.Button("Recrear Bordes"))
        {
            if(EditorApplication.isPlaying)
            {
                myEnemySpawnScriptRef.StartBorderConfig();
            }
            else
            {
                Debug.Log("debes estar en 'Playmode' para poder utilizar esta funcion");
            }
        }
    }
}

#endif