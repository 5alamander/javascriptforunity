using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

using jsval = JSApi.jsval;


public class JSDataExchangeEditor : JSDataExchangeMgr
{
    JSDataExchangeEditor(JSVCall vc):base(vc) {  }

    static Dictionary<Type, JSDataExchange> dict;
    static JSDataExchange enumExchange;
    static JSDataExchange objExchange;
    static JSDataExchange t_Exchange;
    static JSDataExchange_Arr arrayExchange;

    // Editor only
    public static void reset()
    {
        dict = new Dictionary<Type, JSDataExchange>();

        dict.Add(typeof(Boolean), new JSDataExchange_Boolean());
        dict.Add(typeof(Byte), new JSDataExchange_Byte());
        dict.Add(typeof(SByte), new JSDataExchange_SByte());
        dict.Add(typeof(Char), new JSDataExchange_Char());
        dict.Add(typeof(Int16), new JSDataExchange_Int16());
        dict.Add(typeof(UInt16), new JSDataExchange_UInt16());
        dict.Add(typeof(Int32), new JSDataExchange_Int32());
        dict.Add(typeof(UInt32), new JSDataExchange_UInt32());
        dict.Add(typeof(Int64), new JSDataExchange_Int64());
        dict.Add(typeof(UInt64), new JSDataExchange_UInt64());
        dict.Add(typeof(Single), new JSDataExchange_Single());
        dict.Add(typeof(Double), new JSDataExchange_Double());
        dict.Add(typeof(IntPtr), new JSDataExchange_IntPtr());

        dict.Add(typeof(String), new JSDataExchange_String());
        dict.Add(typeof(System.Object), new JSDataExchange_SystemObject());

        enumExchange = new JSDataExchange_Enum();
        objExchange = new JSDataExchange_Obj();
        t_Exchange = new JSDataExchange_T();
        arrayExchange = new JSDataExchange_Arr();
    }

