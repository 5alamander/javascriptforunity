using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;

/// <summary>
/// JSSerializer
/// Serialize variables to JavaScript
/// Support: Primitive Type, string, enum, [], etc.
/// List<> is not supported now.
/// </summary>
public class JSSerializer : MonoBehaviour
{
    public string jsClassName = string.Empty;
    public string[] arrString = null;
    public UnityEngine.Object[] arrObject = null;

    public enum UnitType
    {
        ST_Unknown = 0,

        ST_Boolean = 1,

        ST_Byte = 2,
        ST_SByte = 3,
        ST_Char = 4,
        ST_Int16 = 5,
        ST_UInt16 = 6,
        ST_Int32 = 7,
        ST_UInt32 = 8,
        ST_Int64 = 9,
        ST_UInt64 = 10,

        ST_Single = 11,
        ST_Double = 12,

        ST_String = 13,

        ST_Enum = 14,
        ST_UnityEngineObject = 15,
        ST_JavaScriptMonoBehaviour = 16,

        ST_MAX = 100,
    }
    /// <summary>
    /// Save a value in JavaScript and return ID
    /// </summary>
    /// <param name="eType">Data Type</param>
    /// <param name="strValue">Value of the variable in string format</param>
    /// <returns></returns>
    int toID(UnitType eType, string strValue)
    {
        switch ((UnitType)eType)
        {
            case UnitType.ST_Boolean:
                {
                    bool v = strValue == "True";
                    JSApi.setBooleanS((int)JSApi.SetType.SaveAndTempTrace, v);
                    return JSApi.getSaveID();
                }
                //break;

            case UnitType.ST_SByte:
            case UnitType.ST_Char:
            case UnitType.ST_Int16:
            case UnitType.ST_Int32:
                {
                    int v;
                    if (int.TryParse(strValue, out v))
                    {
                        JSApi.setInt32((int)JSApi.SetType.SaveAndTempTrace, v);
                        return JSApi.getSaveID();
                    }
                }
                break;

            case UnitType.ST_Byte:
            case UnitType.ST_UInt16:
            case UnitType.ST_UInt32:
            case UnitType.ST_Enum:
                {
                    uint v;
                    if (uint.TryParse(strValue, out v))
                    {
                        JSApi.setUInt32((int)JSApi.SetType.SaveAndTempTrace, v);
                        return JSApi.getSaveID();
                    }
                }
                break;
            case UnitType.ST_Int64:
            case UnitType.ST_UInt64:
            case UnitType.ST_Single:
            case UnitType.ST_Double:
                {
                    double v;
                    if (double.TryParse(strValue, out v))
                    {
                        JSApi.setDouble((int)JSApi.SetType.SaveAndTempTrace, v);
                        return JSApi.getSaveID();
                    }
                }
                break;
            case UnitType.ST_String:
                {
                    JSApi.setStringS((int)JSApi.SetType.SaveAndTempTrace, strValue);
                    return JSApi.getSaveID();
                }
                //break;
            default:
                break;
        }
        return 0;
    }
    /// <summary>
    /// Get JSComponent ID of a GameObject by scriptName.
    /// </summary>
    /// <param name="go">The gameobject.</param>
    /// <param name="scriptName">Name of the script.</param>
    /// <returns></returns>
    public int GetGameObjectMonoBehaviourJSObj(GameObject go, string scriptName, out JSComponent component)
    {
		component = null;

		// go may be null
		// because the serialized MonoBehaviour can be null
		if (go == null)
			return 0;

        JSComponent[] jsComs = go.GetComponents<JSComponent>();
        foreach (var com in jsComs)
        {
			// NOTE: if a script bind to a GameObject twice, it will always return the first one
            if (com.jsClassName == scriptName)
            {
				component = com;
                return com.GetJSObjID(false);
            }
        }
        return 0;
    }
    public class SerializeStruct
    {
        public enum SType { Root, Unit };
        public SType type;
        public string name;
        public string typeName;
        public int __id;
        public int id
        {
            get { return __id; }
            set
            {
//                 if (value != 0)
//                     JSApi.setTrace(value, true);
                __id = value;
            }
        }
        public SerializeStruct father;
        public List<SerializeStruct> lstChildren;
        public void AddChild(SerializeStruct ss)
        {
            if (lstChildren == null) 
                lstChildren = new List<SerializeStruct>();
            lstChildren.Add(ss);
        }
        public SerializeStruct(SType t, string name, SerializeStruct father)
        {
            type = t;
            this.name = name;
            this.father = father;
            typeName = "WRONGTYPENAME!";
            __id = 0;
        }
        public void removeID()
        {
            if (this.id != 0)
            {
                JSApi.removeByID(this.id);
                this.id = 0;
            }
            if (lstChildren != null)
            {
                foreach (var child in lstChildren)
                {
                    child.removeID();
                }
            }
        }
    }

