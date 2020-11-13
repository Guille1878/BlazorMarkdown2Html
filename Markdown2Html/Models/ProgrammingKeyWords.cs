
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Markdown2Html.Models
{
    public sealed partial class ProgrammingKeyWords
    {
        private static Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>> wordLists;

        private readonly static string[] cSynonymous = { "c", "cpp", "c++" };
        private readonly static string[] cSharpSynonymous = { "cs", "csharp", "c#" }; 
        private readonly static string[] psSynonymous = { "sh", "ps", "powershell", "bash", "sudo" };
        private readonly static string[] razorSynonymous = { "razor", "blazor" };
        private readonly static string[] xmlSynonymous = { "xml", "xmla" };

        private readonly static string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\data\\";

        public static Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>> WordLists => 
            wordLists ?? new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();

        private static async Task ImportCSharpWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();
            using (var sr = new StreamReader($"{filePath}csharp-keywords.json"))
                wordLists.Add(CodeLanguages.CSharp, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, DeserializeJsonList(await sr.ReadToEndAsync()) } });

            using (var sr = new StreamReader($"{filePath}csharp-classes.json"))
                wordLists[CodeLanguages.CSharp].Add(CodeWordTypes.Class, DeserializeJsonList(await sr.ReadToEndAsync()));
        }

        private static async Task ImportCWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();

            using (var sr = new StreamReader($"{filePath}c-keywords.json"))
                wordLists.Add(CodeLanguages.C, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, DeserializeJsonList(await sr.ReadToEndAsync()) } });
        }

        private static async Task ImportRazorWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();

            using (var sr = new StreamReader($"{filePath}razor-keywords.json"))
                wordLists.Add(CodeLanguages.Blazor, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, DeserializeJsonList(await sr.ReadToEndAsync()) } });
        }

        private static async Task ImportHtmlWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();

            using (var sr = new StreamReader($"{filePath}html-elements.json"))
                wordLists.Add(CodeLanguages.HTML, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, DeserializeJsonList(await sr.ReadToEndAsync()) } });
        }

        public static bool IsKeyword(string word, CodeLanguage language) => 
            WordLists[language.Language][CodeWordTypes.KeyWord].Contains(word);

        public static bool IsCommonClass(string word, CodeLanguage language)
        {

            if (!WordLists[language.Language].ContainsKey(CodeWordTypes.Class))
                return false;

            return WordLists[language.Language][CodeWordTypes.Class].Contains(word);
        }

        public static async Task<CodeLanguage> GetLanguageAsync(string language)
        {
            var result = new CodeLanguage();
            language = language.ToLower();

            if (cSharpSynonymous.Contains(language))
            {
                result.SetLanguage(CodeLanguages.CSharp, CodeStructures.C);

                if (!WordLists.ContainsKey(result.Language))
                    await ImportCSharpWords();

                return result;
            }


            if (cSynonymous.Contains(language))
            {
                result.SetLanguage(CodeLanguages.C, CodeStructures.C);

                if (!WordLists.ContainsKey(CodeLanguages.C))
                    await ImportCWords();

                return result;
            }


            if (psSynonymous.Contains(language))
            {
                result.SetLanguage(CodeLanguages.PowerShell);
                return result;
            }

            if (xmlSynonymous.Contains(language))
            {
                result.SetLanguage(CodeLanguages.XML, CodeStructures.Xml);
                return result;
            }

            if (language == "html")
            {
                result.SetLanguage(CodeLanguages.HTML, CodeStructures.Xml);

                if (!WordLists.ContainsKey(CodeLanguages.HTML))
                    await ImportHtmlWords();
                  
                return result;
            }

            if (language == "css")
            {
                result.SetLanguage(CodeLanguages.CSS, CodeStructures.None);
                return result;
            }

            if (razorSynonymous.Contains(language))
            {
                result.SetLanguage(CodeLanguages.Blazor, CodeStructures.razor);

                if (!WordLists.ContainsKey(CodeLanguages.Blazor))
                    await ImportRazorWords();

                if (!WordLists.ContainsKey(CodeLanguages.HTML))
                    await ImportHtmlWords();

                if (!WordLists.ContainsKey(CodeLanguages.CSharp))
                    await ImportCSharpWords();

                return result;
            }

            return result;
        }

        private static string[] DeserializeJsonList(in string text)
        {
            var textTrimmed = RemoveWhitespace(text);

            return textTrimmed[(textTrimmed.IndexOf('\"') + 1)..textTrimmed.LastIndexOf('\"')].Split("\",\"");

            static string RemoveWhitespace(string input) => new string(input.Trim().ToCharArray()
                    .Where(c => !Char.IsWhiteSpace(c))
                    .ToArray());
        }
    }
}
