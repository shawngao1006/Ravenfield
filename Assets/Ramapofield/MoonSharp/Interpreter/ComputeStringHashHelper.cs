using System;

namespace MoonSharp.Interpreter
{
	internal static class ComputeStringHashHelper
	{
		// FNV-1a 32-bit
		public static uint ComputeStringHash(string s)
		{
			if (s == null) return 0u;
			uint hash = 2166136261u;
			for (int i = 0; i < s.Length; i++)
			{
				hash ^= s[i];
				hash *= 16777619u;
			}
			return hash;
		}
	}
}
