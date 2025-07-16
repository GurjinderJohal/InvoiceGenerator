namespace invoiceagent.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Class for uploading the file that contains information to create the invoice
/// </summary>
public class FileUploadModel
{
    [Required]
    public IFormFile UploadedFile { get; set; }

}
