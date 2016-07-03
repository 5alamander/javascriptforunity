var gameManager = null;

define_mb("UIController", function() {
    "use strict",

    // gameObject
    this.oTileRoot = null;
    
    // text
    this.oScore = null;
    this.oBestScore = null;

    // button
    this.oNewGame = null;
    
    var size = 4;    
    var uiTiles = [];
    var inputMgr, actuator, storageMgr;
    
    var keyCode =  [273, 275, 274, 276];
    var keyCodeString = ["Up", "Right", "Down", "Left"];
    var keyCodeData = [0, 1, 2, 3];
    
    var createUITile = function (trans) {
        var t = {
            trans: trans,
            image: trans.GetComponent$1(UnityEngine.UI.Image.ctor),
            
            setValue: function(v) {
                this.image.set_sprite(UnityEngine.Resources.Load$1$$String(UnityEngine.Sprite.ctor, "2048/"+v.toString()));
                this.setVisible(true);
            },

            moveFromTo : function (fromPos, toPos, destroyAfterFinish) {
                this.movement.moveFromTo(fromPos, toPos, destroyAfterFinish);
            },

            moveFrom: function (fromPos, destroyAfterFinish) {
                this.moveFromTo(fromPos, this.originPos, destroyAfterFinish);
            },

            setVisible: function (visible) {
                this.image.set_enabled(visible);
            }
        };
        
        t.originPos = t.trans.get_position();
        t.movement = t.trans.GetComponent$1(s.TileMovement);
        t.animator = t.trans.GetComponent$1(UnityEngine.Animator.ctor);
        t.animator.set_enabled(false);
        
        t.playMergedAnim = function () {
            this.animator.set_runtimeAnimatorController(UnityEngine.Resources.Load$1$$String(UnityEngine.RuntimeAnimatorController.ctor, "2048/MergedCtrl"));
            this.animator.set_enabled(true);
            this.animator.Play$$Int32(0);
        }

        t.playBornAnim = function () {
            this.animator.set_runtimeAnimatorController(UnityEngine.Resources.Load$1$$String(UnityEngine.RuntimeAnimatorController.ctor, "2048/BornCtrl"));
            this.animator.set_enabled(true);
            this.animator.Play$$Int32(0);
        }
        
        return t;
    }
    
    this.clearUITiles = function () {
        for (var i = 0; i < uiTiles.length; i++) {
            uiTiles[i].setVisible(false);
        }
    }
    
    this.getUITile = function (i, j) {
        return uiTiles[i + j * size];
    }

    this.createTempUITile = function (copy_i, copy_j) {
        var transCopy = this.getUITile(copy_i, copy_j).trans;
        var goCopy = transCopy.get_gameObject();
        var go = UnityEngine.Object.Instantiate$$Object(goCopy);
        var trans = go.get_transform();
        trans.SetParent$$Transform$$Boolean(transCopy.get_parent(), false);
        trans.set_position(transCopy.get_position());
        trans.SetSiblingIndex(0);
        return createUITile(trans);
    }
    
    this.Start = function () {
        // init ui tiles
        var parent = this.oTileRoot.get_transform();
        var chCount = parent.get_childCount();
        for (var i = 0; i < chCount; i++) {
            var child = parent.GetChild(i);
            uiTiles.push(createUITile(child));
        }
        
        inputMgr = new InputManager();
        actuator = new Actuator(this);
        storageMgr = new UnityStorageManager();

        // 
        this.oNewGame.get_onClick().AddListener$$UnityAction(function () {
            inputMgr.emit("restart");
        });
        
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

