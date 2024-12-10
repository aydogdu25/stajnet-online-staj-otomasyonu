using Online_staj_otomasyonu.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace Online_staj_otomasyonu.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            // Eğer GetirOnayBekleyenSayisi ve GetirOnaylananSayisi int türünde döndürüyorsa, direkt atama yapılabilir
            ViewBag.Staj1OnayBekleyen = Kullanici.GetirOnayBekleyenSayisi("Staj 1");
            ViewBag.Staj1Onaylanan = Kullanici.GetirOnaylananSayisi("Staj 1");

            ViewBag.Staj2OnayBekleyen = Kullanici.GetirOnayBekleyenSayisi("Staj 2");
            ViewBag.Staj2Onaylanan = Kullanici.GetirOnaylananSayisi("Staj 2");

            ViewBag.IMEOnayBekleyen = Kullanici.GetirOnayBekleyenSayisi("IME");
            ViewBag.IMEOnaylanan = Kullanici.GetirOnaylananSayisi("IME");

            return View();
        }


        public ActionResult Staj1Onay()
        {
            List<StajOnayModel> stajOnayList = Kullanici.Staj1OnaylariGetir();

            return View(stajOnayList);
        }
        public ActionResult Staj2Onay()
        {
            List<Staj2OnayModel> stajOnayList = Kullanici.Staj2OnaylariGetir();
            return View(stajOnayList);
        }
        public ActionResult ImeOnay()
        {
            List<ImeOnayModel> stajOnayList = Kullanici.ImeOnaylariGetir();
            return View(stajOnayList);
        }

        [HttpGet]
        public ActionResult Staj1Bekleyen()
        {
            var bekleyenStaj1 = Kullanici.GetStaj1Basvurulari();
            return View(bekleyenStaj1);
        }

        [HttpPost]

        public ActionResult Staj1Bekleyen(string OgrNo, string Ad, string Soyad, string Donem, string Program, string Belge, string Baslangic, string Bitis)
        {
            bool result = Kullanici.StajOnayla(OgrNo, Ad, Soyad, Donem, Program, Belge, Baslangic, Bitis);
            if (result)
            {
                return RedirectToAction("Staj1Bekleyen", "Admin");
            }
            else
            {
                TempData["Staj1BekleyenError"] = "Basvuru onayi yapilamadi!";
                return RedirectToAction("Staj1Bekleyen", "Admin");
            }
        }

        [HttpGet]
        public ActionResult Staj2Bekleyen()
        {
            var bekleyenStaj2 = Kullanici.GetStaj2Basvurulari();
            return View(bekleyenStaj2);
        }

        [HttpPost]
        public ActionResult Staj2Bekleyen(string OgrNo2, string Ad2, string Soyad2, string Donem2, string Program2, string Belge2, string Baslangic2, string Bitis2)
        {
            bool result = Kullanici.StajOnayla(OgrNo2, Ad2, Soyad2, Donem2, Program2, Belge2, Baslangic2, Bitis2);
            if (result)
            {
                return RedirectToAction("Staj2Bekleyen", "Admin");
            }
            else
            {
                TempData["Staj2BekleyenError"] = "Basvuru onayi yapilamadi!";
                return RedirectToAction("Staj2Bekleyen", "Admin");
            }
        }

        [HttpGet]
        public ActionResult ImeBekleyen()
        {
            var bekleyenIme = Kullanici.GetImeBasvurulari();
            return View(bekleyenIme);
        }

        [HttpPost]
        public ActionResult ImeBekleyen(string OgrNo3, string Ad3, string Soyad3, string Donem3, string Program3, string Belge3, string Baslangic3, string Bitis3)
        {
            bool result = Kullanici.StajOnayla(OgrNo3, Ad3, Soyad3, Donem3, Program3, Belge3, Baslangic3, Bitis3);
            if (result)
            {
                return RedirectToAction("ImeBekleyen", "Admin");
            }
            else
            {
                TempData["Staj2BekleyenError"] = "Basvuru onayi yapilamadi!";
                return RedirectToAction("ImeBekleyen", "Admin");
            }
        }


        public ActionResult Staj1Raporlar()
        {
            var raporlar = Kullanici.GetirRaporlar("Staj 1");
            return View(raporlar);
        }
        public ActionResult Staj2Raporlar()
        {
            var raporlar = Kullanici.GetirRaporlar("Staj 2");
            return View(raporlar);
        }
        public ActionResult ImeRaporlar()
        {
            var raporlar = Kullanici.GetirRaporlar("IME");
            return View(raporlar);
        }
        public ActionResult DonemAyarlari()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DonemAyarlari(string StajDonem, string DonemBYili1, string DonemSYili1, string DonemBasvuruBaslangic1, string DonemBasvuruBitis1,
                                          string DonemBYili2, string DonemSYili2, string DonemBasvuruBaslangic2, string DonemBasvuruBitis2,
                                          string DonemBYili3, string DonemSYili3, string DonemBasvuruBaslangic3, string DonemBasvuruBitis3)
        {
            bool result = false;
            switch (StajDonem)
            {
                case "Guz":
                    if (isNull(DonemBYili1, DonemSYili1, DonemBasvuruBaslangic1, DonemBasvuruBitis1))
                    {
                        result = Kullanici.DonemGuncelleVeyaEkle("Güz Dönemi", DonemBYili1, DonemSYili1, DonemBasvuruBaslangic1, DonemBasvuruBitis1);
                    }
                    break;
                case "Bahar":
                    if (isNull(DonemBYili2, DonemSYili2, DonemBasvuruBaslangic2, DonemBasvuruBitis2))
                    {
                        result = Kullanici.DonemGuncelleVeyaEkle("Bahar Dönemi", DonemBYili2, DonemSYili2, DonemBasvuruBaslangic2, DonemBasvuruBitis2);
                    }
                    break;
                case "Yaz":
                    if (isNull(DonemBYili3, DonemSYili3, DonemBasvuruBaslangic3, DonemBasvuruBitis3))
                    {
                        result = Kullanici.DonemGuncelleVeyaEkle("Yaz Dönemi", DonemBYili3, DonemSYili3, DonemBasvuruBaslangic3, DonemBasvuruBitis3);
                    }
                    break;
            }
            if (result)
            {
                TempData["SuccessDonem"] = "Donem bilgisi basariyla guncellendi.";
            }
            else
            {
                TempData["ErrorDonem"] = "Donem bilgisi guncellenirken bir hata olustu.";
            }
            return RedirectToAction("DonemAyarlari");
        }


        public ActionResult DuyuruAyarlari()
        {
            return View();
        }
        public ActionResult BelgeAyarlari()
        {
            var belgeler = Kullanici.GetBelgeler();
            ViewBag.Belgeler = belgeler;

            return View();
        }

        private bool isNull(params string[] degerler)
        {
            return degerler.All(d => !string.IsNullOrEmpty(d));
        }


        [HttpPost]
        public ActionResult DuyuruAyarlari(string StajProgram,
                                    string SonBasvuruTarihi1, string SdBaslangicTarihi1, string SdBitisTarihi1,
                                    string SdTeslimTarihi1, string MBaslangicTarihi1, string MBitisTarihi1,
                                    string SonBasvuruTarihi2, string SdBaslangicTarihi2, string SdBitisTarihi2,
                                    string SdTeslimTarihi2, string MBaslangicTarihi2, string MBitisTarihi2,
                                    string SonBasvuruTarihi3, string SdBaslangicTarihi3, string SdBitisTarihi3,
                                    string SdTeslimTarihi3, string MBaslangicTarihi3, string MBitisTarihi3)
        {
            bool result = false;

            switch (StajProgram)
            {
                case "Staj1":
                    if (isNull(SonBasvuruTarihi1, SdBaslangicTarihi1, SdBitisTarihi1, SdTeslimTarihi1, MBaslangicTarihi1, MBitisTarihi1))
                    {
                        result = DuyuruGuncelleVeyaEkle("Staj 1", SonBasvuruTarihi1, SdBaslangicTarihi1, SdBitisTarihi1, SdTeslimTarihi1, MBaslangicTarihi1, MBitisTarihi1);
                    }
                    break;
                case "Staj2":
                    if (isNull(SonBasvuruTarihi2, SdBaslangicTarihi2, SdBitisTarihi2, SdTeslimTarihi2, MBaslangicTarihi2, MBitisTarihi2))
                    {
                        result = DuyuruGuncelleVeyaEkle("Staj 2", SonBasvuruTarihi2, SdBaslangicTarihi2, SdBitisTarihi2, SdTeslimTarihi2, MBaslangicTarihi2, MBitisTarihi2);
                    }
                    break;
                case "Ime":
                    if (isNull(SonBasvuruTarihi3, SdBaslangicTarihi3, SdBitisTarihi3, SdTeslimTarihi3, MBaslangicTarihi3, MBitisTarihi3))
                    {
                        result = DuyuruGuncelleVeyaEkle("İME", SonBasvuruTarihi3, SdBaslangicTarihi3, SdBitisTarihi3, SdTeslimTarihi3, MBaslangicTarihi3, MBitisTarihi3);
                    }
                    break;
            }


            if (result)
            {
                TempData["Duyuru"] = "Duyuru başarıyla guncellendi.";
            }
            else
            {
                TempData["Error"] = "Bir hata olustu. Lutfen tarihleri kontrol ediniz.";
            }

            return RedirectToAction("DuyuruAyarlari");
        }


        //aşağıdaki fonk. yollanan değerler db ye gider
        private bool DuyuruGuncelleVeyaEkle(string StajAdi,
                                             string SonBasvuruTarihi, string SdBaslangicTarihi, string SdBitisTarihi,
                                             string SdTeslimTarihi, string MBaslangicTarihi, string MBitisTarihi)
        {


            // User.cs'deki fonksiyonu çağır
            return Kullanici.DuyuruGuncelleVeyaEkle(StajAdi,
                                                SonBasvuruTarihi,
                                                SdBaslangicTarihi,
                                                SdBitisTarihi,
                                                SdTeslimTarihi,
                                                MBaslangicTarihi,
                                                MBitisTarihi);
        }




        [HttpGet]
        public ActionResult MesajPaneli()
        {
            Kullanici kullanici = new Kullanici();

            List<string> alicilar = kullanici.MesajOgrenciler();
            ViewBag.Alicilar = alicilar;

            string hocaKimlik = Session["Hoca"]?.ToString();

            if (string.IsNullOrEmpty(hocaKimlik))
            {
                return RedirectToAction("", "Login");
            }

            AdminMesaj adminMesajlar = kullanici.GetAdminMesajlar(hocaKimlik);

            return View(adminMesajlar);
        }

        [HttpPost]
        public ActionResult MesajPaneli(string recipient, string messageInput)
        {
            try
            {
                Kullanici.MesajGonder(recipient, messageInput);

                TempData["MessageSuccessMesaj"] = "Mesaj basariyla gonderildi!";
            }
            catch (Exception)
            {
                TempData["ErrorMessageMesaj"] = $"Mesaj gonderilemedi. Hata!";
            }

            return RedirectToAction("MesajPaneli", "Admin");
        }
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
            if (Session["Hoca"] == null)
            {
                TempData["ErrorPass"] = "Oturum süresi doldu. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("", "Login");
            }

            string hocaEpsota = Session["Hoca"].ToString();

            // Kullanıcı sınıfını çağır ve şifreyi güncelle
            bool sonuc = Kullanici.SifreGuncelleAdmin(hocaEpsota, password);

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
        [HttpPost]
        public JsonResult BelgeEkle(string belgeAdi, string belgeFormati, HttpPostedFileBase belgeDosyasi)
        {
            if (Kullanici.BelgeVarMi(belgeAdi, belgeFormati))
            {
                return Json(new { success = false, message = "Aynı ad ve formatta belge zaten mevcut." });
            }

            if (belgeDosyasi != null && belgeDosyasi.ContentLength > 0)
            {
                string belgeKlasor = Server.MapPath("/Content/Belgeler/");
                if (!Directory.Exists(belgeKlasor))
                {
                    Directory.CreateDirectory(belgeKlasor);
                }

                string belgeYolu = Path.Combine(belgeKlasor, belgeDosyasi.FileName);
                belgeDosyasi.SaveAs(belgeYolu);

                bool success = Kullanici.BelgeEkle(belgeAdi, belgeFormati, "/Content/Belgeler/" + belgeDosyasi.FileName);
                return Json(new { success, message = success ? "Belge başarıyla eklendi." : "Belge eklenirken bir hata oluştu." });
            }

            return Json(new { success = false, message = "Belge dosyası yüklenemedi." });
        }

        [HttpPost]
        public JsonResult BelgeSil(int belgeID)
        {
            bool success = Kullanici.BelgeSil(belgeID);
            return Json(new { success, message = success ? "Belge başarıyla silindi." : "Belge silinirken bir hata oluştu." });
        }

        [HttpPost]
        public ActionResult StajRed(string OgrNo, string Ad, string Soyad, string Donem, string Program, string Belge, string Baslangic, string Bitis, string RedNedeni)
        {
            // Gerekli işlemleri yapan metodu çağırıyoruz.
            bool sonuc = Kullanici.StajReddet(OgrNo, Ad, Soyad, Donem, Program, Belge, Baslangic, Bitis, RedNedeni);

            if (sonuc)
            {
                TempData["Staj1BekleyenSuccess"] = "Başvuru reddedildi ve ilgili kayıt kaldırıldı.";
            }
            else
            {
                TempData["Staj1BekleyenSuccessError"] = "Başvuru reddedilirken bir hata oluştu.";
            }

            return RedirectToAction("Staj1Bekleyen", "Admin");
        }

        [HttpPost]
        public ActionResult Staj2Red(string OgrNo2, string Ad2, string Soyad2, string Donem2, string Program2, string Belge2, string Baslangic2, string Bitis2, string RedNedeni2)
        {
            // Gerekli işlemleri yapan metodu çağırıyoruz.
            bool sonuc = Kullanici.StajReddet(OgrNo2, Ad2, Soyad2, Donem2, Program2, Belge2, Baslangic2, Bitis2, RedNedeni2);

            if (sonuc)
            {
                TempData["Staj2BekleyenSuccess"] = "Başvuru reddedildi ve ilgili kayıt kaldırıldı.";
            }
            else
            {
                TempData["Staj2BekleyenError"] = "Başvuru reddedilirken bir hata oluştu.";
            }

            return RedirectToAction("Staj2Bekleyen", "Admin");
        }

        [HttpPost]
        public ActionResult ImeRed(string OgrNo3, string Ad3, string Soyad3, string Donem3, string Program3, string Belge3, string Baslangic3, string Bitis3, string RedNedeni3)
        {

            bool sonuc = Kullanici.StajReddet(OgrNo3, Ad3, Soyad3, Donem3, Program3, Belge3, Baslangic3, Bitis3, RedNedeni3);

            if (sonuc)
            {
                TempData["ImeBekleyenSuccess"] = "Basvuru reddedildi ve ilgili kayit silindi.";
            }
            else
            {
                TempData["ImeBekleyenError"] = "Basvuru reddedilirken bir hata olustu.";
            }

            return RedirectToAction("ImeBekleyen", "Admin");
        }
    }

}
