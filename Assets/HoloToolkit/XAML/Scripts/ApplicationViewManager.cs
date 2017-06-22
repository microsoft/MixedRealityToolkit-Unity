// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if WINDOWS_UWP
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace HoloToolkit.Unity
{


    public delegate void ReturnValueCallback<TReturnValue>(TReturnValue returnValue);

    /// <summary>
    /// ApplicationViewManager can switch to Plan View, populate an Application View (New Window of UAP), then navigate to the page with page name. 
    /// After the page called 'CallbackReturnValue' method, the new Application View will be closed, and the system will switch back to your Full3D view.
    /// The corotine were waiting will get the return value by callback.
    /// </summary>

    public class ApplicationViewManager : MonoBehaviour
    {
        private void Start()
        {
#if WINDOWS_UWP
        UnityEngine.WSA.Application.InvokeOnUIThread(
            () =>
            {
                Full3DViewId = ApplicationView.GetForCurrentView().Id;
            }, true);
#endif
        }

#if WINDOWS_UWP

        static int Full3DViewId { get; set; }
        static System.Collections.Concurrent.ConcurrentDictionary<int, Action<object>> CallbackDictionary
            = new System.Collections.Concurrent.ConcurrentDictionary<int, Action<object>>();
#endif

        /// <summary>
        /// Call this method with Application View Dispatcher， or in Application View Thread, will return to Full3D View and close Application View
        /// </summary>
        /// <param name="returnValue">The return value of the Xaml View Execution</param>
#if WINDOWS_UWP
        public static async void CallbackReturnValue(object returnValue)
        {
            var viewId = ApplicationView.GetForCurrentView().Id;
            var v = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView();
            if (CallbackDictionary.TryRemove(viewId, out var cb))
            {
                try
                {
                    cb(returnValue);
                }
                catch (Exception)
                {

                }
            }
            await Windows.UI.ViewManagement.ApplicationViewSwitcher.TryShowAsStandaloneAsync(ApplicationViewManager.Full3DViewId).AsTask();
            v.CoreWindow.Close();
        }
#else
        public static void CallbackReturnValue(object returnValue)
        {       

        }
#endif
        /// <summary>
        /// Call this method in Unity App Thread can switch to Plan View, create and show a new Xaml View. 
        /// </summary>
        /// <typeparam name="TReturnValue"></typeparam>
        /// <param name="xamlPageName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator OnLaunchXamlView<TReturnValue>(string xamlPageName, Action<TReturnValue> callback)
        {

            bool isCompleted = false;
            object returnValue = null;
#if WINDOWS_UWP
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            var dispt = newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                var pageType = Type.GetType(Windows.UI.Xaml.Application.Current.GetType().AssemblyQualifiedName.Replace(".App,", $".{xamlPageName},"));
                var appv= ApplicationView.GetForCurrentView();
                newViewId =appv.Id;     
                var cb = new Action<object>(rval =>
                {
                    returnValue = rval;
                    isCompleted = true;
                });
                frame.Navigate(pageType, null);
                CallbackDictionary[newViewId] = cb;
                Window.Current.Content = frame;
                Window.Current.Activate();

            }).AsTask();

            yield return new WaitUntil(() => dispt.IsCompleted || dispt.IsCanceled || dispt.IsFaulted);

            Task viewShownTask = null;
            UnityEngine.WSA.Application.InvokeOnUIThread(
                () =>
                    {
                        viewShownTask = ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId).AsTask();
                    },
                    true);
            yield return new WaitUntil(() => viewShownTask.IsCompleted || viewShownTask.IsCanceled || viewShownTask.IsFaulted);
#else
            isCompleted = true;
#endif
            yield return new WaitUntil(() => isCompleted);
            try
            {
                if (callback != null)
                {
                    callback((TReturnValue)returnValue);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }        
    }
}
