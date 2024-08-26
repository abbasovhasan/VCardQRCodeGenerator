using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QRCoder;
using System.Net.Http;
using VCardQRCodeGenerator.Models;
using VCardQRCodeGenerator.Services;
using System.Text;

namespace VCardQRCodeGenerator.Controllers;

public class VCardController : Controller
{
    private readonly RandomUserService _randomUserService;

    public VCardController()
    {
        _randomUserService = new RandomUserService();
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Generate(int count)
    {
        if (count <= 0)
        {
            ModelState.AddModelError("", "Lütfen 1 veya daha fazla sayıda vCard oluşturmak için geçerli bir sayı girin.");
            return View("Index");
        }

        var vCards = new List<VCard>();
        for (int i = 0; i < count; i++)
        {
            var vCard = await _randomUserService.GetRandomUserAsync();
            SaveVCardToFile(vCard);
            vCards.Add(vCard);
        }

        return View("Result", vCards);
    }

    private void SaveVCardToFile(VCard vCard)
    {
        var vCardContent = vCard.ToVCardFormat();
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "vCards");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(directoryPath, $"{vCard.Firstname}_{vCard.Surname}_{vCard.Id}.vcf");
        System.IO.File.WriteAllText(filePath, vCardContent);
    }

    public IActionResult GenerateQRCode(string vCardContent)
    {
        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrCodeData = qrGenerator.CreateQrCode(vCardContent, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            using (var qrCodeImage = qrCode.GetGraphic(20))
            {
                using (var ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }
    }
    public async Task<VCard> GetRandomUserAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("https://randomuser.me/api/");
            var data = JObject.Parse(response);
            var user = data["results"][0];

            return new VCard
            {
                Id = Guid.NewGuid(),
                Firstname = user["name"]["first"].ToString(),
                Surname = user["name"]["last"].ToString(),
                Email = user["email"].ToString(),
                Phone = user["phone"].ToString(),
                Country = user["location"]["country"].ToString(),
                City = user["location"]["city"].ToString()
            };
        }
        catch (Exception ex)
        {
            // Loglama işlemi yapabilirsiniz.
            throw new Exception("Random user verileri alınırken bir hata oluştu.", ex);
        }
    }

}
