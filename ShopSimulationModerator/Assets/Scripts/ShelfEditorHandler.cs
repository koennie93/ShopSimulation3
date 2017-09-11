using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShelfEditorHandler : MonoBehaviour {

    List<Dropdown> dropdowns = new List<Dropdown>();
    List<InputField> inputFields = new List<InputField>();
    List<string> groceryChoices;

    [HideInInspector]
    public List<string> shelfselection;
    [HideInInspector]
    public List<string> priceselection;

    [SerializeField]
    private Dropdown loadSelector;

    public void Awake () {
        loadSelector.captionText.text = "Load";
        List<GameObject> dropdownObjects = new List<GameObject>();
        List<GameObject> inputFieldObjects = new List<GameObject>();

        foreach (Transform child in GameObject.Find("ShelfChoices").transform)
        {
            dropdownObjects.Add(child.gameObject);
        }

        foreach (Transform child in GameObject.Find("PriceInputFields").transform)
        {
            inputFieldObjects.Add(child.gameObject);
        }

        for (int i = 0; i < dropdownObjects.Count; i++)
        {
            // Add all dropdowns in scene to a list for later use.
            dropdowns.Add(dropdownObjects[i].GetComponent<Dropdown>());
        }

        for (int i = 0; i < inputFieldObjects.Count; i++)
        {
            // Add all input fields in scene to a list for later use.
            inputFields.Add(inputFieldObjects[i].GetComponent<InputField>());
        }

#if UNITY_EDITOR
        string path = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data\\Groceries";
#else
        string path = Directory.GetParent(Application.dataPath).FullName + "\\Shared Data\\Groceries";
#endif
        string[] info = Directory.GetFiles(path);
        groceryChoices = new List<string>();
        groceryChoices.Add("Empty");
        for (int i = 0; i < info.Length; i++)
        {
            if(!groceryChoices.Contains(info[i].Split('\\')[info[i].Split('\\').Length - 1].Split('.')[0]))
            groceryChoices.Add(info[i].Split('\\')[info[i].Split('\\').Length - 1].Split('.')[0]);
        }
        for (int i = 0; i < dropdowns.Count; i++)
        {
            // Assign the options for all the shelves.
            dropdowns[i].options.Clear();
            for (int j = 0; j < groceryChoices.Count; j++)
            {
                dropdowns[i].options.Add(new Dropdown.OptionData() { text = groceryChoices[j] });
            }
            dropdowns[i].value = 0;
            dropdowns[i].transform.Find("Label").GetComponent<Text>().text = "Empty";
        }

#if UNITY_EDITOR
        string loadPath = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data\\Data\\ShelfData";
#else
        string loadPath = Directory.GetParent(Application.dataPath).FullName + "\\Shared Data\\Data\\ShelfData";
#endif
        string[] loadInfo = Directory.GetFiles(loadPath);
        List<string> loadChoices = new List<string>();
        for (int i = 0; i < loadInfo.Length; i++)
        {
            if (!loadChoices.Contains(loadInfo[i].Split('\\')[loadInfo[i].Split('\\').Length - 1].Split('.')[0]))
                loadChoices.Add(loadInfo[i].Split('\\')[loadInfo[i].Split('\\').Length - 1].Split('.')[0]);
        }
        loadSelector.options.Clear();
        for (int j = 0; j < loadChoices.Count; j++)
        {
            if(loadChoices[j].Contains("Items"))
            loadSelector.options.Add(new Dropdown.OptionData() { text = loadChoices[j].Split("shelfItems".ToCharArray())[0] });
        }

        LoadSavedSelection(true);
    }

    public void UpdateShelfSelection ()
    {
        // Update the list
        shelfselection = new List<string>();
        priceselection = new List<string>();
        for (int i = 0; i < dropdowns.Count; i++)
        {
            shelfselection.Add(groceryChoices[dropdowns[i].value]);
        }
        for (int i = 0; i < inputFields.Count; i++)
        {
            if (inputFields[i].text != "") priceselection.Add(inputFields[i].text);
            else priceselection.Add("0");
        }

#if UNITY_EDITOR
        File.WriteAllLines(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data" + "\\shelfselection.txt", shelfselection.ToArray());
        File.WriteAllLines(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data" + "\\priceselection.txt", priceselection.ToArray());
#else
        File.WriteAllLines(Directory.GetParent(Application.dataPath).FullName + "\\Shared Data" + "\\shelfselection.txt", shelfselection.ToArray());
        File.WriteAllLines(Directory.GetParent(Application.dataPath).FullName + "\\Shared Data" + "\\priceselection.txt", priceselection.ToArray());
#endif
    }

    public void LoadSavedSelection (bool awake)
    {
        string[] shelf;
        string[] prices;

        if (awake)
        {
#if UNITY_EDITOR
            shelf = File.ReadAllLines(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data\\shelfselection.txt");
            prices = File.ReadAllLines(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data\\priceselection.txt");
#else
        shelf = File.ReadAllLines(Directory.GetParent(Application.dataPath).FullName + "\\Shared Data\\shelfselection.txt");
        prices = File.ReadAllLines(Directory.GetParent(Application.dataPath).FullName + "\\Shared Data\\priceselection.txt");
#endif
        }
        else
        {
#if UNITY_EDITOR
            shelf = File.ReadAllLines(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data\\Data\\ShelfData\\" + loadSelector.options[loadSelector.value].text + "shelfItems.txt");
            prices = File.ReadAllLines(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName + "\\Shared Data\\Data\\ShelfData\\" + loadSelector.options[loadSelector.value].text + "shelfprices.txt");
#else
        shelf = File.ReadAllLines(Directory.GetParent(Application.dataPath).FullName + "\\Shared Data\\Data\\ShelfData\\" + loadSelector.options[loadSelector.value].text + "shelfItems.txt");
        prices = File.ReadAllLines(Directory.GetParent(Application.dataPath).FullName + "\\Shared Data\\Data\\ShelfData\\" + loadSelector.options[loadSelector.value].text + "shelfprices.txt");
#endif
        }

        for (int i = 0; i < dropdowns.Count; i++)
        {
            dropdowns[i].value = groceryChoices.IndexOf(shelf[i]);
        }
        for (int i = 0; i < dropdowns.Count; i++)
        {
            inputFields[i].text = prices[i];
        }
    }

    public void EmptyAll()
    {
        // Empty all selections
        for (int i = 0; i < dropdowns.Count; i++)
        {
            dropdowns[i].value = 0;
            dropdowns[i].transform.Find("Label").GetComponent<Text>().text = "Empty";
        }
    }

}
