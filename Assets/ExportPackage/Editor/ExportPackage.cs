using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Linq;

namespace Hyper {

	[InitializeOnLoad]
	public class ExportPackage : ScriptableObject {
		new public string name = "";
		public ExportPackageOptions options = ExportPackageOptions.Recurse;
		public string[] files;
		public string[] deleteBeforeExport;
		public bool deleteWithoutAsking = false;

		public const string ASSET_PATH = "Assets/ExportPackage/PackageSettings.asset";
		private static ExportPackage _instance;
		public static ExportPackage instance {
			get {
				if(_instance!=null) return _instance;
				if(!File.Exists(ASSET_PATH)) {
					_instance = ScriptableObject.CreateInstance<Hyper.ExportPackage>();
					AssetDatabase.CreateAsset(_instance, ASSET_PATH);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				} else {
					_instance = AssetDatabase.LoadAssetAtPath(ASSET_PATH,typeof(Hyper.ExportPackage)) as Hyper.ExportPackage;
				}
				return _instance;
			}
		}


		void OnEnable() {
			if(name=="") name = Application.dataPath.Replace(Path.DirectorySeparatorChar+"Assets","").Split(Path.DirectorySeparatorChar).Last();
			if (!File.Exists (ExportPackage.ASSET_PATH)) {
				ExportPackage i = ExportPackage.instance;
				Debug.Log("Created settings file "+i.ToString());
				AssetDatabase.Refresh ();
			}

		}

		[MenuItem("Hyper/ExportPackage/Settings %#&p",false,2)]
		public static void Settings ()
		{
			Selection.activeObject = ExportPackage.instance ;
		}

		[MenuItem("Hyper/ExportPackage/Export %&p",false,1)]
		public static void Export ()
		{
			Delete ();
			ExportPackage settings = ExportPackage.instance;

			AssetDatabase.ExportPackage (settings.files, settings.name+".unityPackage", settings.options);
			Debug.Log ("Exported package to " + settings.name + ".unityPackage");
		}

		[MenuItem("Hyper/ExportPackage/Delete files")]
		public static void Delete() {
			ExportPackage settings = ExportPackage.instance;

			if (settings.deleteBeforeExport.Length > 0) {
				foreach(string file in settings.deleteBeforeExport) {
					if(Directory.Exists(file)) {
						if(!settings.deleteWithoutAsking) {
							if(EditorUtility.DisplayDialog("Deleting directory", "Are you sure you want to delete '"+file+"' and all of it's contets?", "Delete it", "NO!")) {
								Directory.Delete(file,true);
							}
						} else Directory.Delete(file,true);
					}

					if(File.Exists(file)) {
						if(!settings.deleteWithoutAsking) {
							if(EditorUtility.DisplayDialog("Deleting file", "Are you sure you want to delete '"+file+"'?", "Delete it", "NO!")) {
								File.Delete(file);
							}
						} else File.Delete(file);
					}
				}
			}

			AssetDatabase.Refresh ();
		}
	}
}