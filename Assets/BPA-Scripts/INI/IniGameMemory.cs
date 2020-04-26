using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ackk.INI.Helpers;
using System.Text;
using System.IO;

public class IniGameMemory : MonoBehaviour 
{
    public static IniGameMemory instance;

    public string FileName = "SaveSlot0";
    const string extensionOfFile = ".ini";
    [Header("Data:")]
    public bool doSave;
    public bool doLoad;
    public string FileLocation;
    string  _FileLocation;
    List<System.Action> doActionsBeforeSave = new List<System.Action>();
    List<System.Action> doActionsOnLoad= new List<System.Action>();
    public void RegisterActionBeforeSave(System.Action newAction)
    {
        doActionsBeforeSave.Add(newAction);
    }
    public void RegisterActionOnLoad(System.Action newAction)
    {
        doActionsOnLoad.Add(newAction);
    }
    public void SetFileName(string newFileName)
    {
        FileName=newFileName;
    }
    public string GetFileName()
    {
        return FileName;
    }
    void Awake()
    {
        instance = this;
        _FileLocation = Application.persistentDataPath;
    }
    public void Update()
    {
        if(doSave)
        {
            doSave = false;
            SaveINIFile();
        }
        if (doLoad)
        {
            doLoad = false;
            LoadINIFile();
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------
    public void SaveINIFile ()
    {
        SaveINIFile(FileName);
    }
    public void SaveINIFile (string _FileName)
    {
        StreamWriter writer;
        FileInfo t = new FileInfo (_FileLocation + "\\" + _FileName + extensionOfFile);

        // Determine the full path of the file just created.
        DirectoryInfo di = t.Directory;

        // Figure out what other entries are in that directory.
       // FileSystemInfo[] fsi = di.GetFileSystemInfos ();

        Debug.Log ("The directory '{0}' contains the following files and directories:" + di.FullName);

        FileLocation = _FileLocation + "\\" + _FileName + extensionOfFile;

        if (!t.Exists) {
            writer = t.CreateText ();
        } else {
            t.Delete ();
            writer = t.CreateText ();
        }
        foreach(System.Action a in doActionsBeforeSave)
        {
            //do these events before you save!!
            //new data may need to be written out
            a();
        }
        writer.Write (MemoryToIniString());
        writer.Close ();
       
        Debug.Log ("File written.");
    }
    public void LoadINIFile ()
    {
        LoadINIFile (FileName);
    }
    public void LoadINIFile (string _FileName)
    {
        string tempIniDataAsString = "";
        FileInfo t = new FileInfo (_FileLocation + "\\" + _FileName + extensionOfFile);

        if (!t.Exists) {
            SaveINIFile (_FileName);
        } else {
            StreamReader r = File.OpenText (_FileLocation + "\\" + _FileName + extensionOfFile);
            string _info = r.ReadToEnd ();
            r.Close ();
            tempIniDataAsString = _info;
            //      Debug.Log("File Read");
        }
        MemoryFromIniString(tempIniDataAsString);
        foreach(System.Action a in doActionsOnLoad)
        {
            a();
        }
    }
    public string MemoryToIniString()
    {
        /*
         Example Data:
            [My GroupX]
                My Key=12
                Another Key=63
                Name = Brian
            [Other group]
                V=D
                M=Q
        */
        StringBuilder sb = new StringBuilder();
        foreach (GroupData g in groups)
        {
            if (g.SaveThisGroup == false)
                continue;
            sb.Append("[");
            sb.Append(g.name);
            sb.Append("]\n");
            foreach (KeyData k in g.keys)
            {
                sb.Append("\t");
                sb.Append(k.name);
                sb.Append("=");
                sb.Append(k.strData);
                sb.Append("\n");
            }
        }
        return sb.ToString();
    }
    public void MemoryFromIniString(string input)
    {
        //Parse INI file:
        //Load data from string.
        groups.Clear();
        //TODO: Parse ini
    /*    const int unknownState=-1;
        const int lookForGroupStart=0;
        const int lookForKeys=1;
        const int lookForValues=2;*/
        //int state=unknownState; 
        StringBuilder sb = new StringBuilder();
        const char endOfLine = '\n';
        const char OpenGroup = '[';
        const char CloseGroup = ']';
        const char assignValue = '=';
        //Note that the ini MUST end in a new line.
        input += "\n"; 
        for (int i = 0; i < input.Length; i++)
        {
            //hit equals sign, so assign value to the current key.
            char c = input[i];
            if (c == OpenGroup)
            {
             //   state = lookForGroupStart;
                sb.Length = 0;
                continue;
            }
            if (c == CloseGroup)
            {
                groups.Add(new GroupData(sb.ToString().Trim(), true));
              //  state = lookForKeys;
                sb.Length = 0;
                continue;
            }
            if (c == assignValue)
            {
             //   state = lookForValues;
                //add new key every  new line.
                if (groups.Count != -1 && sb.Length > 0)
                {
                    //  groups[groups.Count].keys.Add(new KeyData(sb.ToString().Trim()));
                    groups[groups.Count - 1].keys.Add(new KeyData(sb.ToString().Trim()));
                }
                sb.Length = 0;
                continue;
            }
            if (c == endOfLine)
            {
                //add new key every  new line.
                if (groups.Count - 1 != -1 && groups[groups.Count - 1].keys.Count - 1 != -1 && sb.Length > 0)
                {
                    groups[groups.Count - 1].keys[groups[groups.Count - 1].keys.Count - 1].SetKeyData(sb.ToString().Trim());
                }
                sb.Length = 0;
                continue;
            }
           
            sb.Append(c);
        }

    }
    //------------------------------------------------------------------------------------------------------------------------------------------
    public List<GroupData> groups = new List<GroupData>();
    //core writers.............................................................................
    void AddGroup(string group)
    {
        //unsafe to use outright. please first check to make sure the group does not exist before using
        groups.Add(new GroupData(group));
    }
    void AddKey(int groupIndex, string key, string data)
    {
        //unsafe to use outright. please first check to make sure the group does not exist before using
        groups[groupIndex].keys.Add(new KeyData(key,data));
    }
    void AddKey(int groupIndex, string key, int data)
    {
        //unsafe to use outright. please first check to make sure the group does not exist before using
        groups[groupIndex].keys.Add(new KeyData(key,data));
    }
    //index finders.......................................................................................
    public int GetGroupIndex(string group)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].name == group)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetKeyIndex(int groupIndex,string key)
    {
        for (int i = 0; i < groups[groupIndex].keys.Count; i++)
        {
            if (groups[groupIndex].keys[i].name == key)
            {
             //   print(key + " has index of " + i);
                return i;
            }
        }
        return -1;
    }
    //safe writers.....................................................................................
    public void WriteData(string group,string key,string data)
    {
        int groupIndex = AddGroupIfNotFound(group);
        int keyIndex = AddKeyIfNotFound(groupIndex, key, data);
        //override the data
        groups[groupIndex].keys[keyIndex].SetKeyData(data);
    }
   
