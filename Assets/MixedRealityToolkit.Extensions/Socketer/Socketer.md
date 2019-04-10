 Summary:

      There are a thousand libraries for using sockets on Windows.  The problem is that each one tends to support 
      only one protocol, doesn't support UWP or *only* supports UWP, are still complex to use, and/or require very 
      different coding patterns.  Supporting another protocol means a different library with a completely different
      usage convention.  There are also unfortunately a number of caveats when using sockets.  They mostly just work, 
      until you find the situation where they just don't.  So, this is Yet Another Socket Library, that hopefully 
      gives greater flexibility for those situations, and greater ease of use.  It is intended for fast projects,
      not production code.
 
 Use (C#):

      SocketerClient l = SocketerClient.CreateListener(protocol, port);
      l.Start();
      l.Message += Socketer_Message;

      SocketerClient s = SocketerClient.CreateSender(protocol, host, port);
      s.Start();
      s.SendNetworkMessage(MessageType : uint, Message : string or byte[]);

 Use (Unity):

      Add a Socketer component in the scene.  Add your consumer code as another component:
 
      public class MyClass : MonoBehaviour
      {
          public Socketer Network;
          void Start()
          {
              Network.Message += Network_Message;
              Network.SendNetworkMessage(MessageType : uint, Message : string or byte[]);
          }
      }



  Considerations:

      The first choice is protocol:  TCP or UDP?

          UDP:  fast, simple, connectionless, unidirectional, no guaranteed ordering, no error checking, no way to 
          know if your packets are arriving
          TCP:  still pretty fast, connections (so you know if you're talking), bidirectional, guaranteed order of 
          arrival and error checking

      Port.  9999?  12345?

          This is a number that you make up and (usually) hardcode to your app.  Choose one between 1024 and 65535, 
          I recommend between 10000 and 40000 to reduce risk of collisions.  To see what's already being used on
          your machine, run "netstat -an" from a cmd window.

      Host.

          This is usually the IP address (run "ipconfig" from a cmd window).  It's worth it to put some thought into
          who the "server" will be in your app and who the "client" will be.  The client needs to know the server's
          hostname, and will usually store it in a config file or accept it via a UI.  Socketer calls these "sender"
          for client and "listener" for server.  "127.0.0.1" means "the same machine I'm on now".
      
      Protocol differences

          For Socketer, you can use either protocol with nearly no difference.  However, there are some subtle behaviors:

          *  The connect and disconnect events only fire for TCP.
          *  With TCP, both sender and listener can SendNetworkMessage.  With UDP, only sender can SendNetworkMessage.
          *  Also with TCP, multiple senders can talk to one listener, and when the listener calls SendNetworkMessage
              all of the connected senders will receive it.
          *  SendNetworkMessages that are made when the other side are unavailable are lost.  With UDP, there is no way to
              know if this happens.  With TCP, you may listen to the connection events.
          *  All Socketer components automatically try to connect/send, and if the recipient is lost (eg, the other side
              closes their app) they will reconnect/resume.
          *  Socketer automatically tries to make a TCP connection as soon as you call Start(), and continues to try until
              it succeeds.  This is convenient, but sometimes the attempts have a long timeout and this can cause it to take 
              awhile to connect if the timing of when the host becomes available is unlucky.

      Special cases
      
          *  UWP loopback.  If you need to talk to a UWP app that's running on your computer (this most often happens during
              development), localhost is blocked from receiving incoming connections.  It can initiate connections to another
              server that is NOT UWP.  This breaks UDP completely, and for TCP the UWP app must be the sender role.  Once
              the connection is established, you can use the bidirectional nature of TCP to communciate back and forth.
