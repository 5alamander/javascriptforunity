using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;

public static class JSSerializerEditor
{
    static string ValueToString(object value, Type type)
    {
        StringBuilder sb = new StringBuilder();
        if (type.IsPrimitive)
        {
            sb.AppendFormat("{0}", value.ToString());
        }
        else if (type.IsEnum)
        {
            sb.AppendFormat("{0}", (int)Enum.Parse(type, value.ToString()));
        }
        else if (type == typeof(string))
        {
            sb.AppendFormat("{0}", value == null ? "" : value.ToString());
        }
        else if (type == typeof(Vector2))
        {
            Vector2 v2 = (Vector2)value;
            sb.AppendFormat("{0}/{1}", v2.x, v2.y);
        }
        else if (type == typeof(Vector3))
        {
            Vector3 v3 = (Vector3)value;
            sb.AppendFormat("{0}/{1}/{2}", v3.x, v3.y, v3.z);
        }
        return sb.ToString();
    }

    public static bool WillTypeBeAvailableInJavaScript(Type type)
    {
        return WillTypeBeExportedToJavaScript(type);
    }

    public static bool WillTypeBeExportedToJavaScript(Type type)
    {
        foreach (var t in JSBindingSettings.classes)
        {
            if (t == type)
                return true;
        }
        return false;
    }
}