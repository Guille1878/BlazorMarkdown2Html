#### Blazor

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