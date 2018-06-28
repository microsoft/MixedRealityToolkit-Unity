# Overview
Graph APIs are meant to ease the work required to access the data from [MS Graph](https://developer.microsoft.com/en-us/graph) in the Unity world.

# Authentication
To call Microsoft Graph, your app must acquire an [access token](https://developer.microsoft.com/en-us/graph/docs/concepts/auth_overview) from Azure Active Directory (Azure AD), Microsoft's cloud identity service. 

To that end, the toolkit comes with a implementation using [Microsoft Authentication Library](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet). However users can override with their own implementation, by extending GraphConnector, similarly to **MsalGraphAuthentication.cs**.

# Using Graph
1. Register application
	Your app must be registered with Azure AD. Registering your app establishes a unique application ID and other values that your app uses to authenticate with Azure AD and get tokens. For the Azure AD v2.0 endpoint, you register your app with the Microsoft App Registration Portal. You can use either a Microsoft account or a work or school account to register your app. Depending on the type of app you are developing, you will need to copy one or more properties during registration to use when you configure authentication and authorization for your app.

	Follow the [instructions](https://developer.microsoft.com/en-us/graph/docs/concepts/auth_register_app_v2) to register a **Native** application.

2. Enabling .NET Framework 4.x is required.

3. Declare any app capabilities required for authentication. For UWP capabilities check documentation [here](https://docs.microsoft.com/en-us/windows/uwp/packaging/app-capability-declarations).

4. Identify Graph data needed for your scenario.

	In the left panel of MS Graph [documentation](https://developer.microsoft.com/en-us/graph/docs/concepts/overview) identify any data and APIs needed for your scenario. For example to get the signed-in user profile the [API](https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/api/user_get) is "/me" and "User.Read" scope is enough as stated in the documentation.
	
	Alternatively, use [Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer) to find APIs available.
	
	Once you have identified all APIs and what Graph scopes (i.e. permissions) required, update the list of scopes declared in [Microsoft App Registration Portal](https://apps.dev.microsoft.com/).

5. Setup Unity scene.

	Add **MsalGraphConnector.cs** to any game object of your scene. Alternatively, you can add the GraphConnector.prefab to the scene, as it already includes **MsalGraphConnector.cs**.

6. Setup MS Graph permissions.

	Select the game object that has MsalGraphConnector.cs and update settings accordingly in the inspector.
	* **Graph App Id** is the application ID registered in [Microsoft App Registration Portal](https://apps.dev.microsoft.com/).
	* **Graph Access Scopes** is the array that lists all [access permissions](https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference) required in your scenario.

7. All set. 

	Look up **GraphConnectorTestAsync.cs** for examples on how to use the GrahConnector.

# Testing using the Unity editor
There are two options to test MS Graph in the editor:
1. Use a network monitor app like Fiddler to inspect your Bearer auth token, while you execute a query using [Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer). Copy & paste the auth token into the GraphConnector inspector, as your Test Auth Token.
2. Provide custom implementation of IGraphAuthentication instead of using MsalGraphAuthentication. It requires implementing [OAuth 2.0](https://oauth.net/2/) using Bearer Tokens authentication.

# Managed bytecode stripping with IL2CPP
IL2CPP analyzes all assemblies and removes methods that are never directly called. If something is only accessed through reflection, it will be removed unless it is specified in link.xml files. Read more about it [here](https://docs.unity3d.com/Manual/IL2CPP-BytecodeStripping.html) 

# Known issues
Unity runtime bugs preventing Microsoft.Identity.Client.dll from working in UWP with IL2CPP:

	1. IL2CPP project failing to P/Invoke into kernel32.dll!GetNativeSystemInfo. 

		Issue Tracker: https://issuetracker.unity3d.com/issues/uwp-il2cpp-project-failing-to-p-slash-invoking-into-etw-logging-functions

		Workaround: build Microsoft.Identity.Client.dll for WinRT, using "[DllImport("__Internal")]" syntax for this P/Invoke.

	2. IL2CPP project failure during runtime serialization, when using NET 4.x API compatibility profile. 

		Issue Tracker: https://issuetracker.unity3d.com/issues/system-dot-configuration-dot-configurationerrorsexception-failed-to-load-configuration-section-for-datacontractserializer

		There is a bug in the Unity runtime code: in "Il2CppOutputProject\IL2CPP\libil2cpp\vm\Runtime.cpp", function "framework_version_for" should return "4.5" instead of "4.0".
		
		Workaround 1: changing it locally, but keep in mind that it gets overriden every time you build the project from Unity. 
		Workaround 2: Using .NET Standard 2.0 profile.

	3. IL2CPP conversion bug causes ApplicationDataCompositeValue to fail with "Error trying to serialize the value to be written to the application data store".

	    https://forum.unity.com/threads/uwp-assemblies-not-working-when-using-il2cpp-but-work-on-net-scripting-backend.533401/

		Error trying to serialize the value to be written to the application data store
		at Windows.Storage.ApplicationDataCompositeValue.Insert (System.String key, System.Object value) [0x00000] in <00000000000000000000000000000000>:0 
		at System.Runtime.InteropServices.WindowsRuntime.IMapToIDictionaryAdapter`2[TKey,TValue].System.Collections.IEnumerable.GetEnumerator () [0x00000] in <00000000000000000000000000000000>:0 
		at Windows.Storage.ApplicationDataCompositeValue.get_Item (System.String key) [0x00000] in <00000000000000000000000000000000>:0 
		at Microsoft.Identity.Client.TokenCacheAccessor.SetCacheValue (Windows.Storage.ApplicationDataCompositeValue composite, System.String stringValue) [0x00000] in <00000000000000000000000000000000>:0 
		at Microsoft.Identity.Client.TokenCacheAccessor.SaveAccessToken (System.String cacheKey, System.String item) [0x00000] in <00000000000000000000000000000000>:0 
		at Microsoft.Identity.Client.Internal.Telemetry.TelemetryTokenCacheAccessor.SaveAccessToken (System.String cacheKey, System.String item, Microsoft.Identity.Client.Internal.RequestContext requestContext) [0x00000] in <00000000000000000000000000000000>:0 
