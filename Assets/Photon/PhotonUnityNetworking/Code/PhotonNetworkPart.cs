// ----------------------------------------------------------------------------
// <copyright file="PhotonNetworkPart.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// PhotonNetwork is the central class of the PUN package.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using System;
    using System.Linq;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Reflection;

    using ExitGames.Client.Photon;
    using Photon.Realtime;

    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClassPun = ExitGames.Client.Photon.SupportClass;

    public static partial class PhotonNetwork
    {
        private static HashSet<byte> allowedReceivingGroups = new HashSet<byte>();

        private static HashSet<byte> blockedSendingGroups = new HashSet<byte>();


        /// <summary>
        /// The photon view list.
        /// </summary>
        static Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>();

        /// <summary>
        /// Gets the photon views.
        /// </summary>
        /// <remarks>
        /// This is an expensive operation as it returns  a copy of the internal list.
        /// </remarks>
        /// <value>The photon views.</value>
        public static PhotonView[] PhotonViews
        {
            get
            {
                return photonViewList.Values.ToArray ();
            }

        }

        /// <summary>Parameters: PhotonView for which ownership changed, previous owner of the view.</summary>
        private static event Action<PhotonView, Player> OnOwnershipRequestEv;
        /// <summary>Parameters: PhotonView for which ownership was requested, player who requests ownership.</summary>
        private static event Action<PhotonView, Player> OnOwnershipTransferedEv;


        /// <summary>
        /// Registers an object for callbacks for the implemented callback-interfaces.
        /// </summary>
        /// <remarks>
        /// The covered callback interfaces are: IConnectionCallbacks, IMatchmakingCallbacks,
        /// ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback and IWebRpcCallback.
        ///
        /// See: <a href="https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks">.Net Callbacks</a>
        /// </remarks>
        /// <param name="target">The object that registers to get callbacks from PUN's LoadBalancingClient.</param>
        public static void AddCallbackTarget(object target)
        {
            if (target is PhotonView)
            {
                return;
            }

            IPunOwnershipCallbacks punOwnershipCallback = target as IPunOwnershipCallbacks;
            if (punOwnershipCallback != null)
            {
                OnOwnershipRequestEv += punOwnershipCallback.OnOwnershipRequest;
                OnOwnershipTransferedEv += punOwnershipCallback.OnOwnershipTransfered;
            }

            NetworkingClient.AddCallbackTarget(target);
        }


        /// <summary>
        /// Removes the target object from callbacks for its implemented callback-interfaces.
        /// </summary>
        /// <remarks>
        /// The covered callback interfaces are: IConnectionCallbacks, IMatchmakingCallbacks,
        /// ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback and IWebRpcCallback.
        ///
        /// See: <a href="https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks">.Net Callbacks</a>
        /// </remarks>
        /// <param name="target">The object that unregisters from getting callbacks.</param>
        public static void RemoveCallbackTarget(object target)
        {
            if (target is PhotonView || NetworkingClient == null)
            {
                return;
            }

            IPunOwnershipCallbacks punOwnershipCallback = target as IPunOwnershipCallbacks;
            if (punOwnershipCallback != null)
            {
                OnOwnershipRequestEv -= punOwnershipCallback.OnOwnershipRequest;
                OnOwnershipTransferedEv -= punOwnershipCallback.OnOwnershipTransfered;
            }

            NetworkingClient.RemoveCallbackTarget(target);
        }

        internal static string CallbacksToString()
        {
            var x = NetworkingClient.ConnectionCallbackTargets.Select(m => m.ToString()).ToArray();
            return string.Join(", ", x);
        }

        internal static byte currentLevelPrefix = 0;

        /// <summary>Internally used to flag if the message queue was disabled by a "scene sync" situation (to re-enable it).</summary>
        internal static bool loadingLevelAndPausedNetwork = false;

        /// <summary>For automatic scene syncing, the loaded scene is put into a room property. This is the name of said prop.</summary>
        internal const string CurrentSceneProperty = "curScn";
        internal const string CurrentScenePropertyLoadAsync = "curScnLa";


        /// <summary>
        /// An Object Pool can be used to keep and reuse instantiated object instances. Replaces Unity's default Instantiate and Destroy methods.
        /// </summary>
        /// <remarks>
        /// Defaults to the DefaultPool type.
        /// To use a GameObject pool, implement IPunPrefabPool and assign it here.
        /// Prefabs are identified by name.
        /// </remarks>
        public static IPunPrefabPool PrefabPool
        {
            get
            {
                return prefabPool; 
            }
            set
            {
                if (value == null)
                {
                    Debug.LogError("PhotonNetwork.PrefabPool cannot be set to null. Please check your code.");
                    return;
                }

                prefabPool = value;
            }
        }

        private static IPunPrefabPool prefabPool;

        /// <summary>
        /// While enabled, the MonoBehaviours on which we call RPCs are cached, avoiding costly GetComponents&lt;MonoBehaviour&gt;() calls.
        /// </summary>
        /// <remarks>
        /// RPCs are called on the MonoBehaviours of a target PhotonView. Those have to be found via GetComponents.
        ///
        /// When set this to true, the list of MonoBehaviours gets cached in each PhotonView.
        /// You can use photonView.RefreshRpcMonoBehaviourCache() to manually refresh a PhotonView's
        /// list of MonoBehaviours on demand (when a new MonoBehaviour gets added to a networked GameObject, e.g.).
        /// </remarks>
        public static bool UseRpcMonoBehaviourCache;

        private static readonly Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

        private static readonly Dictionary<string, int> rpcShortcuts;  // lookup "table" for the index (shortcut) of an RPC name


        // for asynchronous network synched loading.
        private static AsyncOperation _AsyncLevelLoadingOperation;

        private static float _levelLoadingProgress = 0f;

        /// <summary>
        /// Gets the networked level loading progress. Value will be be zero until the first loading, and remain at one in between loadings
        /// Use PhotonNetwork.LoadLevel() to initiate a networked level Loading
        /// </summary>
        /// <value>The level loading progress. Ranges from 0 to 1</value>
        public static float LevelLoadingProgress
        {
            get
            {
                if (_AsyncLevelLoadingOperation != null)
                {
                    _levelLoadingProgress = _AsyncLevelLoadingOperation.progress;
                }
                else if (_levelLoadingProgress > 0f)
                {
                    _levelLoadingProgress = 1f;
                }

                return _levelLoadingProgress;
            }
        }

        /// <summary>
        /// Called when "this client" left a room to clean up.
        /// </summary>
        /// <remarks>
        /// if (Server == ServerConnection.GameServer && (state == ClientState.Disconnecting || state == ClientState.DisconnectingFromGameserver))
        /// </remarks>
        private static void LeftRoomCleanup()
        {
            // Clean up if we were loading asynchronously.
            if (_AsyncLevelLoadingOperation != null)
            {
                _AsyncLevelLoadingOperation.allowSceneActivation = false;
                _AsyncLevelLoadingOperation = null;
            }


            bool wasInRoom = NetworkingClient.CurrentRoom != null;
            // when leaving a room, we clean up depending on that room's settings.
            bool autoCleanupSettingOfRoom = wasInRoom && CurrentRoom.AutoCleanUp;


            allowedReceivingGroups = new HashSet<byte>();
            blockedSendingGroups = new HashSet<byte>();

            // Cleanup all network objects (all spawned PhotonViews, local and remote)
            if (autoCleanupSettingOfRoom)
            {
                LocalCleanupAnythingInstantiated(true);
            }
        }


        /// <summary>
        /// Cleans up anything that was instantiated in-game (not loaded with the scene).
        /// </summary>
        internal static void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
        {
            //if (tempInstantiationData.Count > 0)
            //{
            //    Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite ");
            //}

            // Destroy GO's (if we should)
            if (destroyInstantiatedGameObjects)
            {
                // Fill list with Instantiated objects
                HashSet<GameObject> instantiatedGos = new HashSet<GameObject>();
                foreach (PhotonView view in photonViewList.Values)
                {
                    if (view.isRuntimeInstantiated)
                    {
                        instantiatedGos.Add(view.gameObject); // HashSet keeps each object only once
                    }
                }

                foreach (GameObject go in instantiatedGos)
                {
                    RemoveInstantiatedGO(go, true);
                }
            }

            // photonViewList is cleared of anything instantiated (so scene items are left inside)
            // any other lists can be
            PhotonNetwork.lastUsedViewSubId = 0;
            PhotonNetwork.lastUsedViewSubIdStatic = 0;
        }


        /// <summary>
        /// Resets the PhotonView "lastOnSerializeDataSent" so that "OnReliable" synched PhotonViews send a complete state to new clients (if the state doesnt change, no messages would be send otherwise!).
        /// Note that due to this reset, ALL other players will receive the full OnSerialize.
        /// </summary>
        private static void ResetPhotonViewsOnSerialize()
        {
            foreach (PhotonView photonView in photonViewList.Values)
            {
                photonView.lastOnSerializeDataSent = null;
            }
        }

        // PHOTONVIEW/RPC related

        /// <summary>
        /// Executes a received RPC event
        /// </summary>
        internal static void ExecuteRpc(Hashtable rpcData, Player sender)
        {
            if (rpcData == null || !rpcData.ContainsKey((byte)0))
            {
                Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClassPun.DictionaryToString(rpcData));
                return;
            }

            // ts: updated with "flat" event data
            int netViewID = (int)rpcData[(byte)0]; // LIMITS PHOTONVIEWS&PLAYERS
            int otherSidePrefix = 0;    // by default, the prefix is 0 (and this is not being sent)
            if (rpcData.ContainsKey((byte)1))
            {
                otherSidePrefix = (short)rpcData[(byte)1];
            }


            string inMethodName;
            if (rpcData.ContainsKey((byte)5))
            {
                int rpcIndex = (byte)rpcData[(byte)5];  // LIMITS RPC COUNT
                if (rpcIndex > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
                {
                    Debug.LogError("Could not find RPC with index: " + rpcIndex + ". Going to ignore! Check PhotonServerSettings.RpcList");
                    return;
                }
                else
                {
                    inMethodName = PhotonNetwork.PhotonServerSettings.RpcList[rpcIndex];
                }
            }
            else
            {
                inMethodName = (string)rpcData[(byte)3];
            }

            object[] inMethodParameters = null;
            if (rpcData.ContainsKey((byte)4))
            {
                inMethodParameters = (object[])rpcData[(byte)4];
            }

            if (inMethodParameters == null)
            {
                inMethodParameters = new object[0];
            }

            PhotonView photonNetview = GetPhotonView(netViewID);
            if (photonNetview == null)
            {
                int viewOwnerId = netViewID / PhotonNetwork.MAX_VIEW_IDS;
                bool owningPv = (viewOwnerId == NetworkingClient.LocalPlayer.ActorNumber);
                bool ownerSent = (viewOwnerId == sender.ActorNumber);

                if (owningPv)
                {
                    Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! View was/is ours." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender.ActorNumber);
                }
                else
                {
                    Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! Was remote PV." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender.ActorNumber + " Maybe GO was destroyed but RPC not cleaned up.");
                }
                return;
            }

            if (photonNetview.Prefix != otherSidePrefix)
            {
                Debug.LogError("Received RPC \"" + inMethodName + "\" on viewID " + netViewID + " with a prefix of " + otherSidePrefix + ", our prefix is " + photonNetview.Prefix + ". The RPC has been ignored.");
                return;
            }

            // Get method name
            if (string.IsNullOrEmpty(inMethodName))
            {
                Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClassPun.DictionaryToString(rpcData));
                return;
            }

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
                Debug.Log("Received RPC: " + inMethodName);


            // SetReceiving filtering
            if (photonNetview.Group != 0 && !allowedReceivingGroups.Contains(photonNetview.Group))
            {
                return; // Ignore group
            }

            Type[] argTypes = new Type[0];
            if (inMethodParameters.Length > 0)
            {
                argTypes = new Type[inMethodParameters.Length];
                int i = 0;
                for (int index = 0; index < inMethodParameters.Length; index++)
                {
                    object objX = inMethodParameters[index];
                    if (objX == null)
                    {
                        argTypes[i] = null;
                    }
                    else
                    {
                        argTypes[i] = objX.GetType();
                    }

                    i++;
                }
            }

            int receivers = 0;
            int foundMethods = 0;
            if (!PhotonNetwork.UseRpcMonoBehaviourCache || photonNetview.RpcMonoBehaviours == null || photonNetview.RpcMonoBehaviours.Length == 0)
            {
                photonNetview.RefreshRpcMonoBehaviourCache();
            }

            for (int componentsIndex = 0; componentsIndex < photonNetview.RpcMonoBehaviours.Length; componentsIndex++)
            {
                MonoBehaviour monob = photonNetview.RpcMonoBehaviours[componentsIndex];
                if (monob == null)
                {
                    Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
                    continue;
                }

                Type type = monob.GetType();

                // Get [PunRPC] methods from cache
                List<MethodInfo> cachedRPCMethods = null;
                bool methodsOfTypeInCache = monoRPCMethodsCache.TryGetValue(type, out cachedRPCMethods);

                if (!methodsOfTypeInCache)
                {
                    List<MethodInfo> entries = SupportClassPun.GetMethods(type, typeof(PunRPC));

                    monoRPCMethodsCache[type] = entries;
                    cachedRPCMethods = entries;
                }

                if (cachedRPCMethods == null)
                {
                    continue;
                }

                // Check cache for valid methodname+arguments
                for (int index = 0; index < cachedRPCMethods.Count; index++)
                {
                    MethodInfo mInfo = cachedRPCMethods[index];
                    if (mInfo.Name.Equals(inMethodName))
                    {
                        foundMethods++;
                        ParameterInfo[] pArray = mInfo.GetCachedParemeters();

                        if (pArray.Length == argTypes.Length)
                        {
                            // Normal, PhotonNetworkMessage left out
                            if (CheckTypeMatch(pArray, argTypes))
                            {
                                receivers++;
                                mInfo.Invoke((object)monob, inMethodParameters);
                            }
                        }
                        else if ((pArray.Length - 1) == argTypes.Length)
                        {
                            // Check for PhotonNetworkMessage being the last
                            if (CheckTypeMatch(pArray, argTypes))
                            {
                                if (pArray[pArray.Length - 1].ParameterType == typeof(PhotonMessageInfo))
                                {
                                    receivers++;

                                    int sendTime = (int)rpcData[(byte)2];
                                    object[] deParamsWithInfo = new object[inMethodParameters.Length + 1];
                                    inMethodParameters.CopyTo(deParamsWithInfo, 0);
                                    deParamsWithInfo[deParamsWithInfo.Length - 1] = new PhotonMessageInfo(sender, sendTime, photonNetview);

                                    mInfo.Invoke((object)monob, deParamsWithInfo);
                                }
                            }
                        }
                        else if (pArray.Length == 1 && pArray[0].ParameterType.IsArray)
                        {
                            receivers++;
                            mInfo.Invoke((object)monob, new object[] { inMethodParameters });
                        }
                    }
                }
            }

            // Error handling
            if (receivers != 1)
            {
                string argsString = string.Empty;
                for (int index = 0; index < argTypes.Length; index++)
                {
                    Type ty = argTypes[index];
                    if (argsString != string.Empty)
                    {
                        argsString += ", ";
                    }

                    if (ty == null)
                    {
                        argsString += "null";
                    }
                    else
                    {
                        argsString += ty.Name;
                    }
                }

                if (receivers == 0)
                {
                    if (foundMethods == 0)
                    {
                        Debug.LogError("PhotonView with ID " + netViewID + " has no method \"" + inMethodName + "\" marked with the [PunRPC](C#) or @PunRPC(JS) property! Args: " + argsString);
                    }
                    else
                    {
                        Debug.LogError("PhotonView with ID " + netViewID + " has no method \"" + inMethodName + "\" that takes " + argTypes.Length + " argument(s): " + argsString);
                    }
                }
                else
                {
                    Debug.LogError("PhotonView with ID " + netViewID + " has " + receivers + " methods \"" + inMethodName + "\" that takes " + argTypes.Length + " argument(s): " + argsString + ". Should be just one?");
                }
            }
        }

        /// <summary>
        /// Check if all types match with parameters. We can have more paramters then types (allow last RPC type to be different).
        /// </summary>
        /// <param name="methodParameters"></param>
        /// <param name="callParameterTypes"></param>
        /// <returns>If the types-array has matching parameters (of method) in the parameters array (which may be longer).</returns>
        private static bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
        {
            if (methodParameters.Length < callParameterTypes.Length)
            {
                return false;
            }

            for (int index = 0; index < callParameterTypes.Length; index++)
            {
                #if NETFX_CORE
                TypeInfo methodParamTI = methodParameters[index].ParameterType.GetTypeInfo();
                TypeInfo callParamTI = callParameterTypes[index].GetTypeInfo();

                if (callParameterTypes[index] != null && !methodParamTI.IsAssignableFrom(callParamTI) && !(callParamTI.IsEnum && System.Enum.GetUnderlyingType(methodParamTI.AsType()).GetTypeInfo().IsAssignableFrom(callParamTI)))
                {
                    return false;
                }
                #else
                Type type = methodParameters[index].ParameterType;
                if (callParameterTypes[index] != null && !type.IsAssignableFrom(callParameterTypes[index]) && !(type.IsEnum && System.Enum.GetUnderlyingType(type).IsAssignableFrom(callParameterTypes[index])))
                {
                    return false;
                }
                #endif
            }

            return true;
        }


        
        //internal static GameObject DoInstantiate(Hashtable evData, Player player, GameObject resourceGameObject)
        //{
        //    // some values always present:
        //    string prefabName = (string)evData[(byte)0];
        //    int serverTime = (int)evData[(byte)6];
        //    int instantiationId = (int)evData[(byte)7];

        //    Vector3 position;
        //    if (evData.ContainsKey((byte)1))
        //    {
        //        position = (Vector3)evData[(byte)1];
        //    }
        //    else
        //    {
        //        position = Vector3.zero;
        //    }

        //    Quaternion rotation = Quaternion.identity;
        //    if (evData.ContainsKey((byte)2))
        //    {
        //        rotation = (Quaternion)evData[(byte)2];
        //    }

        //    byte group = 0;
        //    if (evData.ContainsKey((byte)3))
        //    {
        //        group = (byte)evData[(byte)3];
        //    }

        //    short objLevelPrefix = 0;
        //    if (evData.ContainsKey((byte)8))
        //    {
        //        objLevelPrefix = (short)evData[(byte)8];
        //    }

        //    int[] viewsIDs;
        //    if (evData.ContainsKey((byte)4))
        //    {
        //        viewsIDs = (int[])evData[(byte)4];
        //    }
        //    else
        //    {
        //        viewsIDs = new int[1] { instantiationId };
        //    }

        //    object[] incomingInstantiationData;
        //    if (evData.ContainsKey((byte)5))
        //    {
        //        incomingInstantiationData = (object[])evData[(byte)5];
        //    }
        //    else
        //    {
        //        incomingInstantiationData = null;
        //    }

        //    // SetReceiving filtering
        //    if (group != 0 && !allowedReceivingGroups.Contains(group))
        //    {
        //        return null; // Ignore group
        //    }

        //    if (ObjectPool != null)
        //    {
        //        GameObject go = ObjectPool.Instantiate(prefabName, position, rotation);

        //        PhotonView[] photonViews = go.GetPhotonViewsInChildren();
        //        if (photonViews.Length != viewsIDs.Length)
        //        {
        //            throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
        //        }
        //        for (int i = 0; i < photonViews.Length; i++)
        //        {
        //            photonViews[i].didAwake = false;
        //            photonViews[i].ViewID = 0;

        //            photonViews[i].Prefix = objLevelPrefix;
        //            photonViews[i].InstantiationId = instantiationId;
        //            photonViews[i].isRuntimeInstantiated = true;
        //            photonViews[i].instantiationDataField = incomingInstantiationData;

        //            photonViews[i].didAwake = true;
        //            photonViews[i].ViewID = viewsIDs[i];    // with didAwake true and viewID == 0, this will also register the view
        //        }


        //        // if IPunInstantiateMagicCallback is implemented on any script of the instantiated GO, let's call it directly:
        //        var list = go.GetComponents<IPunInstantiateMagicCallback>();
        //        if (list.Length > 0)
        //        {
        //            PhotonMessageInfo pmi = new PhotonMessageInfo(player, serverTime, null);
        //            foreach (IPunInstantiateMagicCallback callbackComponent in list)
        //            {
        //                callbackComponent.OnPhotonInstantiate(pmi);
        //            }
        //        }
        //        return go;
        //    }
        //    else
        //    {
        //        // load prefab, if it wasn't loaded before (calling methods might do this)
        //        if (resourceGameObject == null)
        //        {
        //            if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out resourceGameObject))
        //            {
        //                resourceGameObject = (GameObject)Resources.Load(prefabName, typeof(GameObject));
        //                if (UsePrefabCache)
        //                {
        //                    PrefabCache.Add(prefabName, resourceGameObject);
        //                }
        //            }

        //            if (resourceGameObject == null)
        //            {
        //                Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder.");
        //                return null;
        //            }
        //        }

        //        // now modify the loaded "blueprint" object before it becomes a part of the scene (by instantiating it)
        //        PhotonView[] resourcePVs = resourceGameObject.GetPhotonViewsInChildren();
        //        if (resourcePVs.Length != viewsIDs.Length)
        //        {
        //            throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
        //        }

        //        for (int i = 0; i < viewsIDs.Length; i++)
        //        {
        //            // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
        //            // so we only set the viewID and instantiationId now. the InstantiationData can be fetched
        //            resourcePVs[i].ViewID = viewsIDs[i];
        //            resourcePVs[i].Prefix = objLevelPrefix;
        //            resourcePVs[i].InstantiationId = instantiationId;
        //            resourcePVs[i].isRuntimeInstantiated = true;
        //        }

        //        StoreInstantiationData(instantiationId, incomingInstantiationData);

        //        // load the resource and set it's values before instantiating it:
        //        GameObject go = (GameObject)GameObject.Instantiate(resourceGameObject, position, rotation);

        //        for (int i = 0; i < viewsIDs.Length; i++)
        //        {
        //            // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
        //            // so we only set the viewID and instantiationId now. the InstantiationData can be fetched
        //            resourcePVs[i].ViewID = 0;
        //            resourcePVs[i].Prefix = -1;
        //            resourcePVs[i].prefixField = -1;
        //            resourcePVs[i].InstantiationId = -1;
        //            resourcePVs[i].isRuntimeInstantiated = false;
        //        }


        //        // if IPunInstantiateMagicCallback is implemented on any script of the instantiated GO, let's call it directly:
        //        var list = go.GetComponents<IPunInstantiateMagicCallback>();
        //        if (list.Length > 0)
        //        {
        //            PhotonMessageInfo pmi = new PhotonMessageInfo(player, serverTime, null);
        //            foreach (IPunInstantiateMagicCallback callbackComponent in list)
        //            {
        //                callbackComponent.OnPhotonInstantiate(pmi);
        //            }
        //        }


        //        RemoveInstantiationData(instantiationId);
        //        return go;
        //    }
        //}

        //private static Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();

        //private static void StoreInstantiationData(int instantiationId, object[] instantiationData)
        //{
        //    // Debug.Log("StoreInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        //    tempInstantiationData[instantiationId] = instantiationData;
        //}

        //public static object[] FetchInstantiationData(int instantiationId)
        //{
        //    object[] data = null;
        //    if (instantiationId == 0)
        //    {
        //        return null;
        //    }

        //    tempInstantiationData.TryGetValue(instantiationId, out data);
        //    // Debug.Log("FetchInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        //    return data;
        //}

        //private static void RemoveInstantiationData(int instantiationId)
        //{
        //    tempInstantiationData.Remove(instantiationId);
        //}


        /// <summary>
        /// Destroys all Instantiates and RPCs locally and (if not localOnly) sends EvDestroy(player) and clears related events in the server buffer.
        /// </summary>
        public static void DestroyPlayerObjects(int playerId, bool localOnly)
        {
            if (playerId <= 0)
            {
                Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
                return;
            }

            if (!localOnly)
            {
                // clean server's Instantiate and RPC buffers
                OpRemoveFromServerInstantiationsOfPlayer(playerId);
                OpCleanActorRpcBuffer(playerId);

                // send Destroy(player) to anyone else
                SendDestroyOfPlayer(playerId);
            }

            // locally cleaning up that player's objects
            HashSet<GameObject> playersGameObjects = new HashSet<GameObject>();
            foreach (PhotonView view in photonViewList.Values)
            {
                if (view != null && view.CreatorActorNr == playerId)
                {
                    playersGameObjects.Add(view.gameObject);
                }
            }

            // any non-local work is already done, so with the list of that player's objects, we can clean up (locally only)
            foreach (GameObject gameObject in playersGameObjects)
            {
                RemoveInstantiatedGO(gameObject, true);
            }

            // with ownership transfer, some objects might lose their owner.
            // in that case, the creator becomes the owner again. every client can apply  done below.
            foreach (PhotonView view in photonViewList.Values)
            {
                if (view.OwnerActorNr == playerId)
                {
                    view.OwnerActorNr = view.CreatorActorNr;    //TODO: for scene objects, the Master Client should become owner
                    //Debug.Log("Creator is: " + view.OwnerActorNr);
                }
            }
        }

        public static void DestroyAll(bool localOnly)
        {
            if (!localOnly)
            {
                OpRemoveCompleteCache();
                SendDestroyOfAll();
            }

            LocalCleanupAnythingInstantiated(true);
        }

        /// <summary>Removes GameObject and the PhotonViews on it from local lists and optionally updates remotes. GameObject gets destroyed at end.</summary>
        /// <remarks>
        /// This method might fail and quit early due to several tests.
        /// </remarks>
        /// <param name="go">GameObject to cleanup.</param>
        /// <param name="localOnly">For localOnly, tests of control are skipped and the server is not updated.</param>
        internal static void RemoveInstantiatedGO(GameObject go, bool localOnly)
        {
            if (go == null)
            {
                Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
                return;
            }

            // Don't remove the GO if it doesn't have any PhotonView
            PhotonView[] views = go.GetComponentsInChildren<PhotonView>(true);
            if (views == null || views.Length <= 0)
            {
                Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
                return;
            }

            PhotonView viewZero = views[0];
            int creatorId = viewZero.CreatorActorNr;            // creatorId of obj is needed to delete EvInstantiate (only if it's from that user)
            int instantiationId = viewZero.InstantiationId;     // actual, live InstantiationIds start with 1 and go up
            
            // Don't remove GOs that are owned by others (unless this is the master and the remote player left)
            if (!localOnly)
            {
                //Debug.LogWarning("Destroy " + instantiationId + " creator " + creatorId, go);
                if (!viewZero.IsMine)
                {
                    Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor MasterClient taking over for owner who left: " + viewZero);
                    return;
                }

                // Don't remove the Instantiation from the server, if it doesn't have a proper ID
                if (instantiationId < 1)
                {
                    Debug.LogError("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: " + viewZero + ". Not Destroying GameObject or PhotonViews!");
                    return;
                }
            }


            // cleanup instantiation (event and local list)
            if (!localOnly)
            {
                ServerCleanInstantiateAndDestroy(instantiationId, creatorId, viewZero.isRuntimeInstantiated);   // server cleaning
            }


            // cleanup PhotonViews and their RPCs events (if not localOnly)
            for (int j = views.Length - 1; j >= 0; j--)
            {
                PhotonView view = views[j];
                if (view == null)
                {
                    continue;
                }

                // we only destroy/clean PhotonViews that were created by PhotonNetwork.Instantiate (and those have an instantiationId!)
                if (view.InstantiationId >= 1)
                {
                    LocalCleanPhotonView(view);
                }
                if (!localOnly)
                {
                    OpCleanRpcBuffer(view);
                }
            }

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
            {
                Debug.Log("Network destroy Instantiated GO: " + go.name);
            }

            go.SetActive(false);            // PUN 2 disables objects before the return to the pool
            prefabPool.Destroy(go);         // PUN 2 always uses a PrefabPool (even for the default implementation)
        }



        private static readonly ExitGames.Client.Photon.Hashtable removeFilter = new ExitGames.Client.Photon.Hashtable();
        private static readonly ExitGames.Client.Photon.Hashtable ServerCleanDestroyEvent = new ExitGames.Client.Photon.Hashtable();
        private static readonly RaiseEventOptions ServerCleanOptions = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache };

        /// <summary>
        /// Removes an instantiation event from the server's cache. Needs id and actorNr of player who instantiated.
        /// </summary>
        private static void ServerCleanInstantiateAndDestroy(int instantiateId, int creatorId, bool isRuntimeInstantiated)
        {
            // remove the Instantiate-event from the server cache:
            removeFilter[(byte)7] = instantiateId;
            ServerCleanOptions.CachingOption = EventCaching.RemoveFromRoomCache;

            PhotonNetwork.RaiseEventInternal(PunEvent.Instantiation, removeFilter, ServerCleanOptions, SendOptions.SendReliable);


            // send a Destroy-event to everyone (removing an event from the cache, doesn't send this to anyone else):
            ServerCleanDestroyEvent[(byte)0] = instantiateId;
            ServerCleanOptions.CachingOption = (isRuntimeInstantiated) ? EventCaching.DoNotCache : EventCaching.AddToRoomCacheGlobal;   // if the view got loaded with the scene, cache EvDestroy for anyone (re)joining later

            PhotonNetwork.RaiseEventInternal(PunEvent.Destroy, ServerCleanDestroyEvent, ServerCleanOptions, SendOptions.SendReliable);
        }

        private static void SendDestroyOfPlayer(int actorNr)
        {
            ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();
            evData[(byte)0] = actorNr;

            PhotonNetwork.RaiseEventInternal(PunEvent.DestroyPlayer, evData,null, SendOptions.SendReliable);
          	//NetworkingClient.OpRaiseEvent(PunEvent.DestroyPlayer, evData, null, new SendOptions() { Reliability = true });
            //NetworkingClient.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
        }

        private static void SendDestroyOfAll()
        {
            ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();
            evData[(byte)0] = -1;

            PhotonNetwork.RaiseEventInternal(PunEvent.DestroyPlayer, evData,null, SendOptions.SendReliable);
            //NetworkingClient.OpRaiseEvent(PunEvent.DestroyPlayer, evData, null , new SendOptions() { Reliability = true });
            //NetworkingClient.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
        }

        private static void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
        {
            // removes all "Instantiation" events of player actorNr. this is not an event for anyone else
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNr } };
            PhotonNetwork.RaiseEventInternal(PunEvent.Instantiation, null, options, SendOptions.SendReliable);
            //NetworkingClient.OpRaiseEvent(PunEvent.Instantiation, null, options, new SendOptions() { Reliability = true });
            //NetworkingClient.OpRaiseEvent(PunEvent.Instantiation, null, true, 0, new int[] { actorNr }, EventCaching.RemoveFromRoomCache);
        }

        internal static void RequestOwnership(int viewID, int fromOwner)
        {
            Debug.Log("RequestOwnership(): " + viewID + " from: " + fromOwner + " Time: " + Environment.TickCount % 1000);
            //PhotonNetwork.NetworkingClient.OpRaiseEvent(PunEvent.OwnershipRequest, true, new int[] { viewID, fromOwner }, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
            PhotonNetwork.RaiseEventInternal(PunEvent.OwnershipRequest, new int[] { viewID, fromOwner },new RaiseEventOptions() { Receivers = ReceiverGroup.All },SendOptions.SendReliable);
            //NetworkingClient.OpRaiseEvent(PunEvent.OwnershipRequest, new int[] { viewID, fromOwner }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, new SendOptions() { Reliability = true });   // All sends to all via server (including self)
        }

        internal static void TransferOwnership(int viewID, int playerID)
        {
            Debug.Log("TransferOwnership() view " + viewID + " to: " + playerID + " Time: " + Environment.TickCount % 1000);
            //PhotonNetwork.NetworkingClient.OpRaiseEvent(PunEvent.OwnershipTransfer, true, new int[] {viewID, playerID}, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
            PhotonNetwork.RaiseEventInternal(PunEvent.OwnershipTransfer, new int[] { viewID, playerID }, new RaiseEventOptions() { Receivers = ReceiverGroup.All },SendOptions.SendReliable);
            //NetworkingClient.OpRaiseEvent(PunEvent.OwnershipTransfer, new int[] { viewID, playerID }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, new SendOptions() { Reliability = true });   // All sends to all via server (including self)
        }

        public static bool LocalCleanPhotonView(PhotonView view)
        {
            view.removedFromLocalViewList = true;
            return photonViewList.Remove(view.ViewID);
        }

        public static PhotonView GetPhotonView(int viewID)
        {
            PhotonView result = null;
            photonViewList.TryGetValue(viewID, out result);

            if (result == null)
            {
                PhotonView[] views = GameObject.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];

                for (int i = 0; i < views.Length; i++)
                {
                    PhotonView view = views[i];
                    if (view.ViewID == viewID)
                    {
                        if (view.didAwake)
                        {
                            Debug.LogWarning("Had to lookup view that wasn't in photonViewList: " + view);
                        }
                        return view;
                    }
                }
            }

            return result;
        }

        public static void RegisterPhotonView(PhotonView netView)
        {
            if (!Application.isPlaying)
            {
                photonViewList = new Dictionary<int, PhotonView>();
                return;
            }

            if (netView.ViewID == 0)
            {
                // don't register views with ID 0 (not initialized). they register when a ID is assigned later on
                Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + netView);
                return;
            }

            PhotonView listedView = null;
            bool isViewListed = photonViewList.TryGetValue(netView.ViewID, out listedView);
            if (isViewListed)
            {
                // if some other view is in the list already, we got a problem. it might be undestructible. print out error
                if (netView != listedView)
                {
                    Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.ViewID, netView, listedView));
                }
                else
                {
                    return;
                }

                RemoveInstantiatedGO(listedView.gameObject, true);
            }

            // Debug.Log("adding view to known list: " + netView);
            photonViewList.Add(netView.ViewID, netView);
            //Debug.LogError("view being added. " + netView);	// Exit Games internal log

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
            {
                Debug.Log("Registered PhotonView: " + netView.ViewID);
            }
        }


        /// <summary>
        /// Removes the RPCs of someone else (to be used as master).
        /// This won't clean any local caches. It just tells the server to forget a player's RPCs and instantiates.
        /// </summary>
        /// <param name="actorNumber"></param>
        public static void OpCleanActorRpcBuffer(int actorNumber)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
            PhotonNetwork.RaiseEventInternal(PunEvent.RPC, null, options, SendOptions.SendReliable);
            //NetworkingClient.OpRaiseEvent(PunEvent.RPC, null, options, new SendOptions() { Reliability = true });
            //NetworkingClient.OpRaiseEvent(PunEvent.RPC, null, true, 0, new int[] { actorNumber }, EventCaching.RemoveFromRoomCache);
        }

        /// <summary>
        /// Instead removing RPCs or Instantiates, this removed everything cached by the actor.
        /// </summary>
        /// <param name="actorNumber"></param>
        public static void OpRemoveCompleteCacheOfPlayer(int actorNumber)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
            PhotonNetwork.RaiseEventInternal(0, null, options, SendOptions.SendReliable);
            //NetworkingClient.OpRaiseEvent(0, null, options, new SendOptions() { Reliability = true });
        }


        public static void OpRemoveCompleteCache()
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, Receivers = ReceiverGroup.MasterClient };
            PhotonNetwork.RaiseEventInternal(0, null, options, SendOptions.SendReliable);
            //NetworkingClient.OpRaiseEvent(0, null, options, new SendOptions() { Reliability = true });  // TODO check if someone gets this event
        }

        /// This clears the cache of any player/actor who's no longer in the room (making it a simple clean-up option for a new master)
        private static void RemoveCacheOfLeftPlayers()
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters[ParameterCode.Code] = (byte)0;		// any event
            opParameters[ParameterCode.Cache] = (byte)EventCaching.RemoveFromRoomCacheForActorsLeft;    // option to clear the room cache of all events of players who left

            NetworkingClient.LoadBalancingPeer.SendOperation((byte)OperationCode.RaiseEvent, opParameters, SendOptions.SendReliable);   // TODO: Check if this is the best implementation possible
        }

        // Remove RPCs of view (if they are local player's RPCs)
        public static void CleanRpcBufferIfMine(PhotonView view)
        {
            if (view.OwnerActorNr != NetworkingClient.LocalPlayer.ActorNumber && !NetworkingClient.LocalPlayer.IsMasterClient)
            {
                Debug.LogError("Cannot remove cached RPCs on a PhotonView thats not ours! " + view.Owner + " scene: " + view.IsSceneView);
                return;
            }

            OpCleanRpcBuffer(view);
        }


        private static readonly Hashtable rpcFilterByViewId = new ExitGames.Client.Photon.Hashtable();
        private static readonly RaiseEventOptions OpCleanRpcBufferOptions = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache };

        /// <summary>Cleans server RPCs for PhotonView (without any further checks).</summary>
        public static void OpCleanRpcBuffer(PhotonView view)
        {
            rpcFilterByViewId[(byte)0] = view.ViewID;
            PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcFilterByViewId, OpCleanRpcBufferOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Remove all buffered RPCs from server that were sent in the targetGroup, if this is the Master Client or if this controls the individual PhotonView.
        /// </summary>
        /// <remarks>
        /// This method requires either:
        /// - This client is the Master Client (can remove any RPCs per group).
        /// - Any other client: each PhotonView is checked if it is under this client's control. Only those RPCs are removed.
        /// </remarks>
        /// <param name="group">Interest group that gets all RPCs removed.</param>
        public static void RemoveRPCsInGroup(int group)
        {
            foreach (PhotonView view in photonViewList.Values)
            {
                if (view.Group == group)
                {
                    CleanRpcBufferIfMine(view);
                }
            }
        }


        /// <summary>
        /// Sets level prefix for PhotonViews instantiated later on. Don't set it if you need only one!
        /// </summary>
        /// <remarks>
        /// Important: If you don't use multiple level prefixes, simply don't set this value. The
        /// default value is optimized out of the traffic.
        ///
        /// This won't affect existing PhotonViews (they can't be changed yet for existing PhotonViews).
        ///
        /// Messages sent with a different level prefix will be received but not executed. This affects
        /// RPCs, Instantiates and synchronization.
        ///
        /// Be aware that PUN never resets this value, you'll have to do so yourself.
        /// </remarks>
        /// <param name="prefix">Max value is short.MaxValue = 255</param>
        public static void SetLevelPrefix(byte prefix)
        {
            // TODO: check can use network

            currentLevelPrefix = prefix;
            // TODO: should we really change the prefix for existing PVs?! better keep it!
            //foreach (PhotonView view in photonViewList.Values)
            //{
            //    view.prefix = prefix;
            //}
        }


        /// RPC Hashtable Structure
        /// (byte)0 -> (int) ViewId (combined from actorNr and actor-unique-id)
        /// (byte)1 -> (short) prefix (level)
        /// (byte)2 -> (int) server timestamp
        /// (byte)3 -> (string) methodname
        /// (byte)4 -> (object[]) parameters
        /// (byte)5 -> (byte) method shortcut (alternative to name)
        ///
        /// This is sent as event (code: 200) which will contain a sender (origin of this RPC).

        internal static void RPC(PhotonView view, string methodName, RpcTarget target, Player player, bool encrypt, params object[] parameters)
        {
            if (blockedSendingGroups.Contains(view.Group))
            {
                return; // Block sending on this group
            }

            if (view.ViewID < 1)
            {
                Debug.LogError("Illegal view ID:" + view.ViewID + " method: " + methodName + " GO:" + view.gameObject.name);
            }

            if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
            {
                Debug.Log("Sending RPC \"" + methodName + "\" to target: " + target + " or player:" + player + ".");
            }


            //ts: changed RPCs to a one-level hashtable as described in internal.txt
            ExitGames.Client.Photon.Hashtable rpcEvent = new ExitGames.Client.Photon.Hashtable();
            rpcEvent[(byte)0] = (int)view.ViewID; // LIMITS NETWORKVIEWS&PLAYERS
            if (view.Prefix > 0)
            {
                rpcEvent[(byte)1] = (short)view.Prefix;
            }
            rpcEvent[(byte)2] = PhotonNetwork.ServerTimestamp;


            // send name or shortcut (if available)
            int shortcut = 0;
            if (rpcShortcuts.TryGetValue(methodName, out shortcut))
            {
                rpcEvent[(byte)5] = (byte)shortcut; // LIMITS RPC COUNT
            }
            else
            {
                rpcEvent[(byte)3] = methodName;
            }

            if (parameters != null && parameters.Length > 0)
            {
                rpcEvent[(byte)4] = (object[])parameters;
            }

            SendOptions _reliableEncrypt =	new SendOptions () { Reliability = true, Encrypt = encrypt };

            // if sent to target player, this overrides the target
            if (player != null)
            {
                if (NetworkingClient.LocalPlayer.ActorNumber == player.ActorNumber)
                {
                    ExecuteRpc(rpcEvent, player);
                }
                else
                {
                    RaiseEventOptions options = new RaiseEventOptions() { TargetActors = new int[] { player.ActorNumber } };
                    PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                   // NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });
                }

                return;
            }

            // send to a specific set of players
            if (target == RpcTarget.All)
            {
                RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.Group };
                PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                //NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });

                // Execute local
                ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
            }
            else if (target == RpcTarget.Others)
            {
                RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.Group };
                PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                //NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });
            }
            else if (target == RpcTarget.AllBuffered)
            {
                RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache };
                PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                //NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });

                // Execute local
                ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
            }
            else if (target == RpcTarget.OthersBuffered)
            {
                RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache };
                PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                //NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });
            }
            else if (target == RpcTarget.MasterClient)
            {
                if (NetworkingClient.LocalPlayer.IsMasterClient)
                {
                    ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                }
                else
                {
                    RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient };
                    PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                    //NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });
                }
            }
            else if (target == RpcTarget.AllViaServer)
            {
                RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.Group, Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                //NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });
                if (PhotonNetwork.OfflineMode)
                {
                    ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                }
            }
            else if (target == RpcTarget.AllBufferedViaServer)
            {
                RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.Group, Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache };
                PhotonNetwork.RaiseEventInternal(PunEvent.RPC, rpcEvent, options, _reliableEncrypt);
                //NetworkingClient.OpRaiseEvent(PunEvent.RPC, rpcEvent, options, new SendOptions() { Reliability = true, Encrypt = encrypt });
                if (PhotonNetwork.OfflineMode)
                {
                    ExecuteRpc(rpcEvent, NetworkingClient.LocalPlayer);
                }
            }
            else
            {
                Debug.LogError("Unsupported target enum: " + target);
            }
        }


        /// <summary>Enable/disable receiving on given Interest Groups (applied to PhotonViews).</summary>
        /// <remarks>
        /// A client can tell the server which Interest Groups it's interested in.
        /// The server will only forward events for those Interest Groups to that client (saving bandwidth and performance).
        ///
        /// See: https://doc.photonengine.com/en-us/pun/v2/gameplay/interestgroups
        ///
        /// See: https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/package-demos/culling-demo
        /// </remarks>
        /// <param name="disableGroups">The interest groups to disable (or null).</param>
        /// <param name="enableGroups">The interest groups to enable (or null).</param>
        public static void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
        {
            // TODO: check can use network

            if (disableGroups != null)
            {
                if (disableGroups.Length == 0)
                {
                    // a byte[0] should disable ALL groups in one step and before any groups are enabled. we do this locally, too.
                    allowedReceivingGroups.Clear();
                }
                else
                {
                    for (int index = 0; index < disableGroups.Length; index++)
                    {
                        byte g = disableGroups[index];
                        if (g <= 0)
                        {
                            Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + g + ". The Group number should be at least 1.");
                            continue;
                        }

                        if (allowedReceivingGroups.Contains(g))
                        {
                            allowedReceivingGroups.Remove(g);
                        }
                    }
                }
            }

            if (enableGroups != null)
            {
                if (enableGroups.Length == 0)
                {
                    // a byte[0] should enable ALL groups in one step. we do this locally, too.
                    for (byte index = 0; index <= byte.MaxValue; index++)
                    {
                        allowedReceivingGroups.Add(index);
                    }
                }
                else
                {
                    for (int index = 0; index < enableGroups.Length; index++)
                    {
                        byte g = enableGroups[index];
                        if (g <= 0)
                        {
                            Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + g + ". The Group number should be at least 1.");
                            continue;
                        }

                        allowedReceivingGroups.Add(g);
                    }
                }
            }

            NetworkingClient.OpChangeGroups(disableGroups, enableGroups);
        }


        /// <summary>Enable/disable sending on given group (applied to PhotonViews)</summary>
        /// <remarks>
        /// This does not interact with the Photon server-side.
        /// It's just a client-side setting to suppress updates, should they be sent to one of the blocked groups.
        ///
        /// This setting is not particularly useful, as it means that updates literally never reach the server or anyone else.
        /// Use with care.
        /// </remarks>
        /// <param name="group">The interest group to affect.</param>
        /// <param name="enabled">Sets if sending to group is enabled (or not).</param>
        public static void SetSendingEnabled(byte group, bool enabled)
        {
            // TODO: check can use network

            if (!enabled)
            {
                blockedSendingGroups.Add(group); // can be added to HashSet no matter if already in it
            }
            else
            {
                blockedSendingGroups.Remove(group);
            }
        }



        /// <summary>Enable/disable sending on given groups (applied to PhotonViews)</summary>
        /// <remarks>
        /// This does not interact with the Photon server-side.
        /// It's just a client-side setting to suppress updates, should they be sent to one of the blocked groups.
        ///
        /// This setting is not particularly useful, as it means that updates literally never reach the server or anyone else.
        /// Use with care.
        /// <param name="enableGroups">The interest groups to enable sending on (or null).</param>
        /// <param name="disableGroups">The interest groups to disable sending on (or null).</param>
        public static void SetSendingEnabled(byte[] disableGroups, byte[] enableGroups)
        {
            // TODO: check can use network

            if (disableGroups != null)
            {
                for (int index = 0; index < disableGroups.Length; index++)
                {
                    byte g = disableGroups[index];
                    blockedSendingGroups.Add(g);
                }
            }

            if (enableGroups != null)
            {
                for (int index = 0; index < enableGroups.Length; index++)
                {
                    byte g = enableGroups[index];
                    blockedSendingGroups.Remove(g);
                }
            }
        }


        internal static void NewSceneLoaded()
        {
            if (loadingLevelAndPausedNetwork)
            {
                if (_AsyncLevelLoadingOperation != null)
                {
                    _AsyncLevelLoadingOperation = null;
                }

                loadingLevelAndPausedNetwork = false;
                PhotonNetwork.IsMessageQueueRunning = true;
            }

            // Debug.Log("OnLevelWasLoaded photonViewList.Count: " + photonViewList.Count); // Exit Games internal log

            List<int> removeKeys = new List<int>();
            foreach (KeyValuePair<int, PhotonView> kvp in photonViewList)
            {
                PhotonView view = kvp.Value;
                if (view == null)
                {
                    removeKeys.Add(kvp.Key);
                }
            }

            for (int index = 0; index < removeKeys.Count; index++)
            {
                int key = removeKeys[index];
                photonViewList.Remove(key);
            }

            if (removeKeys.Count > 0)
            {
                if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                    Debug.Log("New level loaded. Removed " + removeKeys.Count + " scene view IDs from last level.");
            }
        }


        /// <summary>
        /// Defines how many OnPhotonSerialize()-calls might get summarized in one message.
        /// </summary>
        /// <remarks>
        /// A low number increases overhead, a high number might mean fragmentation.
        /// </remarks>
        public static int ObjectsInOneUpdate = 10;


        private static readonly PhotonStream serializeStreamOut = new PhotonStream(true, null);
        private static readonly PhotonStream serializeStreamIn = new PhotonStream(false, null);


        ///<summary> cache the RaiseEventOptions to prevent redundant Memory Allocation</summary>
        private static RaiseEventOptions serializeRaiseEvOptions = new RaiseEventOptions();

    private struct RaiseEventBatch: IEquatable<RaiseEventBatch>
    {
        public byte Group;
        public bool Reliable;

        public override int GetHashCode()
        {
            return (this.Group << 1) + (this.Reliable ? 1 : 0);
        }

        public bool Equals(RaiseEventBatch other)
        {
            return this.Reliable == other.Reliable && this.Group == other.Group;
        }
    }


    private class SerializeViewBatch : IEquatable<SerializeViewBatch>, IEquatable<RaiseEventBatch>
    {
        public readonly RaiseEventBatch Batch;
        public int Count;
        public object[] ObjectUpdates;
        private int defaultSize = 20;
        private int offset;


        // the offset enables us to skip the first X entries in the ObjectUpdate(s), leaving room for (e.g.) timestamp of sending and level prefix
        public SerializeViewBatch(RaiseEventBatch batch, int offset)
        {
            this.Batch = batch;
            this.ObjectUpdates = new object[this.defaultSize];  // TODO: if the number of photonviews is less than defaultSize, use less entries
            this.Count = offset;
            this.offset = offset;
        }

        public override int GetHashCode()
        {
            return (this.Batch.Group << 1) + (this.Batch.Reliable ? 1 : 0);
        }

        public bool Equals(SerializeViewBatch other)
        {
            return this.Equals(other.Batch);
        }

        public bool Equals(RaiseEventBatch other)
        {
            return this.Batch.Reliable == other.Reliable && this.Batch.Group == other.Group;
        }

        public override bool Equals(object obj)
        {
            SerializeViewBatch other = obj as SerializeViewBatch;
            return other != null && this.Batch.Equals(other.Batch);
        }

        public void Clear()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.ObjectUpdates[i] = null;
            }
            this.Count = this.offset;
        }

        public void Add(object[] viewData)
        {
            if (this.Count >= this.ObjectUpdates.Length)
            {
                // TODO: trim to new size
                throw new Exception("Can't add. Size exceeded.");
            }

            this.ObjectUpdates[this.Count++] = viewData;
        }
    }


        private static readonly Dictionary<RaiseEventBatch, SerializeViewBatch> serializeViewBatches = new Dictionary<RaiseEventBatch, SerializeViewBatch>();


        /// <summary>Calls all locally controlled PhotonViews to write their updates in OnPhotonSerializeView. Called by a PhotonHandler.</summary>
        internal static void RunViewUpdate()
        {
            if (_cachedRegionHandler != null)
            {
                BestRegionSummaryInPreferences = _cachedRegionHandler.SummaryToCache;
                _cachedRegionHandler = null;
            }

            if (PhotonNetwork.OfflineMode || CurrentRoom == null || CurrentRoom.Players == null)
            {
                return;
            }


            // no need to send OnSerialize messages while being alone (these are not buffered anyway)
            #if !PHOTON_DEVELOP
            if (CurrentRoom.Players.Count <= 1)
            {
                return;
            }
            #else
            serializeRaiseEvOptions.Receivers = (CurrentRoom.Players.Count == 1) ? ReceiverGroup.All : ReceiverGroup.Others;
            #endif



           /* Format of the event's data object[]:
            *  [0] = PhotonNetwork.ServerTimestamp;
            *  [1] = currentLevelPrefix;  OPTIONAL!
            *  [2] = object[] of PhotonView x
            *  [3] = object[] of PhotonView y or NULL
            *  [...]
            *
            *  We only combine updates for XY objects into one RaiseEvent to avoid fragmentation.
            *  The Reliability and Interest Group are only used for RaiseEvent and not contained in the event/data that reaches the other clients.
            *  This is read in OnEvent().
            */


            var enumerator = photonViewList.GetEnumerator();   // replacing foreach (PhotonView view in this.photonViewList.Values) for memory allocation improvement
            while (enumerator.MoveNext())
            {
                PhotonView view = enumerator.Current.Value;

                // a client only sends updates for active, synchronized PhotonViews that are under it's control (isMine)
                if (view.Synchronization == ViewSynchronization.Off || view.IsMine == false || view.isActiveAndEnabled == false)
                {
                    continue;
                }

                if (blockedSendingGroups.Contains(view.Group))
                {
                    continue; // Block sending on this group
                }


                // call the PhotonView's serialize method(s)
                object[] evData = OnSerializeWrite(view);
                if (evData == null)
                {
                    continue;
                }

                RaiseEventBatch eventBatch = new RaiseEventBatch();
                eventBatch.Reliable = view.Synchronization == ViewSynchronization.ReliableDeltaCompressed || view.mixedModeIsReliable;
                eventBatch.Group = view.Group;

                SerializeViewBatch svBatch = null;
                bool found = serializeViewBatches.TryGetValue(eventBatch, out svBatch);
                if (!found)
                {
                    svBatch = new SerializeViewBatch(eventBatch, 2);    // NOTE: the 2 first entries are kept empty for timestamp and level prefix
                    serializeViewBatches.Add(eventBatch, svBatch);
                }

                svBatch.Add(evData);
                if (svBatch.Count == svBatch.ObjectUpdates.Length)
                {
                    SendSerializeViewBatch(svBatch);
                }
            }

            var enumeratorB = serializeViewBatches.GetEnumerator();
            while (enumeratorB.MoveNext())
            {
                SendSerializeViewBatch(enumeratorB.Current.Value);
            }
        }


        private static void SendSerializeViewBatch(SerializeViewBatch batch)
        {
            if (batch == null || batch.Count <= 2)
            {
                return;
            }

            serializeRaiseEvOptions.InterestGroup = batch.Batch.Group;
            batch.ObjectUpdates[0] = PhotonNetwork.ServerTimestamp;
            batch.ObjectUpdates[1] = (currentLevelPrefix != 0) ? (object)currentLevelPrefix : null;
            byte code = batch.Batch.Reliable ? PunEvent.SendSerializeReliable : PunEvent.SendSerialize;
            //NetworkingClient.OpRaiseEvent(code, batch.ObjectUpdates, batch.Batch.Reliable, options);
            PhotonNetwork.RaiseEventInternal(code, batch.ObjectUpdates, serializeRaiseEvOptions, batch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
            //NetworkingClient.OpRaiseEvent(code, batch.ObjectUpdates, serializeRaiseEvOptions, batch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
            batch.Clear();
        }


        // calls OnPhotonSerializeView (through ExecuteOnSerialize)
        // the content created here is consumed by receivers in: ReadOnSerialize
        private static object[] OnSerializeWrite(PhotonView view)
        {
            if (view.Synchronization == ViewSynchronization.Off)
            {
                return null;
            }


            // each view creates a list of values that should be sent
            PhotonMessageInfo info = new PhotonMessageInfo(NetworkingClient.LocalPlayer, PhotonNetwork.ServerTimestamp, view);
            serializeStreamOut.ResetWriteStream();
            serializeStreamOut.SendNext(null);
            serializeStreamOut.SendNext(null);
            serializeStreamOut.SendNext(null);
            view.SerializeView(serializeStreamOut, info);

            // check if there are actual values to be sent (after the "header" of viewId, (bool)compressed and (int[])nullValues)
            if (serializeStreamOut.Count <= SyncFirstValue)
            {
                return null;
            }


            object[] currentValues = serializeStreamOut.ToArray();
            currentValues[0] = view.ViewID;
            currentValues[1] = false;
            currentValues[2] = null;

            if (view.Synchronization == ViewSynchronization.Unreliable)
            {
                return currentValues;
            }


            // ViewSynchronization: Off, Unreliable, UnreliableOnChange, ReliableDeltaCompressed
            if (view.Synchronization == ViewSynchronization.UnreliableOnChange)
            {
                if (AlmostEquals(currentValues, view.lastOnSerializeDataSent))
                {
                    if (view.mixedModeIsReliable)
                    {
                        return null;
                    }

                    view.mixedModeIsReliable = true;
                    view.lastOnSerializeDataSent = currentValues;
                }
                else
                {
                    view.mixedModeIsReliable = false;
                    view.lastOnSerializeDataSent = currentValues;
                }

                return currentValues;
            }

            if (view.Synchronization == ViewSynchronization.ReliableDeltaCompressed)
            {
                // compress content of data set (by comparing to view.lastOnSerializeDataSent)
                // the "original" dataArray is NOT modified by DeltaCompressionWrite
                object[] dataToSend = DeltaCompressionWrite(view.lastOnSerializeDataSent, currentValues);

                // cache the values that were written this time (not the compressed values)
                view.lastOnSerializeDataSent = currentValues;

                return dataToSend;
            }

            return null;
        }

        /// <summary>
        /// Reads updates created by OnSerializeWrite
        /// </summary>
        private static void OnSerializeRead(object[] data, Player sender, int networkTime, short correctPrefix)
        {
            // read view ID from key (byte)0: a int-array (PUN 1.17++)
            int viewID = (int)data[SyncViewId];


            // debug:
            //LogObjectArray(data);

            PhotonView view = GetPhotonView(viewID);
            if (view == null)
            {
                Debug.LogWarning("Received OnSerialization for view ID " + viewID + ". We have no such PhotonView! Ignored this if you're leaving a room. State: " + NetworkingClient.State);
                return;
            }

            if (view.Prefix > 0 && correctPrefix != view.Prefix)
            {
                Debug.LogError("Received OnSerialization for view ID " + viewID + " with prefix " + correctPrefix + ". Our prefix is " + view.Prefix);
                return;
            }

            // SetReceiving filtering
            if (view.Group != 0 && !allowedReceivingGroups.Contains(view.Group))
            {
                return; // Ignore group
            }




            if (view.Synchronization == ViewSynchronization.ReliableDeltaCompressed)
            {
                object[] uncompressed = DeltaCompressionRead(view.lastOnSerializeDataReceived, data);
                //LogObjectArray(uncompressed,"uncompressed ");
                if (uncompressed == null)
                {
                    // Skip this packet as we haven't got received complete-copy of this view yet.
                    if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                    {
                        Debug.Log("Skipping packet for " + view.name + " [" + view.ViewID +
                                  "] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game.");
                    }
                    return;
                }

                // store last received values (uncompressed) for delta-compression usage
                view.lastOnSerializeDataReceived = uncompressed;
                data = uncompressed;
            }

            // TODO: re-r-re check if ownership needs to be adjusted based on updates.
            // most likely, only the PhotonView.Controller should be affected, if anything at all.
            // TODO: find a way to sync the owner of a PV for late joiners.

            //// This is when joining late to assign ownership to the sender
            //// this has nothing to do with reading the actual synchronization update.
            //// We don't do anything if OwnerShip Was Touched, which means we got the infos already. We only possibly act if ownership was never transfered.
            //// We do override OwnershipWasTransfered if owner is the masterClient.
            //if (sender.ID != view.OwnerActorNr && (!view.OwnershipWasTransfered || view.OwnerActorNr == 0) && view.currentMasterID == -1)
            //{
            //    // obviously the owner changed and we didn't yet notice.
            //    //Debug.Log("Adjusting owner to sender of updates. From: " + view.OwnerActorNr + " to: " + sender.ID);
            //    view.OwnerActorNr = sender.ID;
            //}

            serializeStreamIn.SetReadStream(data, 3);
            PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, view);

            view.DeserializeView(serializeStreamIn, info);
        }


        // compresses currentContent by using NULL as value if currentContent equals previousContent
        // skips initial indexes, as defined by SyncFirstValue
        // to conserve memory, the previousContent is re-used as buffer for the result! duplicate the values before using this, if needed
        // returns null, if nothing must be sent (current content might be null, which also returns null)
        // SyncFirstValue should be the index of the first actual data-value (3 in PUN's case, as 0=viewId, 1=(bool)compressed, 2=(int[])values that are now null)
        public const int SyncViewId = 0;
        public const int SyncCompressed = 1;
        public const int SyncNullValues = 2;
        public const int SyncFirstValue = 3;

        private static object[] DeltaCompressionWrite(object[] previousContent, object[] currentContent)
        {
            if (currentContent == null || previousContent == null || previousContent.Length != currentContent.Length)
            {
                return currentContent; // the current data needs to be sent (which might be null)
            }

            if (currentContent.Length <= SyncFirstValue)
            {
                return null; // this send doesn't contain values (except the "headers"), so it's not being sent
            }


            object[] compressedContent = previousContent; // the previous content is no longer needed, once we compared the values!
            compressedContent[SyncCompressed] = false;
            int compressedValues = 0;

            Queue<int> valuesThatAreChangedToNull = null;
            for (int index = SyncFirstValue; index < currentContent.Length; index++)
            {
                object newObj = currentContent[index];
                object oldObj = previousContent[index];
                if (AlmostEquals(newObj, oldObj))
                {
                    // compress (by using null, instead of value, which is same as before)
                    compressedValues++;
                    compressedContent[index] = null;
                }
                else
                {
                    compressedContent[index] = newObj;

                    // value changed, we don't replace it with null
                    // new value is null (like a compressed value): we have to mark it so it STAYS null instead of being replaced with previous value
                    if (newObj == null)
                    {
                        if (valuesThatAreChangedToNull == null)
                        {
                            valuesThatAreChangedToNull = new Queue<int>(currentContent.Length);
                        }
                        valuesThatAreChangedToNull.Enqueue(index);
                    }
                }
            }

            // Only send the list of compressed fields if we actually compressed 1 or more fields.
            if (compressedValues > 0)
            {
                if (compressedValues == currentContent.Length - SyncFirstValue)
                {
                    // all values are compressed to null, we have nothing to send
                    return null;
                }

                compressedContent[SyncCompressed] = true;
                if (valuesThatAreChangedToNull != null)
                {
                    compressedContent[SyncNullValues] = valuesThatAreChangedToNull.ToArray(); // data that is actually null (not just cause we didn't want to send it)
                }
            }

            compressedContent[SyncViewId] = currentContent[SyncViewId];
            return compressedContent; // some data was compressed but we need to send something
        }

        private static object[] DeltaCompressionRead(object[] lastOnSerializeDataReceived, object[] incomingData)
        {
            if ((bool)incomingData[SyncCompressed] == false)
            {
                // index 1 marks "compressed" as being true.
                return incomingData;
            }

            // Compression was applied (as data[1] == true)
            // we need a previous "full" list of values to restore values that are null in this msg. else, ignore this
            if (lastOnSerializeDataReceived == null)
            {
                return null;
            }


            int[] indexesThatAreChangedToNull = incomingData[(byte)2] as int[];
            for (int index = SyncFirstValue; index < incomingData.Length; index++)
            {
                if (indexesThatAreChangedToNull != null && indexesThatAreChangedToNull.Contains(index))
                {
                    continue; // if a value was set to null in this update, we don't need to fetch it from an earlier update
                }
                if (incomingData[index] == null)
                {
                    // we replace null values in this received msg unless a index is in the "changed to null" list
                    object lastValue = lastOnSerializeDataReceived[index];
                    incomingData[index] = lastValue;
                }
            }

            return incomingData;
        }


        // startIndex should be the index of the first actual data-value (3 in PUN's case, as 0=viewId, 1=(bool)compressed, 2=(int[])values that are now null)
        // returns the incomingData with modified content. any object being null (means: value unchanged) gets replaced with a previously sent value. incomingData is being modified


        private static bool AlmostEquals(object[] lastData, object[] currentContent)
        {
            if (lastData == null && currentContent == null)
            {
                return true;
            }

            if (lastData == null || currentContent == null || (lastData.Length != currentContent.Length))
            {
                return false;
            }

            for (int index = 0; index < currentContent.Length; index++)
            {
                object newObj = currentContent[index];
                object oldObj = lastData[index];
                if (!AlmostEquals(newObj, oldObj))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if both objects are almost identical.
        /// Used to check whether two objects are similar enough to skip an update.
        /// </summary>
        static bool AlmostEquals(object one, object two)
        {
            if (one == null || two == null)
            {
                return one == null && two == null;
            }

            if (!one.Equals(two))
            {
                // if A is not B, lets check if A is almost B
                if (one is Vector3)
                {
                    Vector3 a = (Vector3)one;
                    Vector3 b = (Vector3)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForVectorSynchronization))
                    {
                        return true;
                    }
                }
                else if (one is Vector2)
                {
                    Vector2 a = (Vector2)one;
                    Vector2 b = (Vector2)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForVectorSynchronization))
                    {
                        return true;
                    }
                }
                else if (one is Quaternion)
                {
                    Quaternion a = (Quaternion)one;
                    Quaternion b = (Quaternion)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForQuaternionSynchronization))
                    {
                        return true;
                    }
                }
                else if (one is float)
                {
                    float a = (float)one;
                    float b = (float)two;
                    if (a.AlmostEquals(b, PhotonNetwork.PrecisionForFloatSynchronization))
                    {
                        return true;
                    }
                }

                // one does not equal two
                return false;
            }

            return true;
        }

        // TODO: Check if still needed!
        internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
        {
            mi = null;

            if (monob == null || string.IsNullOrEmpty(methodType))
            {
                return false;
            }

            List<MethodInfo> methods = SupportClassPun.GetMethods(monob.GetType(), null);
            for (int index = 0; index < methods.Count; index++)
            {
                MethodInfo methodInfo = methods[index];
                if (methodInfo.Name.Equals(methodType))
                {
                    mi = methodInfo;
                    return true;
                }
            }

            return false;
        }

        /// <summary>Internally used to detect the current scene and load it if PhotonNetwork.AutomaticallySyncScene is enabled.</summary>
        internal static void LoadLevelIfSynced()
        {
            if (!PhotonNetwork.AutomaticallySyncScene || PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }

            // check if "current level" is set in props
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentSceneProperty))
            {
                return;
            }

            // if loaded level is not the one defined by master in props, load that level
            object sceneId = PhotonNetwork.CurrentRoom.CustomProperties[CurrentSceneProperty];
            if (sceneId is int)
            {
                if (SceneManagerHelper.ActiveSceneBuildIndex != (int)sceneId)
                {
                    PhotonNetwork.LoadLevel((int)sceneId);
                }
            }
            else if (sceneId is string)
            {
                if (SceneManagerHelper.ActiveSceneName != (string)sceneId)
                {
                    PhotonNetwork.LoadLevel((string)sceneId);
                }
            }
        }

        internal static void SetLevelInPropsIfSynced(object levelId)
        {
            if (!PhotonNetwork.AutomaticallySyncScene || !PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
            {
                return;
            }
            if (levelId == null)
            {
                Debug.LogError("Parameter levelId can't be null!");
                return;
            }

            // Cancel existing loading is already taking place
            if (_AsyncLevelLoadingOperation != null)
            {
                _AsyncLevelLoadingOperation.allowSceneActivation = false;
                _AsyncLevelLoadingOperation = null;
            }

            // check if "current level" is already set in props
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CurrentSceneProperty))
            {

                object levelIdInProps = PhotonNetwork.CurrentRoom.CustomProperties[CurrentSceneProperty];

                // check if scene already active
                if (levelIdInProps is int && SceneManagerHelper.ActiveSceneBuildIndex == (int)levelIdInProps)
                {
                    return;
                }

                if (levelIdInProps is string && SceneManagerHelper.ActiveSceneName != null && SceneManagerHelper.ActiveSceneName.Equals((string)levelIdInProps))
                {
                    return;
                }

                if (_AsyncLevelLoadingOperation != null)
                {
                    // check if the key is different
                    bool _cancelCurrentloading = false;

                    _cancelCurrentloading = (levelIdInProps is int) && (levelId is int) && (int)levelId != (int)levelIdInProps;
                    _cancelCurrentloading = _cancelCurrentloading || ((levelIdInProps is string) && (levelId is string) && (string)levelId != (string)levelIdInProps);

                    if (_cancelCurrentloading)
                    {
                        _AsyncLevelLoadingOperation.allowSceneActivation = false;
                        _AsyncLevelLoadingOperation = null;
                    }
                }

            }

            // current level is not yet in props, or different, so this client has to set it
            Hashtable setScene = new Hashtable();
            if (levelId is int) setScene[CurrentSceneProperty] = (int)levelId;
            else if (levelId is string) setScene[CurrentSceneProperty] = (string)levelId;
            else Debug.LogError("Parameter levelId must be int or string!");

            PhotonNetwork.CurrentRoom.SetCustomProperties(setScene);

            SendAllOutgoingCommands(); // send immediately! because: in most cases the client will begin to load and not send for a while
        }


        private static void OnEvent(EventData photonEvent)
        {
            int actorNr = 0;
            Player originatingPlayer = null;
            if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr))
            {
                actorNr = (int) photonEvent[ParameterCode.ActorNr];
                if (NetworkingClient.CurrentRoom != null)
                {
                    originatingPlayer = NetworkingClient.CurrentRoom.GetPlayer(actorNr);
                }
            }

            switch (photonEvent.Code)
            {
                case EventCode.Join:
                    ResetPhotonViewsOnSerialize();
                    break;
                case PunEvent.RPC:
                    ExecuteRpc(photonEvent[ParameterCode.Data] as Hashtable, originatingPlayer);
                    break;

                case PunEvent.SendSerialize:
                case PunEvent.SendSerializeReliable:
                    // Debug.Log(photonEvent.ToStringFull());

                    /* This case must match definition in RunViewUpdate() and OnSerializeWrite().
                     * Format of the event's data object[]:
                     *  [0] = PhotonNetwork.ServerTimestamp;
                     *  [1] = currentLevelPrefix;  OPTIONAL!
                     *  [2] = object[] of PhotonView x
                     *  [3] = object[] of PhotonView y or NULL
                     *  [...]
                     *
                     *  We only combine updates for XY objects into one RaiseEvent to avoid fragmentation.
                     *  The Reliability and Interest Group are only used for RaiseEvent and not contained in the event/data that reaches the other clients.
                     *  This is read in OnEvent().
                     */

                    object[] pvUpdates = (object[])photonEvent[ParameterCode.Data];
                    int remoteUpdateServerTimestamp = (int)pvUpdates[0];
                    short remoteLevelPrefix = (pvUpdates[1] != null) ? (short)pvUpdates[1] : (short)0;

                    object[] viewUpdate = null;
                    for (int i = 2; i < pvUpdates.Length; i++)
                    {
                        viewUpdate = pvUpdates[i] as object[];
                        if (viewUpdate == null)
                        {
                            break;
                        }
                        OnSerializeRead(viewUpdate, originatingPlayer, remoteUpdateServerTimestamp, remoteLevelPrefix);
                    }
                    break;

                case PunEvent.Instantiation:
                    NetworkInstantiate((Hashtable) photonEvent[ParameterCode.Data], originatingPlayer);
                    break;

                case PunEvent.CloseConnection:

                    // MasterClient "requests" a disconnection from us
                    if (originatingPlayer == null || !originatingPlayer.IsMasterClient)
                    {
                        Debug.LogError("Error: Someone else(" + originatingPlayer + ") then the masterserver requests a disconnect!");
                    }
                    else
                    {
                        PhotonNetwork.LeaveRoom(false);
                    }

                    break;

                case PunEvent.DestroyPlayer:
                    Hashtable evData = (Hashtable) photonEvent[ParameterCode.Data];
                    int targetPlayerId = (int) evData[(byte) 0];
                    if (targetPlayerId >= 0)
                    {
                        DestroyPlayerObjects(targetPlayerId, true);
                    }
                    else
                    {
                        DestroyAll(true);
                    }
                    break;

                case EventCode.Leave:

                    // destroy objects & buffered messages
                    if (CurrentRoom != null && CurrentRoom.AutoCleanUp && CurrentRoom.GetPlayer(actorNr) == null)
                    {
                        DestroyPlayerObjects(actorNr, true);
                    }
                    break;

                case PunEvent.Destroy:
                    evData = (Hashtable) photonEvent[ParameterCode.Data];
                    int instantiationId = (int) evData[(byte) 0];
                    // Debug.Log("Ev Destroy for viewId: " + instantiationId + " sent by owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == actorNr) + " this client is owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == this.LocalPlayer.ID));


                    PhotonView pvToDestroy = null;
                    if (photonViewList.TryGetValue(instantiationId, out pvToDestroy))
                    {
                        RemoveInstantiatedGO(pvToDestroy.gameObject, true);
                    }
                    else
                    {
                        Debug.LogError("Ev Destroy Failed. Could not find PhotonView with instantiationId " + instantiationId + ". Sent by actorNr: " + actorNr);
                    }

                    break;

                case PunEvent.OwnershipRequest:
                {
                    int[] requestValues = (int[]) photonEvent.Parameters[ParameterCode.CustomEventContent];
                    int requestedViewId = requestValues[0];
                    int requestedFromOwnerId = requestValues[1];


                    PhotonView requestedView = PhotonView.Find(requestedViewId);
                    if (requestedView == null)
                    {
                        Debug.LogWarning("Can't find PhotonView of incoming OwnershipRequest. ViewId not found: " + requestedViewId);
                        break;
                    }

                    if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
                    {
                        Debug.Log(string.Format("OwnershipRequest. actorNr {0} requests view {1} from {2}. current pv owner: {3} is {4}. isMine: {6} master client: {5}", actorNr, requestedViewId, requestedFromOwnerId, requestedView.OwnerActorNr, requestedView.IsOwnerActive?"active":"inactive", MasterClient.ActorNumber, requestedView.IsMine));
                    }

                    switch (requestedView.OwnershipTransfer)
                    {
                        case OwnershipOption.Takeover:
                            int currentPvOwnerId = requestedView.OwnerActorNr;
                            if (requestedFromOwnerId == currentPvOwnerId || (requestedFromOwnerId == 0 && currentPvOwnerId == MasterClient.ActorNumber) || currentPvOwnerId == 0)
                            {
                                // a takeover is successful automatically, if taken from current owner
                                Player previousOwner = CurrentRoom.GetPlayer(currentPvOwnerId);
                                requestedView.OwnerActorNr = actorNr;
                                requestedView.OwnershipWasTransfered = true;

                                if (PhotonNetwork.OnOwnershipTransferedEv != null) {
                                    PhotonNetwork.OnOwnershipTransferedEv (requestedView, previousOwner);
                                }
                                // JF IPunOwnershipCallbacks callback handling refactoring
                                //requestedView.OnOwnershipTransfered(requestedView, previousOwner);
                            }
                            else
                            {
                                Debug.LogWarning("requestedView.OwnershipTransfer was ignored! ");
                            }
                            break;

                        case OwnershipOption.Request:

                            if (PhotonNetwork.OnOwnershipRequestEv != null) {
                                PhotonNetwork.OnOwnershipRequestEv (requestedView, originatingPlayer);
                            }

                        // JF IPunOwnershipCallbacks callback handling refactoring
//                            if (requestedView.IsMine)
//                            {
//                                // a request goes to the controller of a PV. the master client might control a view if the actual owner is inactive! this is covered by PV.IsMine
//                                requestedView.OnOwnershipRequest(requestedView, originatingPlayer);
//
//                            }
                            break;

                        default:
                            Debug.LogWarning("Ownership mode == "+ (requestedView.OwnershipTransfer) + ". Ignoring request.");
                            break;
                    }
                }
                break;

                case PunEvent.OwnershipTransfer:
                {
                    int[] transferViewToUserID = (int[]) photonEvent.Parameters[ParameterCode.CustomEventContent];
                    int requestedViewId = transferViewToUserID[0];
                    int newOwnerId = transferViewToUserID[1];

                    if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                    {
                        Debug.Log("Ev OwnershipTransfer. ViewID " + requestedViewId + " to: " + newOwnerId + " Time: " + Environment.TickCount % 1000);
                    }


                    PhotonView requestedView = PhotonView.Find(requestedViewId);
                    if (requestedView != null)
                    {
                        int currentPvOwnerId = requestedView.OwnerActorNr;
                        requestedView.OwnershipWasTransfered = true;
                        requestedView.OwnerActorNr = newOwnerId;
                        Player previousOwner = CurrentRoom.GetPlayer(currentPvOwnerId);

                        if (PhotonNetwork.OnOwnershipTransferedEv != null) {
                            PhotonNetwork.OnOwnershipTransferedEv (requestedView, previousOwner);
                        }
                        // JF IPunOwnershipCallbacks callback handling refactoring
                        //requestedView.OnOwnershipTransfered(requestedView, previousOwner);
                    }
                    break;
                }
            }
        }

        private static void OnOperation(OperationResponse opResponse)
        {
            switch (opResponse.OperationCode)
            {
                case OperationCode.GetRegions:
                    if (ConnectMethod == ConnectMethod.ConnectToBest)
                    {
                        string previousBestRegionSummary = PhotonNetwork.BestRegionSummaryInPreferences;

                        if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
                        {
                            Debug.Log("PUN got region list. Going to ping minimum regions, based on this previous result summary: "+previousBestRegionSummary);
                        }
                        NetworkingClient.RegionHandler.PingMinimumOfRegions(OnRegionsPinged, previousBestRegionSummary);
                    }
                    break;
            }
        }

        // Used in the main thread, OnRegionsPinged is called in a separet thread and so we can't use some of the Unity methods ( like saing in playerPrefs)
        private static RegionHandler _cachedRegionHandler;

        private static void OnRegionsPinged(RegionHandler regionHandler)
        {
            if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
            {
                foreach (Region region in regionHandler.EnabledRegions)
                {
                    Debug.Log(region.ToString());
                }
            }

            _cachedRegionHandler = regionHandler;
            //PhotonNetwork.BestRegionSummaryInPreferences = regionHandler.SummaryToCache; // can not be called here, as it's not in the main thread
            PhotonNetwork.NetworkingClient.ConnectToRegionMaster(regionHandler.BestRegion.Code);
        }
    }
}