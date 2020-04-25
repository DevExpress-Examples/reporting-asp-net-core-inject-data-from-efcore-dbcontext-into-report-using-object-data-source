function Init(args) {
    var designer = args.designerModel;
    $(window).on('beforeunload', function(e) {
        if(designer.isDirty()) {
            designer.navigateByReports.closeAll().done(function() {
                return;
            });
            return "Designer have changes";
        }
    });
}

function Exit() {
    window.location = "/";
}