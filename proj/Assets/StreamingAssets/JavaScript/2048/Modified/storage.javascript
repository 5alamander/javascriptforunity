
var UnityStorageManager = function () {
    this.clearGameState = function () {}
    this.getGameState = function () { return null; }
    this.getBestScore = function () {
        var bs = UnityEngine.PlayerPrefs.GetInt$$String$$Int32("2048BestScroe", 0);
        return bs;
    }
    this.setBestScore = function (bestScore) {
        UnityEngine.PlayerPrefs.SetInt("2048BestScroe", bestScore);
    }
    this.setGameState = function () {}
}