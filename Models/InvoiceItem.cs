namespace invoiceagent.Models;

/// <summary>
/// Represents items for an invoice
/// </summary>
public class InvoiceItem
{
    public string InvoiceNo { get; set; }
    public string Qty { get; set; }
    public DateTime Date { get; set; }
    public string Customer { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }

}
