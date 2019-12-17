# Input animation binary file format specification

## Overall structure

The input animation binary file begins with a 64 bit integer magic number. The value of this number in hexadecimal notation is `0x6a8faf6e0f9e42c6` and can be used to identify valid input animation files.

The next eight bytes are two Int32 values declaring the major and minor version number of the file.

The rest of the file is taken up by animation data, which may change between version numbers.

| Section | Type |
|---------|------|
| Magic Number | Int64 |
| Major Version Number | Int32 |
| Minor Version Number | Int32 |
| Animation Data | _see version section_ |

## Version 1.0

The input animation data consists of a sequence of animation curves. The number and meaning of animation curves is fixed, but each curve can have a different number of keyframes.

| Section | Type |
|---------|------|
| Camera | [Pose Curves](#pose-curves) |
| Hand Tracked Left | [Boolean Curve](#boolean-curve) |
| Hand Tracked Right | [Boolean Curve](#boolean-curve) |
| Hand Pinching Left | [Boolean Curve](#boolean-curve) |
| Hand Pinching Right | [Boolean Curve](#boolean-curve) |
| Hand Joints Left | [Joint Pose Curves](#joint-pose-curves) |
| Hand Joints Right | [Joint Pose Curves](#joint-pose-curves) |

### Joint pose curves

For each hand a sequence of joint animation curves is stored. The number of joints is fixed, and a set of pose curves is stored for each joint.

| Section | Type |
|---------|------|
| None | [Pose Curves](#pose-curves) |
| Wrist | [Pose Curves](#pose-curves) |
| Palm | [Pose Curves](#pose-curves) |
| ThumbMetacarpalJoint | [Pose Curves](#pose-curves) |
| ThumbProximalJoint | [Pose Curves](#pose-curves) |
| ThumbDistalJoint | [Pose Curves](#pose-curves) |
| ThumbTip | [Pose Curves](#pose-curves) |
| IndexMetacarpal | [Pose Curves](#pose-curves) |
| IndexKnuckle | [Pose Curves](#pose-curves) |
| IndexMiddleJoint | [Pose Curves](#pose-curves) |
| IndexDistalJoint | [Pose Curves](#pose-curves) |
| IndexTip | [Pose Curves](#pose-curves) |
| MiddleMetacarpal | [Pose Curves](#pose-curves) |
| MiddleKnuckle | [Pose Curves](#pose-curves) |
| MiddleMiddleJoint | [Pose Curves](#pose-curves) |
| MiddleDistalJoint | [Pose Curves](#pose-curves) |
| MiddleTip | [Pose Curves](#pose-curves) |
| RingMetacarpal | [Pose Curves](#pose-curves) |
| RingKnuckle | [Pose Curves](#pose-curves) |
| RingMiddleJoint | [Pose Curves](#pose-curves) |
| RingDistalJoint | [Pose Curves](#pose-curves) |
| RingTip | [Pose Curves](#pose-curves) |
| PinkyMetacarpal | [Pose Curves](#pose-curves) |
| PinkyKnuckle | [Pose Curves](#pose-curves) |
| PinkyMiddleJoint | [Pose Curves](#pose-curves) |
| PinkyDistalJoint | [Pose Curves](#pose-curves) |
| PinkyTip | [Pose Curves](#pose-curves) |

### Pose curves

Pose curves are a sequence of 3 animation curves for the position vector, followed by 4 animation curves for the rotation quaternion.

| Section | Type |
|---------|------|
| Position X | [Float Curve](#float-curve) |
| Position Y | [Float Curve](#float-curve) |
| Position Z | [Float Curve](#float-curve) |
| Rotation X | [Float Curve](#float-curve) |
| Rotation Y | [Float Curve](#float-curve) |
| Rotation Z | [Float Curve](#float-curve) |
| Rotation W | [Float Curve](#float-curve) |

### Float curve

Floating point curves are fully fledged Bézier curves with a variable number of keyframes. Each keyframe stores a time and a curve value, as well as tangents and weights on the left and right side of each keyframe.

| Section | Type |
|---------|------|
| Pre-Wrap Mode | Int32, [Wrap Mode](#wrap-mode) |
| Post-Wrap Mode | Int32, [Wrap Mode](#wrap-mode) |
| Number of keyframes | Int32 |
| Keyframes | [Float Keyframe](#float-keyframe) |

### Float keyframe

A float keyframe stores tangent and weight values alongside the basic time and value.

| Section | Type |
|---------|------|
| Time | Float32 |
| Value | Float32 |
| InTangent | Float32 |
| OutTangent | Float32 |
| InWeight | Float32 |
| OutWeight | Float32 |
| WeightedMode | Int32, [Weighted Mode](#weighted-mode) |

### Boolean curve

Boolean curves are simple sequences of on/off values. On every keyframe the value of the curve flips immediately.

| Section | Type |
|---------|------|
| Pre-Wrap Mode | Int32, [Wrap Mode](#wrap-mode) |
| Post-Wrap Mode | Int32, [Wrap Mode](#wrap-mode) |
| Number of keyframes | Int32 |
| Keyframes | [Boolean Keyframe](#boolean-keyframe) |

### Boolean keyframe

A boolean keyframe only stores a time and value.

| Section | Type |
|---------|------|
| Time | Float32 |
| Value | Float32 |

### Wrap mode

The semantics of Pre- and Post-Wrap modes follow the [Unity WrapMode](https://docs.unity3d.com/ScriptReference/WrapMode.html) definition. They are a combination of the following bits:

| Value | Meaning |
|-------|---------|
| 0 | Default: Reads the default repeat mode set higher up. |
| 1 | Once: When time reaches the end of the animation clip, the clip will automatically stop playing and time will be reset to beginning of the clip. |
| 2 | Loop: When time reaches the end of the animation clip, time will continue at the beginning. |
| 4 | PingPong: When time reaches the end of the animation clip, time will ping pong back between beginning and end. |
| 8 | ClampForever: Plays back the animation. When it reaches the end, it will keep playing the last frame and never stop playing. |

### Weighted mode

The semantics of the Weighted mode follow the [Unity WeightedMode](https://docs.unity3d.com/ScriptReference/WeightedMode.html) definition.

| Value | Meaning |
|-------|---------|
| 0 | None: Exclude both inWeight or outWeight when calculating curve segments. |
| 1 | In: Include inWeight when calculating the previous curve segment. |
| 2 | Out: Include outWeight when calculating the next curve segment. |
| 3 | Both: Include inWeight and outWeight when calculating curve segments. |
