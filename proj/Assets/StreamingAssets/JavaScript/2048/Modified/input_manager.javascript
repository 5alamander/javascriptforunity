var InputManager = function () {
    var events = {};

    this.on = function (e, cb) {
        if (!events[e]) {
            events[e] = [];
        }
        events[e].push(cb);
    }
    
    this.emit = function (e, data) {
        var cbs = events[e];
        if (cbs) {
            cbs.forEach(function(cb) {
                cb(data);
            });
        }
    }
    
    this.restart = function () {
        print("restart");
    }
    
    this.keepPlaying = function () {
        //print("InputManager.keeyPlaying");
    }
}