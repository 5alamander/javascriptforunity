if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Reflection = {
    fullname: "jsimp.Reflection",
    baseTypeName: "System.Object",
    staticDefinition: {
        // ��2�����������T��C#���ͣ�����˵ GameObject���Ƿ���Ȼ��Ч��
        // ���ǣ��е����棬����Ϊ����Ч�ģ�
        // �ò���һ��
        CreateInstance$1: function (T){
            //return jsb_CallObjectCtor(T.getNativeType());
            return new T();
        },
        SetField: function (obj, fieldName, value){
            if (obj != null) {
                if (obj.hasOwnProperty(fieldName)) {
                    obj[fieldName] = value;
                    return true;
                }
            }
            return false;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};

// replace old Reflection
jsb_ReplaceOrPushJsType(jsimp$Reflection);