# NetworkingBase<T> Class

## Properties

### NetworkingMode

This is the selected type of networking that will be used.
It is selected from the following NetworkingType Enum: [NetworkingType](./NetworkingType.md)

Is set to Photon by default.

### defualtID

| Type |
| --- |
| int |

### defaultReliability

| Type |
| --- |
| int |


## Send Methods

All the below methods are overloads that require at least a piece of data in a generic form and optionally allow you to specify the object you are sending data to as well as the reliability with which you would like the data sent.


### Send(T data)


### Send(int ID, T data)


### Send(T data, float reliability)


### Send(int ID, T data, float reliability)





