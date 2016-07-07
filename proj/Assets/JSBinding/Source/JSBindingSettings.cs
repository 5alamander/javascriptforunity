using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using SharpKit.JavaScript;

public class JSBindingSettings
{
	public static string jsExtension = ".javascript";
	public static string jsDir = Application.streamingAssetsPath + "/JavaScript";
	public static string jsRelDir = "Assets/StreamingAssets/JavaScript";
	
	public static string jsGenFiles { get { return jsDir + "/Lib/Gen" + jsExtension; } }
	public static string csGenDir = Application.dataPath + "/JSBinding/Generated";

    public static Type[] enums = new Type[]
    {
		typeof(KeyCode),
    };

	public static Type[] classes = new Type[]
	{
		typeof(UnityEngine.EventSystems.IEventSystemHandler),
		typeof(UnityEngine.EventSystems.IPointerEnterHandler),
		typeof(UnityEngine.EventSystems.IPointerExitHandler),
		typeof(UnityEngine.EventSystems.IPointerDownHandler),
		typeof(UnityEngine.EventSystems.IPointerUpHandler),
		typeof(UnityEngine.EventSystems.IPointerClickHandler),
		typeof(UnityEngine.EventSystems.IBeginDragHandler),
		typeof(UnityEngine.EventSystems.IInitializePotentialDragHandler),
		typeof(UnityEngine.EventSystems.IDragHandler),
		typeof(UnityEngine.EventSystems.IEndDragHandler),
		typeof(UnityEngine.EventSystems.IDropHandler),
		typeof(UnityEngine.EventSystems.IScrollHandler),
		typeof(UnityEngine.EventSystems.IUpdateSelectedHandler),
		typeof(UnityEngine.EventSystems.ISelectHandler),
		typeof(UnityEngine.EventSystems.IDeselectHandler),
		typeof(UnityEngine.EventSystems.IMoveHandler),
		typeof(UnityEngine.EventSystems.ISubmitHandler),
		typeof(UnityEngine.EventSystems.ICancelHandler),
		typeof(UnityEngine.UI.ICanvasElement),
		typeof(UnityEngine.UI.IMask),
		typeof(UnityEngine.UI.IMaskable),
		typeof(UnityEngine.UI.ILayoutElement),
		typeof(UnityEngine.UI.ILayoutController),
		typeof(UnityEngine.UI.ILayoutGroup),
		typeof(UnityEngine.UI.ILayoutSelfController),
		typeof(UnityEngine.UI.ILayoutIgnorer),
		typeof(UnityEngine.UI.IMaterialModifier),
		typeof(UnityEngine.UI.IVertexModifier),
		typeof(UnityEngine.ICanvasRaycastFilter),
        
        typeof(UnityEngine.Animation),
        typeof(UnityEngine.GameObject),
		typeof(UnityEngine.Animator),
        typeof(UnityEngine.RuntimeAnimatorController),
        typeof(UnityEngine.AnimatorOverrideController),
        typeof(UnityEngine.YieldInstruction),
		typeof(UnityEngine.WaitForSeconds),
		
		typeof(UnityEngine.Resources),
		
		typeof(UnityEngine.Application),
		typeof(UnityEngine.Behaviour),
		typeof(UnityEngine.MonoBehaviour),
		typeof(UnityEngine.Debug),
		
		typeof(UnityEngine.Input),
		typeof(UnityEngine.Object),
		typeof(UnityEngine.Component),
		typeof(UnityEngine.Transform),
		
		typeof(UnityEngine.Time),
		typeof(UnityEngine.PlayerPrefs),
		
		typeof(UnityEngine.Vector2),
		typeof(UnityEngine.Vector3),
		typeof(UnityEngine.Color),
		typeof(UnityEngine.Color32),
        typeof(UnityEngine.Events.UnityEventBase),
        typeof(UnityEngine.Events.UnityEvent),

		typeof(UnityEngine.UI.Graphic),
		typeof(UnityEngine.EventSystems.UIBehaviour),
		typeof(UnityEngine.UI.Selectable),
        typeof(UnityEngine.UI.MaskableGraphic),

        typeof(UnityEngine.UI.Button),
        typeof(UnityEngine.UI.Button.ButtonClickedEvent),
		typeof(UnityEngine.UI.Image),
		typeof(UnityEngine.UI.Text),
		typeof(UnityEngine.RectTransform),
		typeof(UnityEngine.Sprite),
	};