    // Editor only
    public struct ParamHandler
    {
        public string argName; // argN
        public string getter;
        public string updater;
    }
    // Editor only
    public static ParamHandler Get_TType(int index)
    {
        ParamHandler ph = new ParamHandler();
        ph.argName = "t" + index.ToString();

        string get_getParam = dict[typeof(string)].Get_GetParam(null);
        ph.getter = "System.Type " + ph.argName + " = JSDataExchangeMgr.GetTypeByName(" + get_getParam + ");";

        //         string get_getParam = objExchange.Get_GetParam(typeof(Type));
        //         ph.getter = "System.Type " + ph.argName + " = (System.Type)" + get_getParam + ";";
        return ph;
    }
    public static bool IsDelegateSelf(Type type)
    {
        return type == typeof(System.Delegate) || type == typeof(System.MulticastDelegate);
    }
    public static bool IsDelegateDerived(Type type)
    {
        return typeof(System.Delegate).IsAssignableFrom(type) && !IsDelegateSelf(type);
    }
//    public string RecursivelyGetParam(Type type)
//    {
//        if (type.IsByRef)
//        {
//            return RecursivelyGetParam(type.GetElementType());
//        }
//        if (!type.IsArray)
//        {
//
//        }
//    }
    // Editor only
    public static ParamHandler Get_ParamHandler(Type type, int paramIndex, bool isRef, bool isOut)
    {
        ParamHandler ph = new ParamHandler();
        ph.argName = "arg" + paramIndex.ToString();

        if (IsDelegateDerived(type))
        {
            Debug.LogError("Delegate derived class should not get here");
            return ph;
        }

        string typeFullName;
        if (type.IsGenericParameter || type.ContainsGenericParameters)
            typeFullName = "object";
        else
            typeFullName = JSNameMgr.GetTypeFullName(type);

        if (type.IsArray)
        {
            ph.getter = new StringBuilder()
                .AppendFormat("{0} {1} = {2};", typeFullName, ph.argName, arrayExchange.Get_GetParam(type))
                .ToString();
        }
        else
        {
            if (isRef || isOut)
            {
                type = type.GetElementType();
            }
            string keyword = GetMetatypeKeyword(type);
            if (keyword == string.Empty)
            {
                Debug.LogError("keyword is empty: " + type.Name);
                return ph;
            }

            if (isOut)
            {
                ph.getter = new StringBuilder()
                    .AppendFormat("int r_arg{0} = vc.currIndex++;\n", paramIndex)
                    .AppendFormat("{0} {1};", typeFullName, ph.argName)
                    .ToString();
            }
            else if (isRef)
            {
                ph.getter = new StringBuilder()
                    .AppendFormat("int r_arg{0} = vc.currIndex;\n", paramIndex)
                    .AppendFormat("{0} {1} = ({0})vc.datax.get{2}(JSDataExchangeMgr.eGetType.GetARGVRefOut);", typeFullName, ph.argName, keyword)
                    .ToString();
            }
            else
            {
                ph.getter = new StringBuilder()
                    .AppendFormat("{0} {1} = ({0})vc.datax.get{2}(JSDataExchangeMgr.eGetType.GetARGV);", typeFullName, ph.argName, keyword)
                    .ToString();
            }

            if (isOut)
            {
                ph.updater = new StringBuilder()
                    .AppendFormat("vc.currIndex = r_arg{0};\n", paramIndex)
                    .AppendFormat("vc.datax.set{0}(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, {1});", keyword, ph.argName)
                    .ToString();
            }
        }
        return ph;
    }
    // TODO: delete
    public static ParamHandler Get_ParamHandler2(Type type, int paramIndex, bool isRef, bool isOut)
    {
        ParamHandler ph = new ParamHandler();
        ph.argName = "arg" + paramIndex.ToString();

        if (IsDelegateDerived(type))
        {
            Debug.LogError("Delegate derived class should not get here");
            return ph;
        }

        if (isRef || isOut)
        {
            type = type.GetElementType();
        }

        JSDataExchange xcg = null;
        if (type.IsGenericParameter)
        {
            xcg = t_Exchange;
        }
        if (xcg == null)
        {
            dict.TryGetValue(type, out xcg);
        }

        if (xcg == null)
        {
            if (type.IsPrimitive)
            {
                Debug.LogError("Unknown Primitive Type: " + type.ToString());
                return ph;
            }
            if (type.IsEnum)
            {
                xcg = enumExchange;
            }
            else if (type.IsArray)
                xcg = arrayExchange;
            else
            {
                xcg = objExchange;
            }
        }

        string typeFullName;
        if (type.IsGenericParameter || type.ContainsGenericParameters)
            typeFullName = "object";
        else
            typeFullName = JSNameMgr.GetTypeFullName(type);

        string get_getParam = string.Empty;
        if (isOut)
        {
            // don't need to get param but simply add index
            // get_getParam = xcg.Get_GetRefOutParam(type);
            ph.getter = "int r_arg" + paramIndex.ToString() + " = vc.currIndex++;\n";
        }
        else if (isRef)
        {
            get_getParam = xcg.Get_GetRefOutParam(type);
            ph.getter = "int r_arg" + paramIndex.ToString() + " = vc.currIndex;\n";
        }
        else
        {
            get_getParam = xcg.Get_GetParam(type);
            ph.getter = string.Empty;
        }
        if (isOut)
        {
            ph.getter += typeFullName + " " + ph.argName + ";";
        }
        else if (xcg.isGetParamNeedCast)
        {
            ph.getter += typeFullName + " " + ph.argName + " = (" + typeFullName + ")" + get_getParam + ";";
        }
        else
        {
            ph.getter += typeFullName + " " + ph.argName + " = " + get_getParam + ";";
        }

        if (isOut)
        {
            ph.updater = "vc.currIndex = r_arg" + paramIndex.ToString() + ";\n";
            ph.updater += xcg.Get_ReturnRefOut(ph.argName) + ";";
        }
        return ph;
    }

    // Editor only
    public static ParamHandler Get_ParamHandler(ParameterInfo paramInfo, int paramIndex)
    {
        return Get_ParamHandler(paramInfo.ParameterType, paramIndex, paramInfo.ParameterType.IsByRef, paramInfo.IsOut);
    }
    // Editor only
    public static ParamHandler Get_ParamHandler(FieldInfo fieldInfo)
    {
        return Get_ParamHandler(fieldInfo.FieldType, 0, false, false);//fieldInfo.FieldType.IsByRef);
    }
    public static string Get_GetJSReturn(Type type)
    {
        if (type == typeof(void))
            return string.Empty;

        if (type.IsArray)
        {
            arrayExchange.elementType = type.GetElementType();
            if (arrayExchange.elementType.IsArray)
            {
                Debug.LogError("Return [][] not supported");
                return string.Empty;
            }
            else if (arrayExchange.elementType.ContainsGenericParameters)
            {
                Debug.LogError(" Return T[] not supported");
                return "/* Return T[] is not supported */";
            }

            return arrayExchange.Get_GetJSReturn();
        }
        else
        {
            var sb = new StringBuilder();
            var keyword = GetMetatypeKeyword(type);

            sb.AppendFormat("JSMgr.vCall.datax.get{0}(JSDataExchangeMgr.eGetType.GetJSFUNRET)", keyword);
            return sb.ToString();
        }
    }
    public static string Get_Return(Type type, string expVar)
    {
        if (type == typeof(void))
            return expVar + ";";

        if (type.IsArray)
        {
            arrayExchange.elementType = type.GetElementType();
            if (arrayExchange.elementType.IsArray)
            {
                Debug.LogError("Return [][] not supported");
                return string.Empty;
            }
//            else if (arrayExchange.elementType.ContainsGenericParameters)
//            {
//                Debug.LogError(" Return T[] not supported");
//                return "/* Return T[] is not supported */";
//            }

            return arrayExchange.Get_Return(expVar);
        }
        else
        {
            var sb = new StringBuilder();
            var keyword = GetMetatypeKeyword(type);

            if (type.IsPrimitive)
                sb.AppendFormat("JSMgr.vCall.datax.set{0}(JSDataExchangeMgr.eSetType.SetRval, ({1})({2}));", keyword, JSNameMgr.GetTypeFullName(type), expVar);
            else if (type.IsEnum)
                sb.AppendFormat("JSMgr.vCall.datax.set{0}(JSDataExchangeMgr.eSetType.SetRval, (int){1});", keyword, expVar);
            else
                sb.AppendFormat("JSMgr.vCall.datax.set{0}(JSDataExchangeMgr.eSetType.SetRval, {1});", keyword, expVar);
            return sb.ToString();
        }
    }

