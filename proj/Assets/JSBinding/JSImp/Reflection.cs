using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// ������ jsimp �����ռ�����࣬������ CS �� JS �ֱ�ʵ��
/// ������������ CS ʱ������ CS ���ࣻ������ JS ʱ������ JS ���ࡣ
/// ע�⣬Reflection�౾��Ӧ��û�� JsType ��ǩ����Ϊ������JS�ֶ�ʵ�ֵ�
/// ����Ҳû�й�ϵ�� SharpKit ��������� StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/Reflection.javascript (A)
/// �ֶ�ʵ�ֵ�JS�� StreamingAssets/JavaScript/JSImp/Reflection.javascript (B)
/// includes.javascript �ｫ��ͬʱ require A �� B �����ļ������� B ���� A ���棬������B��д�� jsb_ReplaceOrPushJsType ���������� B ���Ḳ�ǵ� A
/// ��ô����A��ʲô�ô����С���Ϊ��дJS����Ҫ���� SharpKit ����ģ����ԣ�ҪдB�����ȸ���һ��A���ٰ�����ĺ���ʵ�ָ����ˡ�
/// </summary>
namespace jsimp
{
[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/Reflection.javascript")]
    public class Reflection
    {
        public static T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
        public static bool SetField(object obj, string fieldName, object value)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                FieldInfo field = type.GetField(fieldName);
                if (field != null)
                {
                    field.SetValue(obj, value);
                    return true;
                }
            }
            return false;
        }
    }

}

