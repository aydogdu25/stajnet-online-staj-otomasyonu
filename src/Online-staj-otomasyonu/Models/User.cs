using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web;
using System.Diagnostics;
using System.Data;
using System.Security.Policy;
using System.Linq;

namespace Online_staj_otomasyonu.Models
{
    public class Belge
    {
        public int belgeID { get; set; }
        public string belgeAdi { get; set; }
        public string belgeFormati { get; set; }
        public string belgeLink { get; set; }
    }

    public class Duyuru
    {
        public int DuyuruID { get; set; }
        public string StajAdi { get; set; }
        public DateTime SonBasvuru { get; set; }
        public DateTime SdBaslangic { get; set; }
        public DateTime SdBitis { get; set; }
        public DateTime SdTeslim { get; set; }
        public DateTime MBaslangic { get; set; }
        public DateTime MBitis { get; set; }
    }

    public class DuyuruModel
    {
        public int DuyuruID { get; set; }
        public string StajAdi { get; set; }
        public DateTime SonBasvuruTarihi { get; set; }
        public DateTime SdBaslangicTarihi { get; set; }
        public DateTime SdBitisTarihi { get; set; }
        public DateTime SdTeslimTarihi { get; set; }
        public DateTime MBaslangicTarihi { get; set; }
        public DateTime MBitisTarihi { get; set; }
    }

    public class StajBasvuruModel
    {
        public string OgrNo { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Donem { get; set; }
        public string Baslangic { get; set; }
        public string Bitis { get; set; }
        public string Belge { get; set; }
    }

    public class Staj2BasvuruModel
    {
        public string OgrNo { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Donem { get; set; }
        public string Baslangic { get; set; }
        public string Bitis { get; set; }
        public string Belge { get; set; }
    }

    public class ImeBasvuruModel
    {
        public string OgrNo { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Donem { get; set; }
        public string Baslangic { get; set; }
        public string Bitis { get; set; }
        public string Belge { get; set; }
    }

    public class BasvuruDurumuModel
    {
        public string Program { get; set; }
        public string Donem { get; set; }
        public string Durum { get; set; }
        public string Neden { get; set; }
    }


    public class StajOnayModel
    {
        public string OgrenciNo { get; set; }
        public string OgrenciAd { get; set; }
        public string OgrenciSoyad { get; set; }
        public string StajDonem { get; set; }
        public string StajProgram { get; set; }  
    }

    public class Staj2OnayModel
    {
        public string OgrenciNo { get; set; }
        public string OgrenciAd { get; set; }
        public string OgrenciSoyad { get; set; }
        public string StajDonem { get; set; }
        public string StajProgram { get; set; }
    }

    public class ImeOnayModel
    {
        public string OgrenciNo { get; set; }
        public string OgrenciAd { get; set; }
        public string OgrenciSoyad { get; set; }
        public string StajDonem { get; set; }
        public string StajProgram { get; set; }
    }

    public class RaporModel
    {
        public string RaporNo { get; set; }
        public string RaporAd { get; set; }
        public string RaporSoyad { get; set; }
        public string RaporDonem { get; set; }
        public string RaporBelge { get; set; }
    }

    public class HomeMesaj
    {
        public List<MesajDetay> GelenKutusu { get; set; }
        public List<MesajDetay> GonderilenMesajlar { get; set; }
    }

    public class MesajDetay
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Content { get; set; }
    }

    public class AdminMesaj
    {
        public List<MesajDetay2> GelenKutusu { get; set; }
        public List<MesajDetay2> GonderilenMesajlar { get; set; }
    }

    public class MesajDetay2
    {
        public string Ad { get; set; }      
        public string Soyad { get; set; }  
        public string Content { get; set; } 
        public string EkBilgi { get; set; } 
    }

    public class Kullanici
    {
        private static string connectionString = "Data Source=SQL_BAGLANTI_DIZGESI;Initial Catalog=Staj_Otomasyon;Integrated Security=True;";
        public static (string ogrenciAd, string ogrenciSoyad) KullaniciDogrulaOgrenci(int ogrNo, string kullaniciSifre)
        {
            string ogrenciAd = null;
            string ogrenciSoyad = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Önce kullanıcıyı doğrulamak için COUNT(*) sorgusu
                    string countQuery = "SELECT COUNT(*) FROM tbl_Ogrenciler WHERE ogrNo = @KullaniciIdentifier AND ogrSifre = @KullaniciSifre";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    countCmd.Parameters.AddWithValue("@KullaniciSifre", kullaniciSifre);
                    countCmd.Parameters.AddWithValue("@KullaniciIdentifier", ogrNo);

                    int count = (int)countCmd.ExecuteScalar();

                    // Eğer kullanıcı varsa, adını ve soyadını çek
                    if (count > 0)
                    {
                        string nameQuery = "SELECT ogrAd, ogrSoyad FROM tbl_Ogrenciler WHERE ogrNo = @KullaniciIdentifier";
                        SqlCommand nameCmd = new SqlCommand(nameQuery, conn);
                        nameCmd.Parameters.AddWithValue("@KullaniciIdentifier", ogrNo);

                        using (SqlDataReader reader = nameCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ogrenciAd = reader["ogrAd"].ToString();
                                ogrenciSoyad = reader["ogrSoyad"].ToString();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                    foreach (SqlError err in ex.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error Code: {err.Number}, Message: {err.Message}");
                    }
                }
            }

            return (ogrenciAd, ogrenciSoyad);  // Ad ve soyadı döndür
        }

