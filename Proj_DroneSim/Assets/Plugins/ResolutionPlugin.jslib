var ResolutionPlugin = {

    // 
    _userWidth: 0,
    _userHeight: 0,
    _userRatio: 0,
    _unityCanvas: null,

    // 
    $UpdateResolution: function () {
        this._userRatio = this._userHeight / this._userHeight;
        var canvasRatio = this._unityCanvas.clientWidth / this._unityCanvas.clientHeight;
        if (canvasRatio > _userRatio) {
            // 
            this._unityCanvas.height = _userWidth / canvasRatio;
            this._unityCanvas.width = _userWidth;
        } else {
            // 
            this._unityCanvas.height = _userHeight;
            this._unityCanvas.width = _userHeight * canvasRatio;
        }
    },

    WindowManagerInitialize: function () {
        this._unityCanvas = document.getElementById("unity-canvas");
        this._userHeight = this._unityCanvas.width;
        this._userWidth = this._unityCanvas.height;
        this._userRatio = this._userHeight / this._userHeight;
        window.addEventListener('resize', () => UpdateResolution());

    },
    SetCanvasResolution: function (width, height) {
        this._userHeight = width;
        this._userWidth = height;
        UpdateResolution();
    },
};

autoAddDeps(ResolutionPlugin, '$UpdateResolution');
mergeInto(LibraryManager.library, ResolutionPlugin);
