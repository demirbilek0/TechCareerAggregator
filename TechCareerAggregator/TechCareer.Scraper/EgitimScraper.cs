using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace TechCareer.Scraper
{
    public class EgitimScraper
    {
        private VeritabaniYonetici _dbYonetici;

        public EgitimScraper(VeritabaniYonetici dbYonetici)
        {
            _dbYonetici = dbYonetici;
        }

        public void UludagDuyurulariCek()
        {
            Console.WriteLine("Uludağ Üni. Bilgisayar Müh. duyuruları Selenium ile taranıyor...");

            string url = "https://www.uludag.edu.tr/bm/duyuru";

            ChromeOptions ayarlar = new ChromeOptions();
            ayarlar.AddArgument("--headless");
            ayarlar.AddArgument("--disable-gpu");
            ayarlar.AddArgument("--log-level=3");

            using (IWebDriver driver = new ChromeDriver(ayarlar))
            {
                try
                {
                    driver.Navigate().GoToUrl(url);
                    Console.WriteLine("Sayfa yüklendi. Geçmiş duyurular için sayfa aşağı kaydırılıyor...");

                    Thread.Sleep(2000); // İlk yüklemeyi bekle

                    // Sayfayı aşağı kaydırmak için JavaScript motorunu devreye sokuyoruz
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                    // Sayfanın en altına inip yeni verilerin yüklenmesi için bu işlemi 5 kez tekrarlıyoruz.
                    // (Daha eskiye gitmek istersen döngü sayısını artırabilirsin)
                    for (int i = 0; i < 5; i++)
                    {
                        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                        Thread.Sleep(1500); // Her kaydırmadan sonra verinin gelmesi için bekleme süresi
                    }

                    // Bu sefer sadece linkleri değil, tarihi de kontrol edebilmek için tüm tablo satırlarını (<tr>) alıyoruz
                    var satirlar = driver.FindElements(By.XPath("//table[@id='anyTable']/tbody/tr"));

                    if (satirlar.Count > 0)
                    {
                        Console.WriteLine($"Toplam {satirlar.Count} adet satır bulundu. 2026 yılına ait olanlar ayıklanıyor...");

                        foreach (var satir in satirlar)
                        {
                            // Satırın içindeki metin (Başlık ve Tarih kısımları) "2026" içermiyorsa, bu satırı direkt atla
                            if (!satir.Text.Contains("2026"))
                            {
                                continue;
                            }

                            try
                            {
                                // Sadece 2026 yılına ait olan bu satırın içindeki linki bul
                                var linkElementi = satir.FindElement(By.XPath(".//a"));

                                string baslik = linkElementi.Text.Trim();
                                string link = linkElementi.GetAttribute("href");

                                if (!string.IsNullOrEmpty(link) && baslik.Length > 5)
                                {
                                    if (!link.StartsWith("http"))
                                    {
                                        link = "https://www.uludag.edu.tr" + (link.StartsWith("/") ? "" : "/") + link;
                                    }

                                    _dbYonetici.IcerikEkle(baslik, "Bursa Uludağ Üniversitesi", "Eğitim/Duyuru", link);
                                }
                            }
                            catch
                            {
                                // Eğer satırın içinde link (<a> etiketi) yoksa programın çökmesini engellemek için sessizce geç
                                continue;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Duyuru bulunamadı. Tablo hala boş olabilir.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Selenium Bağlantı hatası: " + ex.Message);
                }
                finally
                {
                    driver.Quit();
                }
            }
        }
    }
}