        public static (string hocaAd, string hocaSoyad) KullaniciDogrulaAkademisyen(string hocaEposta, string kullaniciSifre)
        {
            string hocaAd = string.Empty;
            string hocaSoyad = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Kullanıcıyı doğrula
                    string countQuery = "SELECT COUNT(*) FROM tbl_Hocalar WHERE hocaEposta = @KullaniciIdentifier AND hocaSifre = @KullaniciSifre";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    countCmd.Parameters.AddWithValue("@KullaniciSifre", kullaniciSifre);
                    countCmd.Parameters.AddWithValue("@KullaniciIdentifier", hocaEposta);

                    int count = (int)countCmd.ExecuteScalar();

                    // Eğer kullanıcı geçerliyse, ad ve soyadını çek
                    if (count > 0)
                    {
                        string nameQuery = "SELECT hocaAd, hocaSoyad FROM tbl_Hocalar WHERE hocaEposta = @KullaniciIdentifier";
                        SqlCommand nameCmd = new SqlCommand(nameQuery, conn);
                        nameCmd.Parameters.AddWithValue("@KullaniciIdentifier", hocaEposta);

                        using (SqlDataReader reader = nameCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hocaAd = reader["hocaAd"].ToString();
                                hocaSoyad = reader["hocaSoyad"].ToString();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("SQL Error: " + ex.Message);
                    foreach (SqlError err in ex.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error Code: {err.Number}, Message: {err.Message}");
                    }
                }
            }

            return (hocaAd, hocaSoyad); 
        }

        public static bool CheckEmailExists(string email, bool isStudent)
        {
            bool exists = false;
            string query = isStudent
                ? "SELECT COUNT(*) FROM tbl_Ogrenciler WHERE ogrEposta = @Email"
                : "SELECT COUNT(*) FROM tbl_Hocalar WHERE hocaEposta = @Email";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                int count = (int)cmd.ExecuteScalar();
                exists = count > 0;
            }

            return exists;
        }
        public static void UpdatePassword(string tableName, string emailColumn, string email, string passwordColumn, string newPassword)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $"UPDATE {tableName} SET {passwordColumn} = @NewPassword WHERE {emailColumn} = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NewPassword", newPassword);
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static List<Belge> GetBelgeler()
        {
            List<Belge> belgeler = new List<Belge>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT belgeID, belgeAdi, belgeFormati, belgeLink FROM tbl_Belgeler";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Belge belge = new Belge
                        {
                            belgeID = reader.GetInt32(0),
                            belgeAdi = reader.GetString(1),
                            belgeFormati = reader.GetString(2),
                            belgeLink = reader.GetString(3)
                        };
                        belgeler.Add(belge);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }

