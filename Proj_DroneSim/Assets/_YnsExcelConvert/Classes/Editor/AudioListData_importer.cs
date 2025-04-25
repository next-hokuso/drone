using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class AudioListData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/_GameData/AudioListData.xls";
	private static readonly string exportPath = "Assets/Resources/_GameData/AudioListData.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			GDM_AudioData data = (GDM_AudioData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(GDM_AudioData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<GDM_AudioData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					GDM_AudioData.Sheet s = new GDM_AudioData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						GDM_AudioData.Param p = new GDM_AudioData.Param ();
						
					cell = row.GetCell(0); p.Id = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Prio = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.FilePath_JP = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.FilePath_EN = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.Volume = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Comment = (cell == null ? "" : cell.StringCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
