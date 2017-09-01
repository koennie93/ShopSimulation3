    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.IO;

public class DataHandler : MonoBehaviour {

    public float[,] matrix = new float[4,5];
    List<Item> items = new List<Item>();
    List<string> matrixList = new List<string>();
    JsonData itemJson;
    JsonData matrixJson;

    public void UpdateMatrix(int x, int y, int amount)
    {
        matrix[x -1 , y -1] += amount;
    }

    public void UpdateItems(string itemName, int amount)
    {
        bool isInList = false;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == itemName) //Check if item is already in the list.
            {
                items[i].number += amount; //if its in the list, update quantity.
                isInList = true;
            }
        }

        if(isInList == false)
        {
            items.Add(new Item(itemName, amount));
        }
    }

    public void ExportData()
    {
        itemJson = JsonMapper.ToJson(items);
        File.WriteAllText(Application.dataPath + "/Data/Items.json", itemJson.ToString());

        System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(Application.dataPath + "/Resources/Data/MatrixData/" + System.DateTime.Now.ToString("MM-dd-yy_hh-mm-ss") + "-Matrix.txt");
        string output = "";
        for (int y = 0; y < matrix.GetLength(1); y++)
        {
            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                output += matrix[x, y].ToString();
                if(x != 3)
                output += ",";
                
            }
            streamWriter.WriteLine(output);
            matrixList.Add(output);
            output = "";
        }
        streamWriter.Close();

        itemJson = JsonMapper.ToJson(items);
        itemJson += JsonMapper.ToJson(matrixList);
        File.WriteAllText(Application.dataPath + "/Data/ItemsTest.json", itemJson.ToString());
    }
}

public class Item
{
    public string name;
    public int number;

    public Item(string newName, int amount)
    {
        name = newName;
        number = amount;
    }
}
