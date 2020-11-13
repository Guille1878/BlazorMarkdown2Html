namespace Markdown2Html.Models
{

        public class CodeLanguage
        {
            public CodeLanguage()
            {
            }

            public CodeLanguage(CodeLanguages language, CodeStructures codeStructure = CodeStructures.None)
            {
                if (codeStructure == CodeStructures.None)
                {
                    switch (language)
                    {
                        case CodeLanguages.HTML:
                        case CodeLanguages.XML:
                            codeStructure = CodeStructures.Xml;
                            break;
                        case CodeLanguages.C:
                        case CodeLanguages.CSharp:
                            codeStructure = CodeStructures.C;
                            break;
                        case CodeLanguages.Blazor:
                        case CodeLanguages.Razor:
                            codeStructure = CodeStructures.razor;
                            break;
                    }
                }

                FoundSynonymous = true;
                CodeStructure = codeStructure;
                Language = language;
            }

            public void SetLanguage(CodeLanguages language, CodeStructures codeStructure = CodeStructures.None) 
            {
                FoundSynonymous = true;
                CodeStructure = codeStructure;
                Language = language;
            }

            public CodeLanguages Language { get; set; }
            public bool FoundSynonymous { get; set; }
            public CodeStructures CodeStructure { get; set; }
        }
}

