using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// XmlParser
/// ����һ��ʾ����ʾ�����ʹ�� jsimp ��ĺ���������ཫҪ�������JavaScript��
/// ���������������ʹ�� Activator.CreateInstance<T> ��������ģ��������������JS��������ִ��
/// ���Ի��� jsimp.Reflection.CreateInstance ���Ϳ�����JS��ִ����
/// </summary>
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/JSImpTest/XmlParser.javascript")]
public class XmlParser
{
    public static T ComvertType<T>(Dictionary<string, string> dict)
    {
        T obj = jsimp.Reflection.CreateInstance<T>();
        foreach (var ele in dict)
        {
            var fieldName = ele.Key;
            var fieldValue = ele.Value;
            jsimp.Reflection.SetField(obj, fieldName, fieldValue);
        }
        return obj;
    }
}
