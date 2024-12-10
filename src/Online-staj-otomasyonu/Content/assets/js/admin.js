$(document).ready(function () {
  $("#dataTables-example1").DataTable();
  $("#dataTables-example2").DataTable();
  $("#dataTables-example3").DataTable();
  $("#dataTables-example4").DataTable();
  $("#dataTables-example5").DataTable();
  $("#dataTables-example6").DataTable();
  $("#dataTables-example7").DataTable();
  $("#dataTables-example8").DataTable();
  $("#dataTables-example9").DataTable();
});

flatpickr(".date-picker", {
  dateFormat: "d.m.Y",
  minDate: "today",
  allowInput: false,
  locale: "tr",
});

// Güncelle butonuna tıklandığında modal'ı aç ve bilgileri doldur
document.querySelectorAll(".btn-warning").forEach((button) => {
  button.addEventListener("click", function () {
    const row = this.closest("tr");
    const documentName = row.cells[0].textContent.trim();
    const documentFormat = row.cells[1]
      .querySelector("i")
      .classList.contains("fa-file-word")
      ? "word"
      : row.cells[1].querySelector("i").classList.contains("fa-file-excel")
      ? "excel"
      : "pdf";

    // Güncelle modal'ındaki alanları doldur
    document.getElementById("updateDocumentName").value = documentName;
    document.getElementById("updateDocumentFormat").value = documentFormat;
  });
});

const currentYear = new Date().getFullYear();

// Yılları 2024'ten 10 yıl sonrasına kadar oluştur
const yearOptions = Array.from({ length: 11 }, (v, i) => currentYear + i);

document.querySelectorAll(".year-picker").forEach((select) => {
  yearOptions.forEach((year) => {
    select.innerHTML += `<option value="${year}">${year}</option>`;
  });
});

function updateSecondTerm(select) {
  const firstTermYear = parseInt(select.value);
  const secondTermInput =
    select.parentElement.nextElementSibling.querySelector("input");

  if (firstTermYear) {
    secondTermInput.value = firstTermYear + 1; // 2. dönem yılı 1. dönem yılından 1 fazla
  } else {
    secondTermInput.value = ""; // İlk dönem yılı seçilmediğinde 2. dönem yılı boş
  }
}
