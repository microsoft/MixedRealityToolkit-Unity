# IPhotonNetworking<T> Class

## Properties

### Identity

| Type |
| --- |
| int

### typeCode

| Type |
| --- |
| TypeCode |

### dataStream

| Type |
| --- |
| Generic <T> |

### NED

| Type |
| --- |
| [NetworkEventData<T>](./NetworkEventData.md) |


## Methods


### Send(int ID, T data, float reliability)


### getObjectById(int id)

| Return Type |
| --- |
| GameObject |

### OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)

Photon function which synchronizes the Identity, typeCode, and dataStream in that order.

When data is recieved this function generates a NetworkEventData with the dataStream as the value.



