using System.Collections.Generic;
using UnityEngine;

public class SystemInfoManager : MonoBehaviour
{
    private static SystemInfoManager instance;

    // Your dictionary to store system information
    private Dictionary<string, Tuple<int, int>> systemInfoDict = new Dictionary<string, Tuple<int, int>>();

    private int BondIndexGrabbed = -1;
    // Public property to access the instance from other scripts
    public static SystemInfoManager Instance
    {
        get
        {
            if (instance == null)
            {
                // If the instance doesn't exist, find or create it
                instance = FindObjectOfType<SystemInfoManager>();

                if (instance == null)
                {
                    // If still null, create a new GameObject and attach the script
                    GameObject singletonObject = new GameObject("SystemInfoManager");
                    instance = singletonObject.AddComponent<SystemInfoManager>();
                }
            }

            return instance;
        }
    }

    // Method to add or update information in the dictionary
    public void SetInfo(string key, Tuple<int, int> value)
    {
        if (systemInfoDict.ContainsKey(key))
        {
            systemInfoDict[key] = value;
        }
        else
        {
            systemInfoDict.Add(key, value);
        }
    }

    // Method to get information from the dictionary
    public Tuple<int, int> GetInfo(string key)
    {
        if (systemInfoDict.ContainsKey(key))
        {
            return systemInfoDict[key];
        }

        // Return null or handle the case where the key doesn't exist
        return null;
    }

    public void SetGrabbedBond(int value)
    {
        BondIndexGrabbed = value;
    }

    
    public int GetGrabbedBond()
    {
        return BondIndexGrabbed;
    }
}