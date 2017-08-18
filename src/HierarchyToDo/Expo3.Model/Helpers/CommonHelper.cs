using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SharpFuncExt;

namespace Expo3.Model.Helpers
{
    public class CommonHelper
    {
		private static Regex _emailValidator = new Regex(@"^(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

	    public static bool IsValidEmail(string email)
		    => email.NotNullOrDefault(_emailValidator.IsMatch);

    }
}
