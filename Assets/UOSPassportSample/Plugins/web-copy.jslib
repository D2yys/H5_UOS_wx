mergeInto(LibraryManager.library, {
    webCopy: function(utf8Str) {
        const str = UTF8ToString(utf8Str);
        navigator.clipboard.writeText(str).then(
        () => {
            console.log("copy success")
        },
        (e) => {
            console.log("copy fail")
            console.log(e)
        });
    }
})