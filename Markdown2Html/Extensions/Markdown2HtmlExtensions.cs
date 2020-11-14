using Microsoft.JSInterop;
using System.Threading.Tasks;
using BlazorMarkdown2Html.Services;
using Microsoft.Extensions.DependencyInjection;
using BlazorMarkdown2Html.Data;

namespace BlazorMarkdown2Html.Models
{
    public static class Markdown2HtmlExtensions
    {
        /// <summary>
        /// Add 2 nessecary services for Markdown2Html. IMarkDownService and ICodeTextService
        /// </summary>
        /// <param name="cssPath">Give a custom css otherwise you use the default one</param>
        /// <returns></returns>
        public static IServiceCollection AddBlazorMarkdown2Html(this IServiceCollection serviceCollection, string cssPath = null)
        {
            serviceCollection.AddScoped<IMarkDownService, MarkDownService>();
            serviceCollection.AddScoped<ICodeTextService, CodeTextService>();

            if (!string.IsNullOrEmpty(cssPath))
                Settings.CssPath = cssPath;

            return serviceCollection;
        }
    }
}
