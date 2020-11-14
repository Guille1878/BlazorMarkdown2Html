using Microsoft.AspNetCore.Components;
using Microsoft.Toolkit.Parsers.Markdown;
using System.Threading.Tasks;

namespace BlazorMarkdown2Html.Services
{
    public interface IMarkDownService
    {
        Task<MarkdownDocument> GetMarkdown(string documentName);

        Task<MarkupString> GetMdAsHtmlAsync(string documentName);

        Task<string> GetMdAsStringAsync(string documentName);
    }
}
