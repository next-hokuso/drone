using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StageSetList_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/_GameData/StageSetList.xls";
	private static readonly string exportPath = "Assets/Resources/_GameData/StageSetList.asset";
	private static readonly string[] sheetNames = { "Stage1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			GDM_StageSet data = (GDM_StageSet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(GDM_StageSet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<GDM_StageSet> ();
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

					GDM_StageSet.Sheet s = new GDM_StageSet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						GDM_StageSet.Param p = new GDM_StageSet.Param ();
						
					cell = row.GetCell(1); p.X0a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.X0b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.X1a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.X1b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.X2a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.X2b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.X3a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(8); p.X3b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(9); p.X4a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(10); p.X4b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(11); p.X5a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(12); p.X5b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(13); p.X6a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(14); p.X6b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(15); p.X7a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(16); p.X7b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(17); p.X8a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(18); p.X8b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(19); p.X9a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(20); p.X9b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(21); p.X10a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(22); p.X10b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(23); p.X11a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(24); p.X11b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(25); p.X12a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(26); p.X12b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(27); p.X13a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(28); p.X13b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(29); p.X14a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(30); p.X14b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(31); p.X15a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(32); p.X15b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(33); p.X16a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(34); p.X16b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(35); p.X17a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(36); p.X17b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(37); p.X18a = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(38); p.X18b = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(39); p.Comment2 = (cell == null ? "" : cell.StringCellValue);
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
