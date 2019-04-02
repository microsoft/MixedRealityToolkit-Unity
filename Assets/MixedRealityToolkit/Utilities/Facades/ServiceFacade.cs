// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Facades
{
    public class ServiceFacade : MonoBehaviour
    {
        public static Dictionary<Type, ServiceFacade> FacadeLookup = new Dictionary<Type, ServiceFacade>();

        private IMixedRealityService service = null;
        public IMixedRealityService Service { get { return service; } }

        private Type serviceType = null;
        public Type ServiceType { get { return serviceType; } }

        private bool destroyed = false;
        public bool Destroyed { get { return destroyed; } }

        public void SetService(IMixedRealityService service)
        {
            this.service = service;

            if (service == null)
            {
                serviceType = null;
                name = "(Destroyed)";
                gameObject.SetActive(false);
                FacadeLookup.Remove(serviceType);
                return;
            }
            else
            {
                this.serviceType = service.GetType();
                name = service.GetType().Name;
                gameObject.SetActive(true);

                if (!FacadeLookup.ContainsKey(serviceType))
                {
                    FacadeLookup.Add(serviceType, this);
                }
                else
                {
                    FacadeLookup[serviceType] = this;
                }
            }
        }

        private void OnDestroy()
        {
            FacadeLookup.Remove(serviceType);
            destroyed = true;
        }
    }
}
