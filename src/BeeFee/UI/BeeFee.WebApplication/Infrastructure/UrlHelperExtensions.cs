using System;
using BeeFee.ImageApp;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BeeFee.WebApplication.Infrastructure
{
	public static class UrlHelperExtensions
	{
		public static string GetImageUrl(this UrlHelper helper, string filename)
			=> String.Concat(BeeFeeWebAppSettings.Instance.ImagesUrl, "/", filename);

		public static string GetImageUrl(this UrlHelper helper, string filename, int width, int height)
			=> String.Concat(BeeFeeWebAppSettings.Instance.ImagesUrl, "/", new ImageSize(width, height).ToString(), "/", filename);
	}
}