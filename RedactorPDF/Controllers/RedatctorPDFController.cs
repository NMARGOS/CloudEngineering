using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions; // Regex is for finding specific text patterns
using iText.Kernel.Pdf; // This is for PDF manipulation https://www.nuget.org/packages/itext7 
using iText.Layout; //  oR layouts in PDF
using iText.Layout.Element; // Elements for adding stuff to PDFs
using System.IO; // For working with memory and files

[ApiController] // This tells the system that this is an API

[Route("api/[controller]")] // auto set rthe route to RedactorPDFController



public class RedactorPDFController : ControllerBase
{


    [HTtpPost("redactEndpoint")]
    public IActionResult RedactText([FromBody] pdfRedactorRequest request)
    {

        if (request == null || string.IsNullOrEmpty(request.Text))
        {
            return BadRequest("No text was entered by the user")
        }
        else if (request.Text.Length <10) // there could be cases where the string is not empty but for some reason has very few chars
        {
            return BadRequest("Insert more than 10 characters");
        }


        string pdfRedactedText = request.Text; //take the original input


        //Start by checking hten names first, can use https://regex101.com/
        string nameRegex = @"\b[A-Z]*\b"; // use string.ToUpper() below since I only inlcuded upper case
        MatchCollection nameMatcher = Regex.Matches(pdfRedactedText.ToUpper(), nameRegex);
        for each (Match namematch in nameMatcher)
            {
            pdfRedactedText = pdfRedactedText.Replace(namematch.Value, "[REDACTED_NAME]");
        }

        //Continue by checking emails, can use https://regex101.com/
        // emails have a structure of string + @ + string + .com
        // will need to have a matching regex for this
        string emailRegex = @"\b[A-Za-z0-9]+@[A-Za-z0-9]+\.[A-Za-z0-9]\b"; //
        MatchCollection emailMatcher = Regex.Matches(pdfRedactedText, emailRegex);
        for each(Match emailmatch in emailMatcher)
            {
            pdfRedactedText = pdfRedactedText.Replace(emailmatch.Value, "[REDACTED_EMAIL]");
        }
        //Finally checking address, can use https://regex101.com/
        // Ill assume a US address which have a structure https://www.campussims.com/how-to-write-a-us-address/
        // recipient first and last name, street number apartment city state zip code
        // Ill focus on keeping the street number and street name example 698 Geller Street
        string addressRegex = @"\b\d{1,9}\s\w+(\s\w+)*\b"; //
        MatchCollection addressMatcher = Regex.Matches(pdfRedactedText, addressRegex);
        for each(Match addressmatch in addressMatcher)
            {
            pdfRedactedText = pdfRedactedText.Replace(addressmatch.Value, "[REDACTED_ADDRESS]");
        }
    }

}


public class pdfRedactorRequest
{
    public string Text { get; set; } // The text that will be redacted
}