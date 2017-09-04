$(function () {
    var imageId = $("#image").data("image-id");
    var userId = $("#addLike").data("user-id");

    $("#addLike").on('click', function () {
        $.post("/home/addLike", { imageId: imageId, userId: userId }, function () {
        });
        $("#addLike").attr('disabled', true);
    });

    $.get("/home/likedAlready", { imageId: imageId, userId: userId }, function (result) {
        $("#addLike").attr('disabled', result.likedAlready);
    });

    setInterval(function () {
        $.get("/home/getViewCount", { imageId: imageId }, function (result) {
            $("#viewcount").text(`View Count: ${result.VC}`)
        });
        $.get("/home/getLikes", { imageId: imageId }, function (result) {
            $("#likes").text(`Likes: ${result.Likes}`)
        });
    }, 100)
});