    // some public class members can be used
    // some of them are only in editor mode
    // some are because of unknown reason
    //
    // they can't be distinguished by code, but can be known by checking unity docs
    public static bool IsDiscard(Type type, MemberInfo memberInfo)
    {
        string memberName = memberInfo.Name;

        if (typeof(Delegate).IsAssignableFrom(type)/* && (memberInfo is MethodInfo || memberInfo is PropertyInfo || memberInfo is FieldInfo)*/)
        {
            return true;
        }

        if (memberName == "networkView" && (type == typeof(GameObject) || typeof(Component).IsAssignableFrom(type)))
        {
            return true;
        }

        if ((type == typeof(Application) && memberName == "ExternalEval") ||
                        (type == typeof(Input) && memberName == "IsJoystickPreconfigured"))
        {
            return true;
        }
            
        //
        // Temporarily commented out
        // Uncomment them yourself!!
        //
        if ((type == typeof(Motion)) ||
            (type == typeof(AnimationClip) && memberInfo.DeclaringType == typeof(Motion)) ||
            (type == typeof(Application) && memberName == "ExternalEval") ||
            (type == typeof(Input) && memberName == "IsJoystickPreconfigured") ||
            (type == typeof(AnimatorOverrideController) && memberName == "PerformOverrideClipListCleanup") ||
            (type == typeof(Caching) && (memberName == "ResetNoBackupFlag" || memberName == "SetNoBackupFlag")) ||
            (type == typeof(Light) && (memberName == "areaSize")) ||
            (type == typeof(Security) && memberName == "GetChainOfTrustValue") ||
            (type == typeof(Texture2D) && memberName == "alphaIsTransparency") ||
            (type == typeof(WebCamTexture) && (memberName == "isReadable" || memberName == "MarkNonReadable")) ||
            (type == typeof(StreamReader) && (memberName == "CreateObjRef" || memberName == "GetLifetimeService" || memberName == "InitializeLifetimeService")) ||
            (type == typeof(StreamWriter) && (memberName == "CreateObjRef" || memberName == "GetLifetimeService" || memberName == "InitializeLifetimeService")) ||
            (type == typeof(UnityEngine.Font) && memberName == "textureRebuildCallback")
#if UNITY_4_6 || UNITY_4_7
             || (type == typeof(UnityEngine.EventSystems.PointerEventData) && memberName == "lastPress")
             || (type == typeof(UnityEngine.UI.InputField) && memberName == "onValidateInput") // property delegate
		    || (type == typeof(UnityEngine.UI.Graphic) && memberName == "OnRebuildRequested")
		    || (type == typeof(UnityEngine.UI.Text) && memberName == "OnRebuildRequested")
#endif
)
        {
            return true;
        }

#if UNITY_ANDROID || UNITY_IPHONE
        if (type == typeof(WWW) && (memberName == "movie"))
            return true;
#endif
        return false;
	}
	
	public static bool IsSupportByDotNet2SubSet(string functionName)
	{
		if (functionName == "Directory_CreateDirectory__String__DirectorySecurity" ||
		    functionName == "Directory_GetAccessControl__String__AccessControlSections" ||
		    functionName == "Directory_GetAccessControl__String" ||
		    functionName == "Directory_SetAccessControl__String__DirectorySecurity" ||
		    functionName == "DirectoryInfo_Create__DirectorySecurity" ||
		    functionName == "DirectoryInfo_CreateSubdirectory__String__DirectorySecurity" ||
		    functionName == "DirectoryInfo_GetAccessControl__AccessControlSections" ||
		    functionName == "DirectoryInfo_GetAccessControl" ||
		    functionName == "DirectoryInfo_SetAccessControl__DirectorySecurity" ||
		    functionName == "File_Create__String__Int32__FileOptions__FileSecurity" ||
		    functionName == "File_Create__String__Int32__FileOptions" ||
		    functionName == "File_GetAccessControl__String__AccessControlSections" ||
		    functionName == "File_GetAccessControl__String" ||
		    functionName == "File_SetAccessControl__String__FileSecurity")
		{
			return false;
		}
		return true;
	}

