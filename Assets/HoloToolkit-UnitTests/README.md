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

