$(document).ready(function () {
    // Ajax işlemleri için Belge Ekle
    $("#belgeEkleForm").on("submit", function (e) {
        e.preventDefault();
        let formData = new FormData(this);

        $.ajax({
            type: "POST",
            url: "/Admin/BelgeEkle",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                alert(response.message);
                if (response.success) location.reload();
            },
            error: function () {
                alert("Hata oluştu.");
            }
        });
    });

    // Silme işlemi
    $(".delete-button").on("click", function () {
        let belgeID = $(this).data("id");

        if (confirm("Belgeyi silmek istediğinize emin misiniz?")) {
            $.ajax({
                type: "POST",
                url: "/Admin/BelgeSil",
                data: { belgeID: belgeID },
                success: function (response) {
                    alert(response.message);
                    if (response.success) location.reload();
                },
                error: function () {
                    alert("Hata oluştu.");
                }
            });
        }
    });

    // Güncelleme işlemi
    $(document).ready(function () {
        $(".guncelle-button").on("click", function () {
            var belgeID = $(this).data("id");
            var belgeAdi = $("#updateDocumentName").val();
            var belgeFormati = $("#updateDocumentFormat").val();
            var belgeDosyasi = $("#updateDocumentUpload")[0].files[0];

            var formData = new FormData();
            formData.append("belgeID", belgeID);
            formData.append("belgeAdi", belgeAdi);
            formData.append("belgeFormati", belgeFormati);
            if (belgeDosyasi) {
                formData.append("belgeDosyasi", belgeDosyasi);
            }

            $.ajax({
                url: '@Url.Action("BelgeGuncelle", "Admin")',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.success) {
                        alert(response.message);
                        // Tabloyu güncelleme
                        location.reload(); // Sayfayı yeniden yükleyerek güncellenmiş listeyi göster
                    } else {
                        alert(response.message);
                    }
                    $('#updateModal-' + belgeID).modal('hide');
                },
                error: function () {
                    alert("Bir hata oluştu!");
                }
            });
        });
    });
    document.getElementById("documentFormat").addEventListener("change", function () {
        var selectedFormat = this.value;
        var fileInput = document.getElementById("documentUpload");

        // Seçilen belge formatına göre dosya yükleme inputunun accept özelliğini değiştir
        if (selectedFormat == "pdf") {
            fileInput.accept = ".pdf";
        } else if (selectedFormat == "word") {
            fileInput.accept = ".doc,.docx";
        } else if (selectedFormat == "excel") {
            fileInput.accept = ".xls,.xlsx";
        } else {
            fileInput.accept = "";
        }
    });
});
