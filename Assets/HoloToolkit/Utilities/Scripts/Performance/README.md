# Adaptive Performance
An overview of this tool and how to improve performance in your app in general is availible here:
https://developer.microsoft.com/en-us/windows/mixed-reality/performance_recommendations_for_immersive_headset_apps#optimizing_performance_for_unity_apps

To use the tool in your app, you can import the unity package: 
Holotoolkit-Unity\External\Unitypackages\AdaptivePerformance.unitypackage

Add the AdaptivePerformance prefab to your scene and the PerformanceDisplay prefab to your scene as a child 
of the main camera.  With this infrastructure in place you can use the following keys hotkeys:
 * Performance Display:  Toggle the display on/off using the 'm' key.
 * Adaptive Performance: Change bucket up with the ‘d’ key and down with the ‘s’ key.
 * Viewport Scale Manager: Increase the viewport scale with the ‘]‘ key and 
     decrease it with ‘[‘ in increments of 0.05f. 
 * Quality Manager: Go to the next quality level with the ‘.’ Or ‘+’ key and 
     go to the previous level with ‘,’ or ‘-‘.
	 
More information on this toolset is provided in this subsection of the previously mentioned article:
https://developer.microsoft.com/en-us/windows/mixed-reality/performance_recommendations_for_immersive_headset_apps#Performance_toolkit

A description of how to setup your buckets is provided in this subsection of the same article:
https://developer.microsoft.com/en-us/windows/mixed-reality/performance_recommendations_for_immersive_headset_apps#Adapting_app_quality_to_each_machine

The starting performance bucket is determined in the following way:
 1) If we're running in Unity editor, the starting bucket is picked from the startBucket field in the inspector.
 2) If adaptive performance was run or a performance bucket was selected explicitly. The last performance bucket 
     used is saved to a file and loaded the next time the game starts.
 3) If GPUWhitelist returns a value, we have some commented out logic for you to fill in to inform the app as to 
     which bucket it should start in.
 4) If we don't have a last known bucket saved in a file and we're not running in the editor, the starting bucket 
     will be picked from the startBucket field in the class with the value that was saved in the Unity scene from 
	 the inspector.
	 
Adaptive performance API: 

    // public property for retrieving the current bucket index
    public int CurrentBucketId;

    // public method for getting the performance parameters of the current bucket
    public PerformanceBucket GetCurrentBucket()

    // public method for explicitly moving a bucket up. (lower perf, higher quality)
    public int SwitchToHigherBucket();

    // public method for explicitly moving a bucket down. (higher perf, lower quality)
    public int SwitchToLowerBucket()

    // public method for starting adaptive performance management
    // during adaptive performance, performance settings will automatically changed depending
    // on the current performance. Bad performance moves to lower bucket, exceeded performance
    // moves to higher bucket
    // Adaptive performance will run until StopAdaptivePerformance() is called
    public void StartAdaptivePerformance();

    // public method for starting adaptive performance with a timeout
    // Adaptive performance stops running after the given amount of seconds
    public void StartAdaptivePerformance(float time);

    // Event that is flagged when a performance bucket changes
    public class PerformanceBucketChangedEvent : UnityEvent<PerformanceBucket> { }

    // instance of the event that clients can subscribe to and receive notifications
    // when a performance bucket changes
    // to subscribe call OnPerformanceBucketChanged.AddListener(callbackFunc)
    // when done call OnPerformanceBucketChanged.RemoveListener(callbackFunc) to unsubscribe
    public PerformanceBucketChangedEvent OnPerformanceBucketChanged;