using EstecheAssemblies;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Markdown2Html.Services
{
    public partial class MarkDownService : IMarkDownService
    {
        private readonly IJSRuntime _javascript;
        private readonly ICodeTextService _codeTextService;

        public MarkDownService(IJSRuntime javascript, ICodeTextService codeTextService)
        {
            _javascript = javascript;
            _codeTextService = codeTextService;
        }

        private async void AddCss()
        {
            try
            {
                await _javascript.InvokeVoidAsync("blazorJsGetWindowsDimensions");
            } 
            catch
            {
            }
        }
        
        private int listNum = 1;
        
        public async Task<MarkdownDocument> GetMarkdown(string documentName)
        {

            using var sr = new StreamReader($"{documentName}.md");

            string md = await sr.ReadToEndAsync(); 
            MarkdownDocument document = new MarkdownDocument();
            document.Parse(md);

            return document;
        }


        public async Task<string> GetMdAsStringAsync(string documentName)
        {

            if (!documentName.Contains(".md"))
                documentName = $"{documentName}.md";

            using var sr = new StreamReader(documentName);

            string md = await sr.ReadToEndAsync();

            MarkdownDocument document = new MarkdownDocument();
            document.Parse(md);

            string html = "<div class='md-body'>";

            foreach (var block in document.Blocks)
                html += await GetBlockAsync(block);

            return html += "</div>";

        }

        public async Task<MarkupString> GetMdAsHtmlAsync(string documentName) => (MarkupString)(await GetMdAsStringAsync(documentName));

        private async Task<string> GetBlockAsync(MarkdownBlock block)
        {
            switch (block.Type)
            {
                case MarkdownBlockType.Header:
                    var header = (HeaderBlock)block;
                    return $"<h{header.HeaderLevel}>{GetMarkdownInlines(header.Inlines)}</h{header.HeaderLevel}>";

                case MarkdownBlockType.HorizontalRule:
                    return $"<hr/>";

                case MarkdownBlockType.Paragraph:
                    var paragraphBlock = (ParagraphBlock)block;
                    return $"<p>{GetMarkdownInlines(paragraphBlock.Inlines)}</p>";

                case MarkdownBlockType.Code:
                    var codeBlock = (CodeBlock)block;
                    return $"<div class='md-code'>{await _codeTextService.GetCodeTextAsync(codeBlock.Text, codeBlock.CodeLanguage)}</div>";

                case MarkdownBlockType.List:
                    var list = (ListBlock)block;
                    var listString = $"<div class='md-list'>";
                    foreach (var item in list.Items)
                    {
                        listString += $"<div class='md-list-row'>" +
                                    $"<div class='md-list-point'>{GetPointImg(list.Style)}</div>" +
                                    $"<div class='md-list-item'>";
                        foreach (var itemBlock in item.Blocks)
                        {
                            listString += $"<div>{await GetBlockAsync(itemBlock) }</div>";
                        }
                        listString += $"</div></div>";
                    }
                    listString += $"</div>";
                    listNum = 1;
                    return listString;

                case MarkdownBlockType.LinkReference:
                    var linkBlock = (LinkReferenceBlock)block;
                    return $"<a href='{linkBlock.Tooltip}'>).Text){linkBlock.Url}</a>";

                case MarkdownBlockType.Quote:
                    var quoteBlock = (QuoteBlock)block;
                    var result = "<div class='md-list-row'><div class='md-quote-img'> </div><div class='md-quote-text'>";
                    foreach (var itemBlock in quoteBlock.Blocks)
                        result += $"{await GetBlockAsync(itemBlock)}";
                    result += $"</div></div>";
                    return result;

                default:
                    return GetCustomBlock(block.ToString());

            }
        }

        private string GetPointImg(ListStyle style)
        {
            switch (style)
            {
                case ListStyle.Bulleted:
                    return "<div class='md-point-img'> </div>";
                case ListStyle.Numbered:
                    return $"<div class='md-point-num'>{listNum++}. </div>";
                default:
                    return "*";
            }
        }

        private string GetMarkdownInlines(IEnumerable<MarkdownInline> inlines)
        {
            var html = "";
            foreach (var inline in inlines)
                html += GetMarkdownInline(inline);
            return html;
        }

        private string GetMarkdownInline(MarkdownInline inline)
        {
            if (inline == null)
                return "";

            switch (inline.Type)
            {
                case MarkdownInlineType.TextRun:
                    return GetCustomBlock(inline.ToString());

                case MarkdownInlineType.MarkdownLink:
                    var linkLine = (MarkdownLinkInline)inline;
                    return $"<a href='{linkLine.Url}' target='_blank'>{GetMarkdownInlines(linkLine.Inlines)}</a>";

                case MarkdownInlineType.Italic:
                    var italiicLine = (ItalicTextInline)inline;
                    return $"<span class='md-italic'>{GetMarkdownInlines(italiicLine.Inlines)}</a>";

                case MarkdownInlineType.Bold:
                    var boldLine = (BoldTextInline)inline;
                    return $"<span class='md-bold'>{GetMarkdownInlines(boldLine.Inlines)}</a>";

                case MarkdownInlineType.Code:
                    var codeLine = (CodeInline)inline;
                    return $"<div class='md-code'>{GetCustomBlock(codeLine.Text)}</div>";

                default:
                    return GetCustomBlock(inline.ToString());
            }

        }

        private string GetCustomBlock(string line)
        {

            line = line.ToHtml();

            if (line.Contains("# "))
                if (line.StartsWith('#'))
                {
                    var level = line.IndexOf(' ');
                    var trimmedText = line.Replace("#", "");
                    line = $"<h{level}>{trimmedText}</h{level}>";
                }

            if (line.Contains("**"))
            {
                var resultLine = line;
                var firstBoldMark = resultLine.IndexOf("**");
                var secoundBoldMark = resultLine.Substring(firstBoldMark + 2).IndexOf("**");

                while (firstBoldMark != -1 && secoundBoldMark != -1)
                {
                    resultLine = $"{resultLine.Substring(0, firstBoldMark)}<span class='md-bold'>{resultLine.Substring(firstBoldMark + 2, secoundBoldMark)}</span>{resultLine.Substring(secoundBoldMark + 4)}";

                    firstBoldMark = resultLine.IndexOf("**");
                    secoundBoldMark = resultLine.Substring(firstBoldMark + 2).IndexOf("**");
                }

                line = resultLine;
            }

            return line;
        }

    }
}
