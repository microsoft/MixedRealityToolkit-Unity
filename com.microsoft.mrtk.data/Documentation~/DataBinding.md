# Data Binding Framework

**NOTE: This framework is still under development and APIs may change. Also see Known Limitations section below for more information.**

Welcome to the data binding framework of MRTK. This framework is designed to make it easy to create
visual elements that can be populated dynamically at runtime with data being provided from an external
data source, and also dynamically update as the supplied data changes.
It enables the following functionality:

1. Visualize variable data via data consumers. Currently supported:
    * TextMeshPro text
    * TextMesh text
    * Text stylesheets (for theming)
    * Sprite - via sprite lookup
    * Sprite/Quad - via image fetch and replace
    * Lists - prefabs populated with variable data
    * Any other consumer that supports the IDataConsumer interface
1. Provide variable data via a variety of data sources:
    * JSON text (directly or via URL fetch)
    * Dictionary of variable data elements
    * Object - Node based structured data
    * Reflection of any C# object
    * Programmatically altered data
    * Any other method that supports the IDataSource interface
1. List item placer to manage the visual manifestation of a list
1. List paging, scrolling and virtualization
    * Data is only fetched when visible or in process
    * Supports arbitrarily large back-end data sets
    * Fetching is load balanced across multiple frames
1. List prefab pooling
    * Prefabs are reused and repopulated to reduce GC and instantiation time.
1. Apply themes dynamically to elements at runtime.

Functionality on the roadmap:

1. Data Manipulator
    * Conversion between data side and view side values
    * Localization (seamless integration with Unity localization)
    * Formatting
    * Validation
1. Predictive list item pre-fetch for faster/smoother scrolling/paging
1. More Data Consumers
    * Set any public property on a Component
    * Set checkbox on/off state
    * Set slider value
    * Set a radio button in a group
    * Individual Material properties such as set color
1. Theming
    * See themes applied in Editor even when not running application
    * Change default "baked" theme of a set of prefabs
    * Theme / style inheritance

## Design Objectives

This framework is designed to solve for the following objectives:

* Fully independent package with no external dependencies on other MRTK subsystems
* Easy to integrate into existing or greenfield code bases
* Can be enabled / disabled at any time during application life cycle
* Any number and combination of data sources and data consumers can co-exist in a single application
* Al theming configuration can exist on a single GameObject as a wrapper over a stock UX prefab
* Easy to write new data sources and data consumers using flexible base classes
* Support for arbitrarily large lists
* Nested lists
* Low garbage collection footprint
* Low impact on frame rate
* Low RAM footprint for list objects
* Cross platform
* Reusability of data visualization prefabs. Eg: Contact Info slate prefab, standard list entries)
* Easy to white label and/or apply branding to stock assets with minimal effort

## Key Concepts

### Data Source

A data source is any managed set of data of arbitrary type(s) and complexity
that can be used to populate data views via data consumers. The data managed by a data source
can be static or dynamic. Any changes to data items will be reported to any data consumers
that have registered to receive change notifications.

### Data Source Provider

A simple interface that has a single method to retrieve a data source. This is designed to allow a MonoBehavior scripting component to be auto-discovered in the game object hierarchy by data consumer components, but without the need to implement a data source directly on the game object itself.  This is useful when an existing MonoBehaviour must derive from another class and multiple inheritance prevents deriving from DataSourceGOBase. It also allows more code to have no Unity dependencies.

### Key Path (string)

A key path is the mechanism to uniquely identify any piece of information in a data source.

Although a key path can be any unique identifier per data item, all
current implementations use the concept of a logical user readable
specifier that indicates the navigational position of the data of interest
relative to the entire structured data set. It is modelled on javascript's concept of
lists, dictionaries and primitives, such that key paths are syntactically correct javascript
statements for accessing data that can be represented in JSON. The advantage
of this approach is that it correlates very well with both JSON and XML,
which are the two most prevalent means of transferring information from back*end services.

Example key paths:

* temperature
* contacts[10].firstName
* contacts
* contacts[10].addresses[3].city
* [10].title
* kingdom.animal.mammal.aardvark.diet.foodtypes.termites

Given that a key path is an arbitrary string with no required taxonomy, the actual
data specifiers could be any method of describing what data to retrieve. XML's XPath is an
example of a viable key path schema that would work with data sources. As long as key paths provided by the data consumer
are consistent with the keypaths expected by the data source, everything will work.
Furthermore Key Path Mappers can be implemented to translate between different schemas.

### Resolving a Key Path

Resolving a key path means combining two keypaths:

1. An absolute keypath that describes how to access a specific
subset of a larger dataset, such as one entry in a list of many entries.
2. A partial (relative) keypath that represents a specific datum within that list or map entry.

This makes it possible to treat a subset of the data in such a way that it does
not matter where in a larger data set hierarchy it actually exists. The most critical
use of this ability is to describe the data of a single entry in a list without worrying about which
entry in that list the current instance is referencing.

Since a "fully resolved" Key path is always generated and consumed by a DataSource and should never (or at least rarely) be modified by a DataConsumer or other external component, it can have any structure that makes sense to the DataSource.  For example, if a prefab to show a list entry for a photo and it's title, date taken and other attributes, the local key path in the prefab might look like this:

* "photo_url"
* "title"
* "date_taken"
* "description"

The fully resolved key paths for one prefab entry in a list might look like this:

* "f3cb1906-d8b3-489d-9f74-725e5542b55d/photo_url"
* "f3cb1906-d8b3-489d-9f74-725e5542b55d/title"
* "f3cb1906-d8b3-489d-9f74-725e5542b55d/date_taken"
* "f3cb1906-d8b3-489d-9f74-725e5542b55d/description"

### Key Path Mapper (IDataKeyPathMapper)

A key path mapper allows data sources and data consumers to use different namespaces and conventions for key paths and still work together.

A prefab for a commonly used element, such as a slate to show a persons contact information, can contain variable fields managed by data consumers. To make this possible , the identifier used for any variable aspect of the prefab needs a way to map to the identifier for the correct datum in the data source that will, in each use of the prefab, determine the contents of that variable element. The Key Path Mapper makes this possible.

The prefab may be used with different data sources where the data is stored in a different organizational structure and uses field names. To use a
template prefab with each data source, a key path mapper can resolve any differences in how the data is organized.

### Data Consumer (IDataConsumer)

An object that knows how to consume information being managed by
a data source and use that data to populate data views.

Data Consumers can register with a data source to be notified of any changes to a
data item that exists at a specified key path in a dataset. Whenever the data specified has changed (or suspected to have changed), the
Data Consumer(s) will be notified.

### Data Consumer Collection

A collection data consumer has the added ability to manage a list of similar items. This list can be the entire data set
managed by a data source, or just a subset. Typically the data for each item in the list contains similar types of information, but this
is not a requirement. Data sources and data consumers can support nested lists, such as a list of keywords
associated with each photo in a list of photos associated with each person in a contact list. The keypath for the keywords would be relative to the photo,
and the keypath for the photos would be relative to the person, and the keypath of the person would be relative
to either the nearest parent list, or the root of the data set.

When processing collections, the correct resolved keypath for the specific entry in the collection is assigned to each data consumer found in the prefab
that is instantiated for each collection item. That is then used to full resolve the key path for any relative (local) view data within that prefab.

### Data Collection Item Placer

A collection data consumer needs a means to populate user experiences with lists of repeating visual elements, such as might be found in a scrollable
list of products, photos, or contacts.

This is accomplished by assigning an item placer to the collection data consumer. This item placer is the logic tha knows how to request list items, accept prefabs
that have been populated with variable data, and then present them to the user, typically by inserting them into a list managed by a ux layout component for lists.

# Theming

Theming uses all of the plumbing of data sources and data consumers.  It is possible to theme any hierarchy of GameObjects whether they are static or are dynamically data bound to other data sources.  This allows for both data binding and theming to be applied in combination. It is even possible to theme the data coming from another data source.

## Theming UXComponents

The standard UXComponents controls provided in the UXComponents package are all configured to support theming.  It is turned OFF by default, but is easy to enable.

Each control, typically on the topmost GameObject of the root prefab, has a script called UXBindingConfigurator. This script, if enabled, will pull in the needed data binding scripts to turn on theming. Make sure to import the Data Binding and Theming package as well.

**Note on TextMeshPro StyleSheets**: It is not currently possible to use StyleSheets to style the TextMeshPro *Normal* style.  Any other style that is included in TextMeshPro's *Default Style Sheet* can be used. The examples use *Body* to work around this limitation.

## Data Consumer Theming

Theming is accomplished by Data Consumers, particularly ones that inherit from DataConsumerThemeBase\<T\>, DataConsumerTextStyle and custom DataConsumer classes that any developer can implement to enhance the theming support.