    // Editor only
    // TODO: delete
    public static string Get_GetJSReturn2(Type type)
    {
        if (type == typeof(void))
            return string.Empty;

        JSDataExchange xcg = null;
        dict.TryGetValue(type, out xcg);
        if (xcg == null)
        {
            if (type.IsPrimitive)
            {
                Debug.LogError("Unknown Primitive Type: " + type.ToString());
                return string.Empty;
            }

            if (type.IsArray)
            {
                xcg = arrayExchange;
                arrayExchange.elementType = type.GetElementType();
                if (arrayExchange.elementType.IsArray)
                {
                    Debug.LogError("Return [][] not supported");
                    return string.Empty;
                }
                else if (arrayExchange.elementType.ContainsGenericParameters)
                {
                    Debug.LogError(" Return T[] not supported");
                    return "/* Return T[] is not supported */";
                }
            }
            else if (type.IsEnum)
            {
                xcg = enumExchange;
            }
            else
            {
                xcg = objExchange;
            }
        }
        return xcg.Get_GetJSReturn();
    }
    // Editor only
    // TODO: delete
    public static string Get_Return2(Type type, string expVar)
    {
        if (type == typeof(void))
            return expVar + ";";

        JSDataExchange xcg = null;
        dict.TryGetValue(type, out xcg);
        if (xcg == null)
        {
            if (type.IsPrimitive)
            {
                Debug.LogError("Unknown Primitive Type: " + type.ToString());
                return "";
            }

            if (type.IsArray)
            {
                xcg = arrayExchange;
                arrayExchange.elementType = type.GetElementType();
                if (arrayExchange.elementType.IsArray)
                {
                    Debug.LogError("Return [][] not supported");
                    return "";
                }
//                 else if (arrayExchange.elementType.ContainsGenericParameters)
//                 {
//                     Debug.LogError(" Return T[] not supported");
//                     return "/* Return T[] is not supported */";
//                 }
            }
            else if (type.IsEnum)
            {
                xcg = enumExchange;
            }
            else
            {
                xcg = objExchange;
            }
        }
        return xcg.Get_Return(expVar) + ";";
    }

