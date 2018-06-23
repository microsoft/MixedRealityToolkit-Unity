# Overview
Graph APIs are meant to ease the work required to access the data from [MS Graph](https://developer.microsoft.com/en-us/graph) in the Unity world.

# Authentication
Accessing MS Graph requires user authentication. To that end, the toolkit comes with a implementation using [Microsoft Authentication Library](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) tested against UWP with .NET scripting backend. 
However users can override with their own implementation, by extending GraphConnector, similarly to **MsalGraphAuthentication.cs**.

# Using Graph
1. Register application
	Your app must be registered with Azure AD. Registering your app establishes a unique application ID and other values that your app uses to authenticate with Azure AD and get tokens. For the Azure AD v2.0 endpoint, you register your app with the Microsoft App Registration Portal. You can use either a Microsoft account or a work or school account to register your app. Depending on the type of app you are developing, you will need to copy one or more properties during registration to use when you configure authentication and authorization for your app.

	Follow the [instructions](https://developer.microsoft.com/en-us/graph/docs/concepts/auth_register_app_v2) to register a **Native** application.

2. Declare any app capabilities required for authentication. For UWP capabilities check documentation [here](https://docs.microsoft.com/en-us/windows/uwp/packaging/app-capability-declarations).

3. Identify Graph data needed for your scenario.

	In the left panel of MS Graph [documentation](https://developer.microsoft.com/en-us/graph/docs/concepts/overview) identify any data and APIs needed for your scenario. For example to get the signed-in user profile the [API](https://developer.microsoft.com/en-us/graph/docs/api-reference/v1.0/api/user_get) is "/me" and "User.Read" scope is enough as stated in the documentation.
	
	Alternatively, use [Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer) to find APIs available.
	
	Once you have identified all APIs and what Graph scopes (i.e. permissions) required, update the list of scopes declared in [Microsoft App Registration Portal](https://apps.dev.microsoft.com/).

4. Setup Unity scene.

	Create a GraphConnectorProfile for your project via Assets/Create/Mixed Reality Toolkit/Graph/Graph Connector Profile.

5. Setup MS Graph permissions.

	Select the new GraphConnectorProfile and update settings accordingly in the inspector.
	* **Graph App Id** is the application ID registered in [Microsoft App Registration Portal](https://apps.dev.microsoft.com/).
	* **Graph Access Scopes** is the array that lists all [access permissions](https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference) required in your scenario.

6. All set. 

	Look up **GraphConnectorTestAsync.cs** for examples on how to use the GraphConnectorProfile.

# Managed bytecode stripping with IL2CPP
IL2CPP analyzes all assemblies and removes methods that are never directly called. If something is only accessed through reflection, it will be removed unless it is specified in link.xml files. Read more about it [here](https://docs.unity3d.com/Manual/IL2CPP-BytecodeStripping.html) 

# Known issues
Two Unity bugs prevent Microsoft.Identity.Client.dll from working in UWP:

	1. IL2CPP project failing to P/Invoke into kernel32.dll!GetNativeSystemInfo. 

		Issue Tracker: https://issuetracker.unity3d.com/issues/uwp-il2cpp-project-failing-to-p-slash-invoking-into-etw-logging-functions

		Workaround: build Microsoft.Identity.Client.dll for WinRT, using "[DllImport("__Internal")]" syntax for this P/Invoke.

	2. IL2CPP project failure during runtime serialization, when using NET 4.x API compatibility profile. 

		Issue Tracker: https://issuetracker.unity3d.com/issues/system-dot-configuration-dot-configurationerrorsexception-failed-to-load-configuration-section-for-datacontractserializer

		There is a bug in the Unity runtime code: in "Il2CppOutputProject\IL2CPP\libil2cpp\vm\Runtime.cpp", function "framework_version_for" should return "4.5" instead of "4.0".
		
		Workaround 1: changing it locally, but keep in mind that it gets overriden every time you build the project from Unity. 
		Workaround 2: Using .NET Standard 2.0 profile.
