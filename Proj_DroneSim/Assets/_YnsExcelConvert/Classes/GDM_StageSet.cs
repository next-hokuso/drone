using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GDM_StageSet : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public string X0a;
		public string X0b;
		public string X1a;
		public string X1b;
		public string X2a;
		public string X2b;
		public string X3a;
		public string X3b;
		public string X4a;
		public string X4b;
		public string X5a;
		public string X5b;
		public string X6a;
		public string X6b;
		public string X7a;
		public string X7b;
		public string X8a;
		public string X8b;
		public string X9a;
		public string X9b;
		public string X10a;
		public string X10b;
		public string X11a;
		public string X11b;
		public string X12a;
		public string X12b;
		public string X13a;
		public string X13b;
		public string X14a;
		public string X14b;
		public string X15a;
		public string X15b;
		public string X16a;
		public string X16b;
		public string X17a;
		public string X17b;
		public string X18a;
		public string X18b;
		public string Comment2;
	}
}

