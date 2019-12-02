// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Facades
{
    /// <summary>
    /// Lightweight MonoBehavior used to represent active services in scene.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Scripts/MRTK/Core/ServiceFacade")]
    public class ServiceFacade : MonoBehaviour
    {
        public static Dictionary<Type, ServiceFacade> FacadeServiceLookup = new Dictionary<Type, ServiceFacade>();
        public static List<ServiceFacade> ActiveFacadeObjects = new List<ServiceFacade>();

        public IMixedRealityService Service { get; private set; } = null;
        public Type ServiceType { get; private set; } = null;
        public bool Destroyed { get; private set; } = false;

        public void SetService(IMixedRealityService service)
        {
            this.Service = service;

            if (service == null)
            {
                ServiceType = null;
                name = "(Destroyed)";
                gameObject.SetActive(false);
                return;
            }
            else
            {
                this.ServiceType = service.GetType();

                name = ServiceType.Name;
                gameObject.SetActive(true);

                if (!FacadeServiceLookup.ContainsKey(ServiceType))
                {
                    FacadeServiceLookup.Add(ServiceType, this);
                }
                else
                {
                    FacadeServiceLookup[ServiceType] = this;
                }

                if (!ActiveFacadeObjects.Contains(this))
                {
                    ActiveFacadeObjects.Add(this);
                }
            }
        }

        private void OnDestroy()
        {
            Destroyed = true;

            if (FacadeServiceLookup != null && ServiceType != null)
            {
                FacadeServiceLookup.Remove(ServiceType);
            }

            ActiveFacadeObjects.Remove(this);
        }
    }
}
