function previewImage(event) {
    var input = event.target;
    var reader = new FileReader();
    reader.onload = function () {
        var img = document.getElementById("preview-image");
        img.style.display = "block";
        img.src = reader.result;
    };
    reader.readAsDataURL(input.files[0]);
}
