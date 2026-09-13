using MiraAPI.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;

namespace TreeWallMod
{
	[RegisterCustomColors]
	public static class PlayerColors
	{
		public static CustomColor Evergreen { get; } = new("Evergreen",
			new Color32(39, 139, 26, byte.MaxValue),
			new Color32(18,  79, 11, byte.MaxValue))
		{
			ColorBrightness = CustomColorBrightness.Lighter
		};

		public static CustomColor OOOO { get; } = new("OOOOOO",
			new Color32(0, 0, 0, byte.MaxValue),
			new Color32(0, 0, 0, byte.MaxValue))
		{
			ColorBrightness = CustomColorBrightness.Darker
		};

		public static CustomColor Rainbow { get; } = new("RAINDWO?",
			new Color32((byte)(Time.time%255), (byte)(Time.time%255), (byte)(Time.time%255), byte.MaxValue),
			new Color32((byte)((Time.time+128)%255), (byte)((Time.time+128)%255), (byte)((Time.time+128)%255), byte.MaxValue))
		{
			ColorBrightness = CustomColorBrightness.Lighter
		};

    }
}