    public static string GetMethodArg_DelegateFuncionName(Type classType, string methodName, int methodIndex, int argIndex)
    {
        // ��������ظ��ټ� method index
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0}_{1}_GetDelegate_member{2}_arg{3}", classType.Name, methodName, methodIndex, argIndex);
        return JSNameMgr.HandleFunctionName(sb.ToString());
    }
    public static string Build_GetDelegate(string getDelegateFunctionName, Type delType)
    {
        return new StringBuilder()
            .AppendFormat("JSDataExchangeMgr.GetJSArg<{0}>(()=>[[\n", JSNameMgr.GetTypeFullName(delType))
            .AppendFormat("    if (vc.datax.IsArgvFunction())\n")
            .AppendFormat("        return {0}(vc.getJSFunctionValue());\n", getDelegateFunctionName)
            .Append("    else\n")
            .AppendFormat("        return ({0})vc.datax.getObject(JSDataExchangeMgr.eGetType.GetARGV);\n", JSNameMgr.GetTypeFullName(delType))
            .Append("]])\n")
            .ToString();
    }
    public static StringBuilder Build_DelegateFunction(Type classType, MemberInfo memberInfo, Type delType, int methodIndex, int argIndex)
    {
        // building a closure
        // a function having a up-value: jsFunction

        string getDelFunctionName = GetMethodArg_DelegateFuncionName(classType, memberInfo.Name, methodIndex, argIndex);

        var sb = new StringBuilder();
		MethodInfo delInvoke = delType.GetMethod("Invoke");
		ParameterInfo[] ps = delInvoke.GetParameters();
        Type returnType = delType.GetMethod("Invoke").ReturnType;

        var argsParam = new cg.args();
        for (int i = 0; i < ps.Length; i++)
        {
            argsParam.Add(ps[i].Name);
        }

        // <t,u,v> ����ʽ
        string stringTOfMethod = string.Empty;
        if (delType.ContainsGenericParameters)
        {
            var arg = new cg.args();
            foreach (var t in delType.GetGenericArguments())
            {
                arg.Add(t.Name);
            }
            stringTOfMethod = arg.Format(cg.args.ArgsFormat.GenericT);
        }

        // this function name is used in BuildFields, don't change
        sb.AppendFormat("public static {0} {1}{2}(jsval jsFunction)\n[[\n",
            JSNameMgr.GetTypeFullName(delType),  // [0]
            getDelFunctionName, // [2]
            stringTOfMethod  // [1]
            );
        sb.Append("    if (jsFunction.asBits == 0)\n        return null;\n");
        sb.AppendFormat("    {0} action = ({1}) => \n", JSNameMgr.GetTypeFullName(delType), argsParam.Format(cg.args.ArgsFormat.OnlyList));
        sb.AppendFormat("    [[\n");
        sb.AppendFormat("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction{0}{1});\n", (argsParam.Count > 0) ? "," : "", argsParam);

        if (returnType != typeof(void))
            sb.Append("        return (" + JSNameMgr.GetTypeFullName(returnType) + ")" + JSDataExchangeEditor.Get_GetJSReturn(returnType) + ";\n");

        sb.AppendFormat("    ]];\n");
        sb.Append("    return action;\n");
        sb.AppendFormat("]]\n");

        return sb;
    }
    public enum MemberFeature
    {
        Static = 1 << 0,
        Indexer = 1 << 1, // Property ʹ��
        Get = 1 << 2,// Get Set ֻ������֮һ Field Property ʹ��
        Set = 1 << 3,
    }
    //
    // arg: a,b,c
    //
    public static string BuildCallString(Type classType, MemberInfo memberInfo, string argList, MemberFeature features, string newValue = "")
    {
        bool bGenericT = classType.IsGenericTypeDefinition;
        string memberName = memberInfo.Name;
        bool bIndexer = ((features & MemberFeature.Indexer) > 0);
        bool bStatic = ((features & MemberFeature.Static) > 0);
        bool bStruct = classType.IsValueType;
        string typeFullName = JSNameMgr.GetTypeFullName(classType);
        bool bField = (memberInfo is FieldInfo);
        bool bProperty = (memberInfo is PropertyInfo);
        bool bGet = ((features & MemberFeature.Get) > 0);
        bool bSet = ((features & MemberFeature.Set) > 0);
        if ((bGet && bSet) || (!bGet && !bSet)) { return ">>>> sorry >>>>"; }

        StringBuilder sb = new StringBuilder();

        if (bField || bProperty)
        {
            if (!bGenericT)
            {
                var strThis = typeFullName;
                if (!bStatic)
                {
                    strThis = "_this";
                    sb.AppendFormat("        {0} _this = ({0})vc.csObj;\n", typeFullName);
                }

                var result = string.Empty;
                if (bGet)
                {
                    // Լ�������صĽ���� result
                    result = "var result = ";
                }

                if (bIndexer)
                    sb.AppendFormat("        {2}{0}[{1}]", strThis, argList, result);
                else
                    sb.AppendFormat("        {2}{0}.{1}", strThis, memberName, result);

                if (bGet)
                {
                    sb.Append(";\n");
                }
                else
                {
                    sb.AppendFormat(" = {0};\n", newValue);
                    if (!bStatic && bStruct)
                    {
                        sb.Append("        JSMgr.changeJSObj(vc.jsObj, _this);\n");
                    }
                }
            }
            else
            {
                // Լ���������Ǹ��ý� member
                if (bIndexer || !bIndexer) // 2��һ��
                {
                    if (bProperty)
                    {
                        sb.AppendFormat("        {4}member.{0}({1}, {2}new object[][[{3}]]);\n",
                            bGet ? "GetValue" : "SetValue",
                            bStatic ? "null" : "vc.csObj",
                            bSet ? newValue + ", " : "",
                            argList,
                            bGet ? "var result = " : "");
                    }
                    else
                    {
                        sb.AppendFormat("        {3}member.{0}({1}{2});\n",
                            bGet ? "GetValue" : "SetValue",
                            bStatic ? "null" : "vc.csObj",
                            bSet ? ", " + newValue : "",
                            bGet ? "var result = " : "");
                    }
                }
            }
        }
        return sb.ToString();
    }
}