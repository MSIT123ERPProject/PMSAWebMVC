(function ($) {
    // USE STRICT
    "use strict";

    try {
        $("body").tooltip({ selector: '[data-toggle=tooltip]', placement: "top" });
        //$('[data-toggle="tooltip"]').tooltip();

        $("body").popover({
            selector: '[data-toggle="popover-dropdown"]', html: true,
            trigger: 'focus',
            placement: 'bottom',
            content: function () { return $(this).children(".dropdown-menu").html(); }
        });

    } catch (error) {
        console.log(error);
    }

})(jQuery);