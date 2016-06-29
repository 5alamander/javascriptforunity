using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using SharpKit.JavaScript;
using System.IO;

public static class JSAnalyzer
{
    /// <summary>
    /// Exams the component to see if there is something not supported.
    /// Currently check List only.
    /// </summary>
    /// <param name="com">The component.</param>
    /// <returns>An error list.</returns>
    static List<string> ExamMonoBehaviour(MonoBehaviour com)
    {
        List<string> lstProblem = new List<string>();
        //StringBuilder sbProblem = new StringBuilder();
        MonoBehaviour behaviour = com as MonoBehaviour;
        //Type type = behaviour.GetType();
        FieldInfo[] fields = JSSerializerEditor.GetMonoBehaviourSerializedFields(behaviour);
        for (var i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            Type fieldType = field.FieldType;
            if (fieldType.IsArray)
            {
                fieldType = fieldType.GetElementType();
            }
            // ! List is not supported
            // 
            if (fieldType.IsGenericType)
            {
                lstProblem.Add(new StringBuilder().AppendFormat("{0} {1}.{2} serialization not supported.", fieldType.Name, com.GetType().Name, field.Name).ToString());
                //continue;
            }

            // if this MonoBehaviour refer to another MonoBehaviour (A)
            // A must be export or compiled to JavaScript as well
            if (typeof(MonoBehaviour).IsAssignableFrom(fieldType))
            {
                if (!JSSerializerEditor.WillTypeBeAvailableInJavaScript(fieldType))
                {
                    lstProblem.Add(new StringBuilder().AppendFormat("{0} {1}.{2} not available in JavaScript.", fieldType.Name, com.GetType().Name, field.Name).ToString());
                }
            }

            //more to exam
        }
        return lstProblem;
    }

    public enum TraverseOp
    {
        Analyze
    }
    public static string GetTempFileNameFullPath(string shortPath)
    {
        Directory.CreateDirectory(Application.dataPath + "/Temp/");
        return Application.dataPath + "/Temp/" + shortPath;
    }

	public static string GetAllExportedMembersFile()
	{
		return GetTempFileNameFullPath("AllExportedMembers.txt");
	}