The DataConsumerThemeBase\<T\> base class provides logic to use an integer or key datum from a primary data source to then look up the desired final value from a secondary theme database. This is accomplished by mapping the input data to a theme keypath, and then using that theme keypath to retrieve the final value. This allows for any element to be both data bound and themed at the same time.  As an example, imagine a status field in a database with statuses of New, Started, and Done represented by values 0, 1 and 2.  Each of these can be represented by a Sprite icon.  For data binding, a value from 0 to 2 is used to lookup the desired sprite.  With theming and data binding, the theme profile points to the correct list of 3 sprites in the theme profile and then the value from 0 to 2 is used to select the correct sprite from that list.  This allows the styling of these icons to differ per theme.

When both runtime theming and dynamic data binding are used together, a DataConsumerThemeHelper class can be specified in any DataConsumerThemeBase derived class to notify when a theme has changed.

Swapping themes at runtime is accomplished by replacing the data at the theme data source with a new data set laid out in the same data object model topology. DataSourceReflection can be used with ScriptableObjects where each profile represents a theme. For all MRTK Core UX controls, the theme profile is a ScriptableObject named MRTK_UXComponents_ThemeProfile. The ThemeProvider.cs helper script makes it easy to use this or any ScriptableObject profile as a Data Source.

The method of applying a theme to dynamic data can be automatically detect in most cases, or it can be explicitly specified.

When the datum is used to select the correct item from the theme data source, the process is:

* a datum from the primary data source is used to select or construct the correct *theme keypath*
* the theme keypath is used to retrieve a value from the theme data source specified on the DataConsumerThemeHelper
* the retrieved theme value is analyzed to auto-detect correct retrieval method
* the final data item of correct type (eg. Material, Sprite, Image) is then retrieved using the auto-detected method.

### Data Types

The expected data type of the datum used to retrieve the desired object can be one of the following:

Data Type | Description
:---: | ---
AutoDetect | The datum is analyzed and the correct interpretation is automatically detected. See "Auto-detect Data Type" below for more information.
DirectValue | The datum is expected to be of desired type T (eg. Material, Sprite, Image) and used directly.
DirectLookup | An integral index or string key used to look up the desired value from a local lookup table.
StaticThemedValue | Static themed object of the correct type is retrieved from the theme data source at specified theme keypath.
ThemeKeypathLookup |  An integral index or string key is used to look up the desired theme keypath.
ThemeKeypathProperty | A string property name that will be appended to the theme base keypath provided in the Theme .
ResourcePath | A resource path for retrieving the value from a Unity resource. (May begin with "resource://")
FilePath | A file path for retrieving a Unity streaming asset. (May begin with "file://")

### Auto-detect Data Type

Autodetect analyzes the data received and decides the retrieval method automatically. In the table below, T represents the desired type such as Material, Sprite, Image. Autodetect can occur at two places in the process:

* On the primary datum value itself.
* On the themed value retrieved via the primary datum.

Datum Type | Considerations | Has Theme Helper | Behavior
:---:|---|:---:|---
T | n/a | Y/N | Used directly with no theming
int | any integral primitive or Int32 parsable string | No | Passed as index to derived GetObjectByIndex(n) to retrieve Nth object of type T.
int | any integral primitive or Int32 parsable string | Yes | Index to fetch Nth theme keypath from local lookup and then retrieve themed object via auto-detect.
string | Format: "resource://{resourcePath}" | Y/N | resourcePath is used to retrieve Unity Resource
string | Format: "file://{filePath} | Y/N | filePath is used to retrieve a streaming asset
string | Other | No | Passed as key to derived GetObjectByKey() to retrieve matching object of type T.
string | Other | Yes | Key to fetch matching theme keypath from local lookup and then retrieve themed object via auto-detect.

An example for retrieving a themed status icon from a database containing a numeric status value:

1. The keypath for a status icon in the database is status.sprite_index.
2. The retrieved value for status.sprite_index is 2 which means "cancelled" status.
3. The N=2 (ie. 3rd) entry in DataConsumerSprite lookup is set to "Status.Icons.Cancelled".
4. This is the keypath used to retrieve a value from the "theme" data source.
5. The value for the "Status.Icons.Cancelled" keypath is "resource://Sprites/sprite_cancelled".
6. Auto-detect determines that it should retrieve the icon via a resource located at "Resources/Sprites/sprite_cancelled"

### TextMeshPro StyleSheets

Theming is able to activate TMPro stylesheets.  "TMP Settings" ScriptableObject dictates where stylesheets are expected to be in the Resources.  It's the "Default Font Asset => Path" property.

Make sure to place any app specific StyleSheets in the same sub-path off of Resources. If you wish to organize them differently, make sure to update "TMP Settings" to match.

### Making New UX Controls Themable

