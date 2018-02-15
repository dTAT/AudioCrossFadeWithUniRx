using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateScriptableObject : MonoBehaviour {

	[MenuItem ("Assets/Create/MusicParamObject")]
	public static void CreateAsset () {
		CreateAsset<MusicParamObject> ();
	}

	public static void CreateAsset<Type> () where Type : ScriptableObject {
		Type item = ScriptableObject.CreateInstance<Type> ();

		string path = AssetDatabase.GenerateUniqueAssetPath ("Assets/" + typeof (Type) + ".asset");

		AssetDatabase.CreateAsset (item, path);
		AssetDatabase.SaveAssets ();

		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = item;
	}

}