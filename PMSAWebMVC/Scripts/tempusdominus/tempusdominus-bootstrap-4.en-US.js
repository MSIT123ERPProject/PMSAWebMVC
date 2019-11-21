$.fn.datetimepicker.Constructor.Default = $.extend({}, $.fn.datetimepicker.Constructor.Default, {
    locale: 'en-us',
    //只顯示日期 2019/11/07
    format: 'YYYY/MM/DD',
    buttons: {
        showToday: true,
        showClear: true,
        showClose: true
    },
    toolbarPlacement: 'top',
    icons: {
        time: 'fa fa-clock-o',
        date: 'fa fa-calendar',
        up: 'fa fa-arrow-up',
        down: 'fa fa-arrow-down',
        previous: 'fa fa-chevron-left',
        next: 'fa fa-chevron-right',
        today: 'fa fa-calendar-day',
        clear: 'fa fa-trash-alt',
        close: 'fa fa-times'
    },
    tooltips: {
        today: 'Go to today',
        clear: 'Clear selection',
        close: 'Close the picker'
    }
});