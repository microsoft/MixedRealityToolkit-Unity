Getting Started                        {#gettingstarted}
============
# Setup the Build Environment
To build the HoloToolkit.Sharing library, you will need to install:
* On Windows, you'll need Visual Studio 2015.  The Community edition has all the support you'll need.  Be sure to include C++ and Windows Universal support
* On OSX, you'll need to install Xcode from the Mac App Store
* For Java support, you'll need Oracle's Java JDK.  The library uses the Java Native Interface provided by the JDK to allow C++ code to interact with Java code.  After installing the JDK, you need to add some system environment variables to tell the build where to find certain resources:
 + JAVA_INCLUDE should point to the folder that contains jni.h.  eg: "C:\Program Files\Java\jdk1.8.0_77\include"
 + JAVA_BIN should point to the folder that contains the javac executable.  eg: "C:\Program Files\Java\jdk1.8.0_77\bin"

# Building the Sharing library
On Windows, run HoloToolkit\Sharing\BuildAll.bat.  This will run all the other scripts necessary to build the library from scratch:  
* **BuildDependencies.bat** Builds all of the open source 3rd party libraries that the Sharing library uses.  These are located in HoloToolkit\Sharing\Src\External
* **BuildSharingLib.bat** Builds the Sharing library itself by using MSBuild with the Visual Studio solution in HoloToolkit\Sharing\Src\Solutions\VisualStudio
* **CopyToSDK.bat** Copies the built binaries, wrapper APIs, headers, and tools ready to be used in your projects to HoloToolkit\Sharing\SDK

On Mac, run HoloToolkit\Sharing\BuildAllMac.sh.  This will build the client dylib and put it in HoloToolkit\Sharing\SDK.

# Running the Server
The Sharing session server is a 64-bit Win32 application that can be run at a command prompt or as a Windows service.  App using the HoloToolkit.Sharing client can connect to this service to discover each other, join into sharing sessions together, and facilitates VoIP communications and Holographic anchor sharing.  

After running BuildAll.bat, the Sharing Service can be found at HoloToolkit\Sharing\SDK\Server\SharingService.exe.  

To run the server from the command prompt, run:
> SharingService.exe -local

To install as a service on the local machine open an administrator command prompt and run:
> SharingService.exe -install

To uninstall as a service on the local machine open an administrator command prompt and run:
> SharingService.exe -remove


# Integrating the Client Plugin
To integrate the HoloToolkit.Sharing client library with an application, first grab the necessary files from HoloToolkit\Sharing\SDK\Client.  This will contain sub-folders for the supported programming languages.  Within the sub-folder for your application's language, add the files under the API folder to your project.  Then find the appropriate SharingClient.dll under the bin folder, and add it to a folder that your application will look for DLLs in, ideally the same folder as the application itself.  

> _Notes on C# integration:_
> To reduce the overhead of passing large binary data between managed C# code and the unmanaged C++ of the library, the Sharing library marks several of the C# wrapper functions as 'unsafe'.  So when adding this library, you will have to enable unsafe code in your compiler settings.  

### Loading the DLL
To you the library, your application first has to load the SharingClient native library.
* In C#, the dll is loaded automatically by the wrapper code
* In Java, you need to explicitly tell your application to load the library by adding a static block like this:  
    static {  
        System.loadLibrary("SharingClient");  
    }
* In UWP apps, the dll must be included in appx, so be sure in include the DLL in your Visual Studio solution

### Running the Client
The starting point for running the sharing client is to create a SharingManager for your application.  

    // Create a client configuration object.  This will optionally let you specify settings for how 
    // the sharing client will act.  For brevity, the code below will configure the SharingManager as 
    // a Primary client with default options.  
    HoloToolkit.Sharing.ClientConfig config = new HoloToolkit.Sharing.ClientConfig(HoloToolkit.Sharing.ClientRole.Primary);

    // Create the sharing manager.  This will initialize all the subsystems of the library
    HoloToolkit.Sharing.SharingManager sharingMgr = HoloToolkit.Sharing.SharingManager.Create(config);

While the Sharing client code does use multiple threads, most of the user-facing API is designed to be used from a single thread, so as not to force complicated multithreading requirements on existing single-threaded applications that may want to use the library.  But to enable the library to run on the application's main thread, the application must regularly call the SharingManager's Update() function from the main thread.  In 3D applications, SharingManager.Update() should be called once per frame.  For 2D apps, it may be necessary to use a timed event to regularly call Update 15-30 times per second.  

For example, in a C# XAML app you could use a DispatchTimer:

    using System.Windows.Threading;
    using HoloToolkit.Sharing;
    public partial class App : Application
    {
        static public SharingManager SharingMgr;
        static private DispatcherTimer dispatcherTimer;

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                dispatcherTimer = null;
            }

            if (SharingMgr != null)
            {
                SharingMgr.Dispose();
                SharingMgr = null;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ClientConfig config = new ClientConfig(ClientRole.Primary);
            SharingMgr = SharingManager.Create(config);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += NetworkUpdate;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 60);
            dispatcherTimer.Start();
        }

        private void NetworkUpdate(object sender, EventArgs e)
        {
            SharingMgr.Update();
        }
    }