If you are developing new UX controls, it is relatively easy to make them themable. To the extent that the control uses Materials, Sprites, and other assets already in use by other UX controls, it is generally a matter of naming the various game objects in a discoverable way.

It's possible to inherit from the MRTK_UXCore_ThemeProfile and add more themable fields, or point your controls to your own ScriptableObject.  There is nothing magical about the ones provided other than the organization of the ScriptableObject will determine the keypaths need to access individual data items, via C# Reflection.

By adding a BindingConfigurator.cs script to the top level of the new control, you can then specify your own serialized BindingProfile ScriptableObject to provide the necessary GameObject name to KeyPath mappings needed to associate your themable elements with the data provided in the theme profile. This script will automatically add any needed DataConsumerXXX components at runtime to support the theming you wish to use.

## Getting Started

### Requirements

* Unity 2019.4 or later
* TextMeshPro 2.1.4 or later

### Sample Scene

For a first step, take a close look at the "Data Binding Test" scene that demonstrates a variety of variable data scenarios.  Simply load the scene and play. A few things to notice:

* The Text Input field of TextMeshPro components contain variables that look like this: {{ firstName }}
* Game objects for sprites and text have some form of Data Consumer component that manages receiving data and updating views.
* A single Data Consumer may be shared by multiple components of the same type by being placed higher in the GO hierarchy.
* A Data Consumer can find its own Data Source so long as it is on the same game object or higher in the GO hierarchy.
* A parent game object has a Data Source component that provides data for all child game objects that present a related set of variable information.
* A collection Data Consumer specifies a prefab which itself contains data consumers that will be used to populate that prefab with variable data.

### First Data Binding project

Here's a simple example to help you get started quickly:

Create a new scene. On the Mixed Reality Toolkit menu, select "Add to Scene and Configure" option.

Create an empty game object and rename it to "Data Binding". Add a DataSourceJsonTest component.

In the inspector, change the Url to: <https://www.boredapi.com/api/activity>

Add a UI -> Text - TextMeshPro object to the Data Binding game object.  It will add a canvas and then a "Text (TMP)" object.

Select the Text (TMP) object, and in the inspector change the Text Input to:

`{{ activity }}. It's {{ type }}.`

Select the canvas object and add a Data Consumer Text component to it.

Run the project.  Every 15 seconds, a different activity will be shown.

Congratulations. You've created your first Data Binding project with MRTK!

### Writing a new Data Source

A data source provides data to one or more data consumers.  It's data can be anything: algorithmically generated, in RAM, on disk, or fetched from a central database.

All Data sources must provide the IDataSource interface. Some of the basic functionality is offered in a base class called DataSourceBase. You most likely want to derive from this class to add the specific data management functionality specific to your need.

To make it possible to drop a data source as a component onto a game object, another base object exists called DataSourceGOBase where GO stands for GameObject. This is a MonoBehavior that can be dropped onto a GameObject as a Component. It is a thin proxy that is designed to delegate work to a non-Unity specific core data source.

A data source may expose the ability to manipulate data within Unity Editor. If this is the case, the derived class can contain all of the data source logic, or it can leverage a "stock" data source, but also add inspector fields or other means of configuring the data.

### Writing a new Data Consumer

A data consumer gets notifications when data has changed and then updates some aspect of the user experience, such as the text shown in a TextMeshPro Component.

All data consumers must provide the IDataConsumer interface. Some of the basic functionality is offered in a base class called DataConsumerGOBase, where GO stands for GameObject.

The majority of the work of a data consumer is to accept new data and then prepare it for presentation.  This may be as simple as selecting the right prefab, or it could mean fetching more data from a cloud service such as a content management system.

### Writing a data collection item placer

A data collection item placer is responsible for managing which parts of a collection are currently visible and how to present that visible collection, whether the collection is a small static list, or a giant million record database.

All item placers must provide the IDataCollectionItemPlacer interface.  Some of the basic functionality is offered in a base class called DataCollectionItemPlacerGOBase.  All item placers should derive from this class.

# Known Limitations and Missing Features

* Need to verify proper cleanup of resources in all use cases, particularly lists.
* Dynamic changes to list data completely refreshes entire list instead of incrementally updating.
* Data validation (eg. number in correct range) and formatting (eg. data and currency formats)
* Predictive data fetching for faster list navigation
* Material selector is work in progress
* Font style selector is a work in progress
* Populating other UX elements such as sliders, radio buttons, and checkboxes not yet supported
* DataSourceJson nodes should implement IDataNode interface to be interoperable with DataSourceObjects
* Not yet integrated with Volumetric Layouts and Scrolling Object Collection
