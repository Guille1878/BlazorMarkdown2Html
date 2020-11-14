
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorMarkdown2Html.Data.StaticWordsData;

namespace BlazorMarkdown2Html.Models
{
    public sealed partial class ProgrammingKeyWords
    {
        private static Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>> wordLists;

        private readonly static string[] cSynonymous = { "c", "cpp", "c++" };
        private readonly static string[] cSharpSynonymous = { "cs", "csharp", "c#" }; 
        private readonly static string[] psSynonymous = { "sh", "ps", "powershell", "bash", "sudo" };
        private readonly static string[] razorSynonymous = { "razor", "blazor" };
        private readonly static string[] xmlSynonymous = { "xml", "xmla" };

        public static Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>> WordLists => 
            wordLists ?? new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();

        private static void ImportCSharpWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();
            wordLists.Add(CodeLanguages.CSharp, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, KeywordsCSharp } });
            wordLists[CodeLanguages.CSharp].Add(CodeWordTypes.Class, ClassesCSharp );
        }

        private static void ImportCWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();
            wordLists.Add(CodeLanguages.C, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, KeywordsC } });
        }

        private static void ImportRazorWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();
            wordLists.Add(CodeLanguages.Blazor, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, KeywordsRazor } });
        }

        private static void ImportHtmlWords()
        {
            wordLists??= new Dictionary<CodeLanguages, Dictionary<CodeWordTypes, IEnumerable<string>>>();
            wordLists.Add(CodeLanguages.HTML, new Dictionary<CodeWordTypes, IEnumerable<string>> { { CodeWordTypes.KeyWord, ElementsHtml } });
        }

        public static bool IsKeyword(string word, CodeLanguage language) => 
            WordLists[language.Language][CodeWordTypes.KeyWord].Contains(word);

        public static bool IsCommonClass(string word, CodeLanguage language)
        {

            if (!WordLists[language.Language].ContainsKey(CodeWordTypes.Class))
                return false;

            return WordLists[language.Language][CodeWordTypes.Class].Contains(word);
        }

        public static CodeLanguage GetLanguage(string language)
        {
            var result = new CodeLanguage();
            language = language.ToLower();

            if (cSharpSynonymous.Contains(language))
            {
                result.SetLanguage(CodeLanguages.CSharp, CodeStructures.C);

                if (!WordLists.ContainsKey(result.Language))
                    ImportCSharpWords();

                return result;
            }


            if (cSynonymous.Contains(language))
            {
                result.SetLanguage(CodeLanguages.C, CodeStructures.C);

                if (!WordLists.ContainsKey(CodeLanguages.C))
                    ImportCWords();

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
                    ImportHtmlWords();
                  
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
                    ImportRazorWords();

                if (!WordLists.ContainsKey(CodeLanguages.HTML))
                    ImportHtmlWords();

                if (!WordLists.ContainsKey(CodeLanguages.CSharp))
                    ImportCSharpWords();

                return result;
            }

            return result;
        }

    }
}
