using UnityEngine;
using System.Collections;
using Ackk.INI.Helpers;
public class MemTest : MonoBehaviour {
    string g="TestGroup";

    string k="Modify This key";
    string k2="Testkey2";
    string k3="Testkey3";
    string floatKey="floatKey";
    public float myFloatValue=3.005f;
	// Use this for initialization
	void Start () 
    {
        if (IniGameMemory.instance == null)
            return;
        //Declare a "variable" on the fly....
        if (IniGameMemory.instance.GetDataValue(g, k) == -1)
        {
            //only declare if not found to have it be modable in editor.
            IniGameMemory.instance.WriteData(g, k, "5");
        }

        print(IniGameMemory.instance.GetDataValue(g,k)+" + 2 =");
        //add to the value of hte variable...
        IniGameMemory.instance.IncrementKeyValue(g, k, 2, 0);
        //print the result... should be 7...
        print(IniGameMemory.instance.GetDataValue(g,k));
        //write any number of variables inside of the same group.
        IniGameMemory.instance.WriteData(g,k2,2);
        print("value of "+k2+" is "+IniGameMemory.instance.GetDataString(g,k2));
        //int variable can be overriden with a string.
        IniGameMemory.instance.WriteData(g,k2, "bob");
        print("now, value of "+k2+" is "+IniGameMemory.instance.GetDataString(g,k2));

      
        //try printing a variable that does not exist...
        //it can be given a default value if it doesn't find it in the list...
        print(IniGameMemory.instance.GetDataValue(g,k3,32));
        //...write to that variable now...
        IniGameMemory.instance.WriteData(g,k3,69);
        //if you write to that variable later on, it will print the new value ignoring the default value!
        print(IniGameMemory.instance.GetDataValue(g,k3,32));
        //names can also have a default value:
        const string anotherGroup = "AnotherGroup";
        const string nameKey = "Name";
        string resultingName = IniGameMemory.instance.GetDataString(anotherGroup, nameKey, "Brian");
        print("Hello, my name is "+resultingName);
        //a static class is included in Ackk.INI.Helpers.INI in order to make the instance easier to access...
        INI.Get().WriteData(g, floatKey, myFloatValue);
        print(INI.Get().GetDataFloat(g,floatKey,0f));
        //...........................................
        string RPG="RPG.Hero.Stats.Loto";
        //use shortcut features....
        INI.Write(RPG, "Name","Loto");
        INI.Write(RPG,"MaxHp",10);
        INI.Write(RPG,"MaxMp",6);
        INI.Write(RPG,"Atk",8);
        INI.Write(RPG,"Def",3);
        INI.Write(RPG,"Luk",4);
        INI.Write(RPG,"Spd",5);
        RPG="RPG.Hero.Stats.Wanda";
        INI.Write(RPG, "Name","Wanda");
        INI.Write(RPG,"MaxHp",9);
        INI.Write(RPG,"MaxMp",10);
        INI.Write(RPG,"Atk",6);
        INI.Write(RPG,"Def",8);
        INI.Write(RPG,"Luk",2);
        INI.Write(RPG,"Spd",4);
        //or if writing the group is tedious:
        RPG="RPG.Hero.Stats.Brian";
        INI.SetWorkingGroup(RPG);
        INI.Write("Name","Brian");
        INI.Write("MaxHp",10);
        INI.Write("MaxMp",6);
        INI.Write("Atk",8);
        INI.Write("Def",3);
        INI.Write("Luk",4);
        INI.Write("Spd",5);
        //or you can just add to the working group via an array:
        RPG="RPG.Hero.Stats.Anon";
        INI.SetWorkingGroup(RPG);
        string[] anonStats={
            "Name:Anon",
            "MaxHp:10",
            "MaxMp:6",
            "Atk:75",
            "Def:18",
            "Luk:40",
            "Spd:2"
        };
        INI.AddArray(anonStats);
        //disable saving of any group (marks it so it doesn't write to the file)
        //this means you can have temp variables in a group!
        INI.Get().SetGroupSavability(INI.workingGroupStr,false);
        //if you dont want to use an array, use the string to array converter:
        RPG="RPG.Hero.Stats.Master";
        INI.SetWorkingGroup(RPG);
        string defineMasterStats = "Name:Master,MaxHp:10,MaxMp:6,Atk:99999,Def:90,Spd:76";

        //don't forget to use chars for splitters and not strings!
        INI.AddArray(INI.Get().StringToIniKeyArray(defineMasterStats,','),':');


        //cache the address of a group/key //prints using  INI.Get().GetDataStringAtAddress(x,y) format (returns string use GetDataValueAtAddress() for int...
        int[] anonNameAddress=INI.Get().GetGroupKeyAddress("RPG.Hero.Stats.Anon","Name");
        print("The name at address" + anonNameAddress[0] + "," + anonNameAddress[1] + " is:" + INI.Get().GetDataStringAtAddress(anonNameAddress[0],anonNameAddress[1]));
        print("Or insert the array as a single arg to get the same data:" + INI.Get().GetDataStringAtAddress(anonNameAddress));

        //note that caching can be dangerous for groups that are subject to sorting/deletion...
        print("if the address is not found it returns a value of:" + INI.Get().GetDataStringAtAddress(100,100));


       
	}
	
	
}
