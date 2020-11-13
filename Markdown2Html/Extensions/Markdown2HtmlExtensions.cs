using Microsoft.JSInterop;
using System.Threading.Tasks;
using Markdown2Html.Services;
using Microsoft.Extensions.DependencyInjection;
using Markdown2Html.Data;

namespace Markdown2Html.Models
{
    public static class Markdown2HtmlExtensions
    {
        /// <summary>
        /// Add 2 nessecary services for Markdown2Html. IMarkDownService and ICodeTextService
        /// </summary>
        /// <param name="cssPath">Give a custom css otherwise you use the default one</param>
        /// <returns></returns>
        public static IServiceCollection AddMarkdown2Html(this IServiceCollection serviceCollection, string cssPath = null)
        {
            serviceCollection.AddScoped<IMarkDownService, MarkDownService>();
            serviceCollection.AddScoped<ICodeTextService, CodeTextService>();

            if (!string.IsNullOrEmpty(cssPath))
                Settings.CssPath = cssPath;

            return serviceCollection;
        }
    }
}
