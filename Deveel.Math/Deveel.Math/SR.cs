using System;

namespace Deveel.Math {
	internal class SR {
		public static string GetString(string s) {
			s = s.Replace(".", "");
			return Messages.ResourceManager.GetString(s);
		}

		public static string GetString(string s, params object[] args) {
			string format = GetString(s);
			if (format == null || format.Length == 0)
				return format;

			format = String.Format(format, args);
			return format;
		}
	}
}