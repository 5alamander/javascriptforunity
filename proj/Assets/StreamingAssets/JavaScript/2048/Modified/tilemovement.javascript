define_mb("TileMovement", function () {
    this.Start = function () {
        this.trans = this.get_transform();
        this.originPos = this.trans.get_position();
    }
    
    var moved = false;
    
    var $cv = { Value: UnityEngine.Vector3.get_zero() };
    this.Update = function () {
        if (!moved) {
            return;
        }
    
        this.trans.set_position(
            UnityEngine.Vector3.SmoothDamp$$Vector3$$Vector3$$Vector3$$Single(
                this.trans.get_position(),
                this.originPos,
                $cv,
                0.05)
            );
    }
    
    this.moveFrom = function (fromPos) {
        moved = true;
        this.trans.set_position(fromPos);
    }
});