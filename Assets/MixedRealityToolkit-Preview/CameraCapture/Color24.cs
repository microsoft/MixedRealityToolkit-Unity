using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraCapture
{
	public struct Color24 {
		public byte r;
		public byte g;
		public byte b;

		public Color24(byte red, byte blue, byte green)
		{
			r = red;
			b = blue;
			g = green;
		}
	}
}
