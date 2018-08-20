using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Linq;

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
    /*void Update()
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

            Save(table, numParams, "Save_Data");
        }
    }*/

    public void Save(List<List<double>> saveTable, int numParameters, string fileName) // Any script can call this and save something to excel
    {
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < numParameters; index++)
            sb.AppendLine(string.Join(delimiter, saveTable[index]));


        string filePath = getPath(fileName);
        while (File.Exists(filePath)) //If file exists, change the number
        {
            saveNum++;
            filePath = getPath(fileName);
        }
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();

    }

    public string[][] Load(string filePath)
    {
        string[][] loadedData = File.ReadLines(filePath).Select(x => x.Split(',')).ToArray();     
        return loadedData;
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath(string name)
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + name +"_" + saveNum + ".csv";
#elif UNITY_ANDROID
            return Application.persistentDataPath+ name +"_" + saveNum + ".csv";
#elif UNITY_IPHONE
            return Application.persistentDataPath+"/"+ name +"_" + saveNum + ".csv";
#else
            return Application.dataPath +"/CSV/"+ name +"_" + saveNum + ".csv";
#endif
    }

}
