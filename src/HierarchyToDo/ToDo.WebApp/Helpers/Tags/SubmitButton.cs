using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SharpFuncExt;

namespace ToDo.WebApp.Helpers.Tags
{
	[HtmlTargetElement(tag:"submit", TagStructure = TagStructure.WithoutEndTag)]
	public class SubmitButtonTagHelper : TagHelper
	{
		private readonly bool _isAjax;

		public SubmitButtonTagHelper(IHttpContextAccessor httpContextAccessor)
		{
			_isAjax = httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Requested-With") &&
					httpContextAccessor.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		[HtmlAttributeName("text")]
		public string Text { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output) =>
			_isAjax.If(output.SuppressOutput,
				() =>
				{
					output.TagName = "input";
					output.Attributes.Add("type", "submit");
					if (!String.IsNullOrWhiteSpace(Text))
						output.Attributes.Add("value", Text);
				});
	}
}