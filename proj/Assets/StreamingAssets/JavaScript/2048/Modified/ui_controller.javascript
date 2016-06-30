var gameManager = null;

define_mb("UIController", function() {
    
    // gameObject
    this.oTileRoot = null;
    
    // text
    this.oScore = null;
    this.oBestScore = null;
    
    var size = 4;    
    var uiTiles = [];
    var inputMgr, actuator, storageMgr;
    
    var keyCode =  [273, 275, 274, 276];
    var keyCodeString = ["Up", "Right", "Down", "Left"];
    var keyCodeData = [0, 1, 2, 3];
    
    var createUITile = function (trans, text) {
        var t = {
            trans: trans,
            text: text,
            
            clearText: function () {
                this.text.set_text("");
            },
            
            setValue: function(v) {
                this.text.set_text(v.toString());
            },
            
            moveFrom: function (fromPos) {
                this.movement.moveFrom(fromPos);
            }
        };
        
        t.originPos = t.trans.get_position();
        t.movement = t.trans.GetComponent$1(s.TileMovement);
        t.scaleAnim = t.text.GetComponent$1(UnityEngine.Animator.ctor);
        t.scaleAnim.set_enabled(false);
        
        t.playScaleAnim = function () {
            t.scaleAnim.set_enabled(true);
            this.scaleAnim.Play$$Int32(0);
        }
        
        return t;
    }
    
    this.clearUITiles = function () {
        for (var i = 0; i < uiTiles.length; i++) {
            uiTiles[i].clearText();
        }
    }
    
    this.getUITile = function (i, j) {
        return uiTiles[i + j * size];
    }
    
    this.Start = function () {
        // init ui tiles
        var parent = this.oTileRoot.get_transform();
        var chCount = parent.get_childCount();
        for (var i = 0; i < chCount; i++) {
            var child = parent.GetChild(i);
            uiTiles.push(createUITile(child, child.GetComponentInChildren$1(UnityEngine.UI.Text.ctor)));
        }
        
        inputMgr = new InputManager();
        actuator = new Actuator(this);
        storageMgr = new UnityStorageManager();
        
        // start game
        gameManager = new GameManager(size, inputMgr, actuator, storageMgr);
    }
    
    this.listen = function () {
        for (var i = 0; i < keyCode.length; i++) {
            if (UnityEngine.Input.GetKeyDown$$KeyCode(keyCode[i])) {
                inputMgr.emit("move", keyCodeData[i]);
                break;
            }
        }
    }
    
    this.Update = function () {
        this.listen();
    };
});

