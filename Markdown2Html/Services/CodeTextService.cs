using EstecheAssemblies;
using Markdown2Html.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Markdown2Html.Models.ProgrammingKeyWords;

namespace Markdown2Html.Services
{
    public class CodeTextService: ICodeTextService
    {
        private static char[] codeTextSeparators = new char[15] { ' ', ';', '<', '>', '(', ')', '.', '{', '}', '[', ']', '=', '\r', '\n', '\t' };
        private static char[] blazorRowOpenSeparators = new char[2] { '{', '(' };
        private static char[] blazorRowCloseSeparators = new char[2] { ')', '}' };
        private const string codeTabSpace = "&nbsp;&nbsp;&nbsp;";

        public async Task<string> GetCodeTextAsync(string text, string language = null)
        {
            if (language == null)
                return text;

            var lang = await GetLanguageAsync(language);

            if (lang.FoundSynonymous)
            {
                return lang.CodeStructure switch
                {
                    CodeStructures.C => GetCCodeText(text, lang),
                    CodeStructures.Xml => GetXmlCodeText(text, lang),
                    CodeStructures.razor => GetRazorCodeText(text, lang),
                    _ => GetPSCodeText(text),
                };
            }

            return text.ToHtml();

        }

        private string GetRazorCodeText(string text, CodeLanguage language = null)
        {
            language ??= new CodeLanguage(CodeLanguages.HTML, CodeStructures.Xml);

            short braces = 0;

            string newText = "",
                   currentWord = "",
                   lastWord = "";

            char currentLetter = '\n',
                lastLetter,
                comingLetter;

            bool isInHtml = true,
                 isInElement = false,
                 firstCodeRow = true;

            Elements elements = new Elements();
              

            CodeTextBuildParts buildingPart = CodeTextBuildParts.None;

            text = text.ToHtml();
             
            for (int index = 0; index < text.Length; index++)
            {
                lastLetter = currentLetter;
                currentLetter = text[index];
                comingLetter = text.Length > index + 1 ? text[index + 1] : new char();


                if (isInHtml)
                {

                    if (currentLetter == '\n')
                    {
                        newText += $"{currentWord}\n</span>";

                        if (buildingPart == CodeTextBuildParts.BlazorKeyRow)
                            buildingPart = CodeTextBuildParts.None;

                        currentWord = "";
                        continue;
                    }

                    if (currentLetter == '@')
                    {
                        newText += currentWord;

                        if (buildingPart != CodeTextBuildParts.Comment && comingLetter == '*')
                        {
                            newText += currentWord;
                            currentWord = "";
                            buildingPart = CodeTextBuildParts.Comment;
                            index++;
                            continue;
                        }

                        if (buildingPart == CodeTextBuildParts.Comment && lastLetter == '*')
                        {  // 
                            newText += $"<span class='md-code-text-razor-keyword'>@*</span><span class='md-code-text-comments'>{currentWord}</span><span class='md-code-text-razor-keyword'>*@</span>";
                            currentWord = "";
                            buildingPart = CodeTextBuildParts.None;
                            continue;
                        }
                        
                        if (text.Length > index + 6 && text.Substring(index, 7) == "@code {")
                        {
                            newText += "<span class='md-code-text-razor-keyword'>@code</span>&nbsp;<span class='md-code-text-razor-keyword'>{</span>";
                            index += 6;
                            isInHtml = false;
                            braces++;
                            currentWord = "";
                            continue;
                        }

                        if (blazorRowOpenSeparators.Contains(comingLetter))
                        {
                            newText += $"<span class='md-code-text-razor-keyword'>@{comingLetter}</span><span class='md-code-text-razor-code-bg'>";
                            index += 1;
                            isInHtml = false;
                            braces++;
                            currentWord = "";
                            buildingPart = CodeTextBuildParts.BlazorKeyRow;
                            continue;
                        }

                        newText += $"<span class='md-code-text-razor-keyword'>@</span><span class='md-code-text-razor-code-bg'>";
                        buildingPart = CodeTextBuildParts.BlazorKeyWord;
                        currentWord = "";
                        continue;
                    }

                    if (buildingPart == CodeTextBuildParts.BlazorKeyWord && currentLetter == ' ' && !string.IsNullOrEmpty(currentWord.Replace(" &nbsp;", "").Trim()))
                    {

                        newText += (ProgrammingKeyWords.IsKeyword(currentWord, language)) ?
                            $"<span class='md-code-text-razor-keyword'>{currentWord}</span>&nbsp;<span class='md-code-text-razor-code-bg'>" :
                            $"<span class='md-code-text-keyword'>{currentWord}</span>&nbsp;";
                        buildingPart = CodeTextBuildParts.BlazorKeyRow;
                        currentWord = "";
                        continue;
                    }


                    if (buildingPart == CodeTextBuildParts.ElementPropTitle && codeTextSeparators.Contains(currentLetter))
                    {
                        newText += "</span>";
                        buildingPart = CodeTextBuildParts.ElementProp;
                    }

                    //if (buildingPart == CodeTextBuildParts.BlazorKeyRow && codeTextSeparators.Contains(currentLetter))
                    //{
                    //    var candidateInterface = currentWord.Trim();
                    //    newText += (candidateInterface.Length > 2 && candidateInterface[0] == 'I' && char.IsUpper(candidateInterface[1])) ?
                    //        $"<span class='md-code-text-interface md-code-text-razor-code-bg'>{currentWord}</span>{currentLetter}" :
                    //        $"{currentWord}{currentLetter}";

                    //    currentWord = "";
                    //    continue;

                    //}

                    if (currentLetter == '&')
                    {
                        if (text.Length > index + 5
                            && text.Substring(index, 6) == "&quot;")
                        {
                            buildingPart = buildingPart == CodeTextBuildParts.ElementPropValue 
                                ? CodeTextBuildParts.Text 
                                : CodeTextBuildParts.ElementProp;
                            newText += buildingPart == CodeTextBuildParts.Text 
                                ? $"<span class='md-code-text-{(language.Language == CodeLanguages.XML ? "xml" : "html")}-text'>&quot;" 
                                : $"{currentWord}&quot;";
                            index += 5;
                            currentWord = "";
                            continue;
                        }

                        if (buildingPart != CodeTextBuildParts.Text && buildingPart != CodeTextBuildParts.ElementPropValue)
                        {

                            if (text.Length > index + 3 && text.Substring(index, 4) == "&lt;")
                            {
                                newText += "</span>";

                                newText += currentWord;
                                if (text.Length > index + 1 && text[index + 4] == '/')
                                {
                                    newText += $"<span class='md-code-text-html-braces'>&lt;/</span>";
                                    index++;
                                    elements.CloseIfCurrent(currentWord);
                                }
                                else
                                {
                                    newText += "<span class='md-code-text-html-braces'>&lt;</span>";
                                }
                                currentWord = "";
                                isInElement = true;
                                buildingPart = CodeTextBuildParts.Element;
                                index += 3;
                                continue;

                            }

                            if (text.Length > index + 3 && text.Substring(index, 4) == "&gt;")
                            {
                                newText += "</span>";

                                if (isInElement && lastLetter == '/')
                                {
                                    newText += "<span class='md-code-text-html-braces'>/&gt;</span>";
                                    buildingPart = CodeTextBuildParts.None;
                                    elements.Close();
                                }
                                else if (buildingPart == CodeTextBuildParts.ElementPropTitle)
                                {
                                    elements.CloseIfCurrent(currentWord);
                                    newText += $"{currentWord}<span class='md-code-text-html-braces'>&gt;</span>";
                                    buildingPart = CodeTextBuildParts.None;
                                    currentWord = "";
                                }
                                else if (currentWord == elements.Current)
                                {
                                    elements.Close();
                                    newText += $"{currentWord}<span class='md-code-text-html-braces'>/&gt;</span>";
                                    buildingPart = CodeTextBuildParts.None;
                                    currentWord = "";
                                }
                                else
                                {                                    
                                    newText += (ProgrammingKeyWords.IsKeyword(currentWord, new CodeLanguage(CodeLanguages.HTML)) ? $"<span class='md-code-text-html-element'>{currentWord}</span>" : $"<span class='md-code-text-html-component'>{currentWord}</span>") + "<span class='md-code-text-html-braces'>&gt;</span>";

                                    if (new string[] { "html", "meta" }.Contains(currentWord.Trim()))
                                    {
                                        buildingPart = CodeTextBuildParts.None;
                                        elements.Close();  
                                    }
                                    else
                                        buildingPart = CodeTextBuildParts.ElementProp; 
                                    currentWord = "";
                                }
                                index += 3;
                                isInElement = false;
                                continue;
                            }
                        }
                    }

                    if (buildingPart == CodeTextBuildParts.ElementProp && currentLetter != ' ' && lastLetter == ' ')
                    {
                        buildingPart = CodeTextBuildParts.ElementPropTitle;
                        newText += $"<span class='md-code-text-html-prop'>"; 
                    }

                    if (buildingPart == CodeTextBuildParts.ElementPropTitle && currentLetter == '=')
                    {
                        newText += $"{currentWord}</span>=";
                        buildingPart = CodeTextBuildParts.ElementPropValue;
                        currentWord = "";
                        continue;
                    }

                    if (buildingPart == CodeTextBuildParts.Element && currentLetter == ' ')
                    {
                        newText += ProgrammingKeyWords.IsKeyword(currentWord, new CodeLanguage(CodeLanguages.HTML)) ? $"<span class='md-code-text-html-element'>{currentWord}</span> " : $"<span class='md-code-text-html-component'>{currentWord}</span> ";
                        buildingPart = CodeTextBuildParts.ElementProp;
                        currentWord = "";
                        continue;
                    }

                    if (blazorRowOpenSeparators.Contains(currentLetter) || blazorRowCloseSeparators.Contains(currentLetter))
                    {
                        newText += $"{currentWord}<span class='md-code-text-razor-code-bg'>{currentLetter}</span>";
                        currentWord = "";
                        continue;
                    }
                }
                else //then in C# Code (not in html)
                {

                    if (buildingPart == CodeTextBuildParts.Comment)
                    {
                        if (currentLetter == '\n')
                        {
                            newText += $"{currentWord}</span>";
                            currentWord = "";
                            buildingPart = CodeTextBuildParts.None;
                        }
                        currentWord += currentLetter;
                        continue;
                    }

                    if (currentLetter == '\n')
                    {
                        newText += currentWord;
                        if (!firstCodeRow)
                            newText += $"{currentWord}</span>";

                        firstCodeRow = false;

                        newText += currentLetter;
                        index++;

                        var isNextSpace = true;
                        do
                        {
                            var vad = text[index];
                            if (text[index] == ' ')
                            {
                                index++;
                                newText += ' ';
                            }
                            else if (text[index] == '&' && text.Length > index + 5 && text[index..(index + 6)] == "&nbsp;")
                            {
                                index += 6;
                                newText += "&nbsp;";
                            }
                            else if (text[index] == '<' && text.Length > index + 5 && text[index..(index + 6)] == "<br>\r\n")
                            {
                                index += 6;
                                newText += "<br>\r\n";
                            }
                            else
                                isNextSpace = false;

                        }
                        while (isNextSpace);

                        newText += $"<span class='md-code-text-razor-code-bg'>";
                        currentLetter = text[index];

                    }


                    if (buildingPart == CodeTextBuildParts.BlazorKeyRow)
                    {

                        if (blazorRowOpenSeparators.Contains(currentLetter))
                            braces++;

                        if (blazorRowCloseSeparators.Contains(currentLetter))
                            braces--;

                        if (braces == 0)
                        {
                            newText += $"<span class='md-code-text-razor-keyword'>{currentLetter}</span>";
                            currentWord = "";
                            continue;
                        }
                    }

                    if (currentLetter == '/')
                        if (text.Length > index + 1 && text[index + 1] == '/')
                        {
                            buildingPart = CodeTextBuildParts.Comment;
                            newText += "<span class='md-code-text-comments'>//";
                            index++;
                            continue;
                        }

                    if (currentLetter == '&')
                        if (text.Length > index + 5 && text.Substring(index, 6) == "&quot;")
                        {
                            buildingPart = buildingPart == CodeTextBuildParts.Text ? CodeTextBuildParts.None : CodeTextBuildParts.Text;
                            newText += buildingPart == CodeTextBuildParts.Text ? "<span class='md-code-text-text'>&quot;" : $"{currentWord}&quot;</span>";
                            index += 5;
                            currentWord = "";
                            continue;
                        }

                    if (currentLetter == '{')
                        braces++;
                    
                    if (currentLetter == '}')
                    {
                        braces--;
                        if (braces == 0)
                        {
                            newText += $"{currentWord}<span class='md-code-text-razor-keyword'>}}</span></span>";
                            currentWord = "";
                            continue;
                        }
                    }

                    if (buildingPart == CodeTextBuildParts.Text)
                    {
                        currentWord += currentLetter;
                        continue;
                    }

                    if (codeTextSeparators.Contains(currentLetter))
                    {
                        if (currentWord == "")
                        {
                            newText += $"{currentWord}{currentLetter}";
                            continue;
                        }


                        if (ProgrammingKeyWords.IsKeyword(currentWord, new CodeLanguage(CodeLanguages.CSharp)))
                        {
                            newText += $"<span class='md-code-text-razor-code-keyword'>{currentWord}</span>{currentLetter}";
                        }
                        else if (currentWord.Length > 2 && char.IsUpper(currentWord[1]) && currentWord[0] == 'I')
                        {
                            newText += $"<span class='md-code-text-interface'>{currentWord}</span>{currentLetter}";
                        }
                        else if (char.IsUpper(currentWord[0]) &&
                                    (ProgrammingKeyWords.IsCommonClass(currentWord, language) ||
                                     lastWord == "new" ||
                                     lastWord == "class" ||
                                     (lastLetter == '(' && currentLetter == ')') ||
                                     (lastLetter == '<' && currentLetter == '>') ||
                                     lastLetter == '\n'))
                        {
                            newText += $"<span class='md-code-text-class'>{currentWord}</span>{currentLetter}";
                        }
                        else
                        {
                            newText += $"{currentWord}{currentLetter}";
                        }
                        currentWord = "";
                        continue;
                    }
                }

                currentWord += currentLetter;
            }

            newText += currentWord;

            return newText;
        }

