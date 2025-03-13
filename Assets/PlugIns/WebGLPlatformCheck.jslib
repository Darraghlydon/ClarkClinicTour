mergeInto(LibraryManager.library, {
    IsMobile: function() {
        // Simple check for common mobile browsers
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;
        return /android|ipad|iphone|ipod|mobile|silk/i.test(userAgent) ? 1 : 0;
    }
});