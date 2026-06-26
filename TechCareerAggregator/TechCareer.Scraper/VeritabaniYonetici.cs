using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace TechCareer.Scraper
{
    public class VeritabaniYonetici
    {
        // Veritabanı dosyasının adı. Çalıştığı klasöre otomatik oluşturulur.
        private readonly string baglantiCumlesi = "Data Source=StajVeritabanı.db";

        public void VeritabaniniHazirla()
        {
            using (var baglanti = new SqliteConnection(baglantiCumlesi))
            {
                baglanti.Open();

                // İçerikleri tutacağımız ana tablonun SQL sorgusu
                string tabloSorgusu = @"
                    CREATE TABLE IF NOT EXISTS Icerikler (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Baslik TEXT NOT NULL,
                        Kaynak TEXT NOT NULL,
                        Kategori TEXT NOT NULL,
                        Detay TEXT,
                        Link TEXT UNIQUE, -- Aynı ilanın tekrar eklenmesini önlemek için benzersiz yapıyoruz
                        Tarih TEXT,
                        OkunduMu INTEGER DEFAULT 0
                    );";

                using (var komut = new SqliteCommand(tabloSorgusu, baglanti))
                {
                    komut.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Veritabanı ve tablolar başarıyla hazırlandı.");
        }

        public void IcerikEkle(string baslik, string kaynak, string kategori, string link)
        {
            using (var baglanti = new SqliteConnection(baglantiCumlesi))
            {
                baglanti.Open();

                // OR IGNORE komutu, link daha önce eklendiyse hata vermeden geçmesini sağlar
                string ekleSorgusu = @"
            INSERT OR IGNORE INTO Icerikler (Baslik, Kaynak, Kategori, Link, Tarih)
            VALUES (@baslik, @kaynak, @kategori, @link, @tarih);";

                using (var komut = new SqliteCommand(ekleSorgusu, baglanti))
                {
                    // Parametreleri güvenli bir şekilde ekliyoruz (SQL Injection'ı önlemek için)
                    komut.Parameters.AddWithValue("@baslik", baslik);
                    komut.Parameters.AddWithValue("@kaynak", kaynak);
                    komut.Parameters.AddWithValue("@kategori", kategori);
                    komut.Parameters.AddWithValue("@link", link);
                    komut.Parameters.AddWithValue("@tarih", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                    int etkilenenSatir = komut.ExecuteNonQuery();

                    if (etkilenenSatir > 0)
                    {
                        Console.WriteLine($"+ Yeni İçerik Eklendi: {baslik}");
                    }
                }
            }
        }
    }
}