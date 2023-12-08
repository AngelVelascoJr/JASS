using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    //Json Save System
    public static readonly string LEVELS_SCORE_PATH = Application.persistentDataPath + "/CanonLevelScores/"; //persistentDataPath para guardar archivos de usuario
    public static readonly string LEVELS_PATH = Application.streamingAssetsPath + "/CanonLevels/";    //streamingAssetsPath para obtener datos de las builds
                                                                                                      //ej: niveles canon
    public static readonly string EXTENSION = ".SIP";

    /// <summary>
    /// Initializes the save/load system
    /// </summary>
    public static void Initialize()
    {
        if(!Directory.Exists(LEVELS_SCORE_PATH))
        {
            Directory.CreateDirectory(LEVELS_SCORE_PATH);
        }
        if (!Directory.Exists(LEVELS_PATH))
        {
            Directory.CreateDirectory(LEVELS_PATH);
        }
    }

    /// <summary>
    /// Saves the level score into players persistent data path
    /// </summary>
    /// <param name="data">The level score as Json string</param>
    /// <param name="name"></param>
    public static void SaveCanonLevelScore(string data, string name)
    {
        File.WriteAllText(LEVELS_SCORE_PATH + name + EXTENSION, data);
    }
    
    /// <summary>
    /// Loads the level
    /// </summary>
    /// <param name="name">name of the level to load</param>
    /// <returns>the Json string of the data of the level</returns>
    public static string LevelLoad(string name)
    {
        if (File.Exists(LEVELS_PATH + name + EXTENSION))
        {
            string data = File.ReadAllText(LEVELS_PATH + name + EXTENSION);
            return data;
        }
        else
        return null;
    }

    /// <summary>
    /// Loads the level score, if theres one
    /// </summary>
    /// <param name="name"></param>
    /// <returns>returns the Json string of the score, return null if thers not one</returns>
    public static string LevelScoreLoad(string name)
    {
        if (File.Exists(LEVELS_SCORE_PATH + name + EXTENSION))
        {
            string data = File.ReadAllText(LEVELS_SCORE_PATH + name + EXTENSION);
            return data;
        }
        else
            return null;
    }

    /// <summary>
    /// checks for the existense of a level in the folder
    /// </summary>
    /// <param name="levelName">name of the level to try</param>
    /// <returns>Returns true if the level exists, false if not </returns>
    public static bool LevelExists(string levelName)
    {
        return File.Exists(LEVELS_PATH + levelName + EXTENSION);
    }

}

public class LevelData2Save
{
    public int Index;
    public int Score = 0;
    public string Name;
    public string Data;
}

public class ScoreData2Save
{
    public int Score = 0;
}
