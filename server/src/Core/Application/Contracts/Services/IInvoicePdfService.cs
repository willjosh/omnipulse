using Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

namespace Application.Contracts.Services;

/// <summary>
/// Generates invoice PDFs
/// </summary>
public interface IInvoicePdfService
{
    /// <summary>
    /// Generates a PDF invoice for a work order
    /// </summary>
    /// <param name="dto">The work order invoice data to generate the PDF from</param>
    /// <returns>PDF as byte array</returns>
    Task<byte[]> GenerateInvoicePdfAsync(GetWorkOrderInvoicePdfDTO dto);
}