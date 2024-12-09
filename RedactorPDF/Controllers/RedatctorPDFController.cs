using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions; // Regex is for finding specific text patterns
using iText.Kernel.Pdf; // This is for PDF manipulation https://www.nuget.org/packages/itext7 
using iText.Layout; //  oR layouts in PDF
using iText.Layout.Element; // Elements for adding stuff to PDFs
using System.IO; // For working with memory and files

[ApiController] // This tells the system that this is an API

[Route("api/[controller]")]



public class RedactorPDFController : ControllerBase
{


    [HTtpPost("redactEndpoint")]
    public IActionResult RedactText([FromBody] pdfRedactorRequest request)
    {

        if re

    }

}


public class pdfRedactorRequest
{
    public string Text { get; set; } // The text that will be redacted
}