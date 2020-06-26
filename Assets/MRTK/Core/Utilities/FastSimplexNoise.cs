// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A conglomeration of open-source simplex libraries in C# with an emphasis on performance
    /// </summary>
    public class FastSimplexNoise
    {
        private const double STRETCH__2_D = -1.0 / 4.73205;
        private const double STRETCH__3_D = -1.0 / 6.0;
        private const double STRETCH__4_D = -1.0 / 7.23607;
        private const double SQUISH__2_D = 1.0 / 2.73205;
        private const double SQUISH__3_D = 1.0 / 3.0;
        private const double SQUISH__4_D = 1.0 / 7.23607;
        private const double NORM__2_D = 1.0 / 47.0;
        private const double NORM__3_D = 1.0 / 103.0;
        private const double NORM__4_D = 1.0 / 30.0;

        private const long SEEDVAL_1 = 6364136223846793005L;
        private const long SEEDVAL_2 = 1442695040888963407L;

        private readonly byte[] perm;
        private readonly byte[] perm2D;
        private readonly byte[] perm3D;
        private readonly byte[] perm4D;

        private static readonly double[] Gradients2D = {
             5,  2,    2,  5,
            -5,  2,   -2,  5,
             5, -2,    2, -5,
            -5, -2,   -2, -5
        };

        private static readonly double[] Gradients3D =
        {
            -11,  4,  4,     -4,  11,  4,    -4,  4,  11,
             11,  4,  4,      4,  11,  4,     4,  4,  11,
            -11, -4,  4,     -4, -11,  4,    -4, -4,  11,
             11, -4,  4,      4, -11,  4,     4, -4,  11,
            -11,  4, -4,     -4,  11, -4,    -4,  4, -11,
             11,  4, -4,      4,  11, -4,     4,  4, -11,
            -11, -4, -4,     -4, -11, -4,    -4, -4, -11,
             11, -4, -4,      4, -11, -4,     4, -4, -11
        };

        private static readonly double[] Gradients4D =
        {
             3,  1,  1,  1,      1,  3,  1,  1,      1,  1,  3,  1,      1,  1,  1,  3,
            -3,  1,  1,  1,     -1,  3,  1,  1,     -1,  1,  3,  1,     -1,  1,  1,  3,
             3, -1,  1,  1,      1, -3,  1,  1,      1, -1,  3,  1,      1, -1,  1,  3,
            -3, -1,  1,  1,     -1, -3,  1,  1,     -1, -1,  3,  1,     -1, -1,  1,  3,
             3,  1, -1,  1,      1,  3, -1,  1,      1,  1, -3,  1,      1,  1, -1,  3,
            -3,  1, -1,  1,     -1,  3, -1,  1,     -1,  1, -3,  1,     -1,  1, -1,  3,
             3, -1, -1,  1,      1, -3, -1,  1,      1, -1, -3,  1,      1, -1, -1,  3,
            -3, -1, -1,  1,     -1, -3, -1,  1,     -1, -1, -3,  1,     -1, -1, -1,  3,
             3,  1,  1, -1,      1,  3,  1, -1,      1,  1,  3, -1,      1,  1,  1, -3,
            -3,  1,  1, -1,     -1,  3,  1, -1,     -1,  1,  3, -1,     -1,  1,  1, -3,
             3, -1,  1, -1,      1, -3,  1, -1,      1, -1,  3, -1,      1, -1,  1, -3,
            -3, -1,  1, -1,     -1, -3,  1, -1,     -1, -1,  3, -1,     -1, -1,  1, -3,
             3,  1, -1, -1,      1,  3, -1, -1,      1,  1, -3, -1,      1,  1, -1, -3,
            -3,  1, -1, -1,     -1,  3, -1, -1,     -1,  1, -3, -1,     -1,  1, -1, -3,
             3, -1, -1, -1,      1, -3, -1, -1,      1, -1, -3, -1,      1, -1, -1, -3,
            -3, -1, -1, -1,     -1, -3, -1, -1,     -1, -1, -3, -1,     -1, -1, -1, -3
        };

        private static readonly int[] P2D = { 0, 0, 1, -1, 0, 0, -1, 1, 0, 2, 1, 1, 1, 2, 2, 0, 1, 2, 0, 2, 1, 0, 0, 0 };

        private static readonly int[] P3D = { 0, 0, 1, -1, 0, 0, 1, 0, -1, 0, 0, -1, 1, 0, 0, 0, 1, -1, 0, 0, -1, 0, 1, 0, 0, -1, 1, 0, 2, 1, 1, 0, 1, 1, 1, -1, 0, 2, 1, 0, 1, 1, 1, -1, 1, 0, 2, 0, 1, 1, 1, -1, 1, 1, 1, 3, 2, 1, 0, 3, 1, 2, 0, 1, 3, 2, 0, 1, 3, 1, 0, 2, 1, 3, 0, 2, 1, 3, 0, 1, 2, 1, 1, 1, 0, 0, 2, 2, 0, 0, 1, 1, 0, 1, 0, 2, 0, 2, 0, 1, 1, 0, 0, 1, 2, 0, 0, 2, 2, 0, 0, 0, 0, 1, 1, -1, 1, 2, 0, 0, 0, 0, 1, -1, 1, 1, 2, 0, 0, 0, 0, 1, 1, 1, -1, 2, 3, 1, 1, 1, 2, 0, 0, 2, 2, 3, 1, 1, 1, 2, 2, 0, 0, 2, 3, 1, 1, 1, 2, 0, 2, 0, 2, 1, 1, -1, 1, 2, 0, 0, 2, 2, 1, 1, -1, 1, 2, 2, 0, 0, 2, 1, -1, 1, 1, 2, 0, 0, 2, 2, 1, -1, 1, 1, 2, 0, 2, 0, 2, 1, 1, 1, -1, 2, 2, 0, 0, 2, 1, 1, 1, -1, 2, 0, 2, 0 };

        private static readonly int[] P4D = { 0, 0, 1, -1, 0, 0, 0, 1, 0, -1, 0, 0, 1, 0, 0, -1, 0, 0, -1, 1, 0, 0, 0, 0, 1, -1, 0, 0, 0, 1, 0, -1, 0, 0, -1, 0, 1, 0, 0, 0, -1, 1, 0, 0, 0, 0, 1, -1, 0, 0, -1, 0, 0, 1, 0, 0, -1, 0, 1, 0, 0, 0, -1, 1, 0, 2, 1, 1, 0, 0, 1, 1, 1, -1, 0, 1, 1, 1, 0, -1, 0, 2, 1, 0, 1, 0, 1, 1, -1, 1, 0, 1, 1, 0, 1, -1, 0, 2, 0, 1, 1, 0, 1, -1, 1, 1, 0, 1, 0, 1, 1, -1, 0, 2, 1, 0, 0, 1, 1, 1, -1, 0, 1, 1, 1, 0, -1, 1, 0, 2, 0, 1, 0, 1, 1, -1, 1, 0, 1, 1, 0, 1, -1, 1, 0, 2, 0, 0, 1, 1, 1, -1, 0, 1, 1, 1, 0, -1, 1, 1, 1, 4, 2, 1, 1, 0, 4, 1, 2, 1, 0, 4, 1, 1, 2, 0, 1, 4, 2, 1, 0, 1, 4, 1, 2, 0, 1, 4, 1, 1, 0, 2, 1, 4, 2, 0, 1, 1, 4, 1, 0, 2, 1, 4, 1, 0, 1, 2, 1, 4, 0, 2, 1, 1, 4, 0, 1, 2, 1, 4, 0, 1, 1, 2, 1, 2, 1, 1, 0, 0, 3, 2, 1, 0, 0, 3, 1, 2, 0, 0, 1, 2, 1, 0, 1, 0, 3, 2, 0, 1, 0, 3, 1, 0, 2, 0, 1, 2, 0, 1, 1, 0, 3, 0, 2, 1, 0, 3, 0, 1, 2, 0, 1, 2, 1, 0, 0, 1, 3, 2, 0, 0, 1, 3, 1, 0, 0, 2, 1, 2, 0, 1, 0, 1, 3, 0, 2, 0, 1, 3, 0, 1, 0, 2, 1, 2, 0, 0, 1, 1, 3, 0, 0, 2, 1, 3, 0, 0, 1, 2, 2, 3, 1, 1, 1, 0, 2, 1, 1, 1, -1, 2, 2, 0, 0, 0, 2, 3, 1, 1, 0, 1, 2, 1, 1, -1, 1, 2, 2, 0, 0, 0, 2, 3, 1, 0, 1, 1, 2, 1, -1, 1, 1, 2, 2, 0, 0, 0, 2, 3, 1, 1, 1, 0, 2, 1, 1, 1, -1, 2, 0, 2, 0, 0, 2, 3, 1, 1, 0, 1, 2, 1, 1, -1, 1, 2, 0, 2, 0, 0, 2, 3, 0, 1, 1, 1, 2, -1, 1, 1, 1, 2, 0, 2, 0, 0, 2, 3, 1, 1, 1, 0, 2, 1, 1, 1, -1, 2, 0, 0, 2, 0, 2, 3, 1, 0, 1, 1, 2, 1, -1, 1, 1, 2, 0, 0, 2, 0, 2, 3, 0, 1, 1, 1, 2, -1, 1, 1, 1, 2, 0, 0, 2, 0, 2, 3, 1, 1, 0, 1, 2, 1, 1, -1, 1, 2, 0, 0, 0, 2, 2, 3, 1, 0, 1, 1, 2, 1, -1, 1, 1, 2, 0, 0, 0, 2, 2, 3, 0, 1, 1, 1, 2, -1, 1, 1, 1, 2, 0, 0, 0, 2, 2, 1, 1, 1, -1, 0, 1, 1, 1, 0, -1, 0, 0, 0, 0, 0, 2, 1, 1, -1, 1, 0, 1, 1, 0, 1, -1, 0, 0, 0, 0, 0, 2, 1, -1, 1, 1, 0, 1, 0, 1, 1, -1, 0, 0, 0, 0, 0, 2, 1, 1, -1, 0, 1, 1, 1, 0, -1, 1, 0, 0, 0, 0, 0, 2, 1, -1, 1, 0, 1, 1, 0, 1, -1, 1, 0, 0, 0, 0, 0, 2, 1, -1, 0, 1, 1, 1, 0, -1, 1, 1, 0, 0, 0, 0, 0, 2, 1, 1, 1, -1, 0, 1, 1, 1, 0, -1, 2, 2, 0, 0, 0, 2, 1, 1, -1, 1, 0, 1, 1, 0, 1, -1, 2, 2, 0, 0, 0, 2, 1, 1, -1, 0, 1, 1, 1, 0, -1, 1, 2, 2, 0, 0, 0, 2, 1, 1, 1, -1, 0, 1, 1, 1, 0, -1, 2, 0, 2, 0, 0, 2, 1, -1, 1, 1, 0, 1, 0, 1, 1, -1, 2, 0, 2, 0, 0, 2, 1, -1, 1, 0, 1, 1, 0, 1, -1, 1, 2, 0, 2, 0, 0, 2, 1, 1, -1, 1, 0, 1, 1, 0, 1, -1, 2, 0, 0, 2, 0, 2, 1, -1, 1, 1, 0, 1, 0, 1, 1, -1, 2, 0, 0, 2, 0, 2, 1, -1, 0, 1, 1, 1, 0, -1, 1, 1, 2, 0, 0, 2, 0, 2, 1, 1, -1, 0, 1, 1, 1, 0, -1, 1, 2, 0, 0, 0, 2, 2, 1, -1, 1, 0, 1, 1, 0, 1, -1, 1, 2, 0, 0, 0, 2, 2, 1, -1, 0, 1, 1, 1, 0, -1, 1, 1, 2, 0, 0, 0, 2, 3, 1, 1, 0, 0, 0, 2, 2, 0, 0, 0, 2, 1, 1, 1, -1, 3, 1, 0, 1, 0, 0, 2, 0, 2, 0, 0, 2, 1, 1, 1, -1, 3, 1, 0, 0, 1, 0, 2, 0, 0, 2, 0, 2, 1, 1, 1, -1, 3, 1, 1, 0, 0, 0, 2, 2, 0, 0, 0, 2, 1, 1, -1, 1, 3, 1, 0, 1, 0, 0, 2, 0, 2, 0, 0, 2, 1, 1, -1, 1, 3, 1, 0, 0, 0, 1, 2, 0, 0, 0, 2, 2, 1, 1, -1, 1, 3, 1, 1, 0, 0, 0, 2, 2, 0, 0, 0, 2, 1, -1, 1, 1, 3, 1, 0, 0, 1, 0, 2, 0, 0, 2, 0, 2, 1, -1, 1, 1, 3, 1, 0, 0, 0, 1, 2, 0, 0, 0, 2, 2, 1, -1, 1, 1, 3, 1, 0, 1, 0, 0, 2, 0, 2, 0, 0, 2, -1, 1, 1, 1, 3, 1, 0, 0, 1, 0, 2, 0, 0, 2, 0, 2, -1, 1, 1, 1, 3, 1, 0, 0, 0, 1, 2, 0, 0, 0, 2, 2, -1, 1, 1, 1, 3, 3, 2, 1, 0, 0, 3, 1, 2, 0, 0, 4, 1, 1, 1, 1, 3, 3, 2, 0, 1, 0, 3, 1, 0, 2, 0, 4, 1, 1, 1, 1, 3, 3, 0, 2, 1, 0, 3, 0, 1, 2, 0, 4, 1, 1, 1, 1, 3, 3, 2, 0, 0, 1, 3, 1, 0, 0, 2, 4, 1, 1, 1, 1, 3, 3, 0, 2, 0, 1, 3, 0, 1, 0, 2, 4, 1, 1, 1, 1, 3, 3, 0, 0, 2, 1, 3, 0, 0, 1, 2, 4, 1, 1, 1, 1, 3, 3, 2, 1, 0, 0, 3, 1, 2, 0, 0, 2, 1, 1, 1, -1, 3, 3, 2, 0, 1, 0, 3, 1, 0, 2, 0, 2, 1, 1, 1, -1, 3, 3, 0, 2, 1, 0, 3, 0, 1, 2, 0, 2, 1, 1, 1, -1, 3, 3, 2, 1, 0, 0, 3, 1, 2, 0, 0, 2, 1, 1, -1, 1, 3, 3, 2, 0, 0, 1, 3, 1, 0, 0, 2, 2, 1, 1, -1, 1, 3, 3, 0, 2, 0, 1, 3, 0, 1, 0, 2, 2, 1, 1, -1, 1, 3, 3, 2, 0, 1, 0, 3, 1, 0, 2, 0, 2, 1, -1, 1, 1, 3, 3, 2, 0, 0, 1, 3, 1, 0, 0, 2, 2, 1, -1, 1, 1, 3, 3, 0, 0, 2, 1, 3, 0, 0, 1, 2, 2, 1, -1, 1, 1, 3, 3, 0, 2, 1, 0, 3, 0, 1, 2, 0, 2, -1, 1, 1, 1, 3, 3, 0, 2, 0, 1, 3, 0, 1, 0, 2, 2, -1, 1, 1, 1, 3, 3, 0, 0, 2, 1, 3, 0, 0, 1, 2, 2, -1, 1, 1, 1 };

        private static readonly int[] LookupPairs2D = { 0, 1, 1, 0, 4, 1, 17, 0, 20, 2, 21, 2, 22, 5, 23, 5, 26, 4, 39, 3, 42, 4, 43, 3 };

        private static readonly int[] LookupPairs3D = { 0, 2, 1, 1, 2, 2, 5, 1, 6, 0, 7, 0, 32, 2, 34, 2, 129, 1, 133, 1, 160, 5, 161, 5, 518, 0, 519, 0, 546, 4, 550, 4, 645, 3, 647, 3, 672, 5, 673, 5, 674, 4, 677, 3, 678, 4, 679, 3, 680, 13, 681, 13, 682, 12, 685, 14, 686, 12, 687, 14, 712, 20, 714, 18, 809, 21, 813, 23, 840, 20, 841, 21, 1198, 19, 1199, 22, 1226, 18, 1230, 19, 1325, 23, 1327, 22, 1352, 15, 1353, 17, 1354, 15, 1357, 17, 1358, 16, 1359, 16, 1360, 11, 1361, 10, 1362, 11, 1365, 10, 1366, 9, 1367, 9, 1392, 11, 1394, 11, 1489, 10, 1493, 10, 1520, 8, 1521, 8, 1878, 9, 1879, 9, 1906, 7, 1910, 7, 2005, 6, 2007, 6, 2032, 8, 2033, 8, 2034, 7, 2037, 6, 2038, 7, 2039, 6 };

        private static readonly int[] LookupPairs4D = { 0, 3, 1, 2, 2, 3, 5, 2, 6, 1, 7, 1, 8, 3, 9, 2, 10, 3, 13, 2, 16, 3, 18, 3, 22, 1, 23, 1, 24, 3, 26, 3, 33, 2, 37, 2, 38, 1, 39, 1, 41, 2, 45, 2, 54, 1, 55, 1, 56, 0, 57, 0, 58, 0, 59, 0, 60, 0, 61, 0, 62, 0, 63, 0, 256, 3, 258, 3, 264, 3, 266, 3, 272, 3, 274, 3, 280, 3, 282, 3, 2049, 2, 2053, 2, 2057, 2, 2061, 2, 2081, 2, 2085, 2, 2089, 2, 2093, 2, 2304, 9, 2305, 9, 2312, 9, 2313, 9, 16390, 1, 16391, 1, 16406, 1, 16407, 1, 16422, 1, 16423, 1, 16438, 1, 16439, 1, 16642, 8, 16646, 8, 16658, 8, 16662, 8, 18437, 6, 18439, 6, 18469, 6, 18471, 6, 18688, 9, 18689, 9, 18690, 8, 18693, 6, 18694, 8, 18695, 6, 18696, 9, 18697, 9, 18706, 8, 18710, 8, 18725, 6, 18727, 6, 131128, 0, 131129, 0, 131130, 0, 131131, 0, 131132, 0, 131133, 0, 131134, 0, 131135, 0, 131352, 7, 131354, 7, 131384, 7, 131386, 7, 133161, 5, 133165, 5, 133177, 5, 133181, 5, 133376, 9, 133377, 9, 133384, 9, 133385, 9, 133400, 7, 133402, 7, 133417, 5, 133421, 5, 133432, 7, 133433, 5, 133434, 7, 133437, 5, 147510, 4, 147511, 4, 147518, 4, 147519, 4, 147714, 8, 147718, 8, 147730, 8, 147734, 8, 147736, 7, 147738, 7, 147766, 4, 147767, 4, 147768, 7, 147770, 7, 147774, 4, 147775, 4, 149509, 6, 149511, 6, 149541, 6, 149543, 6, 149545, 5, 149549, 5, 149558, 4, 149559, 4, 149561, 5, 149565, 5, 149566, 4, 149567, 4, 149760, 9, 149761, 9, 149762, 8, 149765, 6, 149766, 8, 149767, 6, 149768, 9, 149769, 9, 149778, 8, 149782, 8, 149784, 7, 149786, 7, 149797, 6, 149799, 6, 149801, 5, 149805, 5, 149814, 4, 149815, 4, 149816, 7, 149817, 5, 149818, 7, 149821, 5, 149822, 4, 149823, 4, 149824, 37, 149825, 37, 149826, 36, 149829, 34, 149830, 36, 149831, 34, 149832, 37, 149833, 37, 149842, 36, 149846, 36, 149848, 35, 149850, 35, 149861, 34, 149863, 34, 149865, 33, 149869, 33, 149878, 32, 149879, 32, 149880, 35, 149881, 33, 149882, 35, 149885, 33, 149886, 32, 149887, 32, 150080, 49, 150082, 48, 150088, 49, 150098, 48, 150104, 47, 150106, 47, 151873, 46, 151877, 45, 151881, 46, 151909, 45, 151913, 44, 151917, 44, 152128, 49, 152129, 46, 152136, 49, 152137, 46, 166214, 43, 166215, 42, 166230, 43, 166247, 42, 166262, 41, 166263, 41, 166466, 48, 166470, 43, 166482, 48, 166486, 43, 168261, 45, 168263, 42, 168293, 45, 168295, 42, 168512, 31, 168513, 28, 168514, 31, 168517, 28, 168518, 25, 168519, 25, 280952, 40, 280953, 39, 280954, 40, 280957, 39, 280958, 38, 280959, 38, 281176, 47, 281178, 47, 281208, 40, 281210, 40, 282985, 44, 282989, 44, 283001, 39, 283005, 39, 283208, 30, 283209, 27, 283224, 30, 283241, 27, 283256, 22, 283257, 22, 297334, 41, 297335, 41, 297342, 38, 297343, 38, 297554, 29, 297558, 24, 297562, 29, 297590, 24, 297594, 21, 297598, 21, 299365, 26, 299367, 23, 299373, 26, 299383, 23, 299389, 20, 299391, 20, 299584, 31, 299585, 28, 299586, 31, 299589, 28, 299590, 25, 299591, 25, 299592, 30, 299593, 27, 299602, 29, 299606, 24, 299608, 30, 299610, 29, 299621, 26, 299623, 23, 299625, 27, 299629, 26, 299638, 24, 299639, 23, 299640, 22, 299641, 22, 299642, 21, 299645, 20, 299646, 21, 299647, 20, 299648, 61, 299649, 60, 299650, 61, 299653, 60, 299654, 59, 299655, 59, 299656, 58, 299657, 57, 299666, 55, 299670, 54, 299672, 58, 299674, 55, 299685, 52, 299687, 51, 299689, 57, 299693, 52, 299702, 54, 299703, 51, 299704, 56, 299705, 56, 299706, 53, 299709, 50, 299710, 53, 299711, 50, 299904, 61, 299906, 61, 299912, 58, 299922, 55, 299928, 58, 299930, 55, 301697, 60, 301701, 60, 301705, 57, 301733, 52, 301737, 57, 301741, 52, 301952, 79, 301953, 79, 301960, 76, 301961, 76, 316038, 59, 316039, 59, 316054, 54, 316071, 51, 316086, 54, 316087, 51, 316290, 78, 316294, 78, 316306, 73, 316310, 73, 318085, 77, 318087, 77, 318117, 70, 318119, 70, 318336, 79, 318337, 79, 318338, 78, 318341, 77, 318342, 78, 318343, 77, 430776, 56, 430777, 56, 430778, 53, 430781, 50, 430782, 53, 430783, 50, 431000, 75, 431002, 72, 431032, 75, 431034, 72, 432809, 74, 432813, 69, 432825, 74, 432829, 69, 433032, 76, 433033, 76, 433048, 75, 433065, 74, 433080, 75, 433081, 74, 447158, 71, 447159, 68, 447166, 71, 447167, 68, 447378, 73, 447382, 73, 447386, 72, 447414, 71, 447418, 72, 447422, 71, 449189, 70, 449191, 70, 449197, 69, 449207, 68, 449213, 69, 449215, 68, 449408, 67, 449409, 67, 449410, 66, 449413, 64, 449414, 66, 449415, 64, 449416, 67, 449417, 67, 449426, 66, 449430, 66, 449432, 65, 449434, 65, 449445, 64, 449447, 64, 449449, 63, 449453, 63, 449462, 62, 449463, 62, 449464, 65, 449465, 63, 449466, 65, 449469, 63, 449470, 62, 449471, 62, 449472, 19, 449473, 19, 449474, 18, 449477, 16, 449478, 18, 449479, 16, 449480, 19, 449481, 19, 449490, 18, 449494, 18, 449496, 17, 449498, 17, 449509, 16, 449511, 16, 449513, 15, 449517, 15, 449526, 14, 449527, 14, 449528, 17, 449529, 15, 449530, 17, 449533, 15, 449534, 14, 449535, 14, 449728, 19, 449729, 19, 449730, 18, 449734, 18, 449736, 19, 449737, 19, 449746, 18, 449750, 18, 449752, 17, 449754, 17, 449784, 17, 449786, 17, 451520, 19, 451521, 19, 451525, 16, 451527, 16, 451528, 19, 451529, 19, 451557, 16, 451559, 16, 451561, 15, 451565, 15, 451577, 15, 451581, 15, 451776, 19, 451777, 19, 451784, 19, 451785, 19, 465858, 18, 465861, 16, 465862, 18, 465863, 16, 465874, 18, 465878, 18, 465893, 16, 465895, 16, 465910, 14, 465911, 14, 465918, 14, 465919, 14, 466114, 18, 466118, 18, 466130, 18, 466134, 18, 467909, 16, 467911, 16, 467941, 16, 467943, 16, 468160, 13, 468161, 13, 468162, 13, 468163, 13, 468164, 13, 468165, 13, 468166, 13, 468167, 13, 580568, 17, 580570, 17, 580585, 15, 580589, 15, 580598, 14, 580599, 14, 580600, 17, 580601, 15, 580602, 17, 580605, 15, 580606, 14, 580607, 14, 580824, 17, 580826, 17, 580856, 17, 580858, 17, 582633, 15, 582637, 15, 582649, 15, 582653, 15, 582856, 12, 582857, 12, 582872, 12, 582873, 12, 582888, 12, 582889, 12, 582904, 12, 582905, 12, 596982, 14, 596983, 14, 596990, 14, 596991, 14, 597202, 11, 597206, 11, 597210, 11, 597214, 11, 597234, 11, 597238, 11, 597242, 11, 597246, 11, 599013, 10, 599015, 10, 599021, 10, 599023, 10, 599029, 10, 599031, 10, 599037, 10, 599039, 10, 599232, 13, 599233, 13, 599234, 13, 599235, 13, 599236, 13, 599237, 13, 599238, 13, 599239, 13, 599240, 12, 599241, 12, 599250, 11, 599254, 11, 599256, 12, 599257, 12, 599258, 11, 599262, 11, 599269, 10, 599271, 10, 599272, 12, 599273, 12, 599277, 10, 599279, 10, 599282, 11, 599285, 10, 599286, 11, 599287, 10, 599288, 12, 599289, 12, 599290, 11, 599293, 10, 599294, 11, 599295, 10 };

        private static readonly int[][] Base2D = {
            new[] { 1, 1, 0, 1, 0, 1, 0, 0, 0 },
            new[] { 1, 1, 0, 1, 0, 1, 2, 1, 1 }
        };

        private static readonly int[][] Base3D = {
            new[] { 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1 },
            new[] { 2, 1, 1, 0, 2, 1, 0, 1, 2, 0, 1, 1, 3, 1, 1, 1 },
            new[] { 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 2, 1, 1, 0, 2, 1, 0, 1, 2, 0, 1, 1 }
        };

        private static readonly int[][] Base4D = {
            new[] { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1 },
            new[] { 3, 1, 1, 1, 0, 3, 1, 1, 0, 1, 3, 1, 0, 1, 1, 3, 0, 1, 1, 1, 4, 1, 1, 1, 1 },
            new[] { 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 2, 1, 1, 0, 0, 2, 1, 0, 1, 0, 2, 1, 0, 0, 1, 2, 0, 1, 1, 0, 2, 0, 1, 0, 1, 2, 0, 0, 1, 1 },
            new[] { 3, 1, 1, 1, 0, 3, 1, 1, 0, 1, 3, 1, 0, 1, 1, 3, 0, 1, 1, 1, 2, 1, 1, 0, 0, 2, 1, 0, 1, 0, 2, 1, 0, 0, 1, 2, 0, 1, 1, 0, 2, 0, 1, 0, 1, 2, 0, 0, 1, 1 }
        };

        private static readonly Contribution2[] Lookup2D;
        private static readonly Contribution3[] Lookup3D;
        private static readonly Contribution4[] Lookup4D;

        static FastSimplexNoise()
        {
            var contributions2D = new Contribution2[P2D.Length / 4];

            for (int i = 0; i < P2D.Length; i += 4)
            {
                var baseSet = Base2D[P2D[i]];
                Contribution2 previous = null;
                Contribution2 current = null;

                for (int k = 0; k < baseSet.Length; k += 3)
                {
                    current = new Contribution2(baseSet[k], baseSet[k + 1], baseSet[k + 2]);

                    if (previous == null)
                    {
                        contributions2D[i / 4] = current;
                    }
                    else
                    {
                        previous.Next = current;
                    }

                    previous = current;
                }

                current.Next = new Contribution2(P2D[i + 1], P2D[i + 2], P2D[i + 3]);
            }

            Lookup2D = new Contribution2[64];

            for (var i = 0; i < LookupPairs2D.Length; i += 2)
            {
                Lookup2D[LookupPairs2D[i]] = contributions2D[LookupPairs2D[i + 1]];
            }

            var contributions3D = new Contribution3[P3D.Length / 9];

            for (int i = 0; i < P3D.Length; i += 9)
            {
                var baseSet = Base3D[P3D[i]];
                Contribution3 previous = null;
                Contribution3 current = null;

                for (int k = 0; k < baseSet.Length; k += 4)
                {
                    current = new Contribution3(baseSet[k], baseSet[k + 1], baseSet[k + 2], baseSet[k + 3]);

                    if (previous == null)
                    {
                        contributions3D[i / 9] = current;
                    }
                    else
                    {
                        previous.Next = current;
                    }

                    previous = current;
                }

                current.Next = new Contribution3(P3D[i + 1], P3D[i + 2], P3D[i + 3], P3D[i + 4])
                {
                    Next = new Contribution3(P3D[i + 5], P3D[i + 6], P3D[i + 7], P3D[i + 8])
                };
            }

            Lookup3D = new Contribution3[2048];

            for (var i = 0; i < LookupPairs3D.Length; i += 2)
            {
                Lookup3D[LookupPairs3D[i]] = contributions3D[LookupPairs3D[i + 1]];
            }

            var contributions4D = new Contribution4[P4D.Length / 16];

            for (int i = 0; i < P4D.Length; i += 16)
            {
                var baseSet = Base4D[P4D[i]];
                Contribution4 previous = null;
                Contribution4 current = null;

                for (int k = 0; k < baseSet.Length; k += 5)
                {
                    current = new Contribution4(baseSet[k], baseSet[k + 1], baseSet[k + 2], baseSet[k + 3], baseSet[k + 4]);
                    if (previous == null)
                    {
                        contributions4D[i / 16] = current;
                    }
                    else
                    {
                        previous.Next = current;
                    }
                    previous = current;
                }

                current.Next = new Contribution4(P4D[i + 1], P4D[i + 2], P4D[i + 3], P4D[i + 4], P4D[i + 5])
                {
                    Next = new Contribution4(P4D[i + 6], P4D[i + 7], P4D[i + 8], P4D[i + 9], P4D[i + 10])
                    {
                        Next = new Contribution4(P4D[i + 11], P4D[i + 12], P4D[i + 13], P4D[i + 14], P4D[i + 15])
                    }
                };
            }

            Lookup4D = new Contribution4[1048576];

            for (var i = 0; i < LookupPairs4D.Length; i += 2)
            {
                Lookup4D[LookupPairs4D[i]] = contributions4D[LookupPairs4D[i + 1]];
            }
        }

        private static int FastFloor(double x)
        {
            var xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }

        public FastSimplexNoise() : this(DateTime.UtcNow.Ticks) { }

        public FastSimplexNoise(long seed)
        {
            perm = new byte[256];
            perm2D = new byte[256];
            perm3D = new byte[256];
            perm4D = new byte[256];
            var source = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                source[i] = (byte)i;
            }

            seed = seed * SEEDVAL_1 + SEEDVAL_2;
            seed = seed * SEEDVAL_1 + SEEDVAL_2;
            seed = seed * SEEDVAL_1 + SEEDVAL_2;

            for (int i = 255; i >= 0; i--)
            {
                seed = seed * SEEDVAL_1 + SEEDVAL_2;
                int r = (int)((seed + 31) % (i + 1));

                if (r < 0)
                {
                    r += (i + 1);
                }

                perm[i] = source[r];
                perm2D[i] = (byte)(perm[i] & 0x0E);
                perm3D[i] = (byte)((perm[i] % 24) * 3);
                perm4D[i] = (byte)(perm[i] & 0xFC);
                source[r] = source[i];
            }
        }

        public double Evaluate(double x, double y)
        {
            var stretchOffset = (x + y) * STRETCH__2_D;
            var xs = x + stretchOffset;
            var ys = y + stretchOffset;

            var xsb = FastFloor(xs);
            var ysb = FastFloor(ys);

            var squishOffset = (xsb + ysb) * SQUISH__2_D;
            var dx0 = x - (xsb + squishOffset);
            var dy0 = y - (ysb + squishOffset);

            var xins = xs - xsb;
            var yins = ys - ysb;

            var inSum = xins + yins;

            var hash =
               (int)(xins - yins + 1) |
               (int)(inSum) << 1 |
               (int)(inSum + yins) << 2 |
               (int)(inSum + xins) << 4;

            var c = Lookup2D[hash];
            var value = 0.0;

            while (c != null)
            {
                var dx = dx0 + c.Dx;
                var dy = dy0 + c.Dy;
                var attn = 2 - dx * dx - dy * dy;
                if (attn > 0)
                {
                    var px = xsb + c.Xsb;
                    var py = ysb + c.Ysb;

                    var i = perm2D[(perm[px & 0xFF] + py) & 0xFF];
                    var valuePart = Gradients2D[i] * dx + Gradients2D[i + 1] * dy;

                    attn *= attn;
                    value += attn * attn * valuePart;
                }

                c = c.Next;
            }

            return value * NORM__2_D;
        }

        public double Evaluate(double x, double y, double z)
        {
            var stretchOffset = (x + y + z) * STRETCH__3_D;
            var xs = x + stretchOffset;
            var ys = y + stretchOffset;
            var zs = z + stretchOffset;

            var xsb = FastFloor(xs);
            var ysb = FastFloor(ys);
            var zsb = FastFloor(zs);

            var squishOffset = (xsb + ysb + zsb) * SQUISH__3_D;
            var dx0 = x - (xsb + squishOffset);
            var dy0 = y - (ysb + squishOffset);
            var dz0 = z - (zsb + squishOffset);

            var xins = xs - xsb;
            var yins = ys - ysb;
            var zins = zs - zsb;

            var inSum = xins + yins + zins;

            var hash =
               (int)(yins - zins + 1) |
               (int)(xins - yins + 1) << 1 |
               (int)(xins - zins + 1) << 2 |
               (int)inSum << 3 |
               (int)(inSum + zins) << 5 |
               (int)(inSum + yins) << 7 |
               (int)(inSum + xins) << 9;

            var c = Lookup3D[hash];
            var value = 0.0;

            while (c != null)
            {
                var dx = dx0 + c.Dx;
                var dy = dy0 + c.Dy;
                var dz = dz0 + c.Dz;
                var attn = 2 - dx * dx - dy * dy - dz * dz;

                if (attn > 0)
                {
                    var px = xsb + c.Xsb;
                    var py = ysb + c.Ysb;
                    var pz = zsb + c.Zsb;

                    var i = perm3D[(perm[(perm[px & 0xFF] + py) & 0xFF] + pz) & 0xFF];
                    var valuePart = Gradients3D[i] * dx + Gradients3D[i + 1] * dy + Gradients3D[i + 2] * dz;

                    attn *= attn;
                    value += attn * attn * valuePart;
                }

                c = c.Next;
            }
            return value * NORM__3_D;
        }

        public double Evaluate(double x, double y, double z, double w)
        {
            var stretchOffset = (x + y + z + w) * STRETCH__4_D;
            var xs = x + stretchOffset;
            var ys = y + stretchOffset;
            var zs = z + stretchOffset;
            var ws = w + stretchOffset;

            var xsb = FastFloor(xs);
            var ysb = FastFloor(ys);
            var zsb = FastFloor(zs);
            var wsb = FastFloor(ws);

            var squishOffset = (xsb + ysb + zsb + wsb) * SQUISH__4_D;
            var dx0 = x - (xsb + squishOffset);
            var dy0 = y - (ysb + squishOffset);
            var dz0 = z - (zsb + squishOffset);
            var dw0 = w - (wsb + squishOffset);

            var xins = xs - xsb;
            var yins = ys - ysb;
            var zins = zs - zsb;
            var wins = ws - wsb;

            var inSum = xins + yins + zins + wins;

            var hash =
                (int)(zins - wins + 1) |
                (int)(yins - zins + 1) << 1 |
                (int)(yins - wins + 1) << 2 |
                (int)(xins - yins + 1) << 3 |
                (int)(xins - zins + 1) << 4 |
                (int)(xins - wins + 1) << 5 |
                (int)inSum << 6 |
                (int)(inSum + wins) << 8 |
                (int)(inSum + zins) << 11 |
                (int)(inSum + yins) << 14 |
                (int)(inSum + xins) << 17;

            var c = Lookup4D[hash];
            var value = 0.0;

            while (c != null)
            {
                var dx = dx0 + c.Dx;
                var dy = dy0 + c.Dy;
                var dz = dz0 + c.Dz;
                var dw = dw0 + c.Dw;
                var attn = 2 - dx * dx - dy * dy - dz * dz - dw * dw;

                if (attn > 0)
                {
                    var px = xsb + c.Xsb;
                    var py = ysb + c.Ysb;
                    var pz = zsb + c.Zsb;
                    var pw = wsb + c.Wsb;

                    var i = perm4D[(perm[(perm[(perm[px & 0xFF] + py) & 0xFF] + pz) & 0xFF] + pw) & 0xFF];
                    var valuePart = Gradients4D[i] * dx + Gradients4D[i + 1] * dy + Gradients4D[i + 2] * dz + Gradients4D[i + 3] * dw;

                    attn *= attn;
                    value += attn * attn * valuePart;
                }

                c = c.Next;
            }

            return value * NORM__4_D;
        }

        private class Contribution2
        {
            public readonly double Dx;
            public readonly double Dy;
            public readonly int Xsb;
            public readonly int Ysb;
            public Contribution2 Next;

            public Contribution2(double multiplier, int xsb, int ysb)
            {
                Dx = -xsb - multiplier * SQUISH__2_D;
                Dy = -ysb - multiplier * SQUISH__2_D;
                this.Xsb = xsb;
                this.Ysb = ysb;
            }
        }

        private class Contribution3
        {
            public readonly double Dx;
            public readonly double Dy;
            public readonly double Dz;
            public readonly int Xsb;
            public readonly int Ysb;
            public readonly int Zsb;
            public Contribution3 Next;

            public Contribution3(double multiplier, int xsb, int ysb, int zsb)
            {
                Dx = -xsb - multiplier * SQUISH__3_D;
                Dy = -ysb - multiplier * SQUISH__3_D;
                Dz = -zsb - multiplier * SQUISH__3_D;
                this.Xsb = xsb;
                this.Ysb = ysb;
                this.Zsb = zsb;
            }
        }

        private class Contribution4
        {
            public readonly double Dx;
            public readonly double Dy;
            public readonly double Dz;
            public readonly double Dw;
            public readonly int Xsb;
            public readonly int Ysb;
            public readonly int Zsb;
            public readonly int Wsb;
            public Contribution4 Next;

            public Contribution4(double multiplier, int xsb, int ysb, int zsb, int wsb)
            {
                Dx = -xsb - multiplier * SQUISH__4_D;
                Dy = -ysb - multiplier * SQUISH__4_D;
                Dz = -zsb - multiplier * SQUISH__4_D;
                Dw = -wsb - multiplier * SQUISH__4_D;
                this.Xsb = xsb;
                this.Ysb = ysb;
                this.Zsb = zsb;
                this.Wsb = wsb;
            }
        }
    }
}