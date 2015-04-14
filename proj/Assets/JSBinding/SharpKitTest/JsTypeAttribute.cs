
using System;


namespace SharpKit.JavaScript
{
    // ժҪ:
    //     Specifies the export and interoperability mode of a C# type in JavaScript
    public enum JsMode
    {
        // ժҪ:
        //     Specifies a global function export mode, in which only static members are
        //     allowed, static methods become global functions static fields become global
        //     variables static constrctor becomes global code
        Global = 0,
        //
        // ժҪ:
        //     Specifies a prototype object export mode, in which a single constructor is
        //     allowed, and both static and instance members.  constructor becomes a constructor
        //     function instance members become the equivalent members on the constructor
        //     function's prototype.  static members become members on the constructor function
        //     itself.
        Prototype = 1,
        //
        // ժҪ:
        //     Specifies a .NET style class, in which all C# elements are supported, this
        //     mode requires JsClr library to be included on the client at runtime.
        Clr = 2,
        //
        // ժҪ:
        //     Specifies an invisible unexported json type, this class will not be exported,
        //     instantiation and usage of classes in this mode, will be exported to simple
        //     json elements.
        Json = 3,
        ExtJs = 4,
    }


    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = true)]
    public class JsTypeAttribute : Attribute
    {

        //
        // ժҪ:
        //     Creates an instance of a JsTypeAttribute in the specified JsMode, and exported
        //     to the specified filename
        //
        // ����:
        //   mode:
        //
        //   filename:
        public JsTypeAttribute(JsMode mode, string filename) { this.filename = filename; }
        public string filename;
    }
}