using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCanonLevelData : MonoBehaviour
{
    public string NameSceneToLoad = "";

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void NameToChangeScene(string name)
    {
        NameSceneToLoad = name;
    }
}