    public void WriteData(string group,string key,int data)
    {
        int groupIndex = AddGroupIfNotFound(group);
        int keyIndex = AddKeyIfNotFound(groupIndex, key, data);
        //override the data
        groups[groupIndex].keys[keyIndex].SetKeyData(data);
    }
    /// <summary>
    /// Floats are written as strings.
    /// </summary>
    /// <param name="group">Group.</param>
    /// <param name="key">Key.</param>
    /// <param name="data">Data.</param>
    public void WriteData(string group,string key,float data)
    {
        WriteData(group, key, data.ToString());
    }
    public void ClearGroup(string groupNameToErase)
    {
        int removeGroupNumber = GetGroupIndex(groupNameToErase);
        if (removeGroupNumber != -1)
        {
            groups.Remove(groups[removeGroupNumber]);
        }
    }
    public void IncrementKeyValue(string group,string key,int incrementAmount,int byAnotherAmount, int defaultValue)
    {
        //use this to add 2 values together.
        int groupIndex = AddGroupIfNotFound(group);
        int keyIndex = AddKeyIfNotFound(groupIndex, key, defaultValue);
        int previousData = groups[groupIndex].keys[keyIndex].intData;
        groups[groupIndex].keys[keyIndex].SetKeyData(previousData + incrementAmount);
        groups[groupIndex].keys[keyIndex].SetKeyData(previousData + byAnotherAmount);
    }
    public void IncrementKeyValue(string group,string key,int incrementAmount, int defaultValue)
    {
        int groupIndex = AddGroupIfNotFound(group);
        int keyIndex = AddKeyIfNotFound(groupIndex, key, defaultValue);
        int previousData = groups[groupIndex].keys[keyIndex].intData;
        groups[groupIndex].keys[keyIndex].SetKeyData(previousData + incrementAmount);
    }
    public void MultiplyKeyValue(string group,string key,int multiplyBy, int defaultValue)
    {
        int groupIndex = AddGroupIfNotFound(group);
        int keyIndex = AddKeyIfNotFound(groupIndex, key, defaultValue);
        int previousData = groups[groupIndex].keys[keyIndex].intData;
        groups[groupIndex].keys[keyIndex].SetKeyData(previousData * multiplyBy);
    }
    /// <summary>
    /// Divides the key value. 0 wil automatically become a 1.
    /// </summary>
    /// <param name="group">Group.</param>
    /// <param name="key">Key.</param>
    /// <param name="divideBy">Divide by.</param>
    /// <param name="defaultValue">Default value.</param>
    public void DivideKeyValue(string group,string key,int divideBy, int defaultValue)
    {
        if (divideBy == 0)
            divideBy = 1;
        int groupIndex = AddGroupIfNotFound(group);
        int keyIndex = AddKeyIfNotFound(groupIndex, key, defaultValue);
        int previousData = groups[groupIndex].keys[keyIndex].intData;
        groups[groupIndex].keys[keyIndex].SetKeyData(previousData / divideBy);
    }
    //readers...........................................................................................
    public string GetDataString(string group, string key)
    {
        int groupIndex = GetGroupIndex(group);
        if (groupIndex == -1)
        {
            //group not found...
            return "-1";
        }
        int keyIndex = GetKeyIndex(groupIndex, key);
        if (keyIndex == -1)
        {
            //key not found...
            return "-1";
        }
        return groups[groupIndex].keys[keyIndex].strData;
    }
    public string GetDataString(string group, string key,string defaultValue)
    {
        int groupIndex = GetGroupIndex(group);
        if (groupIndex == -1)
        {
            //group not found...
            return defaultValue;
        }
        int keyIndex = GetKeyIndex(groupIndex, key);
        if (keyIndex == -1)
        {
            //key not found...
            return defaultValue;
        }
        return groups[groupIndex].keys[keyIndex].strData;
    }
    public int GetDataValue(string group, string key)
    {
        int groupIndex = GetGroupIndex(group);
        if (groupIndex == -1)
        {
            //group not found...
            return -1;
        }
        int keyIndex = GetKeyIndex(groupIndex, key);
        if (keyIndex == -1)
        {
            //key not found...
            return -1;
        }
        return groups[groupIndex].keys[keyIndex].intData;

    }
    public bool GetDataBool(string group, string key)
    {
     //   Debug.Log(group+","+key + GetDataValue(group, key));
        const int threshold = 1;
        int data = GetDataValue(group, key);
        if (data >= threshold) return true;
        return false;
    }
    public bool GetDataBool(string group, string key,int threshold)
    {
        int data = GetDataValue(group, key);
        if (data >= threshold) return true;
        return false;
    }
    public int GetDataValue(string group, string key,int defaultValue)
    {
        int groupIndex = GetGroupIndex(group);
        if (groupIndex == -1)
        {
            //group not found...
            return defaultValue;
        }
        int keyIndex = GetKeyIndex(groupIndex, key);
        if (keyIndex == -1)
        {
            //key not found...
            return defaultValue;
        }
        return groups[groupIndex].keys[keyIndex].intData;

    }
    public int[] GetGroupKeyAddress(string group,string key)
    {
        int g = AddGroupIfNotFound(group);
        int k =AddKeyIfNotFound(g,key,-1);
        int[] results = { g, k };
        return results;
    }
    public int GetDataValueAtAddress(int g, int k)
    {
        return GetDataValueAtAddress(g, k, -1);
    }
    public string GetDataStringAtAddress(int g, int k)
    {
        return GetDataStringAtAddress(g, k, "-1");
    }
    public string GetDataStringAtAddress(int[] arg)
    {
        return GetDataStringAtAddress(arg[0],arg[1], "-1");
    }
    public int GetDataValueAtAddress(int g, int k,int defaultValue)
    {
        if (groups.Count <= g)
            return defaultValue;
        if (groups[g].keys.Count <= k)
            return defaultValue;
        return groups[g].keys[k].intData;
    }
    public string GetDataStringAtAddress(int g, int k, string defaultValue)
    {
        if (groups.Count <= g)
            return defaultValue;
        if (groups[g].keys.Count <= k)
            return defaultValue;
        return groups[g].keys[k].strData;
    }
    /// <summary>
    /// Returns the string as a float
    /// </summary>
    /// <returns>The data float.</returns>
    /// <param name="group">Group.</param>
    /// <param name="key">Key.</param>
    /// <param name="defaultValue">Default value.</param>
    public float GetDataFloat(string group, string key,float defaultValue)
    {
        //because strings and ints are used most frequently, they get actual variable space in the KeyData class.
        //floats are simply written as strings, but this function will let you treat them like floats,
        //but it is more efficiant to use a float and only write to the key when finished.
        float resultValue = defaultValue;
        string data = GetDataString(group, key, defaultValue.ToString());
        float.TryParse(data,out resultValue);
        return resultValue;
    }

