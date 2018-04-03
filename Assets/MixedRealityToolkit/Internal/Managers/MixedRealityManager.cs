// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Internal.Definitions;
using Microsoft.MixedReality.Internal.Definitons;
using Microsoft.MixedReality.Internal.Interfaces;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Internal.Managers
{
    public class MixedRealityManager : Utilities.Singleton<MixedRealityManager>
    {
        public MixedRealityConfigurationProfile ActiveProfile;

        #region Mixed Reality Manager Events

        public event InitialiseHandler InitializeEvent;
        public event UpdateHandler UpdateEvent;
        public event DestroyHandler DestroyEvent;
        public delegate void InitialiseHandler();
        public delegate void UpdateHandler();
        public delegate void DestroyHandler();

        #endregion

        #region Active SDK components

        [SerializeField]
        private Controller[] activeControllers;
        public Controller[] ActiveControllers
        {
            get { return activeControllers; }
            set { activeControllers = value; }
        }

        [SerializeField]
        private Headset activeHeadset;

        public Headset ActiveHeadset
        {
            get { return activeHeadset; }
            set { activeHeadset = value; }
        }

        #endregion

        protected override void InitializeInternal()
        {
            base.Awake();

            //MixedRealityToolkit - Active SDK Discovery

            //MixedRealityToolkit - SDK Initialization

            //MixedRealityToolkit - Managers initialization

            if (ActiveProfile.EnableBoundary)
            {
                //Enable Boundary
            }

            if (ActiveProfile.EnableControllers)
            {
                //Enable Motion Controllers
            }

            if (ActiveProfile.EnableInputSystem)
            {
                //Enable Input
            }

            if (ActiveProfile.EnableFocus)
            {
                //Enable Focus
            }

            if (InitializeEvent != null) InitializeEvent.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            if (UpdateEvent != null) UpdateEvent.Invoke();
        }

        protected override void OnDestroy()
        {
            if (DestroyEvent != null) DestroyEvent.Invoke();

            //Final destroy - needs to be last
            base.OnDestroy();
        }

        #region Manager Container Management
        public void AddManager(Type type, IManager manager)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (manager == null)
                throw new ArgumentNullException("manager");

            if (GetManager(type) == null)
            {
                ActiveProfile.ActiveManagers.Add(type, manager);
            }
        }

        public IManager GetManager(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            IManager manager;
            if (ActiveProfile.ActiveManagers.TryGetValue(type, out manager))
                return manager;

            return null;
        }

        public void RemoveManager(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            ActiveProfile.ActiveManagers.Remove(type);
        }

        public T GetManager<T>() where T : class
        {
            var manager = GetManager(typeof(T));

            if (manager == null)
                return null;

            return (T)manager;
        }

        public bool ManagerExists<T>() where T : class
        {
            var manager = GetManager(typeof(T));

            if (manager == null)
                return true;

            return false;
        }
        #endregion

    }
}
