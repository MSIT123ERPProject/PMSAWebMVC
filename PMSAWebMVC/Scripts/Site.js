(function ($) {
    // USE STRICT
    "use strict";

    try {
        $("body").tooltip({ selector: '[data-toggle=tooltip]' });
        //$('[data-toggle="tooltip"]').tooltip();
    } catch (error) {
        console.log(error);
    }

})(jQuery);