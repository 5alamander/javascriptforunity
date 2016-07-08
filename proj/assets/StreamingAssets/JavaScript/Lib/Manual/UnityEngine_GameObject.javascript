_jstype = jst_find("UnityEngine.GameObject");

if (_jstype) {
    // extend gameObject.AddComponent<T>()
    _jstype.definition.AddComponent$1 = function(t0, iOfJsComp /* for custom JSComponent */ ) { 
        var native_t0 = t0.getNativeType();
        return CS.Call(4, 1, 2, false, this, native_t0, iOfJsComp); 
    }
}