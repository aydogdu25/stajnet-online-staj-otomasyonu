using Online_staj_otomasyonu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_staj_otomasyonu.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var duyurular = Kullanici.GetirDuyurular();
            return View(duyurular);
        }
        [HttpGet]
        public ActionResult StajBasvurusu()
        {
            var donemler = Kullanici.DonemleriGetir();
            ViewBag.Donemler = donemler;

            return View();
        }

        [HttpPost]
        public ActionResult StajBasvurusu(string stajDonem, string stajProgramlari, string stajBaslangic, string stajBitis, HttpPostedFileBase stajBelge)
        {
            int ogrenciNo = Convert.ToInt32(Session["Ogrenci"]);
            string ogrenciAd = Session["OgrenciAd"].ToString();
            string ogrenciSoyad = Session["OgrenciSoyad"].ToString();

            if (string.IsNullOrWhiteSpace(stajBaslangic) || string.IsNullOrWhiteSpace(stajBitis))
            {
                TempData["ErrorStajBasvurusu"] = "Lütfen başlangıç ve bitiş tarihlerini doldurun!";
                return RedirectToAction("StajBasvurusu", "Home");
            }

            if (stajBelge != null && stajBelge.ContentLength > 0)
            {

                string basvuruKlasor = Server.MapPath("/Content/stajBasvurulari/");

                // Klasör yoksa oluşturuluyor
                if (!Directory.Exists(basvuruKlasor))
                {
                    Directory.CreateDirectory(basvuruKlasor);
                }

                // Dosya yolu
                string belgeYolu = Path.Combine(basvuruKlasor, stajBelge.FileName);

                // Dosyayı kaydet
                stajBelge.SaveAs(belgeYolu);

                string urlYolu = "/Content/stajBasvurulari/" + stajBelge.FileName;

                // Veritabanı işlemi
                bool basvuruSonuc = Kullanici.StajBasvurusuEkle(ogrenciNo, ogrenciAd, ogrenciSoyad, stajDonem, stajProgramlari, urlYolu, stajBaslangic, stajBitis);


                if (basvuruSonuc)
                {
                    TempData["SuccessStajBasvurusu"] = "Başvurunuz başarıyla alınmıştır!";
                    return RedirectToAction("StajBasvurusu", "Home");
                }
                else
                {
                    TempData["ErrorStajBasvurusu"] = "Bu döneme veya programa zaten bir başvuru yapmışsınız!";
                    return RedirectToAction("StajBasvurusu", "Home");
                }
            }
            else
            {
                TempData["ErrorStajBasvurusu"] = "Lütfen bir dosya seçin!";
                return RedirectToAction("StajBasvurusu", "Home");
            }
        }


        public ActionResult BasvuruDurumu()
        {
            int ogrenciId = (int)Session["Ogrenci"];
            Kullanici kullanici = new Kullanici();
            List<BasvuruDurumuModel> basvuruListesi = kullanici.BasvuruDurumlariniGetir(ogrenciId);

            return View(basvuruListesi);
        }

        [HttpGet]
        public ActionResult RaporYukleme()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RaporYukleme(string stajProgramlari, HttpPostedFileBase file)
        {
            Kullanici kullanici = new Kullanici();

            // Oturumdan öğrenci bilgilerini alın
            string ogrenciNo = Session["Ogrenci"]?.ToString();
            string ogrenciAd = Session["OgrenciAd"]?.ToString();
            string ogrenciSoyad = Session["OgrenciSoyad"]?.ToString();

            if (string.IsNullOrEmpty(ogrenciNo) || string.IsNullOrEmpty(ogrenciAd) || string.IsNullOrEmpty(ogrenciSoyad))
            {
                TempData["Error"] = "Oturum bilgisi eksik. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("RaporYukleme");
            }

            // Staj kaydını al
            var stajOnay = kullanici.GetStajOnayByOgrenciAndProgram(ogrenciNo, stajProgramlari);
            if (stajOnay == null)
            {
                TempData["Error"] = "Bu programa ait herhangi bir staj kaydı bulunamadı.";
                return RedirectToAction("RaporYukleme");
            }
            var duyuru = kullanici.GetDuyuruByStajProgram(stajProgramlari);
            if (duyuru != null)
            {
                DateTime sonYuklemeTarihi = Convert.ToDateTime(duyuru["sdTeslim"]);
                if (DateTime.Now > sonYuklemeTarihi)
                {
                    TempData["Error"] = "Son yükleme tarihi geçti.";
                    return RedirectToAction("RaporYukleme");
                }
            }

            DateTime stajBitis = Convert.ToDateTime(stajOnay["stajOBitis"]);
            if (DateTime.Now < stajBitis)
            {
                TempData["Error"] = "Staj bitmeden rapor yüklenemez.";
                return RedirectToAction("RaporYukleme");
            }

            // Dönem bilgisi
            string donem = stajOnay["stajODonem"].ToString();

            if (kullanici.IsRaporAlreadyUploaded(ogrenciNo, stajProgramlari))
            {
                TempData["Error"] = "Bu programa ait rapor zaten yüklendi.";
                return RedirectToAction("RaporYukleme");
            }

            if (file == null || file.ContentLength <= 0)
            {
                TempData["Error"] = "Lütfen bir dosya seçin.";
                return RedirectToAction("RaporYukleme");
            }

            // Raporlar klasörünü kontrol et ve yoksa oluştur
            string folderPath = Server.MapPath("~/Content/Raporlar");
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            // Dosya adı ve yolu
            string fileName = ogrenciNo + "_" + stajProgramlari + "_" + System.IO.Path.GetFileName(file.FileName);
            string filePath = System.IO.Path.Combine(folderPath, fileName);

            // Dosyayı kaydet
            file.SaveAs(filePath);

            // Veritabanına kaydet
            kullanici.InsertRapor(ogrenciNo, ogrenciAd, ogrenciSoyad, donem, stajProgramlari, "/Content/Raporlar/" + fileName);

            // Staj onayı tablosundan kaydı sil
            kullanici.DeleteStajOnay(ogrenciNo, stajProgramlari);

            TempData["Success"] = "Rapor başarıyla yüklendi!";
            return RedirectToAction("RaporYukleme");
        }

        public ActionResult Belgeler()
        {
            var belgeler = Kullanici.GetBelgeler();
            ViewBag.Belgeler = belgeler;

            return View();
        }

        [HttpGet]
        public ActionResult MesajPanel()
        {
            Kullanici kullanici = new Kullanici();

            List<string> alicilar = kullanici.MesajHocalar();
            ViewBag.Alicilar = alicilar;

            string ogrenciId = Session["Ogrenci"]?.ToString();
            if (ogrenciId == null)
            {
                return RedirectToAction("", "Login");
            }

            var model = new HomeMesaj
            {
                GelenKutusu = kullanici.GelenKutusuMesajlari(ogrenciId),
                GonderilenMesajlar = kullanici.GonderilenMesajlar(ogrenciId)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult MesajPanel(string recipient2, string messageInput2)
        {

            var ogrenciNo = Session["Ogrenci"].ToString();
            var ogrenciAd = Session["OgrenciAd"].ToString();
            var ogrenciSoyad = Session["OgrenciSoyad"].ToString();

            Kullanici kullanici = new Kullanici();
            try
            {
                kullanici.MesajGonder2(ogrenciNo, ogrenciAd, ogrenciSoyad, recipient2, messageInput2);

                TempData["SuccessMessageMesaj"] = "Mesaj basariyla gonderildi!";
                return RedirectToAction("MesajPanel");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessageMesaj"] = "Mesaj gonderilemedi. Hata: " + ex.Message;
                return RedirectToAction("MesajPanel");
            }
        }

        [HttpGet]
        public ActionResult SifreGuncelle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SifreGuncelle(string password, string passwordConfirm)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordConfirm))
            {
                TempData["ErrorPass"] = "Şifre alanları boş olamaz!";
                return RedirectToAction("SifreGuncelle");
            }

            if (password != passwordConfirm)
            {
                TempData["ErrorPass"] = "Şifreler eşleşmiyor!";
                return RedirectToAction("SifreGuncelle");
            }

            // Session'dan kullanıcı numarasını al
            if (Session["Ogrenci"] == null)
            {
                TempData["ErrorPass"] = "Oturum süresi doldu. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("", "Login");
            }

            int ogrNo = Convert.ToInt32(Session["Ogrenci"]);

            // Kullanıcı sınıfını çağır ve şifreyi güncelle
            bool sonuc = Kullanici.SifreGuncelle(ogrNo, password);

            if (sonuc)
            {
                TempData["SuccessPass"] = "Şifre başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorPass"] = "Şifre güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("SifreGuncelle");
        }


    }

}