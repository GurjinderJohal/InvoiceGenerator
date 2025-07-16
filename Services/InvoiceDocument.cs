namespace invoiceagent.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

/// <summary>
/// Represents an invoice with customer details and items
/// </summary>
public class InvoiceDocument : IDocument
{
    private readonly List<InvoiceItem> _items;
    private readonly string _invoiceNumber;
    private readonly DateTime _date;

/// <summary>
/// Constructor
/// </summary>
/// <param name="items">The list of items for invoice</param>
/// <param name="invoiceNumber">The invoice number</param>
/// <param name="date">Todays date that the invoice is being generated</param>
    public InvoiceDocument(List<InvoiceItem> items, string invoiceNumber, DateTime date)
    {
        _items = items;
        _invoiceNumber = invoiceNumber;
        _date = date;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(50);

            //add header with business infomation such as address
            page.Header().Row(row =>
            {
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text("Company Name").Bold().FontSize(20);
                    col.Item().Text("Address").Bold().FontSize(10);
                    col.Item().Text("City, Province").Bold().FontSize(10);
                    col.Item().Text("Postal Code/Zipcode").Bold().FontSize(10);
                    col.Item().Text("Email").Bold().FontSize(10);
                });
            });

            page.Content().Column(col =>
            {
                col.Item().Text("Bill To: " + _items[0].Customer).Bold().FontSize(20);
                col.Spacing(5);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Description
                        columns.RelativeColumn(1); // Total
                    });

                    // Header row
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Description").Bold();
                        header.Cell().Element(CellStyle).Text("Total").Bold();
                    });
                });

                // Data rows
                foreach (var item in _items)
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        table.Cell().Element(CellStyle).Text(item.Date.ToString("yyyy-MM-dd")).FontSize(10);
                        table.Cell().Element(CellStyle).Text(decimal.Parse(item.Price).ToString("C")).FontSize(10);
                    });

                }
                col.Item().AlignRight().Text($"Grand Total: {_items.Sum(i => decimal.Parse(i.Price)):C}").Bold();

            });

            //Footer adding when invoice is generated and who to make cheques to
            page.Footer().Row(row =>
            {
                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().Text($"Generated on {_date:yyyy-MM-dd}");
                    col.Item().Text("Make cheques payable to: Company");
                });

            });
        });
    }

/// <summary>
/// Styles the container
/// </summary>
/// <param name="container"></param>
/// <returns>The styled container</returns>
    private IContainer CellStyle(IContainer container)
    {
        return container
            .PaddingVertical(5)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2);
    }

}
