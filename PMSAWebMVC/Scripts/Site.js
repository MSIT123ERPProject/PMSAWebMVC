(function ($) {
    // USE STRICT
    "use strict";

    try {
        $("body").tooltip({ selector: '[data-toggle=tooltip]', placement: "top" });
        //$('[data-toggle="tooltip"]').tooltip();
    } catch (error) {
        console.log(error);
    }

})(jQuery);