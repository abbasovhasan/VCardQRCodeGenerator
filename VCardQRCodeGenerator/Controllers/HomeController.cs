using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using VCardQRGenerator.Controllers;
using QRCoder;
using System.Text;
using VCardQRCodeGenerator.Models;

namespace VCardQRGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexViewModel model)
        {
            string vcfContent = "BEGIN:VCARD\r\n" +
                    $"VERSION:3.0\r\n" +
                    $"FN:{model.VCard.FirstName} {model.VCard.SurName}\r\n" +
                    $"N:{model.VCard.SurName};{model.VCard.FirstName};;;\r\n" +
                    $"EMAIL;TYPE=INTERNET:{model.VCard.Email}\r\n" +
                    $"TEL;TYPE=WORK:{model.VCard.Phone}\r\n" +
                    $"ADR;TYPE=HOME;LABEL=\"{model.VCard.City}, {model.VCard.Country}\"\r\n" +

                    "END:VCARD\r\n";


            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrgen = new QRCodeGenerator();
                var qrcodedata = qrgen.CreateQrCode(vcfContent, QRCodeGenerator.ECCLevel.H);
                var qrcode = new Base64QRCode(qrcodedata);
                model.QrCode = qrcode.GetGraphic(20);
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}