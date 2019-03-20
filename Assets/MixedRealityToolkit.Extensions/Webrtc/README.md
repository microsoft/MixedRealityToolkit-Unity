# Webrtc

Cross-platform Webrtc support for Unity apps ☁🎲

## Features

+ `winuwp` support (x86/x64 - [arm support is coming](https://github.com/webrtc-uwp/webrtc-uwp-sdk/issues/20))
+ `win32` support (x86/x64)
+ Multi-peer support
+ Stream audio/video/data
+ Managed wrapper for native plugins
+ Unity Abstraction layer
  + UnityEvent support
  + Thread-safe wrapping layer
  + Video Materials and Shaders
+ Examples
  + A/V calling
  + HTTP signaling server (for session establishment)
  
## How to use

To use this extension, you'll need to include some native dlls - you can download them yourself [from here](https://github.com/bengreenier/webrtc-unity-plugin/releases/tag/v01) - or build them [as described below](#building-native-plugins).

### Quickstart

1. Add a `Webrtc` Component
2. Add a `WebrtcPeerEvents` Component
3. Add an Event to `Webrtc.OnInitialized` that Invokes `WebrtcPeerEvents.OnInitialized()`
4. Add an Event to `Webrtc.OnInitialized` that Invokes `WebrtcPeerEvents.AddStream(bool audioOnly)` passing `false` as an argument
5. On Start, `Webrtc` will initialize the plugin for the platform, `WebrtcPeerEvents` will configure the peer Unity adapater, `WebrtcPeerEvents.AddStream(bool audioOnly)` will start local a/v

> Note: The order of operations is critical in the above quickstart - if you do not initialize components before adding a stream, webrtc will not work

### WertcBasicSignalExample

An example scene and prefab (the prefab contains all relevant example logic, the scene is provided for convenience) that demonstrate how to use a signaling server to establish a webrtc peer
connection. This scene uses a simple signaling protocol, as defined by [node-dss](https://github.com/bengreenier/node-dss). It also requires unique identifiers for each client, which may
differ from a more advanced signaling implementation.

1. Clone, setup, and run an instance of [node-dss](https://github.com/bengreenier/node-dss) (instructions at link).
2. Modify the `NodeDssSignaler` script inside the prefab (on the `WebrtcSignalControls` object) such that `Http Server Address` points at your server
3. Run the sample, noting the `deviceId` the appears in the top left of the client. This is your client's id. Let's refer to this as `Client A Id`.
4. On another device, run the sample as well, again noting the `deviceId`. Let's refer to this as `Client B Id`.
5. On the first client, enter the `Client B Id` into the `targetId` input field. On the second client, enter the `Client A Id`. This is how we inform each client of the other client's existence.
6. On either client, click the `offerButton`.
7. Session establishment data should be sent between the two clients, using the signaling server as a message broker. Shortly (less than 30s) audio and video should begin to be transmitted.

### Building Native Plugins

To build the native plugins, we'll leverage the [webrtc-uwp](https://github.com/webrtc-uwp) codebase for the webrtc `m71` release. This ensures support for `uwp`, as well as `win32`.

<details>
<summary> Windows build commands </summary>

To build the code, we'll first need the webrtc-uwp-sdk repository, and it's prerequisites. We'll also need to apply our patch that enables support for il2cpp (until it is merged). The detailed instructions can be found [in the webrtc-uwp docs](https://github.com/webrtc-uwp/webrtc-uwp-sdk/tree/James/20190225-m71-docs), and the exact commands that worked at the time of writing can be found below.

```
git clone --recursive https://github.com/webrtc-uwp/webrtc-uwp-sdk.git
git checkout releases/m71
git submodule update
#
# apply https://github.com/webrtc-uwp/webrtc/pull/11 on top of releases/m71
#
cd webrtc\xplatform\webrtc
git remote add bengreenier git@github.com:bengreenier/webrtc.git
git fetch bengreenier
git merge 67bc888e66
#
# apply https://github.com/webrtc-uwp/webrtc-windows/pull/48 on top of releases/m71
#
cd ..\..\windows
git remote add bengreenier git@github.com:bengreenier/webrtc-windows.git
git fetch bengreenier
git merge 7f1fdda64
#
# build (win32)
#
cd ..\..\
set GYP_MSVS_VERSION=2017
python scripts\run.py -a prepare build -u webrtc_unity_plugin -p win -x x64 -c release --clang
```

Note that to build for `winuwp` we use `WebRtc.Universal.sln` and Visual Studio, as documented [here](https://github.com/webrtc-uwp/webrtc-uwp-sdk/tree/James/20190225-m71-docs#building-the-sdk). Instead of building the `PeerConnectionClient.WebRtc` project, we build `Examples.UnityPlugin`.

</details>