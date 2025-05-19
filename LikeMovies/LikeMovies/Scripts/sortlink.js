document.addEventListener("DOMContentLoaded", function () {
    // Lấy tham số 'sortBy' từ URL
    const urlParams = new URLSearchParams(window.location.search);
    const sortBy = urlParams.get('sortBy');  // Lấy giá trị 'sortBy'

    // Lấy tất cả các liên kết có class 'sort-link'
    var links = document.querySelectorAll(".sort-link");

    // Lặp qua tất cả các liên kết và áp dụng lớp 'selected' nếu tham số sortBy khớp
    links.forEach(function (link) {
        // Lấy giá trị 'sortBy' trong href (dựa vào URL của liên kết)
        var linkSortBy = link.href.split("sortBy=")[1]; // Tách giá trị 'sortBy' từ URL

        // Nếu tham số 'sortBy' trong URL trùng với giá trị của liên kết, thêm lớp 'selected'
        if (sortBy === linkSortBy) {
            link.classList.add("selected");
        } else {
            link.classList.remove("selected");
        }
    });
});