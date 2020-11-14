# Hello

Testing my **bold text** nice!

#### Tesing my italic 
> Remember that Azure allow external Cert only in Plans B or higher. F and D are not working with SSL. Even if D allow you to create or add a Custom Domain it does not allow you to have a Cert. You need B or higher.

## Guille is here

Sabrina is:

* Nice
* Beautiful
* Creay

### C# Test

``` csharp

	private int currentCount = 0;
	public string MyString { get; set; }

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
```


#### in my mds

``` csharp
	using Microsoft.AspNetCore.Mvc;
	namespace MySite.Controller 
	{
		[Route("*.well-known/acme-challenge/Bp_2131dd_kn7qQj4V1ek-GM5asaASdIyJa0_T-Ww_Dg")]
		[ApiController]
		public class LetsEncryptController : ControllerBase
		{
			[HttpGet]
			public string Verify()
			{
				return "Bp_2131dd_kn7qQj4V1ek-GM5asaASdIyJa0_T-Ww_Dg.w7Pssadf24564BHDEEHhmd-HZow-Lp64564eMg";
			}
		}
	}
```


### HTML Test

``` html
	<body>
		<app>
			<component type="typeof(App)" render-mode="ServerPrerendered" />
		</app>
		
		<div id="blazor-error-ui">
			<environment include="Staging,Production">
				An error has occurred. This application may no longer respond until reloaded.
			</environment>
			<environment include="Development">
				An unhandled exception has occurred. See browser dev tools for details.
			</environment>
			<a href="" class="reload">Reload</a>
			<a class="dismiss">🗙</a>
		</div>

		<script src="_framework/blazor.server.js"></script>
	</body>

```


### Blazor

``` blazor

	@page "/counter"

	<h1>Counter</h1>

	<p>Current count: @currentCount</p>

	<div>
        @if (mdHtml.Value == null)
        {
            <h3>Non Markdown document found</h3>
        }
        else
        {
            <div class="markdown-cointainer">
                @mdHtml
            </div> 
        }
	</div>

	<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

	@code {
		private int currentCount = 0;

		private void IncrementCount()
		{
			currentCount++;
		}

		[Parameter]
		public string documentName { get; set; }
	}

```

### Sh/Cmd Test

``` sh
	sudo apt-get install git
	git clone https://github.com/letsencrypt/letsencrypt
	cd letsencrypt
	./letsencrypt-auto certonly --manual
```