    /// <summary>
    /// Traverses the serialization.
    /// </summary>
    /// <param name="jsObjID">The js object identifier.</param>
    /// <param name="st">The parent struct.</param>
    public void ParseSerializeData(int jsObjID, SerializeStruct st)
    {
        while (true)
        {
            string s = NextString();
            if (s == null)
                return;

            int x = s.IndexOf('/');
            int y = s.IndexOf('/', x + 1);
            string s0 = s.Substring(0, x);
            string s1 = s.Substring(x + 1, y - x - 1);

			UnitType eUnitType = (UnitType)int.Parse(s0);
			if (eUnitType == UnitType.ST_UnityEngineObject)
			{
				string s2 = s.Substring(y + 1, s.Length - y - 1);
				var valName = s1;
				var objIndex = int.Parse(s2);
				JSMgr.datax.setObject((int)JSApi.SetType.SaveAndTempTrace, this.arrObject[objIndex]);
				
				var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
				child.id = JSApi.getSaveID();
				st.AddChild(child);
			}
			else if (eUnitType == UnitType.ST_JavaScriptMonoBehaviour)
			{
				var valName = s1;
				string s2 = s.Substring(y + 1, s.Length - y - 1);
				var arr = s2.Split('/');
				var objIndex = int.Parse(arr[0]);
				var scriptName = arr[1];
				
				var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
				JSComponent component;
				int refJSObjID = this.GetGameObjectMonoBehaviourJSObj((GameObject)this.arrObject[objIndex], scriptName, out component);
				if (refJSObjID == 0)
				{
					child.id = 0;
				}
				else
				{
					if (waitSerialize == null)
						waitSerialize = new List<JSComponent>();
					waitSerialize.Add(component);
					
					JSApi.setObject((int)JSApi.SetType.SaveAndTempTrace, refJSObjID);
					child.id = JSApi.getSaveID();
				}
				
				st.AddChild(child);
			}
			else
			{
				string s2 = s.Substring(y + 1, s.Length - y - 1);
				var valName = s1;
				int id = toID(eUnitType, s2);
				var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
				//child.val = JSMgr.vCall.valTemp;
				child.id = id;
				st.AddChild(child);
			}
		}
	}
	int arrStringIndex = 0;
	string NextString()
	{
		if (arrString == null) return null;
		if (arrStringIndex >= 0 && arrStringIndex < arrString.Length)
		{
			return arrString[arrStringIndex++];
		}
		return null;
	}
	
	bool dataSerialized = false;
	protected bool DataSerialized { get { return dataSerialized; } }
    protected List<JSComponent> waitSerialize = null;
    /// <summary>
    /// Initializes the serialized data.
    /// </summary>
    /// <param name="jsObjID">The js object identifier.</param>
    public virtual void initSerializedData(int jsObjID)
    {
        if (dataSerialized)
            return;
        
        dataSerialized = true;
        
        if (arrString == null || arrString.Length == 0)
        {
            return;
        }
        
        var root = new SerializeStruct(SerializeStruct.SType.Root, "this-name-doesn't-matter", null);
        ParseSerializeData(jsObjID, root);
        if (root.lstChildren != null)
        {
            foreach (var child in root.lstChildren)
            {
				SetObjectFieldOrProperty(jsObjID, child.name, child.id);
            }
        }
        root.removeID();
    }

	static void SetObjectFieldOrProperty(int jsObjID, string name, int valueID)
	{
		// Field
		JSApi.setProperty(jsObjID, name, valueID);
	}
}