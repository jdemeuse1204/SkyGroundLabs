/*
 * ObjectDataBinding v1.0.0
 * Code: https://github.com/jdemeuse1204/SkyGroundLabs/tree/master/SkyGroundLabs
 * 2013 James Demeuse
 */

(function ($) {

    // Observable Object - Name is the identifier to link back to your object
    // This will return your observable object
    // We use a unique name in case more than one object is bound an the same time
    //----------------------------------------------------
    $.fn.getObserveObject = function getObserveObject(uniqueName) {

        // return the observe object
        return $(this[0]).data('DataBind' + uniqueName);

    }

    // Get the Observable Object - Name is the identifier to link back to your object
    // This will set your observable object
    //----------------------------------------------------
    $.fn.observe = function observe(object, uniqueName) {
        // attach DOM node
        var attachElement = this[0];

        $(attachElement).data('DataBind' + uniqueName, object);

        // get all elements from the DOM
        var items = document.getElementsByTagName("*");

        // loop through all elements and see who has bindings
        for (var i = 0; i < items.length; i++) {
            var element = items[i];
            var parentDataBind = $(element).data().bind;

            if (parentDataBind != undefined && parentDataBind != "") {

                // right now just look for Paths value
                var options = getOptions(element);
                var path = options.Path;
                var converter = options.Converter;
                var propertyName = options.PropertyName
                var objectName = options.ObjectName;

                // In case we bind multiple objects, we need to check which object
                // the binding options belong to
                if (objectName != "" && objectName != uniqueName) {
                    // if the name is not set we assume one binding
                    continue;
                }

                // loop through the objects properties
                $.each($(attachElement).data('DataBind' + uniqueName), function (key, value) {
                    if (key == path) {

                        // set the text
						if ($(element).is("label")){
							element.innerHTML = value;
						}else{
							setElementValue(element, value, converter, propertyName);

							// unbind any old bindings
							$(element).unbind();

							// bind the key up function to the control
							if ($(element).is("input") || $(element).is("textarea")) {
								$(element).keyup(function (sender) {
									onPropertyChanged(sender, attachElement, key, uniqueName);
								});
							} else if ($(element).is("select")) {
								$(element).change(function (sender) {
									onPropertyChanged(sender, attachElement, key, uniqueName);
								});
							}
						}

                        return false;
                    }
                });
            }
        }
    }

    // Supporting Functions
    //--------------------------------------

    // Converts the .NET boolean value to a bit value for UI
    function convertBoolToBit(value) {
        if (value == true) {
            return 1;
        } else {
            return 0;
        }
    }

    // Sets the value of a UI element
    function setElementValue(element, value, converter, propertyName) {
        var propNameToSet = propertyName == "" ? "value" : propertyName;

        // convert the value
        switch (converter.toUpperCase()) {
            case "BOOLTOBIT":
                value = convertBoolToBit(value);
                break;
        }

        switch (propNameToSet.toUpperCase()) {
            case "VALUE":
                element.value = getValue(value);
                break;
            case "LABEL":
                element.value = indexOfFromElement(element, value);
                break;
        }
    }

    function getValue(value) {

        if (value == null) {
            value = "";
        }

        if (value.toString().indexOf("Date") >= 0) {
            return new Date(parseInt(value.replace('/Date(', '').replace(')/', '')));
        } else {
            return value;
        }
    }

    // Returns the data-bind options
    function getOptions(element) {
        var parentDataBind = $(element).data().bind;
        var options = { Path: "", Converter: "", PropertyName: "", ObjectName: "" };

        if (parentDataBind.indexOf(",") >= 0) {
            var splitOptions = parentDataBind.split(",");

            $.each(splitOptions, function (index, value) {
                var propName = value.split(":")[0].replace(" ", "");
                var propValue = value.split(":")[1].replace(" ", "");

                switch (propName.toUpperCase()) {
                    case "PATH":
                        options.Path = propValue;
                        break;
                    case "CONVERTER":
                        options.Converter = propValue;
                        break;
                    case "PROPERTYNAME":
                        options.PropertyName = propValue;
                        break;
                    case "OBJECTNAME":
                        options.ObjectName = propValue;
                        break;
                }
            });

        } else {
            options.Path = parentDataBind.split(":")[1].replace(" ", "");
        }

        return options;
    }

    // fires when the property changes
    function onPropertyChanged(s, element, targetPropertyName, uniqueName) {
        var options = getOptions(s.currentTarget);
        var value = s.currentTarget.value;

        if (options.PropertyName.toUpperCase() == "LABEL") {
            value = getTextFromValue(s.currentTarget);
        }

        $($(element).data('DataBind' + uniqueName)).prop(targetPropertyName, value);
    }

    function indexOfFromElement(element, str) {
        var count = 0;
        $($(element).children()).each(
              function () {
                  if ($(this).text() == str) {
                      return false; // break out of loop
                  } else {
                      count++;
                  }
              }
        )

        if (count == (element.length - 1)) {
            return -1;
        } else {
            return count;
        }
    }

    function indexOfValueFromElement(element, val) {
        var count = 0;
        $($(element).children()).each(
              function () {
                  if ($(this).attr('value') == val) {
                      return false;
                  } else {
                      count++;
                  }
              }
        )

        if (count == 50) {
            return -1;
        } else {
            return count;
        }
    }

    function getTextFromValue(element) {
        var result = "";
        $($(element).children()).each(
              function () {
                  if ($(this).attr('value') == element.value) {
                      result = $(this).text();
                      return false;
                  }
              }
        )

        return result;
    }

}(jQuery));