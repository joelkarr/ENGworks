
$(document).ready(function () {
    $("#selectAlltypes").click(function() {
        $("#typeList").each(function(){
            $("#typeList option").attr("selected","selected"); });
    }) ;
    $("#btnHelp").click(function () {
        // Display an external page using an iframe
        var src = "http://www.ENGworks.net/selector";
        $.modal('<iframe src="' + src + '" height="450" width="830" style="border:0">',
            {
                closeHTML: '<a class="modalCloseImg simplemodal-close" title="Close"></a>',
                containerCss: {
                    backgroundColor: "#fff",
                    borderColor: "#fff",
                    height: 450,
                    padding: 0,
                    width: 830
                },
                overlayClose: true
            });
    });
    $("#help").click(function () {
        // Display an external page using an iframe
        var src = "http://www.ENGworks.net/selector";
        $.modal('<iframe src="' + src + '" height="450" width="830" style="border:0">',
            {
                closeHTML: '<a class="modalCloseImg simplemodal-close" title="Close"></a>',
                containerCss: {
                    backgroundColor: "#fff",
                    borderColor: "#fff",
                    height: 450,
                    padding: 0,
                    width: 830
                },
                overlayClose: true
            });
    });
    $("#eula").click(function () {
        // Display an external page using an iframe
        var src = "http://taco.mepcontent.com/eula/";
        $.modal('<iframe src="' + src + '" height="450" width="830" style="border:0">',
            {
                closeHTML: '<a class="modalCloseImg simplemodal-close" title="Close"></a>',
                containerCss: {
                    backgroundColor: "#fff",
                    borderColor: "#fff",
                    height: 450,
                    padding: 0,
                    width: 830
                },
                overlayClose: true
            });
    });

    $("#request").click(function () {
        $.modal('<img class="modalimage" src="/Content/images/requestsuccess.png" alt="ThankYou">',
            {
                closeHTML: '<a class="modalCloseImg simplemodal-close" title="Close"></a>',
                containerCss: {
                    backgroundColor: "#fff",
                    borderColor: "#fff",
                    height: 207,
                    padding: 0,
                    width: 429
                },
                overlayClose: true
            });
    });

    $("#download").click(function () {
        $.modal('<img class="modalimage" src="/Content/images/importsuccess.png" alt="ThankYou">',
            {

                closeHTML: '<a class="modalCloseImg simplemodal-close" title="Close"></a>',
                containerCss: {
                    backgroundColor: "#fff",
                    borderColor: "#fff",
                    height: 435,
                    padding: 0,
                    width: 210
                },
                overlayClose: true
            });
    });
});