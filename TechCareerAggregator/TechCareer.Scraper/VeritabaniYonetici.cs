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
    }
}