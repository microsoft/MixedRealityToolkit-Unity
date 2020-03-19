# Progress Indicators
## Example scene

You can find examples of how to use progress indicators in the `ProgressIndicatorExamples` scene. This scene demonstrates each of the progress indicator prefabs included in the SDK.

<img src="../Documentation/Images/ProgressIndicator/MRTK_ProgressIndicator_Examples.png">

## Example: Open, update & close a progress indicator

Progress indicators implement the `IProgressIndicator` interface. You can retrieve this interface from a GameObject using `GetComponent`.

```c#
[SerializedField]
private GameObject indicatorObject;
private IProgressIndicator indicator;

private void Start()
{
    indicator = indicatorObject.GetComponent<IProgressIndicator>();
}
```

The `IProgressIndicator.OpenAsnyc()` and `IProgressIndicator.CloseAsync()` methods return `Tasks.` We recommend you await these `Tasks` in an aync method.

Set the indicator's `Progress` property to a value from 0-1 to update its displayed progress. Set its `Message` property to update its displayed message. Different implementations may display this content in different ways.
```c#
private async void OpenProgressIndicator()
{
    await indicator.OpenAsync();

    float progress = 0;
    while (progress < 1)
    {
        progress += Time.deltaTime;
        indicator.Message = "Loading...";
        indicator.Progress = progress;
        await Task.Yield();
    }

    await indicator.CloseAsync();
}
```

## Indicator states

An indicator's `State` property determines which operations are valid. If you call an invalid method the indicator will typically report an error and take no action.

State | Valid Operations
--- | ---
`ProgressIndicatorState.Opening` | `AwaitTransitionAsync()`
`ProgressIndicatorState.Open` | `CloseAsync()`
`ProgressIndicatorState.Closing` | `AwaitTransitionAsync()`
`ProgressIndicatorState.Closed` | `OpenAsync()`

You can use `AwaitTransitionAsync()` to be sure an indicator is fully opened or closed before using it.
```c#
private async void ToggleIndicator(IProgressIndicator indicator)
{
    await indicator.AwaitTransitionAsync();
    
    switch (indicator.State)
    {
        case ProgressIndicatorState.Closed:
            await indicator.OpenAsync();
            break;

        case ProgressIndicatorState.Open:
            await indicator.CloseAsync();
            break;
        }
    }
```