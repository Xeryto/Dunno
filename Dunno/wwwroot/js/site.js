var images = document.getElementsByClassName("imgAdjust");
var descs = document.getElementsByClassName("descAdjust");
var titles = document.getElementsByClassName("titleAdjust");

for (var i = 0; i < images.length; i++) {
    descs[i].style.height = (images[i].clientHeight - 32 - titles[i].clientHeight).toString() + "px";
}

window.addEventListener("resize", function (event) {
    for (var i = 0; i < images.length; i++) {
        descs[i].style.height = (images[i].clientHeight - 32 - titles[i].clientHeight).toString() + "px";
    }
})