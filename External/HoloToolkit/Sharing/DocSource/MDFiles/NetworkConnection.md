NetworkConnection                       {#networkconnectionoverview}
============

## Overview
The NetworkConnection class is an abstract interface that represents a connection to a remote peer, whether that connection is 
established or not.  It is used by all the connected systems, at all levels of the software stack.  It is implemented in C++, 
and used by C++, C#, and Java code.  Code that inherits from the NetworkCallback interface can register itself to receive incoming 
messages of a particular type, as well as notifications when the NetworkConnection is connected, disconnected, or a connection attempt fails.  

## Creation of new outgoing messages
The first byte in a message packet will always be the Message's ID by convention, and enforced throughout the code. 

NetworkConnection maintains a pool of NetworkOutMessages.  Users can get a message from the pool, fill it with data, and send it 
through the NetworkConnection.  A NetworkOutMessage's message ID is set at the time that the user acquires it.

NetworkOutMessage provides several Write() functions for appending data of different types to the message.  Using this per-object type 
interface allows us to convert the endian of the data if necessary.  It is also possible to add a block of data to the message unmodified.  

## Receiving Incoming Messages

To receive a message, the user must implement a class that inherits from NetworkCallback, then register it with the NetworkConnection, 
specifying what message type it wants to receive.  The NetworkConnection will then notify all registered objects when it establishes or 
loses connection with the remote host, and pass incoming messages to the appropriate object.

When receiving a message, the object will receive a NetworkInMessage, which has a Read*() function for each Write() function in NetworkOutMessage. 

## Send Options
The NetworkConnection.Send() function has several additional parameters that allow you to have better control over how your packets are 
delivered. The parameters have default values if you aren’t sure which to use.
If the packet has to pass through multiple machines to get to its destination (eg: for tunneling), then these same settings are used for all
 hops.
 
### MessagePriority
Defines the priority of the message relative to others.  Messages with higher priority will be sent sooner than lower priority messages
* *Immediate:* These message trigger sends immediately, and are generally not buffered or aggregated into a single datagram. Messages below this priority are buffered to be sent in groups at 10 millisecond intervals to reduce UDP overhead and better congestion control.
* *High:* For every 2 Immediate priority messages, 1 High priority will be sent
* *Medium:* For every 2 High priority messages, 1 Medium priority will be sent
* *Low:* For every 2 Medium priority messages, 1 Low priority will be sent


### MessageReliability
Used to specify how hard the system should try to ensure that the messages arrive and arrive in order
* *Unreliable:* Same as regular UDP, except that it will also discard duplicate datagrams.  
* *UnreliableSequenced:* Messages in the same channel will arrive in the sequence you sent it, but are not guaranteed to arrive.  Out or order messages will be dropped. 
* *Reliable:* The message is sent reliably, but not necessarily in any order.  
* *ReliableOrdered:* Message is reliable and will arrive in the order you sent it with other messages in the channel.  Messages will be delayed while waiting for out of order messages. 
* *ReliableSequenced:* Message is reliable and will arrive in the sequence you sent it with other messages in the channel.  Out or order messages will be dropped


### MessageChannel
Messages in the same channel sent with an ordered or sequenced level of reliability will arrive in the order sent.  HoloToolkit.Sharing internal 
ones are defined here; user-defined channels should start at UserMessageChannelStart.  Note that ordered messages in different channels can arrive 
in a different order from each other.  This can create hard to find bugs, so only used channels other than Default for messages that really 
require it.  
* *Default:* The main channel for data
* *Mouse:* Mouse position updates
* *Avatar:* Traffic related to avatar position and movement
* *Audio:* Voice audio traffic
* *UserMessageChannelStart:* User code should use IDs from UserMessageChannelStart to 255

## Broadcast
Similar to Send, Broadcast will wrap the outgoing message with a broadcast header.  Upon arrival at the remote peer, if 
a BroadcastForwarder instance is registered with the receiving NetworkConnection, it will strip off the header and forward the embedded 
message to all the other connected peers.