        private string GetPSCodeText(in string text)
        {
            string newText = "";
            foreach (var line in text.ToHtml().Split('\n'))
            {
                var firstWord = line.Split(' ').FirstOrDefault(w => w.Trim().Replace(" ", "").Replace("&nbsp;", "") != "");
                
                if (string.IsNullOrEmpty(firstWord)) 
                    continue;

                newText += $"<span class='md-code-text-ps'>{line.Insert(line.IndexOf(firstWord) + firstWord.Length, "</span>")}\n";
            }
            return newText.Remove(newText.Length - 1);
        }

        private string GetXmlCodeText(string text, CodeLanguage language = null)
        {
            language ??= new CodeLanguage(CodeLanguages.HTML, CodeStructures.Xml);

            var tabs = 0;
            string newText = "",
                   currentWord = "";

            char currentLetter = '\n',
                 lastLetter;

            CodeTextBuildParts buildingPart = CodeTextBuildParts.None;
            bool isInElement = false;

            text = text.ToHtml();

            for (int index = 0; index < text.Length; index++)
            {
                lastLetter = currentLetter;
                currentLetter = text[index];

                if (currentLetter == '\n' && tabs > 0)
                {
                    newText += $"\n{string.Concat(Enumerable.Repeat(codeTabSpace, tabs))}";
                    continue;
                }

                if (currentLetter == '&')
                {
                    if (text.Length > index + 5
                        && text.Substring(index, 6) == "&quot;")
                    {
                        buildingPart = buildingPart == CodeTextBuildParts.ElementPropValue ? CodeTextBuildParts.Text : CodeTextBuildParts.ElementProp;
                        newText += buildingPart == CodeTextBuildParts.Text ? $"<span class='md-code-text-{(language.Language == CodeLanguages.XML ? "xml" : "html")}-text'>&quot;" : $"{currentWord}&quot;</span>";
                        index += 5;
                        currentWord = "";
                        continue;
                    }

                    if (buildingPart != CodeTextBuildParts.Text && buildingPart != CodeTextBuildParts.ElementPropValue)
                    {

                        if (text.Length > index + 3 && text.Substring(index, 4) == "&lt;")
                        {
                            newText += currentWord;
                            if (text.Length > index + 1 && text[index + 4] == '/')
                            {
                                newText += "<span class='md-code-text-html-braces'>&lt;/</span>";
                                index++;
                                tabs--;
                            }
                            else
                            {
                                newText += "<span class='md-code-text-html-braces'>&lt;</span>";
                            }
                            currentWord = "";
                            isInElement = true;
                            buildingPart = CodeTextBuildParts.Element;
                            index += 3;
                            continue;

                        }

                        if (text.Length > index + 3 && text.Substring(index, 4) == "&gt;")
                        {
                            if (isInElement && lastLetter == '/')
                            {
                                newText += "<span class='md-code-text-html-braces'>/&gt;</span>";
                                buildingPart = CodeTextBuildParts.None;
                                tabs--;
                            }
                            else if (buildingPart == CodeTextBuildParts.ElementPropTitle)
                            {
                                tabs--;
                                newText += $"{currentWord}</span><span class='md-code-text-html-braces'>/&gt;</span>";
                                buildingPart = CodeTextBuildParts.None;
                            }
                            else
                            {
                                newText += (language.Language == CodeLanguages.XML) ? 
                                    $"<span class='md-code-text-xml-element'>{currentWord}</span><span class='md-code-text-html-braces'>&gt;</span>" :
                                    (ProgrammingKeyWords.IsKeyword(currentWord, language) ? $"<span class='md-code-text-html-element'>{currentWord}</span>" : $"<span class='md-code-text-html-component'>{currentWord}</span>") + "<span class='md-code-text-html-braces'>&gt;</span>";

                                if (new string[] { "html", "meta" }.Contains(currentWord))
                                {
                                    buildingPart = CodeTextBuildParts.None;
                                    tabs--;
                                }
                                else
                                    buildingPart = CodeTextBuildParts.ElementProp;
                            }
                            currentWord = "";
                            index += 3;
                            isInElement = false;
                            continue;
                        }
                    }
                }

                if (buildingPart == CodeTextBuildParts.ElementProp && currentLetter != ' ' && lastLetter == ' ')
                {
                    buildingPart = CodeTextBuildParts.ElementPropTitle;
                    newText += $"<span class='md-code-text-html-prop'>";

                }


                if (buildingPart == CodeTextBuildParts.ElementPropTitle && currentLetter == '=')
                {
                    newText += $"{currentWord}</span>=";
                    buildingPart = CodeTextBuildParts.ElementPropValue;
                    currentWord = "";
                    continue;
                }

                if (buildingPart == CodeTextBuildParts.Element && currentLetter == ' ')
                {
                    if (language.Language == CodeLanguages.XML)
                        newText += $"<span class='md-code-text-xml-element'>{currentWord}</span> ";
                    else
                        newText += ProgrammingKeyWords.IsKeyword(currentWord, language) ? $"<span class='md-code-text-html-element'>{currentWord}</span> " : $"<span class='md-code-text-html-component'>{currentWord}</span> ";
                    buildingPart = CodeTextBuildParts.ElementProp;
                    currentWord = "";
                    continue;
                }

                currentWord += currentLetter;
            }

            newText += currentWord;

            return newText;
        }