    public static bool NeedGenDefaultConstructor(Type type)
    {
        if (typeof(Delegate).IsAssignableFrom(type))
            return false;

        if (type.IsInterface)
            return false;

        // don't add default constructor
        // if it has non-public constructors
        // (also check parameter count is 0?)
        if (type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Length != 0)
            return false;

        //foreach (var c in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance))
        //{
        //    if (c.GetParameters().Length == 0)
        //        return false;
        //}

        if (type.IsClass && (type.IsAbstract || type.IsInterface))
            return false;

        if (type.IsClass)
        {
            return type.GetConstructors().Length == 0;
        }
        else
        {
            foreach (var c in type.GetConstructors())
            {
                if (c.GetParameters().Length == 0)
                    return false;
            }
            return true;
        }
    }

	public static bool CheckClassBindings(Type[] types)
	{
		Dictionary<Type, bool> clrLibrary = new Dictionary<Type, bool>();
		{
			//
			// these types are defined in clrlibrary.javascript
			//
			clrLibrary.Add(typeof(System.Object), true);
			clrLibrary.Add(typeof(System.Exception), true);
			clrLibrary.Add(typeof(System.SystemException), true);
			clrLibrary.Add(typeof(System.ValueType), true);
		}
		
		Dictionary<Type, bool> dict = new Dictionary<Type, bool>();
		var sb = new StringBuilder();
		bool ret = true;
		
		// can not export a type twice
		foreach (var type in types)
		{
			if (typeof(System.Delegate).IsAssignableFrom(type))
			{
				sb.AppendFormat("\"{0}\" Delegate can not be exported.\n",
				                JSNameMgr.GetTypeFullName(type));
				ret = false;
			}
			
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				sb.AppendFormat(
					"\"{0}\" is not allowed. Try \"{1}\".\n",
					JSNameMgr.GetTypeFullName(type), JSNameMgr.GetTypeFullName(type.GetGenericTypeDefinition()));
				ret = false;
			}
			
			if (dict.ContainsKey(type))
			{
				sb.AppendFormat(
					"Operation fail. There are more than 1 \"{0}\" in JSBindingSettings.classes, please check.\n",
					JSNameMgr.GetTypeFullName(type));
				ret = false;
			}
			else
			{
				dict.Add(type, true);
			}
		}
		
		// Is BaseType exported?
		foreach (var typeb in dict)
		{
			Type type = typeb.Key;
			Type baseType = type.BaseType;
			if (baseType == null) { continue;  }
			if (baseType.IsGenericType) baseType = baseType.GetGenericTypeDefinition();
			// System.Object is already defined in SharpKit clrlibrary
			if (!clrLibrary.ContainsKey(baseType) && !dict.ContainsKey(baseType))
			{
				sb.AppendFormat("\"{0}\"\'s base type \"{1}\" must also be in JSBindingSettings.classes.\n",
				                JSNameMgr.GetTypeFullName(type),
				                JSNameMgr.GetTypeFullName(baseType));
				ret = false;
			}

			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				Type ti = interfaces[i];
				
				string tiFullName = JSNameMgr.GetTypeFullName(ti);
				
				// some intefaces's name has <>, skip them
				if (!tiFullName.Contains("<") && tiFullName.Contains(">") && 
				    !clrLibrary.ContainsKey(ti) && !dict.ContainsKey(ti))
				{
					sb.AppendFormat("\"{0}\"\'s interface \"{1}\" must also be in JSBindingSettings.classes.\n",
                                    JSNameMgr.GetTypeFullName(type),
                                    JSNameMgr.GetTypeFullName(ti));
                    ret = false;
                }
            }
        }
        if (!ret)
        {
            Debug.LogError(sb);
        }
        return ret;
    }
}