    /// <summary>
    /// Do some actions to GameObject hierachy.
    /// </summary>
    /// <param name="sbLog">The log.</param>
    /// <param name="go">The gameobject.</param>
    /// <param name="tab">The tab.</param>
    /// <param name="op">The operation.</param>
    public static void TraverseGameObject(StringBuilder sbLog, GameObject go, int tab, TraverseOp op)
    {
        for (var t = 0; t < tab; t++)
        {
            sbLog.Append("    ");
        }
        sbLog.AppendFormat("{0}", go.name);

        bool hasChecked = false;
        bool hasError = false;

        // action!
        switch (op)
        {
            case TraverseOp.Analyze:
                {
                    var coms = go.GetComponents(typeof(MonoBehaviour));

                    // Calculate MonoBehaviour's Count
                    // Only check scripts that has JsType attribute
                    Dictionary<Type, int> dictMono = new Dictionary<Type, int>();
                    for (var c = 0; c < coms.Length; c++)
                    {
                        MonoBehaviour mb = (MonoBehaviour)coms[c];
						if (mb == null)
						{
							CheckHasError = true;
							Debug.LogError("Null MonoBehaviour found, gameObject name: " + go.name);
							continue;
						}

                        if (JSSerializerEditor.WillTypeBeTranslatedToJavaScript(mb.GetType()))
                        {
                            if (!dictMono.ContainsKey(mb.GetType()))
                                dictMono.Add(mb.GetType(), 1);
                            else
                                dictMono[mb.GetType()]++;
                        }
                    }
                    foreach (var t in dictMono)
                    {
                        if (!hasChecked)
                        {
                            hasChecked = true;
                            sbLog.Append(" (CHECKED)");
                        }

                        if (t.Value > 1)
                        {
                            if (!hasError) { hasError = true;  sbLog.Append(" ERROR: "); }
							CheckHasError = true;
                            sbLog.AppendFormat("Same MonoBehaviour more than once. Name: {0}, Count: {1} ", t.Key.Name, t.Value);
                        }
                    }

                    for (var c = 0; c < coms.Length; c++)
                    {
                        MonoBehaviour mb = (MonoBehaviour)coms[c];
						if (mb == null)
						{
							continue;
						}
                        if (JSSerializerEditor.WillTypeBeTranslatedToJavaScript(mb.GetType()))
                        {
                            List<string> lstError = ExamMonoBehaviour(mb);
							if (lstError.Count > 0)
								CheckHasError = true;
                            for (var x = 0; x < lstError.Count; x++)
                            {
                                if (!hasError) { hasError = true; sbLog.Append(" ERROR: "); }
                                sbLog.Append(lstError[x] + " ");
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        sbLog.Append("\n");

        // traverse children
        var childCount = go.transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            TraverseGameObject(sbLog, child.gameObject, tab + 1, op);
        }
    }

    /// <summary>
    /// Find all types in whole application
    /// if type has JsType attribute, output a 'CS.require' line to require the file containing the type
    /// </summary>
    //[MenuItem("JSB/Generate SharpKit JsType file CS.require list", false, 53)]
	[MenuItem("JSB/Generate MonoBehaviour to JSComponent_XX", false, 53)]
    public static void OutputAllTypesWithJsTypeAttribute()
    {
        //var sb = new StringBuilder();
        var sb2 = new StringBuilder();

//        sb.Append(@"/* Generated by JSBinding Menu : JSB | Generate SharpKit JsType file CS.require list
//* see JSAnalyzer.cs / OutputAllTypesWithJsTypeAttribute() function
//* better not modify manually.
//*/
//
//");
		sb2.Append(@"/* Generated by JSBinding Menu : JSB | Generate MonoBehaviour to JSComponent_XX
* see JSAnalyzer.cs / OutputAllTypesWithJsTypeAttribute() function
* better not modify manually.
*/

");
        sb2.AppendLine("var MonoBehaviour2JSComponentName =").AppendLine("[");

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types = a.GetTypes();
            foreach (Type t in types)
            {
                if (JSSerializerEditor.WillTypeBeTranslatedToJavaScript(t))
                {
                    System.Object[] attrs = t.GetCustomAttributes(typeof(JsTypeAttribute), false);
                    JsTypeAttribute jsTypeAttr = (JsTypeAttribute)attrs[0];
                    if (jsTypeAttr.Filename != null)
                    {
                        //Debug.Log(jsTypeAttr.filename);

                        string mustBegin = "StreamingAssets/JavaScript/";
                        //string mustBegin = JSBindingSettings.sharpKitGenFileDir;
                        int index = 0;
                        if ((index = jsTypeAttr.Filename.IndexOf(mustBegin)) >= 0)
                        {
                            //sb.AppendFormat("CS.require(\"{0}\");\n", jsTypeAttr.Filename.Substring(index + mustBegin.Length));
                        }
                        else
                        {
                            Debug.LogError(JSNameMgr.GetTypeFullName(t) + " is ignored because JsType.filename doesn't contain \"" + mustBegin + "\"");
                        }
                    }

                    /////
                    if (t.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        string jsComponentName = JSComponentGenerator.GetJSComponentClassName(t);
                        sb2.AppendFormat("    \"{0}|{1}\",\n", JSNameMgr.GetTypeFullName(t, false), jsComponentName);
                    }
                }
            }
        }
        sb2.AppendLine("];");
        sb2.Append(@"

var GetMonoBehaviourJSComponentName = function (i)
{
    if (i < MonoBehaviour2JSComponentName.length)
    {
        return MonoBehaviour2JSComponentName[i];
    }
    return """"; // returning empty string when end
}
");

        //Debug.Log(sb);

        string path = JSBindingSettings.sharpkitGeneratedFiles;
		// 现在不需要这个
//        File.WriteAllText(path, sb.ToString());
//        Debug.Log("OK. File: " + path);
        // AssetDatabase.Refresh();

        path = JSBindingSettings.monoBehaviour2JSComponentName;
        File.WriteAllText(path, sb2.ToString());
        Debug.Log("OK. File: " + path);
    }

    /// <summary>
    /// Does file path end with underscore?
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns></returns>
    static bool FileNameBeginsWithUnderscore(string path)
    {
        string shortName = path.Substring(Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\')) + 1);
        return (shortName[0] == '_');
    }

	static bool CheckHasError = false;

    // delegate return true: not to replace
    public delegate bool DelFilterReplaceFile(string fullpath);

    public static IEnumerable<GameObject> SceneRoots()
    {
        var prop = new HierarchyProperty(HierarchyType.GameObjects);
        var expanded = new int[0];
        while (prop.Next(expanded))
        {
            yield return prop.pptrValue as GameObject;
        }
    }
    /// <summary>
    /// Gets all scene paths.
    /// path begins with 'Assets/'
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
    public static List<string> GetAllScenePaths(DelFilterReplaceFile filter)
    {
        var lst = new List<string>();

        string[] GUIDs = AssetDatabase.FindAssets("t:Scene");
        foreach (var guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (filter(path))
            {
                continue;
            }

            lst.Add(path);
        }
        return lst;
    }
    /// <summary>
    /// Gets all prefab paths.
    /// path begins with 'Assets/'
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
    public static List<string> GetAllPrefabPaths(DelFilterReplaceFile filter)
    {
        var lst = new List<string>();

        string[] GUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (filter(path))
            {
                continue;
            }

            lst.Add(path);
        }
        return lst;
    }
}
