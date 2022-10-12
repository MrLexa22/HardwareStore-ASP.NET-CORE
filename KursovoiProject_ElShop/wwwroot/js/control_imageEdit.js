document.getElementById('olo').addEventListener('change', function () {
    if (this.value) {
        document.getElementById('checker').value = 'true';
        console.log(document.getElementById('checker').value);
    }
});

picturePreview = document.querySelector(".imagePreview");
actionButton = document.querySelector(".action-button");
fileInput = document.querySelector("input[name='uploadedFile']");
fileReader = new FileReader();

DEFAULT_IMAGE_SRC = "https://cs6.pikabu.ru/avatars/852/v852649-75385198.jpg";

actionButton.addEventListener("click", () => {
    if (picturePreview.src !== DEFAULT_IMAGE_SRC) {
        resetImage();
    } else {
        fileInput.click();
    }
});

fileInput.addEventListener("change", () => {
    refreshImagePreview();
});

function resetImage() {
    setActionButtonMode("upload");
    picturePreview.src = DEFAULT_IMAGE_SRC;
    document.getElementById('checker').value = '';
    fileInput.value = "";
}

function setActionButtonMode(mode) {
    let modes = {
        "upload": function () {
            actionButton.innerText = "Загрузить";
            actionButton.classList.remove("mode-remove");
            actionButton.classList.add("mode-upload");
        },
        "remove": function () {
            actionButton.innerText = "Удалить";
            actionButton.classList.remove("mode-upload");
            actionButton.classList.add("mode-remove");
        }
    }
    return (modes[mode]) ? modes[mode]() : console.error("unknown mode");
}

function refreshImagePreview() {
    if (fileInput.files && fileInput.files.length > 0) {
        var fileName = fileInput.value,
            idxDot = fileName.lastIndexOf(".") + 1,
            extFile = fileName.substr(idxDot, fileName.length).toLowerCase();
        console.log("hsjfd  " + fileName);
        if (extFile == "jpg" || extFile == "jpeg" || extFile == "png") {
            fileReader.readAsDataURL(fileInput.files[0]);
            fileReader.onload = (e) => {
                picturePreview.src = e.target.result;
                setActionButtonMode("remove");
            }
        }
        else {
            alert("Только jpg/jpeg и png файлы можно использовать!");
            fileInput.value = "";
            document.getElementById('checker').value = '';
        }
    }
}
document.getElementById('checker').value = 'true';
setActionButtonMode("remove");
refreshImagePreview();