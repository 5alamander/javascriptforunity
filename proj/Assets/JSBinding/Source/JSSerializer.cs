using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;

public class JSSerializer : MonoBehaviour
{
    /*
     * AutoDelete: if true  will be automatically deleted when needed (when press Alt + Shift + Q)
     * DON'T change this manually
     */
    public string jsScriptName = string.Empty;
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
        ST_MonoBehaviour = 16,

        ST_MAX = 100,
    }
    public enum AnalyzeType
    {
        Unit,

        ArrayBegin,
        ArrayObj,
        ArrayEnd,

        StructBegin,
        StructObj,
        StructEnd,

        ListBegin,
        ListObj,
        ListEnd,
    }
    // TODO �޸�ע��
    // and check
    /// <summary>
    /// ���� eType �� strValue ת��Ϊ jsval
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="strValue"></param>
    /// <returns></returns>
    int ToHeapValID(UnitType eType, string strValue)
    {
        switch ((UnitType)eType)
        {
            case UnitType.ST_Boolean:
                {
                    bool v = strValue == "True";
                    JSApi.setBoolean(JSApi.SetType.TempVal, v);
                    return JSApi.moveVal2HeapMap();
                }
                break;

            case UnitType.ST_SByte:
            case UnitType.ST_Char:
            case UnitType.ST_Int16:
            case UnitType.ST_Int32:
                {
                    int v;
                    if (int.TryParse(strValue, out v))
                    {
                        JSApi.setInt32(JSApi.SetType.TempVal, v);
                        return JSApi.moveVal2HeapMap();
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
                        JSApi.setUInt32(JSApi.SetType.TempVal, v);
                        return JSApi.moveVal2HeapMap();
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
                        JSApi.setDouble(JSApi.SetType.TempVal, v);
                        return JSApi.moveVal2HeapMap();
                    }
                }
                break;
            case UnitType.ST_String:
                {
                    // TODO check
                    // JSMgr.vCall.datax.setString(JSDataExchangeMgr.eSetType.Jsval, strValue);
                    JSApi.setString(JSApi.SetType.TempVal, strValue);
                    return JSApi.moveVal2HeapMap();
                }
                break;
            default:
                break;
        }
        return 0;
    }
    public int GetGameObjectMonoBehaviourJSObj(GameObject go, string scriptName)
    {
        JSComponent[] jsComs = go.GetComponents<JSComponent>();
        foreach (var com in jsComs)
        {
            // ע�⣺ͬһ��GameObject���ܰ���ͬ���ֵĽű�2������
            if (com.jsScriptName == scriptName)
            {
                return com.GetJSObjID();
            }
        }
        return 0;
    }
    public class SerializeStruct
    {
        public enum SType { Root, Array, Struct, List, Unit };
        public SType type;
        public string name;
        public string typeName;
        public JSApi.jsval val; // only valid when type == SType.Unit
        public int iHeapVal;
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
            val.asBits = 0;
            typeName = "WRONGTYPENAME!";
            iHeapVal = 0;
        }
        /// <summary>
        /// Calc jsval
        /// save in this.val    and return it
        /// </summary>
        /// <returns></returns>
        public int CalcHeapValID()
        {
            switch (this.type)
            {
                case SType.Unit:
                    // �� TraverseSerialize() ��ʱ����Ѿ��������
                    break;
                case SType.Array:
                    {
                        int Count = lstChildren.Count;
                        for (var i = 0; i < Count; i++)
                        {
                            int iHeapVal = lstChildren[i].CalcHeapValID();
                            JSApi.moveValFromMap2Arr(iHeapVal, i);
                        }
                        JSApi.setArray(JSApi.SetType.TempVal, Count);
                        this.iHeapVal = JSApi.moveVal2HeapMap();
                    }
                    break;
                case SType.Struct:
                    {
                        /*
                         * ����Ĺ��̱Ƚϸ��ӣ�����ͬʱ֧���� CS ������ JS �����
                         * �� Vector3 Ϊ�������ȵ��� UnityEngine.Vector3.ctor ������������C#���࣬����ʵ�ʻ��ߵ�C#ȥ���ɶ���
                         * ������� SetUCProperty ʱ��Ҳ�ᴥ�� JS �� property��ʵ��Ҳ�ǵ��õ�C#ȥ��
                         * 
                         * ������� C# ���࣬��ô�½�������SetUCProperty�������ߵ�C#������JS�����
                         */
                        //JSApi.jsval valParam = new JSApi.jsval(); valParam.asBits = 0;
                        //JSApi.JSh_SetJsvalString(JSMgr.cx, ref valParam, this.typeName);
                        //JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.glob, "jsb_CallObjectCtor", 1, new JSApi.jsval[]{valParam}, ref JSMgr.vCall.rvalCallJS);
                        //IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref JSMgr.vCall.rvalCallJS);
                        IntPtr jsObj = JSMgr.vCall.CallJSClassCtorByName(this.typeName);
                        if (jsObj == IntPtr.Zero)
                        {
                            Debug.LogError("Serialize error: call \"" + this.typeName + "\".ctor return null, , did you forget to export that class?");
                            JSApi.JSh_SetJsvalUndefined(ref this.val);
                        }
                        else
                        {
                            //IntPtr jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null /*JSMgr.mjsFinalizer*/);
                            for (var i = 0; i < lstChildren.Count; i++)
                            {
                                var child = lstChildren[i];
                                JSApi.jsval mVal = child.CalcHeapValID();
                                JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, child.name, -1, ref mVal);
                            }
                            JSApi.JSh_SetJsvalObject(ref this.val, jsObj);
                        }
                        
                        /*
                        IntPtr jstypeObj = JSDataExchangeMgr.GetJSObjectByname(this.typeName);
                        if (jstypeObj == IntPtr.Zero)
                        {
                            Debug.LogError("JSSerialize fail. New object \"" + this.typeName + "\" fail, did you forget to export that class?");
                            this.val.asBits = 0;
                        }
                        else
                        {
                            JSApi.jsval valFun; valFun.asBits = 0;
                            JSApi.GetProperty(JSMgr.cx, jstypeObj, "ctor", -1, ref valFun);
                            if (valFun.asBits == 0 || JSApi.JSh_JsvalIsNullOrUndefined(ref valFun))
                            {
                                Debug.LogError("Serialize error: " + this.typeName + ".ctor is not a function");
                                JSApi.JSh_SetJsvalUndefined(ref this.val);
                            }
                            else
                            {
                                JSMgr.vCall.CallJSFunctionValue(jstypeObj, ref valFun);
                                IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref JSMgr.vCall.rvalCallJS);
                                if (jsObj == IntPtr.Zero)
                                {
                                    Debug.LogError("Serialize error: call " + this.typeName + ".ctor return null");
                                    JSApi.JSh_SetJsvalUndefined(ref this.val);
                                }
                                else
                                {
                                    //IntPtr jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null);// JSMgr.mjsFinalizer);
                                    for (var i = 0; i < lstChildren.Count; i++)
                                    {
                                        var child = lstChildren[i];
                                        JSApi.jsval mVal = child.CalcJSVal();
                                        JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, child.name, -1, ref mVal);
                                    }
                                    JSApi.JSh_SetJsvalObject(ref this.val, jsObj);
                                }
                            }
                        }*/
                    }
                    break;
                case SType.List:
                    {
                        // ����Ҫ����� List �� C# �Ļ��� JS �ģ�
                        // ����� C# ��Ҫ ʹ�� �ȴ���һ�� List ���� Ȼ��JSDataExchangeMgr.setObject 
                        // ����� JS �� ��Ƚϼ�һ��  �ο������
                    }
                    break;
            }
            return this.iHeapVal;
        }
    }
    /// <summary>
    /// ���� arrString �𼶴������л�����
    /// index: arrString ����
    /// st: ��ǰ�����
    /// </summary>
    /// <param name="cx"></param>
    /// <param name="jsObj"></param>
    /// <param name="index"></param>
    /// <param name="st"></param>
    /// <returns></returns>
    public void TraverseSerialize(IntPtr cx, IntPtr jsObj, SerializeStruct st)
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
            switch (s0)
            {
                case "ArrayBegin":
                    {
                        SerializeStruct.SType sType = SerializeStruct.SType.Array;
                        var ss = new SerializeStruct(sType, s1, st);
                        st.AddChild(ss);
                        TraverseSerialize(cx, jsObj, ss);
                    }
                    break;
                // ��2������������
                case "StructBegin":
                case "ListBegin":
                    {
                        SerializeStruct.SType sType = SerializeStruct.SType.Array;
                        if (s0 == "StructBegin") sType = SerializeStruct.SType.Struct;
                        else if (s0 == "ListBegin") sType = SerializeStruct.SType.List;
                        string s2 = s.Substring(y + 1, s.Length - y - 1);

                        var ss = new SerializeStruct(sType, s1, st);
                        ss.typeName = s2;
                        st.AddChild(ss);
                        TraverseSerialize(cx, jsObj, ss);
                    }
                    break;
                case "ArrayEnd":
                case "StructEnd":
                case "ListEnd":
                    {
                        // ! return here
                        return;
                    }
                    break;
                default:
                    {
                        UnitType eUnitType = (UnitType)int.Parse(s0);
                        if (eUnitType == UnitType.ST_UnityEngineObject)
                        {
                            string s2 = s.Substring(y + 1, s.Length - y - 1);
                            var valName = s1;
                            var objIndex = int.Parse(s2);
                            JSMgr.vCall.datax.setObject(JSApi.SetType.TempVal, this.arrObject[objIndex]);

                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            child.iHeapVal = JSApi.moveVal2HeapMap();
                            st.AddChild(child);
                        }
                        else if (eUnitType == UnitType.ST_MonoBehaviour)
                        {
                            var valName = s1;
                            string s2 = s.Substring(y + 1, s.Length - y - 1);
                            var arr = s2.Split('/');
                            var objIndex = int.Parse(arr[0]);
                            var scriptName = arr[1];

                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            int refJSObjID = this.GetGameObjectMonoBehaviourJSObj((GameObject)this.arrObject[objIndex], scriptName);
                            if (refJSObjID == 0)
                            {
                                child.iHeapVal = 0;
                            }
                            else
                            {
                                JSApi.setObject(JSApi.SetType.TempVal, refJSObjID);
                                child.iHeapVal = JSApi.moveVal2HeapMap();
                            }

                            st.AddChild(child);
                        }
                        else
                        {
                            string s2 = s.Substring(y + 1, s.Length - y - 1);
                            var valName = s1;
                            int iHeapVal = ToHeapValID(eUnitType, s2);
                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            //child.val = JSMgr.vCall.valTemp;
                            child.iHeapVal = iHeapVal;
                            st.AddChild(child);
                        }
                    }
                    break;
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
    /// <summary>
    /// �ڽű��� Awake ʱ����������������ʼ�����л����ݸ�JS��
    /// </summary>
    /// <param name="cx"></param>
    /// <param name="jsObj"></param>
    public void initSerializedData(IntPtr cx, IntPtr jsObj)
    {
        if (arrString == null || arrString.Length == 0)
        {
            return;
        }

        var root = new SerializeStruct(SerializeStruct.SType.Root, "this-name-doesn't-matter", null);
        TraverseSerialize(cx, jsObj, root);
        if (root.lstChildren != null)
        {
            foreach (var child in root.lstChildren)
            {
                child.CalcHeapValID();
                JSApi.JSh_SetUCProperty(cx, jsObj, child.name, -1, ref child.val);
            }
        }
    }
}