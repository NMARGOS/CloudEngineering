using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Input { get; set; } = string.Empty;

        public string? PDFpath { get; set; }
        public string? DownloadUrl { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Input))
            {
                ViewData["Error"] = "You haven't inputted text";
                return Page();
            }

            try
            {
                var client = new HttpClient();
                var requestBody = new { Text = Input };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://redactor_pdf:5000/api/RedactorPDF/redactEndpoint", content);

                if (response.IsSuccessStatusCode)
                {
                    var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "redacted.pdf");
                    System.IO.File.WriteAllBytes(pdfPath, pdfBytes);

                    PDFpath = pdfPath;
                    DownloadUrl = "/redacted.pdf";
                }
                else
                {
                    ViewData["Error"] = "Failed to connect to the redaction PDF service.";
                }
            }
            catch
            {
                ViewData["Error"] = "Cannot connect to the redaction PDF service.";
            }

            return Page();
        }
    }
}
