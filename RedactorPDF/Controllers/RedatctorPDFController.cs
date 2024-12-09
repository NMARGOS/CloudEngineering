using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions; // Using regex for sensitive info
using iText.Kernel.Pdf; // This is for PDF manipulation https://www.nuget.org/packages/itext7 
using iText.Layout; //  oR layouts in PDF
using iText.Layout.Element; // Elements for adding stuff to PDFs
using System.IO; // For working with memory and files

[ApiController] // This tells the system that this is an API

[Route("api/[controller]")] // auto set rthe route to RedactorPDFController



public class RedactorPDFController : ControllerBase
{


    [HttpPost("redactEndpoint")]
    public IActionResult RedactText([FromBody] pdfRedactorRequest request)
    {

        if (request == null || string.IsNullOrEmpty(request.Text))
        {
            return BadRequest("No text was entered by the user");
        }
        else if (request.Text.Length <10) // there could be cases where the string is not empty but for some reason has very few chars
        {
            return BadRequest("Insert more than 10 characters");
        }


        string pdfRedactedText = request.Text; //take the original input


        //Start by checking hten names first, can use https://regex101.com/
        string nameRegex = @"\b[A-Z]+\b"; // use string.ToUpper() below since I only inlcuded upper case
        MatchCollection nameMatcher = Regex.Matches(pdfRedactedText.ToUpper(), nameRegex);
        foreach (Match namematch in nameMatcher)
            {
            pdfRedactedText = pdfRedactedText.Replace(namematch.Value, "[REDACTED_NAME]");
        }

        //Continue by checking emails, can use https://regex101.com/
        // emails have a structure of string + @ + string + .com
        // will need to have a matching regex for this
        string emailRegex = @"\b[A-Za-z0-9]+@[A-Za-z0-9]+\.[A-Za-z0-9]\b"; //
        MatchCollection emailMatcher = Regex.Matches(pdfRedactedText, emailRegex);
        foreach(Match emailmatch in emailMatcher)
            {
            pdfRedactedText = pdfRedactedText.Replace(emailmatch.Value, "[REDACTED_EMAIL]");
        }
        //Finally checking address, can use https://regex101.com/
        // Ill assume a US address which have a structure https://www.campussims.com/how-to-write-a-us-address/
        // recipient first and last name, street number apartment city state zip code
        // Ill focus on keeping the street number and street name example 698 Geller Street
        string addressRegex = @"\b\d{1,9}\s\w+(\s\w+)*\b"; //
        MatchCollection addressMatcher = Regex.Matches(pdfRedactedText, addressRegex);
        foreach(Match addressmatch in addressMatcher)
            {
            pdfRedactedText = pdfRedactedText.Replace(addressmatch.Value, "[REDACTED_ADDRESS]");
        }

        //a file to hold pdfs
        byte[] pdfFile = null;

        try
        {
            using (MemoryStream mstream = new MemoryStream())
            {
            
                PdfWriter writer = new PdfWriter(mstream); // Initialize PDF writer
                PdfDocument pdfDocument = new PdfDocument(writer); // Create the PDF document object
                Document doc = new Document(pdfDocument); // Wrap the PDF document in a higher-level layout object

                // Add the redacted text as a paragraph in the PDF
                doc.Add(new Paragraph(pdfRedactedText));

                // Close the document to finalize the PDF
                doc.Close();

                // Convert the PDF data in memory to a byte array
                pdfFile = mstream.ToArray();
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error generating the pdf" + ex.Message+" " + ex.StackTrace); 
        }
        if (pdfFile.Length == 0 || pdfFile == null)
        {
            return StatusCode(500, "PDF is either null or its length is 0");
        }

        return File(pdfFile, "application/pdf", "RedactedDocument.pdf");
    }

}


public class pdfRedactorRequest
{
    public string Text { get; set; } // The text that will be redacted
}