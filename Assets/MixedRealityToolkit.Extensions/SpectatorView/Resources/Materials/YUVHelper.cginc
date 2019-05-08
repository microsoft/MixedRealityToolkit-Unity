// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#include "UnityCG.cginc"

// Target shader model 5 to use bit shifting.
#pragma target 5.0

fixed4 GetYUV(fixed4 col1, fixed4 col2)
{
    int r = (int)(col1.r * 255.0f);
    int g = (int)(col1.g * 255.0f);
    int b = (int)(col1.b * 255.0f);
    
    int r2 = (int)(col2.r * 255.0f);
    int g2 = (int)(col2.g * 255.0f);
    int b2 = (int)(col2.b * 255.0f);

    // Conversion requires > 8 bit precision.
    // https://msdn.microsoft.com/en-us/library/ms893078.aspx
    int y = (int)(((66 * r + 129 * g + 25 * b + 128) >> 8) + 16);
    int y2 = (int)(((66 * r2 + 129 * g2 + 25 * b2 + 128) >> 8) + 16);
    int u = (int)(((-38 * r - 74 * g + 112 * b + 128) >> 8) + 128);
    int v = (int)(((112 * r - 94 * g - 18 * b + 128) >> 8) + 128);

    y = clamp(y, 0, 255);
    y2 = clamp(y2, 0, 255);
    u = clamp(u, 0, 255);
    v = clamp(v, 0, 255);

    return fixed4(
        saturate((float)u / 255.0f),
        saturate((float)y / 255.0f),
        saturate((float)v / 255.0f),
        saturate((float)y2 / 255.0f));
}

fixed4 GetRGBA(fixed4 col1, int x)
{
    int u = (int)(col1.r * 255.0f);
    int y0 = (int)(col1.g * 255.0f);
    int v = (int)(col1.b * 255.0f);
    int y1 = (int)(col1.a * 255.0f);

    float r, g, b;

    // Conversion requires > 8 bit precision.
    // https://msdn.microsoft.com/en-us/library/ms893078.aspx
    int c0 = (int)(y0 - 16);
    int c1 = (int)(y1 - 16);
    int d = (int)(u - 128);
    int e = (int)(v - 128);

    int c = c0;

    if (x == 1)
    {
        c = c1;
    }

    r = (float)((298 * c + 409 * e + 128) >> 8);
    g = (float)((298 * c - 100 * d - 208 * e + 128) >> 8);
    b = (float)((298 * c + 516 * d + 128) >> 8);

	r = clamp(r, 0.0f, 255.0f) / 255.0f;
	g = clamp(g, 0.0f, 255.0f) / 255.0f;
	b = clamp(b, 0.0f, 255.0f) / 255.0f;
	
    return fixed4(r, g, b, 1);
}

fixed4 GetRGBAFromYUVPackedPixel(int packedPixel, int index)
{
    int u = (int)(packedPixel & 0xff);
    int y0 = (int)((packedPixel >> 8) & 0xff);
    int v = (int)((packedPixel >> 16) & 0xff);
    int y1 = (int)((packedPixel >> 24) & 0xff);

    int val = 0;
    if (index % 2 != 0)
    {
        val = 1;
    }

    fixed4 yuv = fixed4((float)u / 255.0f, (float)y0 / 255.0f, (float)v / 255.0f, (float)y1 / 255.0f);
    return GetRGBA(yuv, val);
}

fixed4 GetRGBAFromPackedPixel(uint packedPixel)
{
    int r = (int)(packedPixel & 0xff);
    int g = (int)((packedPixel >> 8) & 0xff);
    int b = (int)((packedPixel >> 16) & 0xff);
    int a = (int)((packedPixel >> 24) & 0xff);

    return fixed4((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, (float)a / 255.0f);
}

fixed4 GetYUVFromPackedPixel(int packedPixel)
{
    int u = (int)(packedPixel & 0xff);
    int y0 = (int)((packedPixel >> 8) & 0xff);
    int v = (int)((packedPixel >> 16) & 0xff);
    int y1 = (int)((packedPixel >> 24) & 0xff);

    return fixed4((float)u / 255.0f, (float)y0 / 255.0f, (float)v / 255.0f, (float)y1 / 255.0f);
}
