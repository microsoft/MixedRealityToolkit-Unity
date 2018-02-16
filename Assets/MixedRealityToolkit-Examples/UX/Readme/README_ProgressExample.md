# Progress Example
<img src="/External/ReadMeImages/MRTK_Progress1.jpg" width="650">
A progress control provides feedback to the user that a long-running operation is underway. It can mean that the user cannot interact with the app when the progress indicator is visible, and can also indicate how long the wait time might be, depending on the indicator used.

This example scene illustrates the use of the ProgressIndicator prefab to create a progress Indicator with several options for appearance.

## Demo Video
https://gfycat.com/JaggedDimLacewing

## Types of Progress
![Progress Types](/External/ReadMeImages/MRTK_Progress2.jpg)
The ProgressIndicator can appear:
1. Default orbiting dots animation
2. A user defined icon which can be static or set to rotate
3. A user defined prefab which can be static or set to rotate
4. A user defined text message only such as "Loading..."
5. Text displayed with a numeric run up of percent loaded
6. A progress bar

![Progress Type Buttons](/External/ReadMeImages/MRTK_Progress3.jpg)

Different types of Progress control can be opened by pressing these buttons in the scene.



## ProgressIndicator Prefab

The **ProgressIndicator prefab** is located in MixedRealityToolkit/UX/Prefabs/Progress. It is composed of **ProgressIndicator** script and **Solver** scripts for the billboarding and tag-along behavior.

![ProgressIndicator Properties](/External/ReadMeImages/MRTK_ProgressIndicatorInspector.jpg)

### ProgressIndicator Prefab Structure
The **ProgressIndicator prefab** contains the elements for the different types of visualization. You can find child components such as  Bar, Message Text and Progress Text. These elements are assigned to ProgressIndicator script.

![Progress Types](/External/ReadMeImages/MRTK_Progress4.jpg)


## How to display a Progress control
In the example scene, each button is labeled with the variation of the ProgressIndicator that is demonstrated. There is an instance of the ProgressIndicator prefab in the scene which initializes and runs with arguments obtained from the example button that is clicked. 

In these buttons, you can find **ProgressButton** script along with Compound Button scripts. **ProgressButton** script contains simple code to call LaunchProgress() in **ProgressExamples** script on OnButtonClicked event. In **ProgressExamples** script, you can find the code for displaying Progress control.

It looks like this: 
<pre>
ProgressIndicator.Instance.Open(
                            ProgressIndicator.IndicatorStyleEnum.None,
                            ProgressIndicator.ProgressStyleEnum.None,
                            ProgressIndicator.MessageStyleEnum.Visible,
                            LeadInMessage);

StartCoroutine(LoadOverTime(LoadTextMessage));
</pre>

The LoadOverTime function, also found in this script, fakes a loading sequence. To implement the ProgressIndicator during an actual loading sequence, Start a Coroutine with a function like LoadOverTime. In your LoadOverTime function, make sure to update the ProgressIndicator object with the actual loading progress.
<pre>
while (your loading operation is not complete)
{
    LoadingDialog.Instance.SetMessage( <loading progress message> );
    LoadingDialog.Instance.SetProgress<percent loaded>
    yield return new WaitForSeconds(<amount of time between updates ie 0.25 seconds>);
}
</pre>

You can give an optional message to be displayed by the ProgressIndicator when load is complete, such as "finished!".
<pre>
ProgressIndicator.Instance.SetMessage(FinishMessage);
ProgressIndicator.Instance.SetProgress(1f);
yield return new WaitForSeconds(<amount of time to hold message ie 1.5 secs>);
</pre>

To close the ProgressIndicator, call the Close() function and wait for the IsLoading state to become false.
<pre>
LoadingDialog.Instance.Close();
while (LoadingDialog.Instance.IsLoading)
{
    yield return null;
}
</pre>


 
 
