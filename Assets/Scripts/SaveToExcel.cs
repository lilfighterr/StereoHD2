using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class SaveToExcel : MonoBehaviour
{
    public static SaveToExcel instance;
    List<List<double>> table = new List<List<double>>();

    public List<double> characterPositionX;
    public List<double> characterPositionY;
    public List<double> carPositionX;
    public List<double> carPositionY;
    public List<double> timeElapsed;
    public List<double> lap;
    public List<double> parameters; //force feedback on/off

    private bool saved = false;
    private int numParams = 7;
    private int saveNum = 0;

    // Use this for initialization
    void Awake()
    { //Always called before start() functions
      //Makes sure that there is only one instance of Matlab Server (singleton)
        if (instance == null) //If no game control found
        {
            instance = this; //Then this is the instance of the game control
        }
        else if (instance != this) //If the game object finds that instance is already on another game object, then this destroys itself as it's not needed
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.instance.gameOver && !saved)
        {
            saved = true;

            //Combine rows to make a table
            table.Add(characterPositionX);
            table.Add(characterPositionY);
            table.Add(carPositionX);
            table.Add(carPositionY);
            table.Add(timeElapsed);
            table.Add(lap);
            table.Add(parameters);

            Save();
        }
    }

    private void Save()
    {
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < numParams; index++)
            sb.AppendLine(string.Join(delimiter, table[index]));


        string filePath = getPath();
        while (File.Exists(filePath)) //If file exists, change the number
        {
            saveNum++;
            filePath = getPath();
        }
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();

    }

    // Following method is used to retrive the relative path as device platform
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "Saved_data_" + saveNum + ".csv";
#elif UNITY_ANDROID
            return Application.persistentDataPath+"Saved_data_"+saveNum+".csv";
#elif UNITY_IPHONE
            return Application.persistentDataPath+"/"+"Saved_data_"+saveNum+".csv";
#else
            return Application.dataPath +"/"+"Saved_data_"+saveNum+".csv";
#endif
    }

}
