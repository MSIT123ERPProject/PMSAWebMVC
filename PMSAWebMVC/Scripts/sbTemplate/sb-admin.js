(function ($) {
    "use strict"; // Start of use strict

    // Toggle the side navigation
    $("#sidebarToggle").on('click', function (e) {

        if ($(".pagesDropdown").hasClass('active')) {
            $(".pagesDropdown").removeClass("active");
            //展開
            $(".sidebar").removeClass("toggled");
            //顯示頭像
            $("#userwrap").removeClass("d-none");
            $("#userwrap").addClass("d-block");
            //顯示頭像字
            $(".userwrap").removeClass("d-none");
            $(".userwrap").addClass("d-block");
            //顯示brand
            $("#navbrand").removeClass("d-none");
            $("#navbrand").addClass("d-block");
        }

        //阻止事件冒泡，修正DataTables在Sidebar縮小後跑版，另外其他內容也會在縮小後跑版
        e.stopPropagation();
        e.preventDefault();
        $(".sidebar").toggleClass("toggled");
        $("#userwrap").toggleClass("d-block");
        $("#navbrand").toggleClass("d-block");
        if ($(".sidebar").width() < 140) {
            $(".userwrap").addClass("d-none");
            $(".userwrap").removeClass("d-block");
            $("#navbrand").addClass("d-none");
            $("#navbrand").removeClass("d-block");
        } else {
            $(".userwrap").removeClass("d-none");
            $(".userwrap").addClass("d-block");
        }
        $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc().draw();
    });
    //mobile
    window.mobileAndTabletcheck = function () {
        var check = false;
        (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
        return check;
    };
    if (window.mobileAndTabletcheck || $(window).width() < 580) {
        $("#sidebarToggle").addClass("d-none");
        $(".userwrap").removeClass("d-none");
        $(".userwrap").addClass("d-block");
        $("#sidebarToggleMobile").on('click', function (e) {
            e.stopPropagation();
            e.preventDefault();
            $(".sidebar").toggleClass("toggled");
            $(".userwrap").removeClass("d-none");
            $(".userwrap").addClass("d-block");
            $("#navbrand").addClass("d-block");
            $("#navbrand").removeClass("d-none");
            $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc().draw();
        });
    } else {
        $(".userwrap").removeClass("d-none");
        $(".userwrap").addClass("d-block");
    }
    resizeNav();
    function resizeNav() {
        $("#sidebarToggle").removeClass("d-none");
        if ($(window).width() <= 580) {
            $(".sidebar").removeClass("toggled");
            $(".sidebar").addClass("toggled");
        }
        if (($(window).width() > 580 && $(".sidebar").width() < 140) || ($(window).width() <= 580)) {
            $(".userwrap").addClass("d-none");
            $(".userwrap").removeClass("d-block");
            $("#navbrand").addClass("d-none");
            $("#navbrand").removeClass("d-block");
        } else {
            $(".userwrap").addClass("d-block");
            $(".userwrap").removeClass("d-none");
            $("#navbrand").addClass("d-block");
            $("#navbrand").removeClass("d-none");
        }
    }
    $(window).on('resize', function () {
        resizeNav();
        //setTimeout(function () {
        //    resizeNav();
        //}, 150);
    });
    // Prevent the content wrapper from scrolling when the fixed side navigation hovered over
    $('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function (e) {
        if ($(window).width() > 580) {
            var e0 = e.originalEvent,
                delta = e0.wheelDelta || -e0.detail;
            this.scrollTop += (delta < 0 ? 1 : -1) * 30;
            e.stopPropagation();
            e.preventDefault();
        }
    });

    // Scroll to top button appear
    $(document).on('scroll', function () {
        var scrollDistance = $(this).scrollTop();
        if (scrollDistance > 100) {
            $('.scroll-to-top').fadeIn();
        } else {
            $('.scroll-to-top').fadeOut();
        }
        //if (scrollDistance < 60) {
        //    $('.navbar').fadeIn();
        //} else {
        //    $('.navbar').fadeOut();
        //}
    });

    // Smooth scrolling using jQuery easing
    $(document).on('click', 'a.scroll-to-top', function (event) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $($anchor.attr('href')).offset().top
        }, 1000, 'easeInOutExpo');
        event.preventDefault();
    });
    //$(document).on('click', 'li.navbar', function (event) {
    //    var $anchor = $(this);
    //    $('html, body').stop().animate({
    //        scrollTop: $($anchor.attr('href')).offset().top
    //    }, 1000, 'easeInOutExpo');
    //    event.preventDefault();
    //});
})(jQuery); // End of use strict

//
$(".sidebar-dropdown > a").click(function () {
    $(".sidebar-submenu").slideUp(200);
    if (
        $(this)
            .parent()
            .hasClass("active")
    ) {
        $(".sidebar-dropdown").removeClass("active");
        $(this)
            .parent()
            .removeClass("active");
    } else {
        $(".sidebar-dropdown").removeClass("active");
        $(this)
            .next(".sidebar-submenu")
            .slideDown(200);
        $(this)
            .parent()
            .addClass("active");
    }
});

$("#close-sidebar").click(function () {
    $(".page-wrapper").removeClass("toggled");
});
$("#show-sidebar").click(function () {
    $(".page-wrapper").addClass("toggled");
});


//點到摺疊就展開
$(".pagesDropdown").click(function (e) {
    //阻止事件冒泡，修正DataTables在Sidebar縮小後跑版，另外其他內容也會在縮小後跑版
    e.stopPropagation();
    //展開
    $(".sidebar").removeClass("toggled");
    //顯示頭像
    $("#userwrap").removeClass("d-none");
    $("#userwrap").addClass("d-block");
    //顯示頭像字
    $(".userwrap").removeClass("d-none");
    $(".userwrap").addClass("d-block");
    //顯示brand
    $("#navbrand").removeClass("d-none");
    $("#navbrand").addClass("d-block");
    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc().draw();
});

$("#userDropdown").click(function (e) {
    //阻止事件冒泡，修正DataTables在Sidebar縮小後跑版，另外其他內容也會在縮小後跑版
    e.stopPropagation();
    //展開
    $(".sidebar").removeClass("toggled");
    //顯示頭像
    $("#userwrap").removeClass("d-none");
    $("#userwrap").addClass("d-block");
    //顯示頭像字
    $(".userwrap").removeClass("d-none");
    $(".userwrap").addClass("d-block");
    //顯示brand
    $("#navbrand").removeClass("d-none");
    $("#navbrand").addClass("d-block");
    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().responsive.recalc().draw();
});