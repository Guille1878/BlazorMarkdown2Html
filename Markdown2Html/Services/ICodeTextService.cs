using System.Threading.Tasks;

namespace BlazorMarkdown2Html.Services
{
    public interface ICodeTextService
    {
        string GetCodeText(string text, string language = null);
    }
}