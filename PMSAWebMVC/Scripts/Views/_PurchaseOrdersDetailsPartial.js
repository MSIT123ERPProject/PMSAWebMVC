$(document).ready(function () {
    //==$(document).ready begin==

    initDataTablesPO();
    //採購明細
    var dtPO;
    function initDataTablesPO() {
        dtPO = $('#dataTablesPO').DataTable({
            ordering: false,
            fixedHeader: true,
            searching: false,
            paging: false,
            info: false,
            //如第1欄是checkbox，會造成只有縮放按鈕有效，checkbox無法使用，所以縮放按鈕要分欄
            responsive: {
                details: {
                    type: 'column',
                    target: 0
                }
            },
            columnDefs: [{
                targets: 0,
                className: 'control'
            },
            ],
        });
        dtPO.on('responsive-resize', function (e, datatable, columns) {
            //修正縮放大小不正確的問題
            dtPO.columns.adjust().responsive.recalc();
        });
        resizeDatatable();
    }

    //==$(document).ready end==
});