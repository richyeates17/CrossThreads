using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveData
{
    //Variables to Save
    public string currentScene;
    public float[] position;
    public string currentDateString;

    //examples
    public int level;
    public int health;

    //Constructor
    public SaveData(Player player)
    {
        currentScene = GameManager.instance.currentScene;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1]= player.transform.position.y;
        position[2] = player.transform.position.z;

        currentDateString = DateTime.Now.ToString();
        Debug.Log(currentDateString);

        level = 3;
        health = 30;
    }
}
