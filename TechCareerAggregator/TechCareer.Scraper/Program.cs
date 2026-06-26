using System;
using TechCareer.Scraper; // VeritabaniYonetici sınıfımızın bulunduğu namespace

Console.WriteLine("--- TechCareer Scraper Motoru Başlatıldı ---");

// Veritabanı yöneticisini çağırıp tabloyu oluşturuyoruz
VeritabaniYonetici dbYonetici = new VeritabaniYonetici();
dbYonetici.VeritabaniniHazirla();

Console.WriteLine("İlk aşama tamamlandı. Kapatmak için bir tuşa basınız...");
Console.ReadKey();