using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Ackk.GameEngine.Helpers
{
    public static class Colorizer
    {
        public static Color RgbToFloat(int r,int g, int b)
        {
            return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
        }
    }
	public static class GameObjectOperations  
	{
		public static void EnableDisableGameObjects(List<GameObject> objectList, bool enable)
		{
			EnableDisableGameObjects (objectList.ToArray (), enable);
		}
		public static void EnableDisableGameObjects(GameObject[] objectAry, bool enable)
		{
			if (objectAry == null)
				return;
			for (int i=0; i<objectAry.Length; i++) {
				if(objectAry[i]!=null)	objectAry[i].SetActive(enable);
			}
		}
		/// <summary>
		/// Enables the disable object name pair. If the name doesn't match, then the object is set to the opposite of the enable flag
		/// </summary>
		/// <param name="objectAry">Object ary.</param>
		/// <param name="namedObject">Named object.</param>
		/// <param name="enable">If set to <c>true</c> enable.</param>
		public static void EnableDisableObjectNamePair(ObjectNamePair[] objectAry,string namedObject, bool enable)
		{
			if (objectAry == null)
				return;
			for (int i=0; i<objectAry.Length; i++) 
			{
				if(objectAry[i]!=null)
				{
					if(objectAry[i].Name.ToLower()==namedObject.ToLower())
					{
						objectAry[i].SetActive(enable);
					}
					else
					{
						objectAry[i].SetActive(!enable);
						
					}
				}
			}
		}
		public static void EnableDisableObjectNamePair(ObjectNamePair[] objectAry,int placeInList, bool enable)
		{
			if (objectAry == null)
				return;
			for (int i=0; i<objectAry.Length; i++) 
			{
				if(objectAry[i]!=null)
				{
					if(i==placeInList)
					{
						objectAry[i].SetActive(enable);
					}
					else
					{
						objectAry[i].SetActive(!enable);
						
					}
				}
			}
		}
	}


	[System.Serializable]
	public class ObjectNamePair
	{
		public string Name;
		public GameObject[] gObject;
		public void SetActive(bool enable)
		{
			GameObjectOperations.EnableDisableGameObjects (gObject, enable);
		}
	
	}
  

}