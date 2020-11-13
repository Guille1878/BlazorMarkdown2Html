using System.Threading.Tasks;

namespace Markdown2Html.Services
{
    public interface ICodeTextService
    {
        Task<string> GetCodeTextAsync(string text, string language = null);
    }
}