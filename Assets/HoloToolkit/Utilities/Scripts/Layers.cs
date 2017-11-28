//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
	public static class Layers
	{
		private const int InvalidLayer = -1;

		#region Local layers
		private static int defaultLayer = Layers.InvalidLayer;
		private static int surfaceLayer = Layers.InvalidLayer;
        private static int interactionLayer = Layers.InvalidLayer;
        private static int activationLayer = Layers.InvalidLayer;
        #endregion

        private static int GetLayerNumber(ref int cache, string layerName)
		{
			if (cache == Layers.InvalidLayer)
			{
				cache = LayerMask.NameToLayer(layerName);
			}
			return cache;
		}
		public static LayerMask ToMask(int layer)
		{
			return 1 << layer;
		}

		public static int Default
		{
			get
			{
				return GetLayerNumber(ref defaultLayer, "Default");
			}
		}

		public static int Surface
		{
			get
			{
				return GetLayerNumber(ref surfaceLayer, "SR");
			}
		}
        public static int Interaction
        {
            get
            {
                return GetLayerNumber(ref interactionLayer, "Interaction");
            }
        }

        public static int Activation
        {
            get
            {
                return GetLayerNumber(ref activationLayer, "Activation");
            }
        }

    }
}
