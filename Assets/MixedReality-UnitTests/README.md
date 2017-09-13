# HoloToolkit-UnitTests
This folder will host unit tests that test individual HoloToolkit-Unity components.

#### Creating a unit test:

1. Unit tests can be located anywhere under an Editor folder but consider placing it under the HoloToolkit-UnitTests\Editor folder so that HoloToolkit-Unity can be easily deployed without the tests if required.
2. Use the **NUnit 2.6.4** features to create the tests.
3. Cover us much as possible the new code so that other people can test your code even without knowing its internals.
4. Execute all the tests before submitting a pull request.

#### Running the unit tests:

1. Unit tests execution is integrated into the Unity editor (since 5.3). 
2. Open the **Editor Tests** window by clicking on the menu item **Window** | **Editor Tests Runner**.
3. Click on the buttons **Run All**, **Run Selected** or **Run Failed** to run the tests. 

#### Testing in Unity

Since MonoBehaviours are highly coupled to the Unity Engine there are some details one has to know when testing them.


##### 1. Editor Tests are run in a new additive loaded scene. After the tests the original scene will be reloaded.

This means any objects from the original scene will still exist next to the test scene and may interfere with your tests. For example if a tests creates a main camera and tries to use it, it may find the main camera of the original scene instead. Furthermore unity will not clear the scene between each test so objects created in other tests might interfere as well.

Clear the scene if required by using `TestUtils.ClearScene()`, either manually at the start of the test or define a `SetUp` method that calls it.

```csharp
[SetUp]
public void ClearScene()
{
    TestUtils.ClearScene();
}
````


##### 2. Any MonoBehaviour methods such as `Awake`, `Start` and `Update` will not be called by Unity in tests.

As the name editor test implies they are only executed in the editor and as such instantiated objects do not get the usual events. 
The `TestUtils` provide a way to simulate some of the behaviour by manually calling the required methods. It is obviously not the goal to recreate the whole Unity Engine by manually calling dozens of methods on an object. The logic in scripts should be seperated enough so that the tests remain small.

```csharp
[Test]
public void CallAwakeTest()
{
    var gameObject = new GameObject();
    var reflectionTest = gameObject.AddComponent<ReflectionTestBehaviour>();
    gameObject.CallAwake();
    Assert.That(reflectionTest.AwakeCalled, Is.True);
}
```

##### 3. Unity overwrites the default behaviour of null comparison

Null checks with classes like `GameObject`, `Transform` and others will behave differently when checked against with `Is.Null` and `Is.Not.Null`. The reason for this is that a destroyed unity `Object` cast to `object` and compared against null returns false, as the `Object` still exists and is marked as destroyed.

For this we provide a custom constraint that captures this behaviour and returns the expected results. `Is.UnityNull` and `Is.Not.UnityNull()` which also works with the default behaviour so that any object can compared against it, not just Unity ones.

```csharp
[Test]
public void TestGameObjectUnityNull()
{
    var gameObject = new GameObject();
    Object.DestroyImmediate(gameObject);
    Assert.That(gameObject, Is.UnityNull);
}

[Test]
public void TestGameObjectNotUnityNull()
{
    Assert.That(new GameObject(), Is.Not.UnityNull());
}
```