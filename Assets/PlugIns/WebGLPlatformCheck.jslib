mergeInto(LibraryManager.library, {
    GetWebGLPlatformNative: function () {
        var ua = navigator.userAgent || navigator.vendor || window.opera;

        // Android
        if (/android/i.test(ua)) {
            return 1; // Android
        }

        // iOS (iPhone, iPad, iPod)
        if (/iPad|iPhone|iPod/.test(ua) && !window.MSStream) {
            return 2; // iOS
        }

        // Desktop (or unknown mobile)
        return 0;
    },

    IsMobile: function () {
        var ua = navigator.userAgent || navigator.vendor || window.opera;
        var isAndroid = /android/i.test(ua);
        var isIOS = /iPad|iPhone|iPod/.test(ua) && !window.MSStream;
        return (isAndroid || isIOS) ? 1 : 0;
    }
});
