using Microsoft.AspNetCore.Components;
using Microsoft.Toolkit.Parsers.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Markdown2Html.Services
{
    public interface IMarkDownService
    {
        Task<MarkdownDocument> GetMarkdown(string documentName);

        Task<MarkupString> GetMdAsHtmlAsync(string documentName);

        Task<string> GetMdAsStringAsync(string documentName);
    }
}
