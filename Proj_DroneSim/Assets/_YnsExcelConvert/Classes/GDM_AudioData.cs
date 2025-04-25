using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GDM_AudioData : ScriptableObject
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
		
		public string Id;
		public string Name;
		public int Prio;
		public string FilePath_JP;
		public string FilePath_EN;
		public float Volume;
		public string Comment;
	}
}