        private string GetCCodeText(string text, CodeLanguage language)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var braces = ((text[0] == ' ') || (text.Length > 5 && text[0..6] == "&nbsp;")) ? 1 : 0;

            string newText = "",
                   currentWord = "",
                   lastWord = "";

            char currentLetter,
                 lastSeparator = '\n';

            CodeTextBuildParts buildingPart = CodeTextBuildParts.None;

            text = text.ToHtml();

            for (int index = 0; index < text.Length; index++)
            {
                currentLetter = text[index];

                if (buildingPart == CodeTextBuildParts.Comment)
                {
                    if (currentLetter == '\n')
                    {
                        newText += $"{currentWord}</span>";
                        currentWord = "";
                        lastSeparator = currentLetter;
                        buildingPart = CodeTextBuildParts.None;
                    }
                    currentWord += currentLetter;
                    continue;
                }

                if (currentLetter == '/')
                    if (text.Length > index + 1 && text[index + 1] == '/')
                    {
                        buildingPart = CodeTextBuildParts.Comment;
                        newText += "<span class='md-code-text-comments'>//";
                        index++;
                        continue;
                    }

                if (currentLetter == '&')
                    if (text.Length > index + 5 && text.Substring(index, 6) == "&quot;")
                    {
                        buildingPart = buildingPart == CodeTextBuildParts.None ? CodeTextBuildParts.Text : CodeTextBuildParts.None;
                        newText += buildingPart == CodeTextBuildParts.Text ? "<span class='md-code-text-text'>&quot;" : $"{currentWord}&quot;</span>";
                        index += 5;
                        currentWord = "";
                        continue;
                    }


                if (buildingPart == CodeTextBuildParts.Text)
                {
                    currentWord += currentLetter;
                    continue;
                }

                if (codeTextSeparators.Contains(currentLetter))
                 {
                    if (currentLetter == '{')
                        braces++;

                    if (currentLetter == '}')
                        braces--;

                    if (currentWord == "")
                    {
                        newText += $"{currentWord}{currentLetter}";
                        lastSeparator = currentLetter;
                        continue;
                    }


                    if (ProgrammingKeyWords.IsKeyword(currentWord, language))//if keyWord
                        newText += $"<span class='md-code-text-keyword'>{currentWord}</span>{currentLetter}";
                    else if (currentWord.Length > 2 && char.IsUpper(currentWord[1]) && currentWord[0] == 'I')//if interface
                        newText += $"<span class='md-code-text-interface'>{currentWord}</span>{currentLetter}";
                    else if (char.IsUpper(currentWord[0]) && //if class
                                (ProgrammingKeyWords.IsCommonClass(currentWord, language) ||
                                 lastWord == "new" ||
                                 lastWord == "class" ||
                                 (lastSeparator == '(' && currentLetter == ')') ||
                                 (lastSeparator == '<' && currentLetter == '>') ||
                                 lastSeparator == '\n'))
                        newText += $"<span class='md-code-text-class'>{currentWord}</span>{currentLetter}";
                    else
                        newText += $"{currentWord}{currentLetter}";

                    lastWord = currentWord;
                    currentWord = "";
                    lastSeparator = currentLetter;
                    continue;
                }
                currentWord += currentLetter;
            }

            while (newText.Contains("&nbsp;&nbsp;&nbsp; &nbsp;"))
                newText = newText.Replace("&nbsp;&nbsp;&nbsp; &nbsp;", "&nbsp;&nbsp;&nbsp;");

            return newText;

        }


    }
}

