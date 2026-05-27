#if NETSTANDARD2_0
using System;

namespace Deveel.Math {
	internal static class Polyfills {
		internal static void ThrowIfNull(object? argument, string? paramName = null) {
			if (argument == null)
				throw new ArgumentNullException(paramName);
		}
	}
}

namespace System.Diagnostics.CodeAnalysis {
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class MaybeNullWhenAttribute : Attribute {
		public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
		public bool ReturnValue { get; }
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
	internal sealed class NotNullAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class DoesNotReturnAttribute : Attribute {
	}
}
#endif
