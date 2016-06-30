
/*
* define javascript monobehaviour under object s
*/
var s = {};

/*
* use this function to definen a javascript monobehaviour
*/
var define_mb = function (name, fun) {
    s[name] = fun;
    s[name].getNativeType = function () { return "s." + name; }
    s[name].prototype = UnityEngine.MonoBehaviour.ctor.prototype;
}