            return belgeler;
        }
        public static bool BelgeSil(int belgeID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM tbl_Belgeler WHERE belgeID = @belgeID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@belgeID", belgeID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static bool BelgeVarMi(string belgeAdi, string belgeFormati)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM tbl_Belgeler WHERE belgeAdi = @belgeAdi AND belgeFormati = @belgeFormati";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@belgeAdi", belgeAdi);
                cmd.Parameters.AddWithValue("@belgeFormati", belgeFormati);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
        public static bool BelgeEkle(string belgeAdi, string belgeFormati, string belgeLink)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Veritabanına ekleme yapacak SQL sorgusu
                string query = "INSERT INTO tbl_Belgeler (belgeAdi, belgeFormati, belgeLink) VALUES (@belgeAdi, @belgeFormati, @belgeLink)";

                SqlCommand cmd = new SqlCommand(query, conn);

                // Parametreleri ekliyoruz
                cmd.Parameters.AddWithValue("@belgeAdi", belgeAdi);
                cmd.Parameters.AddWithValue("@belgeFormati", belgeFormati);
                cmd.Parameters.AddWithValue("@belgeLink", belgeLink);

                try
                {
                    // Veritabanı bağlantısını açıyoruz
                    conn.Open();

                    // SQL komutunu çalıştırıyoruz ve başarı durumu döndürüyoruz
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    // Hata durumunda hata mesajını yazdırıyoruz
                    Console.WriteLine("Hata: " + ex.Message);
                    return false;
                }
            }
        }
        public static bool DuyuruGuncelleVeyaEkle(string StajAdi, string SonBasvuruTarihi, string SdBaslangicTarihi, string SdBitisTarihi, string SdTeslimTarihi, string MBaslangicTarihi, string MBitisTarihi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Veritabanında ilgili kaydın var olup olmadığını kontrol et
                string kontrolQuery = "SELECT COUNT(*) FROM tbl_Duyurular WHERE stajAdi = @StajAdi";
                SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, conn);
                kontrolCmd.Parameters.AddWithValue("@StajAdi", StajAdi);
                int count = (int)kontrolCmd.ExecuteScalar();

                if (count > 0)
                {
                    // Kayıt varsa güncelle
                    string guncelleQuery = "UPDATE tbl_Duyurular SET " +
                                           "sonBasvuru = @SonBasvuruTarihi, " +
                                           "sdBaslangic = @SdBaslangicTarihi, " +
                                           "sdBitis = @SdBitisTarihi, " +
                                           "sdTeslim = @SdTeslimTarihi, " +
                                           "mBaslangic = @MBaslangicTarihi, " +
                                           "mBitis = @MBitisTarihi " +
                                           "WHERE stajAdi = @StajAdi";

                    SqlCommand guncelleCmd = new SqlCommand(guncelleQuery, conn);
                    guncelleCmd.Parameters.AddWithValue("@StajAdi", StajAdi);
                    guncelleCmd.Parameters.AddWithValue("@SonBasvuruTarihi", SonBasvuruTarihi);
                    guncelleCmd.Parameters.AddWithValue("@SdBaslangicTarihi", SdBaslangicTarihi);
                    guncelleCmd.Parameters.AddWithValue("@SdBitisTarihi", SdBitisTarihi);
                    guncelleCmd.Parameters.AddWithValue("@SdTeslimTarihi", SdTeslimTarihi);
                    guncelleCmd.Parameters.AddWithValue("@MBaslangicTarihi", MBaslangicTarihi);
                    guncelleCmd.Parameters.AddWithValue("@MBitisTarihi", MBitisTarihi);

                    int result = guncelleCmd.ExecuteNonQuery();
                    return result > 0; // Güncelleme başarılı mı?
                }
                else
                {
                    // Kayıt yoksa yeni ekle
                    string ekleQuery = "INSERT INTO tbl_Duyurular (stajAdi, sonBasvuru, sdBaslangic, sdBitis, sdTeslim, mBaslangic, mBitis) " +
                                       "VALUES (@StajAdi, @SonBasvuruTarihi, @SdBaslangicTarihi, @SdBitisTarihi, @SdTeslimTarihi, @MBaslangicTarihi, @MBitisTarihi)";

                    SqlCommand ekleCmd = new SqlCommand(ekleQuery, conn);
                    ekleCmd.Parameters.AddWithValue("@StajAdi", StajAdi);
                    ekleCmd.Parameters.AddWithValue("@SonBasvuruTarihi", SonBasvuruTarihi);
                    ekleCmd.Parameters.AddWithValue("@SdBaslangicTarihi", SdBaslangicTarihi);
                    ekleCmd.Parameters.AddWithValue("@SdBitisTarihi", SdBitisTarihi);
                    ekleCmd.Parameters.AddWithValue("@SdTeslimTarihi", SdTeslimTarihi);
                    ekleCmd.Parameters.AddWithValue("@MBaslangicTarihi", MBaslangicTarihi);
                    ekleCmd.Parameters.AddWithValue("@MBitisTarihi", MBitisTarihi);

                    int result = ekleCmd.ExecuteNonQuery();
                    return result > 0; // Yeni kayıt ekleme başarılı mı?
                }
            }
        }

        public static List<Dictionary<string, string>> GetirDuyurular()
        {
            List<Dictionary<string, string>> duyurular = new List<Dictionary<string, string>>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT stajAdi, sonBasvuru, sdBaslangic, sdBitis, sdTeslim, mBaslangic, mBitis FROM tbl_Duyurular";
                SqlCommand cmd = new SqlCommand(query, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var duyuru = new Dictionary<string, string>
                {
                    { "StajAdi", reader["stajAdi"].ToString() },
                    { "SonBasvuru", reader["sonBasvuru"].ToString() },
                    { "SdBaslangic", reader["sdBaslangic"].ToString() },
                    { "SdBitis", reader["sdBitis"].ToString() },
                    { "SdTeslim", reader["sdTeslim"].ToString() },
                    { "MBaslangic", reader["mBaslangic"].ToString() },
                    { "MBitis", reader["mBitis"].ToString() }
                };
                        duyurular.Add(duyuru);
                    }
                }
            }

            return duyurular;
        }

        public static bool DonemGuncelleVeyaEkle(string donemAd, string donemBYili, string donemSYili, string basvuruBaslangic, string basvuruBitis)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string kontrolQuery = "SELECT COUNT(*) FROM tbl_Donemler WHERE donemAd = @DonemAd";
                SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, conn);
                kontrolCmd.Parameters.AddWithValue("@DonemAd", donemAd);
                int count = (int)kontrolCmd.ExecuteScalar();

                if (count > 0)
                {
                    // Güncelle
                    string guncelleQuery = "UPDATE tbl_Donemler SET donemBYili = @DonemBYili, donemSYili = @DonemSYili, " +
                                           "donemBasvuruBaslangic = @BasvuruBaslangic, donemBasvuruBitis = @BasvuruBitis " +
                                           "WHERE donemAd = @DonemAd";
                    SqlCommand guncelleCmd = new SqlCommand(guncelleQuery, conn);
                    guncelleCmd.Parameters.AddWithValue("@DonemBYili", donemBYili);
                    guncelleCmd.Parameters.AddWithValue("@DonemSYili", donemSYili);
                    guncelleCmd.Parameters.AddWithValue("@BasvuruBaslangic", basvuruBaslangic);
                    guncelleCmd.Parameters.AddWithValue("@BasvuruBitis", basvuruBitis);
                    guncelleCmd.Parameters.AddWithValue("@DonemAd", donemAd);

                    return guncelleCmd.ExecuteNonQuery() > 0;
                }
                else
                {
                    // Yeni Kayıt
                    string ekleQuery = "INSERT INTO tbl_Donemler (donemAd, donemBYili, donemSYili, donemBasvuruBaslangic, donemBasvuruBitis) " +
                                       "VALUES (@DonemAd, @DonemBYili, @DonemSYili, @BasvuruBaslangic, @BasvuruBitis)";
                    SqlCommand ekleCmd = new SqlCommand(ekleQuery, conn);
                    ekleCmd.Parameters.AddWithValue("@DonemAd", donemAd);
                    ekleCmd.Parameters.AddWithValue("@DonemBYili", donemBYili);
                    ekleCmd.Parameters.AddWithValue("@DonemSYili", donemSYili);
                    ekleCmd.Parameters.AddWithValue("@BasvuruBaslangic", basvuruBaslangic);
                    ekleCmd.Parameters.AddWithValue("@BasvuruBitis", basvuruBitis);

                    return ekleCmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static List<string> DonemleriGetir()
        {
            List<string> donemler = new List<string>();

            // SQL bağlantısı açıyoruz
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT donemAd, donemBYili, donemSYili FROM tbl_Donemler";

                // Veriyi sorguluyoruz
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string donemBYili = reader["donemBYili"].ToString();
                        string donemSYili = reader["donemSYili"].ToString();
                        string donemAd = reader["donemAd"].ToString();

                        string donem;

                        // Eğer donemBYili "-" ise, sadece donemSYili ve donemAd'yi kullanıyoruz
                        if (donemBYili == "-")
                        {
                            donem = $"{donemSYili} {donemAd}";
                        }
                        else
                        {
                            donem = $"{donemBYili} - {donemSYili} {donemAd}";
                        }

                        donemler.Add(donem);
                    }
                }
            }

            return donemler;
        }

        public static bool StajBasvurusuEkle(int ogrNo, string ogrenciAd, string ogrenciSoyad, string stajDonem, string stajProgramlari, string stajBelge, string stajBaslangic, string stajBitis)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 1. Program veya dönem kontrolü (Başvuru tablosu)
                    string basvuruKontrolQuery = "SELECT COUNT(*) FROM tbl_StajBasvurusu WHERE stajBNo = @OgrNo AND (stajBDonem = @StajDonem OR stajBProgram = @StajProgram)";
                    SqlCommand basvuruKontrolCmd = new SqlCommand(basvuruKontrolQuery, conn);
                    basvuruKontrolCmd.Parameters.AddWithValue("@OgrNo", ogrNo);
                    basvuruKontrolCmd.Parameters.AddWithValue("@StajDonem", stajDonem);
                    basvuruKontrolCmd.Parameters.AddWithValue("@StajProgram", stajProgramlari);
                    int basvuruVarMi = (int)basvuruKontrolCmd.ExecuteScalar();

                    if (basvuruVarMi > 0)
                    {
                        // Aynı programa veya döneme başvuru varsa
                        return false;
                    }

                    // 2. Onay Tablosu Kontrolü (Eğer staj zaten onaylanmışsa, tekrar başvuru yapılamaz)
                    string onayKontrolQuery = "SELECT COUNT(*) FROM tbl_StajOnay WHERE stajONo = @OgrNo AND (stajOProgram = @StajProgram OR stajODonem = @StajDonem)";
                    SqlCommand onayKontrolCmd = new SqlCommand(onayKontrolQuery, conn);
                    onayKontrolCmd.Parameters.AddWithValue("@OgrNo", ogrNo);
                    onayKontrolCmd.Parameters.AddWithValue("@StajDonem", stajDonem);
                    onayKontrolCmd.Parameters.AddWithValue("@StajProgram", stajProgramlari);
                    int onayVarMi = (int)onayKontrolCmd.ExecuteScalar();

                    if (onayVarMi > 0)
                    {
                        // Onaylanmış staj kaydı varsa, tekrar başvuru yapılmasın
                        return false;
                    }

                    // 3. Rapor Tablosu Kontrolü (Eğer rapor yüklenmişse, o program için tekrar başvuru yapılamaz)
                    string raporKontrolQuery = "SELECT COUNT(*) FROM tbl_StajRapor WHERE raporNo = @OgrNo AND (raporProgram = @StajProgram OR raporDonem = @StajDonem)";
                    SqlCommand raporKontrolCmd = new SqlCommand(raporKontrolQuery, conn);
                    raporKontrolCmd.Parameters.AddWithValue("@OgrNo", ogrNo);
                    raporKontrolCmd.Parameters.AddWithValue("@StajDonem", stajDonem);
                    raporKontrolCmd.Parameters.AddWithValue("@StajProgram", stajProgramlari);
                    int raporVarMi = (int)raporKontrolCmd.ExecuteScalar();

                    if (raporVarMi > 0)
                    {
                        // Rapor yüklenmişse, aynı programa tekrar başvuru yapılamaz
                        return false;
                    }

                    // Başvuru yapılabilir, veritabanına ekleme işlemi
                    string insertQuery = "INSERT INTO tbl_StajBasvurusu (stajBAd, stajBSoyad, stajBNo, stajBDonem, stajBProgram, stajBBelge, stajBBaslangic, stajBBitis) " +
                                         "VALUES (@OgrAd, @OgrSoyad, @OgrNo, @StajDonem, @StajProgram, @StajBelge, @StajBaslangic, @StajBitis)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@OgrAd", ogrenciAd);
                    insertCmd.Parameters.AddWithValue("@OgrSoyad", ogrenciSoyad);
                    insertCmd.Parameters.AddWithValue("@OgrNo", ogrNo);
                    insertCmd.Parameters.AddWithValue("@StajDonem", stajDonem);
                    insertCmd.Parameters.AddWithValue("@StajProgram", stajProgramlari);
                    insertCmd.Parameters.AddWithValue("@StajBelge", stajBelge);
                    insertCmd.Parameters.AddWithValue("@StajBaslangic", stajBaslangic);
                    insertCmd.Parameters.AddWithValue("@StajBitis", stajBitis);
                    insertCmd.ExecuteNonQuery();

                    return true; // Başvuru başarıyla eklendi
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return false; // Hata oluştu
                }
            }
        }


        public static List<StajBasvuruModel> GetStaj1Basvurulari()
        {
            List<StajBasvuruModel> stajlar = new List<StajBasvuruModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT stajBNo, stajBAd, stajBSoyad, stajBDonem, stajBBaslangic, stajBBitis, stajBBelge FROM tbl_StajBasvurusu WHERE stajBProgram = @Program";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Program", "Staj 1");

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    StajBasvuruModel staj = new StajBasvuruModel
                    {
                        OgrNo = reader["stajBNo"].ToString(),
                        Ad = reader["stajBAd"].ToString(),
                        Soyad = reader["stajBSoyad"].ToString(),
                        Donem = reader["stajBDonem"].ToString(),
                        Baslangic = Convert.ToDateTime(reader["stajBBaslangic"]).ToString("dd.MM.yyyy"),
                        Bitis = Convert.ToDateTime(reader["stajBBitis"]).ToString("dd.MM.yyyy"),
                        Belge = reader["stajBBelge"].ToString()
                    };
                    stajlar.Add(staj);
                }
            }

            return stajlar;
        }

        public static List<Staj2BasvuruModel> GetStaj2Basvurulari()
        {
            List<Staj2BasvuruModel> stajlar = new List<Staj2BasvuruModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT stajBNo, stajBAd, stajBSoyad, stajBDonem, stajBBaslangic, stajBBitis, stajBBelge FROM tbl_StajBasvurusu WHERE stajBProgram = @Program";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Program", "Staj 2");
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Staj2BasvuruModel staj = new Staj2BasvuruModel
                    {
                        OgrNo = reader["stajBNo"].ToString(),
                        Ad = reader["stajBAd"].ToString(),
                        Soyad = reader["stajBSoyad"].ToString(),
                        Donem = reader["stajBDonem"].ToString(),
                        Baslangic = Convert.ToDateTime(reader["stajBBaslangic"]).ToString("dd.MM.yyyy"),
                        Bitis = Convert.ToDateTime(reader["stajBBitis"]).ToString("dd.MM.yyyy"),
                        Belge = reader["stajBBelge"].ToString()
                    };
                    stajlar.Add(staj);
                }
            }

            return stajlar;
        }

        public static List<ImeBasvuruModel> GetImeBasvurulari()
        {
            List<ImeBasvuruModel> stajlar = new List<ImeBasvuruModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT stajBNo, stajBAd, stajBSoyad, stajBDonem, stajBBaslangic, stajBBitis, stajBBelge FROM tbl_StajBasvurusu WHERE stajBProgram = @Program";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Program", "IME");
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ImeBasvuruModel staj = new ImeBasvuruModel
                    {
                        OgrNo = reader["stajBNo"].ToString(),
                        Ad = reader["stajBAd"].ToString(),
                        Soyad = reader["stajBSoyad"].ToString(),
                        Donem = reader["stajBDonem"].ToString(),
                        Baslangic = Convert.ToDateTime(reader["stajBBaslangic"]).ToString("dd.MM.yyyy"),
                        Bitis = Convert.ToDateTime(reader["stajBBitis"]).ToString("dd.MM.yyyy"),
                        Belge = reader["stajBBelge"].ToString()
                    };
                    stajlar.Add(staj);
                }
            }

            return stajlar;
        }

        public static bool SifreGuncelle(int ogrNo, string yeniSifre)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE tbl_Ogrenciler SET ogrSifre = @YeniSifre WHERE ogrNo = @OgrNo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@YeniSifre", yeniSifre);
                    cmd.Parameters.AddWithValue("@OgrNo", ogrNo);

                    int etkilenenSatir = cmd.ExecuteNonQuery();
                    return etkilenenSatir > 0; 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }

        public static bool SifreGuncelleAdmin(string eposta, string yeniSifre)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE tbl_Hocalar SET hocaSifre = @YeniSifre WHERE hocaEposta = @HocaEposta";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@YeniSifre", yeniSifre);
                    cmd.Parameters.AddWithValue("@HocaEposta", eposta);

                    int etkilenenSatir = cmd.ExecuteNonQuery();
                    return etkilenenSatir > 0;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }

        }

        public static bool StajOnayla(string OgrNo, string Ad, string Soyad, string Donem, string Program, string Belge, string Baslangic, string Bitis)
        {
            // SQL sorguları
            string deleteRedQuery = "DELETE FROM tbl_StajRed WHERE stajRNo = @OgrNo AND (stajRDonem = @Donem OR stajRProgram = @Program)";
            string insertQuery = "INSERT INTO tbl_StajOnay (stajONo, stajOAd, stajOSoyad, stajODonem, stajOProgram, stajOBelge, stajOBaslangic, stajOBitis) " +
                                 "VALUES (@OgrNo, @Ad, @Soyad, @Donem, @Program, @Belge, @Baslangic, @Bitis)";
            string deleteBasvuruQuery = "DELETE FROM tbl_StajBasvurusu WHERE stajBNo = @OgrNo AND stajBBaslangic = @Baslangic AND stajBBitis = @Bitis";

            try
            {
                // SqlConnection kullanarak veritabanına bağlanıyoruz
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // İşlemleri bir transaction içinde gerçekleştiriyoruz
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Red tablosundaki kaydı silme
                            using (SqlCommand deleteRedCmd = new SqlCommand(deleteRedQuery, conn, transaction))
                            {
                                deleteRedCmd.Parameters.AddWithValue("@OgrNo", OgrNo);
                                deleteRedCmd.Parameters.AddWithValue("@Donem", Donem);
                                deleteRedCmd.Parameters.AddWithValue("@Program", Program);

                                deleteRedCmd.ExecuteNonQuery();
                            }

                            // Onay tablosuna yeni kayıt ekleme
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@OgrNo", OgrNo);
                                insertCmd.Parameters.AddWithValue("@Ad", Ad);
                                insertCmd.Parameters.AddWithValue("@Soyad", Soyad);
                                insertCmd.Parameters.AddWithValue("@Donem", Donem);
                                insertCmd.Parameters.AddWithValue("@Program", Program);
                                insertCmd.Parameters.AddWithValue("@Belge", Belge);
                                insertCmd.Parameters.AddWithValue("@Baslangic", Baslangic);
                                insertCmd.Parameters.AddWithValue("@Bitis", Bitis);

                                insertCmd.ExecuteNonQuery();
                            }

                            // Başvuru tablosundaki kaydı silme
                            using (SqlCommand deleteBasvuruCmd = new SqlCommand(deleteBasvuruQuery, conn, transaction))
                            {
                                deleteBasvuruCmd.Parameters.AddWithValue("@OgrNo", OgrNo);
                                deleteBasvuruCmd.Parameters.AddWithValue("@Baslangic", Baslangic);
                                deleteBasvuruCmd.Parameters.AddWithValue("@Bitis", Bitis);

                                deleteBasvuruCmd.ExecuteNonQuery();
                            }

                            // Transaction başarılı, commit ediyoruz
                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            // Hata oluşursa transaction geri alınıyor
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir veya kullanıcıya bilgi verilebilir
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public static bool StajReddet(string ogrNo, string ad, string soyad, string donem, string program, string belge, string baslangic, string bitis, string redNedeni)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Reddedilen kaydı tbl_StajRed tablosuna ekliyoruz
                    string insertQuery = @"
                INSERT INTO tbl_StajRed (stajRNo, stajRAd, stajRSoyad, stajRDonem, stajRProgram, stajRBelge, stajRBaslangic, stajRBitis, stajRNeden) 
                VALUES (@OgrNo, @Ad, @Soyad, @Donem, @Program, @Belge, @Baslangic, @Bitis, @RedNedeni)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OgrNo", ogrNo);
                        cmd.Parameters.AddWithValue("@Ad", ad);
                        cmd.Parameters.AddWithValue("@Soyad", soyad);
                        cmd.Parameters.AddWithValue("@Donem", donem);
                        cmd.Parameters.AddWithValue("@Program", program);
                        cmd.Parameters.AddWithValue("@Belge", belge);
                        cmd.Parameters.AddWithValue("@Baslangic", baslangic);
                        cmd.Parameters.AddWithValue("@Bitis", bitis);
                        cmd.Parameters.AddWithValue("@RedNedeni", redNedeni);

                        cmd.ExecuteNonQuery();
                    }

                    // İlgili kaydı tbl_StajBasvurusu tablosundan siliyoruz
                    string deleteQuery = "DELETE FROM tbl_StajBasvurusu WHERE stajBNo = @OgrNo AND stajBDonem = @Donem AND stajBProgram = @Program";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OgrNo", ogrNo);
                        cmd.Parameters.AddWithValue("@Donem", donem);
                        cmd.Parameters.AddWithValue("@Program", program);

                        cmd.ExecuteNonQuery();
                    }

                    // İşlemleri başarılı bir şekilde tamamlama
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    
                    transaction.Rollback();
                    return false;
                }
            }
        }



        public List<BasvuruDurumuModel> BasvuruDurumlariniGetir(int ogrenciId)
        {
            List<BasvuruDurumuModel> basvuruListesi = new List<BasvuruDurumuModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // tbl_StajOnay verileri
                string queryOnay = "SELECT stajOProgram AS Program, stajODonem AS Donem FROM tbl_StajOnay WHERE stajONo = @ogrenciId";
                using (SqlCommand command = new SqlCommand(queryOnay, connection))
                {
                    command.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            basvuruListesi.Add(new BasvuruDurumuModel
                            {
                                Program = reader["Program"].ToString(),
                                Donem = reader["Donem"].ToString(),
                                Durum = "Onaylandı",
                                Neden = null
                            });
                        }
                    }
                }

                // tbl_StajRed verileri
                string queryRed = "SELECT stajRProgram AS Program, stajRDonem AS Donem, stajRNeden AS Neden FROM tbl_StajRed WHERE stajRNo = @ogrenciId";
                using (SqlCommand command = new SqlCommand(queryRed, connection))
                {
                    command.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var mevcutKayit = basvuruListesi.FirstOrDefault(b =>
                                b.Program == reader["Program"].ToString() &&
                                b.Donem == reader["Donem"].ToString());

                            if (mevcutKayit != null)
                            {
                                mevcutKayit.Durum = "Reddedildi";
                                mevcutKayit.Neden = reader["Neden"].ToString();
                            }
                            else
                            {
                                basvuruListesi.Add(new BasvuruDurumuModel
                                {
                                    Program = reader["Program"].ToString(),
                                    Donem = reader["Donem"].ToString(),
                                    Durum = "Reddedildi",
                                    Neden = reader["Neden"].ToString()
                                });
                            }
                        }
                    }
                }

                // tbl_StajBasvurusu verileri
                string queryBasvuru = "SELECT stajBProgram AS Program, stajBDonem AS Donem FROM tbl_StajBasvurusu WHERE stajBNo = @ogrenciId";
                using (SqlCommand command = new SqlCommand(queryBasvuru, connection))
                {
                    command.Parameters.AddWithValue("@ogrenciId", ogrenciId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var mevcutKayit = basvuruListesi.FirstOrDefault(b =>
                                b.Program == reader["Program"].ToString() &&
                                b.Donem == reader["Donem"].ToString());

                            if (mevcutKayit == null)
                            {
                                basvuruListesi.Add(new BasvuruDurumuModel
                                {
                                    Program = reader["Program"].ToString(),
                                    Donem = reader["Donem"].ToString(),
                                    Durum = "İnceleniyor",
                                    Neden = null
                                });
                            }
                        }
                    }
                }
            }

            return basvuruListesi;
        }

        public bool IsRaporAlreadyUploaded(string ogrenciNo, string program)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM tbl_StajRapor WHERE raporNo = @ogrenciNo AND raporProgram = @program";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                cmd.Parameters.AddWithValue("@program", program);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                return count > 0;
            }
        }

        public void InsertRapor(string ogrenciNo, string ad, string soyad, string donem, string program, string belge)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO tbl_StajRapor (raporNo, raporAd, raporSoyad, raporDonem, raporProgram, raporBelge) " +
                               "VALUES (@ogrenciNo, @ad, @soyad, @donem, @program, @belge)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                cmd.Parameters.AddWithValue("@ad", ad);
                cmd.Parameters.AddWithValue("@soyad", soyad);
                cmd.Parameters.AddWithValue("@donem", donem);
                cmd.Parameters.AddWithValue("@program", program);
                cmd.Parameters.AddWithValue("@belge", belge);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }


        public void DeleteStajOnay(string ogrenciNo, string program)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM tbl_StajOnay WHERE stajONo = @ogrenciNo AND stajOProgram = @program";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                cmd.Parameters.AddWithValue("@program", program);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }


        public DataRow GetStajOnayByOgrenciAndProgram(string ogrenciNo, string program)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM tbl_StajOnay WHERE stajONo = @ogrenciNo AND stajOProgram = @program";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ogrenciNo", ogrenciNo);
                cmd.Parameters.AddWithValue("@program", program);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]; // İlk kaydı döndür
                }
                return null; // Kayıt bulunamadı
            }
        }

        public DataRow GetDuyuruByStajProgram(string stajProgram)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Duyuru tablosundan, staj programına ait sdTeslim tarihini almak için sorgu
                    string query = "SELECT sdTeslim FROM tbl_Duyurular WHERE stajAdi = @StajProgram";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@StajProgram", stajProgram);

                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    // Eğer sonuç varsa, ilk satırı döndür
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]; // İlk satırdaki değeri döndürüyoruz
                    }
                    else
                    {
                        return null; // Eğer hiçbir sonuç yoksa null döndür
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return null; // Hata durumunda null döndür
                }
            }
        }

        public static List<StajOnayModel> Staj1OnaylariGetir()
        {
            List<StajOnayModel> onayList = new List<StajOnayModel>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT stajONo, stajOAd, stajOSoyad, stajODonem, stajOProgram FROM tbl_StajOnay WHERE stajOProgram = 'Staj 1'";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        StajOnayModel model = new StajOnayModel
                        {
                            OgrenciNo = reader["stajONo"].ToString(),
                            OgrenciAd = reader["stajOAd"].ToString(),
                            OgrenciSoyad = reader["stajOSoyad"].ToString(),
                            StajDonem = reader["stajODonem"].ToString(),
                            StajProgram = reader["stajOProgram"].ToString()
                        };
                        onayList.Add(model);
                    }
                    return onayList;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return new List<StajOnayModel>();
                }
            }
        }

        public static List <Staj2OnayModel> Staj2OnaylariGetir()
        {
            List<Staj2OnayModel> onayList = new List<Staj2OnayModel>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT stajONo, stajOAd, stajOSoyad, stajODonem, stajOProgram FROM tbl_StajOnay WHERE stajOProgram = 'Staj 2'";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Staj2OnayModel model = new Staj2OnayModel
                        {
                            OgrenciNo = reader["stajONo"].ToString(),
                            OgrenciAd = reader["stajOAd"].ToString(),
                            OgrenciSoyad = reader["stajOSoyad"].ToString(),
                            StajDonem = reader["stajODonem"].ToString(),
                            StajProgram = reader["stajOProgram"].ToString()
                        };
                        onayList.Add(model);
                    }
                    return onayList;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return new List<Staj2OnayModel>();
                }
            }
        }

        public static List<ImeOnayModel> ImeOnaylariGetir()
        {
            List<ImeOnayModel> onayList = new List<ImeOnayModel>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT stajONo, stajOAd, stajOSoyad, stajODonem, stajOProgram FROM tbl_StajOnay WHERE stajOProgram = 'IME'";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ImeOnayModel model = new ImeOnayModel
                        {
                            OgrenciNo = reader["stajONo"].ToString(),
                            OgrenciAd = reader["stajOAd"].ToString(),
                            OgrenciSoyad = reader["stajOSoyad"].ToString(),
                            StajDonem = reader["stajODonem"].ToString(),
                            StajProgram = reader["stajOProgram"].ToString()
                        };
                        onayList.Add(model);
                    }
                    return onayList;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    return new List<ImeOnayModel>();
                }
            }
        }

        public static List<RaporModel> GetirRaporlar(string program)
        {
            string query = "SELECT raporNo, raporAd, raporSoyad, raporDonem, raporBelge FROM tbl_StajRapor WHERE raporProgram = @Program";
            List<RaporModel> raporlar = new List<RaporModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Program", program);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                raporlar.Add(new RaporModel
                                {
                                    RaporNo = reader["raporNo"].ToString(),
                                    RaporAd = reader["raporAd"].ToString(),
                                    RaporSoyad = reader["raporSoyad"].ToString(),
                                    RaporDonem = reader["raporDonem"].ToString(),
                                    RaporBelge = reader["raporBelge"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return raporlar;
        }

        public static int GetirOnayBekleyenSayisi(string program)
        {
            string query = "SELECT COUNT(*) FROM tbl_StajBasvurusu WHERE stajBProgram = @Program";
            int count = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Program", program);
                        count = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return count;
        }

        public static int GetirOnaylananSayisi(string program)
        {
            string query = "SELECT COUNT(*) FROM tbl_StajOnay WHERE stajOProgram = @Program";
            int count = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Program", program);
                        count = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return count;
        }

        public List<string> MesajOgrenciler()
        {
            List<string> ogrenciler = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ogrNo, ogrAd, ogrSoyad FROM tbl_Ogrenciler";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ogrNo = reader["ogrNo"].ToString();
                        string ogrAd = reader["ogrAd"].ToString();
                        string ogrSoyad = reader["ogrSoyad"].ToString();
                        ogrenciler.Add($"{ogrNo} - {ogrAd} {ogrSoyad}");
                    }
                }
            }
            return ogrenciler;
        }
        public List<string> MesajHocalar()
        {
            List<string> mesajHocalar = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT hocaAd, hocaSoyad FROM tbl_Hocalar";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string hocaAd = reader["hocaAd"].ToString();
                        string hocaSoyad = reader["hocaSoyad"].ToString();
                        mesajHocalar.Add($"{hocaAd} {hocaSoyad}");
                    }
                }
            }
            return mesajHocalar;
        }
        public static void MesajGonder(string recipient, string messageContent)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO tbl_Mesajlar (mesajKimlik, mesajAd, mesajSoyad, mesajAlici, mesajContent) VALUES (@mesajKimlik, @mesajAd, @mesajSoyad, @mesajAlici, @mesajContent)";

                // SQL komutunu oluşturuyoruz
                SqlCommand cmd = new SqlCommand(query, conn);

                // Parametreleri ekliyoruz
                cmd.Parameters.AddWithValue("@mesajKimlik", System.Web.HttpContext.Current.Session["Hoca"]);
                cmd.Parameters.AddWithValue("@mesajAd", System.Web.HttpContext.Current.Session["HocaAd"]);
                cmd.Parameters.AddWithValue("@mesajSoyad", System.Web.HttpContext.Current.Session["HocaSoyad"]);

                // mesajAlici'yi düzenliyoruz, ilk boşluğa kadar alacağız
                string mesajAlici = recipient.Split(' ')[0]; // İlk boşluğa kadar olan kısmı alır
                cmd.Parameters.AddWithValue("@mesajAlici", mesajAlici);

                // Mesaj içeriğini ekliyoruz
                cmd.Parameters.AddWithValue("@mesajContent", messageContent);

                // Bağlantıyı açıyoruz ve komutu çalıştırıyoruz
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<MesajDetay> GelenKutusuMesajlari(string ogrenciId)
        {
            var mesajlar = new List<MesajDetay>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT m.mesajAd, m.mesajSoyad, m.mesajContent
                    FROM tbl_Mesajlar m
                    WHERE m.mesajAlici = @ogrenciId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ogrenciId", ogrenciId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    mesajlar.Add(new MesajDetay
                    {
                        Ad = reader["mesajAd"].ToString(),
                        Soyad = reader["mesajSoyad"].ToString(),
                        Content = reader["mesajContent"].ToString()
                    });
                }
            }

            return mesajlar;
        }

        public List<MesajDetay> GonderilenMesajlar(string ogrenciId)
        {
            var mesajlar = new List<MesajDetay>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT h.hocaAd, h.hocaSoyad, m.mesajContent
                    FROM tbl_Mesajlar m
                    JOIN tbl_Hocalar h ON h.hocaEposta = m.mesajAlici
                    WHERE m.mesajKimlik = @ogrenciId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ogrenciId", ogrenciId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    mesajlar.Add(new MesajDetay
                    {
                        Ad = reader["hocaAd"].ToString(),
                        Soyad = reader["hocaSoyad"].ToString(),
                        Content = reader["mesajContent"].ToString()
                    });
                }
            }

            return mesajlar;
        }

        public AdminMesaj GetAdminMesajlar(string hocaKimlik)
        {
            AdminMesaj adminMesajlar = new AdminMesaj
            {
                GelenKutusu = new List<MesajDetay2>(),
                GonderilenMesajlar = new List<MesajDetay2>()
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Gelen Mesajlar
                string gelenQuery = @"SELECT m.mesajKimlik, m.mesajAd, m.mesajSoyad, m.mesajContent
                                  FROM tbl_Mesajlar m
                                  WHERE m.mesajAlici = @HocaKimlik";

                using (SqlCommand cmd = new SqlCommand(gelenQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@HocaKimlik", hocaKimlik);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            adminMesajlar.GelenKutusu.Add(new MesajDetay2
                            {
                                Ad = reader["mesajAd"].ToString(),
                                Soyad = reader["mesajSoyad"].ToString(),
                                Content = reader["mesajContent"].ToString(),
                                EkBilgi = reader["mesajKimlik"].ToString()
                            });
                        }
                    }
                }

                // Gönderilen Mesajlar
                string gonderilenQuery = @"SELECT m.mesajAlici, o.ogrAd, o.ogrSoyad, m.mesajContent
                                       FROM tbl_Mesajlar m
                                       INNER JOIN tbl_Ogrenciler o ON o.ogrNo = m.mesajAlici
                                       WHERE m.mesajKimlik = @HocaKimlik";

                using (SqlCommand cmd = new SqlCommand(gonderilenQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@HocaKimlik", hocaKimlik);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            adminMesajlar.GonderilenMesajlar.Add(new MesajDetay2
                            {
                                Ad = reader["ogrAd"].ToString(),
                                Soyad = reader["ogrSoyad"].ToString(),
                                Content = reader["mesajContent"].ToString(),
                                EkBilgi = reader["mesajAlici"].ToString()
                            });
                        }
                    }
                }
            }

            return adminMesajlar;
        }

        public void MesajGonder2(string ogrenciNo, string ogrenciAd, string ogrenciSoyad, string recipient2, string messageContent)
        {
            // recipient2'yi boşluktan ayıralım
            var splitRecipient = recipient2.Split(' ');
            if (splitRecipient.Length < 2)
            {
                throw new ArgumentException("Alıcı adı ve soyadı eksik.");
            }
            string hocaAd = splitRecipient[0]; // hocanın adı
            string hocaSoyad = splitRecipient[1]; // hocanın soyadı

            string hocaEposta = string.Empty;

            // Hocanın e-posta adresini almak için tbl_Hocalar tablosuna sorgu
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT hocaEposta FROM tbl_Hocalar WHERE hocaAd = @HocaAd AND hocaSoyad = @HocaSoyad";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HocaAd", hocaAd);
                cmd.Parameters.AddWithValue("@HocaSoyad", hocaSoyad);

                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    hocaEposta = result.ToString(); // E-posta adresini al
                }
                else
                {
                    throw new ArgumentException("Hoca bulunamadı.");
                }
                conn.Close();
            }

            // Mesajı tbl_Mesajlar tablosuna kaydetme
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO tbl_Mesajlar (mesajKimlik, mesajAd, mesajSoyad, mesajAlici, mesajContent) " +
                               "VALUES (@MesajKimlik, @MesajAd, @MesajSoyad, @MesajAlici, @MesajContent)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MesajKimlik", ogrenciNo); // Öğrenci kimliği
                cmd.Parameters.AddWithValue("@MesajAd", ogrenciAd); // Öğrenci adı
                cmd.Parameters.AddWithValue("@MesajSoyad", ogrenciSoyad); // Öğrenci soyadı
                cmd.Parameters.AddWithValue("@MesajAlici", hocaEposta);  // Hocanın e-posta adresi
                cmd.Parameters.AddWithValue("@MesajContent", messageContent); // Mesaj içeriği

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

    }


}
