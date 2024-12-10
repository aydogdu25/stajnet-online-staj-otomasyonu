using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Online_staj_otomasyonu.Models;

namespace Online_staj_otomasyonu.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Forgot()
        {
            return View();
        }
       

        public static void SendResetMail(string email, string newPassword)
        {
            using (MailMessage mail = new MailMessage())
            {
                string smtpUser = "PROJE_GMAILI@gmail.com";
                string smtpPass = "UYGULAMA_SIFRENIZ"; 

                if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
                {
                    throw new Exception("SMTP kullanıcı adı veya şifresi yapılandırılmamış.");
                }

                mail.From = new MailAddress(smtpUser, "Şifre Sıfırlama");
                mail.To.Add(email);
                mail.Subject = "Yeni Şifreniz";
                mail.Body = $@"Merhaba StajNet üyesi,

                Yeni şifreniz: {newPassword}

                Bu şifreyi kullanarak sisteme giriş yapabilirsiniz.";

                mail.IsBodyHtml = false; 

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = true;

                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (SmtpException ex)
                    {
                        throw new Exception($"SMTP Hatası: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"E-posta gönderimi sırasında bir hata oluştu: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Forgot(string email)
        {
              // Öğrenci e-posta adresini kontrol et
            bool isStudent = Kullanici.CheckEmailExists(email, isStudent: true);
            if (isStudent)
            {
                string newPassword = GenerateRandomPassword();
                Kullanici.UpdatePassword("tbl_Ogrenciler", "ogrEposta", email, "ogrSifre", newPassword);

                // Yeni şifreyi mail olarak gönder
                SendResetMail(email, newPassword);

                // JavaScript ile alert ve yönlendirme işlemi
                ViewBag.AlertMessage = "Yeni sifreniz mail adresinize gonderildi.";
                return View();
            }

            // Hoca e-posta adresini kontrol et
            bool isLecturer = Kullanici.CheckEmailExists(email, isStudent: false);
            if (isLecturer)
            {
                string newPassword = GenerateRandomPassword();
                Kullanici.UpdatePassword("tbl_Hocalar", "hocaEposta", email, "hocaSifre", newPassword);

                // Yeni şifreyi mail olarak gönder
                SendResetMail(email, newPassword);

                // JavaScript ile alert ve yönlendirme işlemi
                ViewBag.AlertMessage = "Yeni sifreniz mail adresinize gonderildi.";
                return View();
            }

            // Geçersiz e-posta adresi
            ViewBag.AlertMessage = "Gecersiz e-posta adresi.";
            return View();
        }

        private string GenerateRandomPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            string newPassword = new string(Enumerable.Range(1, 10).Select(_ => validChars[random.Next(validChars.Length)]).ToArray());
            return newPassword;
        }

        [HttpPost]
        public ActionResult OgrenciGiris(string ogrNo, string ogrenciPassword)
        {
            int ogrenciNo;

            // int.TryParse ile girilen değerin geçerli bir tam sayıya dönüştürülüp dönüştürülmediğini kontrol et
            if (!int.TryParse(ogrNo, out ogrenciNo))
            {
                TempData["OgrenciError"] = "Gecersiz ogrenci numarasi!";
                return RedirectToAction("Index");
            }

            var (ogrenciAd, ogrenciSoyad) = Kullanici.KullaniciDogrulaOgrenci(ogrenciNo, ogrenciPassword);

            if (!string.IsNullOrEmpty(ogrenciAd) && !string.IsNullOrEmpty(ogrenciSoyad))
            {
                Session["Ogrenci"] = ogrenciNo;
                Session["OgrenciAd"] = ogrenciAd;
                Session["OgrenciSoyad"] = ogrenciSoyad;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["OgrenciError"] = "Kullanıcı adı veya parola yanlış!";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public ActionResult AkademisyenGiris(string akademisyenEmail, string akademisyenPassword)
        {
            var hocaBilgi = Kullanici.KullaniciDogrulaAkademisyen(akademisyenEmail, akademisyenPassword);

            if (!string.IsNullOrEmpty(hocaBilgi.hocaAd))
            {
                Session["Hoca"] = akademisyenEmail;
                Session["HocaAd"] = hocaBilgi.hocaAd;
                Session["HocaSoyad"] = hocaBilgi.hocaSoyad;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["AkademisyenError"] = "E-posta adresi veya parola yanlis!";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Logout()
        {
            
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("", "Login");  
        }



    }
}
