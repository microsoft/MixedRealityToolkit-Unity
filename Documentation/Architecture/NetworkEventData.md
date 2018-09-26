# NetworkEventData<T> Class extends BaseEventData

The NetworkEventData<T> provides the data shared by all of the spatial awareness event types.

## Properties

### SourceID

| Type |
| --- |
| uint |

### value

| Type |
| --- |
| Generic <T> |

## Methods

### Initialize(T transferData)

Resets NetworkEventData and reinitializes it with value set to transferData.

### NetworkEventData(EventSystem eventSystem) : base(eventSystem)

Constructor method allows for generation of NetworkEventData.


