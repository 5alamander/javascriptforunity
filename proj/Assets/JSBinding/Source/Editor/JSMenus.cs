using UnityEngine;
using UnityEditor;
using System.Collections;

public static class JSMenus
{
	[MenuItem("Assets/JavaScript/Gen Bindings", false, 1)]
	public static void GenBindings()
	{
		if (EditorApplication.isCompiling)
		{
			EditorUtility.DisplayDialog("Tip:", "please wait EditorApplication compiling", "OK");
			return; 
		}
				
		if (!JSBindingSettings.CheckClassBindings(JSBindingSettings.classes))
		{
			return;
		}

//		if (!EditorUtility.DisplayDialog("Tip", 
//		                                 "Files in these directories will all be deleted and re-created: \n" + 
//                                         JSBindingSettings.csGenDir + "\n",
//                                         "OK", "Cancel")
//		    )
//		{
//			return;
//		}

		CSGenerator.Log = JSGenerator.Log = Debug.Log;
		CSGenerator.LogError = JSGenerator.LogError = Debug.LogError;
		JSGenerator.Application_dataPath = Application.dataPath;

		JSDataExchangeEditor.reset();
		UnityEngineManual.InitManual();
		CSGenerator.GenBindings();
		JSGenerator.GenBindings();
		UnityEngineManual.AfterUse();

		AssetDatabase.Refresh();
	}
}
