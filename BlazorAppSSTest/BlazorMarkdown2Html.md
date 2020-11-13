# BlazorMarkdown2Html

BlazorMarkdown2Html is a package to be used with Blazor and it displays a markdown file in html format directly in a Blazor component. 
The only think a developer need to do it is to add the path to the file e.g. "ReadMe.md" into its parameter documentName and the rest it is handled by the component. If you give the default css we handle dark and light mode heritated from the webbrowser.

For the block with code we are formatting follow languages:

- C and C++
- C#
- HTML
- XML
- Blazor and Razor

## Installation

- #### Install Package
  
    ```sh
    Install-Package BlazorMarkdown2Html
    ```

- #### Add in "Setup.cs"
  
    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddMarkdown2Html();
        ...
    }
    ```
    
    In case you would like to give an own custom css file from your project you can add it as a parameter in the setup as following:

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddMarkdown2Html("./css/markdown-own.css");
        ...
    }
    ```
    Later in this document we go trhow all the css-classes you need to match.

## Usage


Into your page or docuemnt in Blazor add the component **MarkdownDoc**
and in the parameter **documentName** put the name to your document and observe you do not need to add extension ".md".
E.g with "using":

```blazor
@using Markdown2Html

<MarkdownDoc documentName="MarkdownTest" />
```

or directly:

```blazor
<Markdown2Html.MarkdownDoc documentName="MarkdownTest" />
```

## Custom css

As we explained before to you to add the path to your own css file in the Setup.cs file. E.g:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddMarkdown2Html();
    ...
}
```

Furthermore you must match those css-classes to give an effect. You do not need to create each one if you are no using them.

- md-body
- md-bold
- md-italic
- md-list-point
- md-list-row
- md-quote-text
- md-point-img
- md-quote-img
- md-code
- md-code-text-comments
- md-code-text-ps
- md-code-text-keyword
- md-code-text-class
- md-code-text-interface
- md-code-text-text
- md-code-text-html-component
- md-code-text-html-element
- md-code-text-html-braces
- md-code-text-html-prop
- md-code-text-html-text
- md-code-text-xml-element
- md-code-text-xml-text
- md-code-text-razor-keyword
- md-code-text-razor-code-bg