    //safely create a group or key.......................................................................
    public int AddGroupIfNotFound(string group)
    {
        int groupIndex = GetGroupIndex(group);
        if (groupIndex == -1)
        {
            AddGroup(group);
            groupIndex = groups.Count-1;

        }
        return groupIndex;
    }
    public int AddKeyIfNotFound(int groupIndex,string key, string defaultValue)
    {
        int keyIndex = GetKeyIndex(groupIndex,key);
        if (keyIndex == -1)
        {
            AddKey(groupIndex, key, defaultValue);
            keyIndex = groups[groupIndex].keys.Count-1;

        }
        return keyIndex;
    }
    public int AddKeyIfNotFound(int groupIndex,string key, int defaultValue)
    {
        int keyIndex = GetKeyIndex(groupIndex,key);
        if (keyIndex == -1)
        {
            AddKey(groupIndex, key, defaultValue);
            keyIndex = groups[groupIndex].keys.Count-1;
        }
      
        return keyIndex;
    }
    public string[] StringToIniKeyArray(string str,char Delimiter)
    {
        string[] splitStr = str.Split( Delimiter.ToString().ToCharArray());
        return splitStr;
    }
    public void AddFromArray(string groupName,string[] ary,char Delimiter)
    {
        foreach (string s in ary)
        {
            string[] splitStr = s.Split( Delimiter.ToString().ToCharArray());
            if (splitStr == null)
                return;
            if (splitStr.Length == 2)
            {
                WriteData(groupName, splitStr[0], splitStr[1]);
            }
            else
            {
                WriteData(groupName, s,-1);

            }
        }
    }
    //Options................................................................................
    public void SetGroupSavability(string groupName,bool canSave)
    {
        int gIndex = GetGroupIndex(groupName);
        groups[gIndex].SaveThisGroup = canSave;
    }
}
namespace Ackk.INI.Helpers
{
    [System.Serializable]
    public class GroupData
    {
        public string name;
        public GroupData(string str,bool enableSaving)
        {
            name = str;
            SaveThisGroup = enableSaving;
        }
        public GroupData(string str)
        {
            name = str;
            SaveThisGroup = true;
        }
        public GroupData()
        {
            name ="Default";
            SaveThisGroup = true;
        }
        public bool SaveThisGroup = true;
        public List<KeyData> keys= new List<KeyData>();
    }
    [System.Serializable]
    public class KeyData
    {
        public KeyData()
        {
            strData = "-1";
            intData = -1;
        }
        public KeyData(string keyName,string data)
        {
            name = keyName;
            SetKeyData(data);
        }
        public KeyData(string keyName)
        {
            name = keyName;
        }
        public KeyData(string keyName,int data)
        {
            name = keyName;
            SetKeyData(data);
        }
        public void SetKeyData(int data)
        {
            strData = data.ToString();
            intData = data;
        }
        public void SetKeyData(string data)
        {
            strData = data;
            int.TryParse(data, out intData);
        }
        public string name;
        public string strData;
        public int intData;
    }
    public static class INI
    {
        public static IniGameMemory Get()
        {
            return IniGameMemory.instance;
        }
        public static void Write(string groupName,string keyName,int value)
        {
            Get().WriteData(groupName, keyName, value);
        }
        public static void Write(string groupName,string keyName,string value)
        {
            Get().WriteData(groupName, keyName, value);
        }
        public static string workingGroupStr="Default";
        public static void SetWorkingGroup(string group)
        {
            workingGroupStr=group;
        }
        public static void Write(string keyName,string value)
        {
            Get().WriteData(workingGroupStr, keyName, value);
        }
        public static void Write(string keyName,int value)
        {
            Get().WriteData(workingGroupStr, keyName, value);
        }
        public static void AddArray(string[] ary,char Delimiter)
        {
            Get().AddFromArray(workingGroupStr, ary, Delimiter);
        }
        public static void AddArray(string[] ary)
        {
            Get().AddFromArray(workingGroupStr, ary, ':');
        }
        public static List<GroupData> GetGroupINIDataFromString(string source,string groupBrackets,char dataSplit,char dataEnd,bool enableSaving)
        {
            //Simple INI parser in ~58 lines :)
            List<GroupData> results = new List<GroupData>();
            StringBuilder sb= new StringBuilder();

            const int openBracket=0;
            const int closeBracket=1;
            const int equalsSign=2;
            const int endLine=3;
            const int key = 4;
            const int value = 5;

            int state = -1;
            int currentGroup = -1;

            if (dataSplit != '\n')
            {
                source.Replace('\n', ' ');
            }
            foreach (char c in source)
            {
                Debug.Log(c.ToString()+"state="+state);
                if (c == groupBrackets[0])
                {
                    state = openBracket;
                    currentGroup++;
                    //continue here so the bracket is not a part of the name
                    continue;
                }
                else if (c == groupBrackets[1])
                {
                    state = closeBracket;
                    //do not use continue here;
                }
                else if (c == dataSplit)
                {
                    state = equalsSign;
                    //do not use continue here;
                }
                else if (c == dataEnd)
                {
                    state = endLine;
                    //do not use continue here;
                }
                switch (state)
                {
                    case openBracket:
                        //begin building the group name
                        sb.Append(c);
                        break;
                    case closeBracket:
                        results.Add(new GroupData(sb.ToString().Trim(), enableSaving));
                        //clear string builder
                        sb.Length = 0;
                        state = key;
                        break;
                    case equalsSign:
                        results[currentGroup].keys.Add(new KeyData(sb.ToString().Trim(),"unknown"));
                        //clear string builder
                        sb.Length=0;
                        state = value;
                        break;
                    case endLine:
                        results[currentGroup].keys[results[currentGroup].keys.Count - 1].SetKeyData(sb.ToString().Trim());
                        //clear string builder
                        sb.Length = 0;
                        state = key;
                        break;
                    case key:
                        //build key name
                        sb.Append(c);
                        break;
                    case value:
                        //build key data
                        sb.Append(c);
                        break;
                }
            }
            return results;
        }
    
    }
  
}
