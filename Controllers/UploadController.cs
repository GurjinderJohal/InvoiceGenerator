using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using invoiceagent.Models;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DocumentFormat.OpenXml.Office.CustomUI;

/// <summary>
/// Controller class to upload the Excel file to generate the invoice
/// </summary>
public class UploadController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Handles the file upload, reads data from Excel, and genearates a PDF invoice on the uploaded file
    /// </summary>
    /// <param name="model">The uploaded Excel file containing the invoice data</param>
    /// <returns>Redirects to a view showing the results or error</returns>
    [HttpPost]
    public async Task<IActionResult> Index(FileUploadModel model)
    {
        if (model.UploadedFile != null && model.UploadedFile.Length > 0)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", model.UploadedFile.FileName);

            // Save the uploaded Excel file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.UploadedFile.CopyToAsync(stream);
            }

            var invoiceItems = new List<InvoiceItem>();

            // Read Excel file using ClosedXML
            using (var workbook = new XLWorkbook(filePath))
            {
                var getDate = workbook.Worksheet(1); //Report Setup Sheet
                int month = getDate.Cell(11, 2).GetValue<int>(); //Get month of the fmp uploaded
                int year = getDate.Cell(12, 2).GetValue<int>(); //get the year of the fmp uploaded

                var worksheet = workbook.Worksheet(7); // Paidout Entry Sheet
                var day = worksheet.Row(2); //get the header for the days

                List<IXLRow> clientList = new List<IXLRow>(); //store the list of rows of clients for the invoices

                //loop from cells 62-64 to get the list of rows of clients for 
                for (int i = 62; i < 65; i++)
                {
                    clientList.Add(worksheet.Row(i));
                }


                //array for the data in the row of cells to be stored in
                string[] client;

                //foreach client in the list create the invoice
                foreach (var cl in clientList)
                {
                    client = cl.CellsUsed().Select(cell => cell.GetValue<string>()).ToArray();
                    invoiceItems = new List<InvoiceItem>();
                    for (int i = 3; i < client.Length - 2; i += 2)//-2 length to avoid the excel total calculation
                    {
                        if (float.Parse(client[i]) > 0) //if total is not zero add to invoice item
                        {
                            var item = new InvoiceItem
                            {
                                InvoiceNo = client[1],
                                Customer = client[1],
                                Qty = client[i - 1],
                                Date = new DateTime(year, month, day.Cell(i).GetValue<int>()),
                                Price = client[i],
                            };

                            invoiceItems.Add(item);
                        }
                    }

                    var invoiceNumber = "INV-001";
                    var date = DateTime.Today;

                    var document = new InvoiceDocument(invoiceItems, invoiceNumber, date);
                    QuestPDF.Settings.License = LicenseType.Community; ///for the community license
                    document.GeneratePdf("/tmp/" + client[1].ToString() + " invoice.pdf"); // temp path invoice is saved to

                    Console.WriteLine("PDF saved to /tmp/" + client[1].ToString() + " invoice.pdf");
                }

                // For now, show total records in view
                ViewBag.Message = $"Successfully read {invoiceItems.Count} invoice items!";
            }
        }

        else
            ViewBag.Message = "Please select a file";

        return View();
    }
}
