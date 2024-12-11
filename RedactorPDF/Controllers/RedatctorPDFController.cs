using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;
//removed itext could not get it to work


[ApiController]
[Route("api/[controller]")]
public class RedactorPDFController : ControllerBase
{
    [HttpPost("redactEndpoint")]
    public IActionResult RedactText([FromBody] PdfRedactorRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Text))
        {
            return BadRequest("No text was entered by the user.");
        }

        if (request.Text.Length < 10) // Ensure input has sufficient length
        {
            return BadRequest("Insert more than 10 characters.");
        }

        // if requst is valid proceed with getting the txt
        string pdfRedactedText = request.Text; //take original input

        //Start by checking hten names first, can use https://regex101.com/
        string nameRegex = @"\b[A-Z]+\b";
        MatchCollection nameMatcher = Regex.Matches(pdfRedactedText.ToUpper(), nameRegex);
        foreach (Match namematch in nameMatcher)
        {
            pdfRedactedText = pdfRedactedText.Replace(namematch.Value, "[REDACTED_NAME]");
        }

        //Continue by checking emails, can use https://regex101.com/
        // emails have a structure of string + @ + string + .com
        // will need to have a matching regex for this
        string emailRegex = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";
        MatchCollection emailMatcher = Regex.Matches(pdfRedactedText, emailRegex);
        foreach (Match emailmatch in emailMatcher)
        {
            pdfRedactedText = pdfRedactedText.Replace(emailmatch.Value, "[REDACTED_EMAIL]");
        }

        //Finally checking address, can use https://regex101.com/
        // Ill assume a US address which have a structure https://www.campussims.com/how-to-write-a-us-address/
        // recipient first and last name, street number apartment city state zip code
        // Ill focus on keeping the street number and street name example 698 Geller Street
        string addressRegex = @"\b\d{1,9}\s\w+(\s\w+)*\b";
        MatchCollection addressMatcher = Regex.Matches(pdfRedactedText, addressRegex);
        foreach (Match addressmatch in addressMatcher)
        {
            pdfRedactedText = pdfRedactedText.Replace(addressmatch.Value, "[REDACTED_ADDRESS]");
        }

        //time to creat pdf
        using (MemoryStream stream = new MemoryStream())
        {


            PdfDocument pdf = new PdfDocument();
            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            //a bit similar to ggplot2
            XFont font = new XFont("Arial", 12, XFontStyle.Regular);


            gfx.DrawString(pdfRedactedText, font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.TopLeft);



            pdf.Save(stream);


            return File(stream.ToArray(), "application/pdf", "RedactedDocument.pdf");
        }
    }
}

// Input model for the redaction request
public class PdfRedactorRequest
{
    public string Text { get; set; }
}
