$.extend(true, $.fn.dataTable.defaults, {
    responsive: true,
    fixedHeader: true,
    "errMode": 'throw',
    "language": {
        "emptyTable": "無資料",
        "processing": "處理中...",
        "loadingRecords": "載入中...",
        "lengthMenu": "顯示 _MENU_ 項結果",
        "zeroRecords": "沒有符合的結果",
        "info": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
        "infoEmpty": "顯示第 0 至 0 項結果，共 0 項",
        "infoFiltered": "(從 _MAX_ 項結果中過濾)",
        "infoPostFix": "",
        "infoThousands": ",",
        "search": "搜尋",
        "paginate": {
            "first": "第一頁",
            "previous": "上一頁",
            "next": "下一頁",
            "last": "最後一頁",
            "SourceListID": "貨源清單編號",
            "PartNumber": "料件編號",
            "QtyPerUnit": "批量",
            "UnitPrice": "單價",
            "UnitsOnOrder": "訂單數量",
            "UnitsInStock": "庫存數量",
            "PurchaseOrderStatus": "採購單狀態",
            "PurchaseOrderID": "採購單編號",
            "ReceiverName": "收貨人姓名",
            "ReceiverTel": "收貨人市話",
            "ReceiverMobile": "收貨人手機",
            "ReceiptAddress": "收貨人地址"
        },
        "aria": {
            "sortAscending": ": 升冪排列",
            "sortDescending": ": 降冪排列"
        }
    }
});