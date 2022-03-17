function ToggleLockState(_isLocked) {
    let lockState; // A number representing the lock state in Unity (0 = NONE, 1 = LOCKED, 2 = CONFINED)
    let unityCanvas = document.getElementById("unity-canvas");
    
    /**
     * Ueberprueft den gesetzen LockState und setzt, wenn true, den GameContainer in HTML auf fokussiert.
     */
    if (_isLocked) {
        //unityCanvas.requestPointerLock();
        lockState = 1;
    } else {
        lockState = 0;
    }

    // Sendet eine Request an Unity zum Setzen des internen LockState.
    console.log("Send to Unity: " + lockState);
    unityGame.SendMessage('ApplicationController', 'SetLockStateFromWeb', lockState);
}