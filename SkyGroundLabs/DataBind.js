function getObserveObject(dataBindName) {

    // return the observe object
    return $("#" + dataBindName).data('DataBind');
}

function observe(object, dataBindName) {

    // attach DOM node
    $("#" + dataBindName).data('DataBind', object);

    // get all elements from the DOM
    var items = document.getElementsByTagName("*");

    // loop through all elements and see who has bindings
    for (var i = 0; i < items.length; i++) {
        var element = items[i];
        var dataBind = $(element).data().bind;

        if (dataBind != undefined && dataBind != "") {

            // right now just look for Paths value
            var dataBind = dataBind.split(":")[1].replace(" ", "");

            // loop through the objects properties
            $.each($("#" + dataBindName).data('DataBind'), function (key, value) {
                if (key == dataBind) {

                    // below data assumes we are using an input box

                    // set the text
                    element.value = value;

                    // unbind any old bindings
                    $(element).unbind();

                    // bind the key up function to the control
                    $(element).keyup(function (sender) {
                        $($("#" + dataBindName).data('DataBind')).prop(key, sender.currentTarget.value);
                    });
                    return false;
                }
            });
        }
    }
}