#import <ARKit/ARKit.h>
#import <CoreVideo/CoreVideo.h>
#include "stdlib.h"
#include "UnityAppController.h"


typedef struct
{
    float x,y,z,w;
} UnityARVector4;

typedef struct
{
    UnityARVector4 column0;
    UnityARVector4 column1;
    UnityARVector4 column2;
    UnityARVector4 column3;
} UnityARMatrix4x4;

enum UnityARAlignment
{
    UnityARAlignmentGravity,
    UnityARAlignmentGravityAndHeading,
    UnityARAlignmentCamera
};

enum UnityARPlaneDetection
{
    UnityARPlaneDetectionNone = 0,
    UnityARPlaneDetectionHorizontal = (1 << 0),
    UnityARPlaneDetectionVertical = (1 << 1)
};


typedef struct
{
    UnityARAlignment alignment;
    uint32_t getPointCloudData;
    uint32_t enableLightEstimation;
    
} ARKitSessionConfiguration;

typedef struct
{
    UnityARAlignment alignment;
    UnityARPlaneDetection planeDetection;
    uint32_t getPointCloudData;
    uint32_t enableLightEstimation;

} ARKitWorldTrackingSessionConfiguration;

enum UnityARSessionRunOptions
{
    UnityARSessionRunOptionResetTracking           = (1 << 0),
    UnityARSessionRunOptionRemoveExistingAnchors   = (1 << 1)

};

typedef struct
{
    void* identifier;
    UnityARMatrix4x4 transform;
    ARPlaneAnchorAlignment alignment;
    UnityARVector4 center;
    UnityARVector4 extent;
} UnityARAnchorData;

typedef struct
{
    void* identifier;
    UnityARMatrix4x4 transform;
} UnityARUserAnchorData;

enum UnityARTrackingState
{
    UnityARTrackingStateNotAvailable,
    UnityARTrackingStateLimited,
    UnityARTrackingStateNormal,
};

enum UnityARTrackingReason
{
    UnityARTrackingStateReasonNone,
    UnityARTrackingStateReasonInitializing,
    UnityARTrackingStateReasonExcessiveMotion,
    UnityARTrackingStateReasonInsufficientFeatures,
};

typedef struct
{
    uint32_t yWidth;
    uint32_t yHeight;
    uint32_t screenOrientation;
    float texCoordScale;
    void* cvPixelBufferPtr;
}UnityVideoParams;

typedef struct
{
    float ambientIntensity;
    float ambientColorTemperature;
}UnityARLightEstimation;

typedef struct
{
    UnityARMatrix4x4 worldTransform;
    UnityARMatrix4x4 projectionMatrix;
    UnityARTrackingState trackingState;
    UnityARTrackingReason trackingReason;
    UnityVideoParams videoParams;
    UnityARLightEstimation lightEstimation;
    UnityARMatrix4x4 displayTransform;
    uint32_t getPointCloudData;
} UnityARCamera;

typedef struct
{
    vector_float3* pointCloud;
    NSUInteger pointCloudSize;
} UnityARPointCloudData;

typedef struct
{
    void* pYPixelBytes;
    void* pUVPixelBytes;
    BOOL bEnable;
}UnityPixelBuffer;



typedef void (*UNITY_AR_FRAME_CALLBACK)(UnityARCamera camera);
typedef void (*UNITY_AR_ANCHOR_CALLBACK)(UnityARAnchorData anchorData);
typedef void (*UNITY_AR_USER_ANCHOR_CALLBACK)(UnityARUserAnchorData anchorData);
typedef void (*UNITY_AR_SESSION_FAILED_CALLBACK)(const void* error);
typedef void (*UNITY_AR_SESSION_VOID_CALLBACK)(void);
typedef void (*UNITY_AR_SESSION_TRACKING_CHANGED)(UnityARCamera camera);

// These don't all need to be static data, but no other better place for them at the moment.
static id <MTLTexture> s_CapturedImageTextureY;
static id <MTLTexture> s_CapturedImageTextureCbCr;
static UnityARMatrix4x4 s_CameraProjectionMatrix;

static float s_AmbientIntensity;
static int s_TrackingQuality;
static float s_ShaderScale;
static const vector_float3* s_PointCloud;
static NSUInteger s_PointCloudSize;

static float unityCameraNearZ;
static float unityCameraFarZ;

static inline ARWorldAlignment GetARWorldAlignmentFromUnityARAlignment(UnityARAlignment& unityAlignment)
{
    switch (unityAlignment)
    {
        case UnityARAlignmentGravity:
            return ARWorldAlignmentGravity;
        case UnityARAlignmentGravityAndHeading:
            return ARWorldAlignmentGravityAndHeading;
        case UnityARAlignmentCamera:
            return ARWorldAlignmentCamera;
    }
}

static inline ARPlaneDetection GetARPlaneDetectionFromUnityARPlaneDetection(UnityARPlaneDetection planeDetection)
{
    ARPlaneDetection ret = ARPlaneDetectionNone;
    if ((planeDetection & UnityARPlaneDetectionNone) != 0)
        ret |= ARPlaneDetectionNone;
    if ((planeDetection & UnityARPlaneDetectionHorizontal) != 0)
        ret |= ARPlaneDetectionHorizontal;
    return ret;
}

static inline UnityARTrackingState GetUnityARTrackingStateFromARTrackingState(ARTrackingState trackingState)
{
    switch (trackingState) {
        case ARTrackingStateNormal:
            return UnityARTrackingStateNormal;
        case ARTrackingStateLimited:
            return UnityARTrackingStateLimited;
        case ARTrackingStateNotAvailable:
            return UnityARTrackingStateNotAvailable;
        default:
            [NSException raise:@"UnrecognizedARTrackingState" format:@"Unrecognized ARTrackingState: %ld", (long)trackingState];
            break;
    }
}

static inline UnityARTrackingReason GetUnityARTrackingReasonFromARTrackingReason(ARTrackingStateReason trackingReason)
{
    switch (trackingReason)
    {
        case ARTrackingStateReasonNone:
            return UnityARTrackingStateReasonNone;
        case ARTrackingStateReasonInitializing:
            return UnityARTrackingStateReasonInitializing;
        case ARTrackingStateReasonExcessiveMotion:
            return UnityARTrackingStateReasonExcessiveMotion;
        case ARTrackingStateReasonInsufficientFeatures:
            return UnityARTrackingStateReasonInsufficientFeatures;
        default:
            [NSException raise:@"UnrecognizedARTrackingStateReason" format:@"Unrecognized ARTrackingStateReason: %ld", (long)trackingReason];
            break;
    }
}

inline ARSessionRunOptions GetARSessionRunOptionsFromUnityARSessionRunOptions(UnityARSessionRunOptions runOptions)
{
    ARSessionRunOptions ret = 0;
    if ((runOptions & UnityARSessionRunOptionResetTracking) != 0)
        ret |= ARSessionRunOptionResetTracking;
    if ((runOptions & UnityARSessionRunOptionRemoveExistingAnchors) != 0)
        ret |= ARSessionRunOptionRemoveExistingAnchors;
    return ret;
}

inline void GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(ARKitWorldTrackingSessionConfiguration& unityConfig, ARWorldTrackingConfiguration* appleConfig)
{
    appleConfig.planeDetection = GetARPlaneDetectionFromUnityARPlaneDetection(unityConfig.planeDetection);
    appleConfig.worldAlignment = GetARWorldAlignmentFromUnityARAlignment(unityConfig.alignment);
    appleConfig.lightEstimationEnabled = (BOOL)unityConfig.enableLightEstimation;
}

inline void GetARSessionConfigurationFromARKitSessionConfiguration(ARKitSessionConfiguration& unityConfig, ARConfiguration* appleConfig)
{
    appleConfig.worldAlignment = GetARWorldAlignmentFromUnityARAlignment(unityConfig.alignment);
    appleConfig.lightEstimationEnabled = (BOOL)unityConfig.enableLightEstimation;
}

inline void ARKitMatrixToUnityARMatrix4x4(const matrix_float4x4& matrixIn, UnityARMatrix4x4* matrixOut)
{
    vector_float4 c0 = matrixIn.columns[0];
    matrixOut->column0.x = c0.x;
    matrixOut->column0.y = c0.y;
    matrixOut->column0.z = c0.z;
    matrixOut->column0.w = c0.w;

    vector_float4 c1 = matrixIn.columns[1];
    matrixOut->column1.x = c1.x;
    matrixOut->column1.y = c1.y;
    matrixOut->column1.z = c1.z;
    matrixOut->column1.w = c1.w;

    vector_float4 c2 = matrixIn.columns[2];
    matrixOut->column2.x = c2.x;
    matrixOut->column2.y = c2.y;
    matrixOut->column2.z = c2.z;
    matrixOut->column2.w = c2.w;

    vector_float4 c3 = matrixIn.columns[3];
    matrixOut->column3.x = c3.x;
    matrixOut->column3.y = c3.y;
    matrixOut->column3.z = c3.z;
    matrixOut->column3.w = c3.w;
}


static inline void GetUnityARCameraDataFromCamera(UnityARCamera& unityARCamera, ARCamera* camera, BOOL getPointCloudData)
{
    CGSize nativeSize = GetAppController().rootView.bounds.size;
    matrix_float4x4 projectionMatrix = [camera projectionMatrixForOrientation:[[UIApplication sharedApplication] statusBarOrientation] viewportSize:nativeSize zNear:(CGFloat)unityCameraNearZ zFar:(CGFloat)unityCameraFarZ];
    
    ARKitMatrixToUnityARMatrix4x4(projectionMatrix, &s_CameraProjectionMatrix);
    ARKitMatrixToUnityARMatrix4x4(projectionMatrix, &unityARCamera.projectionMatrix);
    
    unityARCamera.trackingState = GetUnityARTrackingStateFromARTrackingState(camera.trackingState);
    unityARCamera.trackingReason = GetUnityARTrackingReasonFromARTrackingReason(camera.trackingStateReason);
    unityARCamera.getPointCloudData = getPointCloudData;
}

inline void UnityARAnchorDataFromARAnchorPtr(UnityARAnchorData& anchorData, ARPlaneAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
    anchorData.alignment = nativeAnchor.alignment;
    anchorData.center.x = nativeAnchor.center.x;
    anchorData.center.y = nativeAnchor.center.y;
    anchorData.center.z = nativeAnchor.center.z;
    anchorData.extent.x = nativeAnchor.extent.x;
    anchorData.extent.y = nativeAnchor.extent.y;
    anchorData.extent.z = nativeAnchor.extent.z;
}

inline void UnityARMatrix4x4FromCGAffineTransform(UnityARMatrix4x4& outMatrix, CGAffineTransform displayTransform, BOOL isLandscape)
{
    if (isLandscape)
    {
        outMatrix.column0.x = displayTransform.a;
        outMatrix.column0.y = displayTransform.c;
        outMatrix.column0.z = displayTransform.tx;
        outMatrix.column1.x = displayTransform.b;
        outMatrix.column1.y = -displayTransform.d;
        outMatrix.column1.z = 1.0f - displayTransform.ty;
        outMatrix.column2.z = 1.0f;
        outMatrix.column3.w = 1.0f; 
    }
    else
    {
        outMatrix.column0.x = displayTransform.a;
        outMatrix.column0.y = -displayTransform.c;
        outMatrix.column0.z = -displayTransform.tx;
        outMatrix.column1.x = displayTransform.b;
        outMatrix.column1.y = displayTransform.d;
        outMatrix.column1.z = displayTransform.ty;
        outMatrix.column2.z = 1.0f;
        outMatrix.column3.w = 1.0f;
    }
}

inline void UnityARUserAnchorDataFromARAnchorPtr(UnityARUserAnchorData& anchorData, ARAnchor* nativeAnchor)
{
    anchorData.identifier = (void*)[nativeAnchor.identifier.UUIDString UTF8String];
    ARKitMatrixToUnityARMatrix4x4(nativeAnchor.transform, &anchorData.transform);
}


@protocol UnityARAnchorEventDispatcher
@required
    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor;
    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor;
    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor;
@end

@interface UnityARAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARAnchorCallbackWrapper

    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorAddedCallback(data);
    }

    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorRemovedCallback(data);
    }

    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
    {
        UnityARAnchorData data;
        UnityARAnchorDataFromARAnchorPtr(data, (ARPlaneAnchor*)anchor);
       _anchorUpdatedCallback(data);
    }

@end

@interface UnityARUserAnchorCallbackWrapper : NSObject <UnityARAnchorEventDispatcher>
{
@public
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorAddedCallback;
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorUpdatedCallback;
    UNITY_AR_USER_ANCHOR_CALLBACK _anchorRemovedCallback;
}
@end

@implementation UnityARUserAnchorCallbackWrapper

    -(void)sendAnchorAddedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorAddedCallback(data);
    }

    -(void)sendAnchorRemovedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorRemovedCallback(data);
    }

    -(void)sendAnchorUpdatedEvent:(ARAnchor*)anchor
    {
        UnityARUserAnchorData data;
        UnityARUserAnchorDataFromARAnchorPtr(data, anchor);
       _anchorUpdatedCallback(data);
    }

@end
static UnityPixelBuffer s_UnityPixelBuffers;

@interface UnityARSession : NSObject <ARSessionDelegate>
{
@public
    ARSession* _session;
    UNITY_AR_FRAME_CALLBACK _frameCallback;
    UNITY_AR_SESSION_FAILED_CALLBACK _arSessionFailedCallback;
    UNITY_AR_SESSION_VOID_CALLBACK _arSessionInterrupted;
    UNITY_AR_SESSION_VOID_CALLBACK _arSessionInterruptionEnded;
    UNITY_AR_SESSION_TRACKING_CHANGED _arSessionTrackingChanged;

    NSMutableDictionary* _classToCallbackMap;
    
    id <MTLDevice> _device;
    CVMetalTextureCacheRef _textureCache;
    BOOL _getPointCloudData;
}
@end

@implementation UnityARSession

- (id)init
{
    if (self = [super init])
    {
        _classToCallbackMap = [[NSMutableDictionary alloc] init];
    }
    return self;
}

- (void)setupMetal
{
    _device = MTLCreateSystemDefaultDevice();
    CVMetalTextureCacheCreate(NULL, NULL, _device, NULL, &_textureCache);
}

- (void)teardownMetal
{
    if (_textureCache) {
        CFRelease(_textureCache);
    }
}

static CGAffineTransform s_CurAffineTransform;

- (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame
{
    s_AmbientIntensity = frame.lightEstimate.ambientIntensity;
    s_TrackingQuality = (int)frame.camera.trackingState;
    s_PointCloud = frame.rawFeaturePoints.points;
    s_PointCloudSize = frame.rawFeaturePoints.count;

    UIInterfaceOrientation orient = [[UIApplication sharedApplication] statusBarOrientation];

    CGRect nativeBounds = [[UIScreen mainScreen] nativeBounds];
    CGSize nativeSize = GetAppController().rootView.bounds.size;
    UIInterfaceOrientation orientation = [[UIApplication sharedApplication] statusBarOrientation];
    s_CurAffineTransform = CGAffineTransformInvert([frame displayTransformForOrientation:orientation viewportSize:nativeSize]);

    UnityARCamera unityARCamera;

    GetUnityARCameraDataFromCamera(unityARCamera, frame.camera, _getPointCloudData);

    
    CVPixelBufferRef pixelBuffer = frame.capturedImage;
    
    size_t imageWidth = CVPixelBufferGetWidth(pixelBuffer);
    size_t imageHeight = CVPixelBufferGetHeight(pixelBuffer);
    
    float imageAspect = (float)imageWidth / (float)imageHeight;
    float screenAspect = nativeBounds.size.height / nativeBounds.size.width;
    unityARCamera.videoParams.texCoordScale =  screenAspect / imageAspect;
    s_ShaderScale = screenAspect / imageAspect;
    
    unityARCamera.lightEstimation.ambientIntensity = frame.lightEstimate.ambientIntensity;
    unityARCamera.lightEstimation.ambientColorTemperature = frame.lightEstimate.ambientColorTemperature;

    unityARCamera.videoParams.yWidth = (uint32_t)imageWidth;
    unityARCamera.videoParams.yHeight = (uint32_t)imageHeight;
    unityARCamera.videoParams.cvPixelBufferPtr = (void *) pixelBuffer;
    UnityARMatrix4x4 displayTransform;
    memset(&displayTransform, 0, sizeof(UnityARMatrix4x4));
    UnityARMatrix4x4FromCGAffineTransform(displayTransform, s_CurAffineTransform, UIInterfaceOrientationIsLandscape(orientation));
    unityARCamera.displayTransform = displayTransform;

    if (_frameCallback != NULL)
    {

        matrix_float4x4 rotatedMatrix = matrix_identity_float4x4;
        unityARCamera.videoParams.screenOrientation = 3;

        // rotation  matrix
        // [ cos    -sin]
        // [ sin     cos]
        switch (orient) {
            case UIInterfaceOrientationPortrait:
                rotatedMatrix.columns[0][0] = 0;
                rotatedMatrix.columns[0][1] = 1;
                rotatedMatrix.columns[1][0] = -1;
                rotatedMatrix.columns[1][1] = 0;
                unityARCamera.videoParams.screenOrientation = 1;
                break;
            case UIInterfaceOrientationLandscapeLeft:
                rotatedMatrix.columns[0][0] = -1;
                rotatedMatrix.columns[0][1] = 0;
                rotatedMatrix.columns[1][0] = 0;
                rotatedMatrix.columns[1][1] = -1;
                unityARCamera.videoParams.screenOrientation = 4;
                break;
            case UIInterfaceOrientationPortraitUpsideDown:
                rotatedMatrix.columns[0][0] = 0;
                rotatedMatrix.columns[0][1] = -1;
                rotatedMatrix.columns[1][0] = 1;
                rotatedMatrix.columns[1][1] = 0;
                unityARCamera.videoParams.screenOrientation = 2;
                break;
            default:
                break;
        }

        matrix_float4x4 matrix = matrix_multiply(frame.camera.transform, rotatedMatrix);

        ARKitMatrixToUnityARMatrix4x4(matrix, &unityARCamera.worldTransform);

        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
            _frameCallback(unityARCamera);
        });
    }

    
    if (CVPixelBufferGetPlaneCount(pixelBuffer) < 2 || CVPixelBufferGetPixelFormatType(pixelBuffer) != kCVPixelFormatType_420YpCbCr8BiPlanarFullRange) {
        return;
    }
    
    if (s_UnityPixelBuffers.bEnable)
    {
        
        CVPixelBufferLockBaseAddress(pixelBuffer, kCVPixelBufferLock_ReadOnly);
        
        if (s_UnityPixelBuffers.pYPixelBytes)
        {
            unsigned long numBytes = CVPixelBufferGetBytesPerRowOfPlane(pixelBuffer, 0) * CVPixelBufferGetHeightOfPlane(pixelBuffer,0);
            void* baseAddress = CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,0);
            memcpy(s_UnityPixelBuffers.pYPixelBytes, baseAddress, numBytes);
        }
        if (s_UnityPixelBuffers.pUVPixelBytes)
        {
            unsigned long numBytes = CVPixelBufferGetBytesPerRowOfPlane(pixelBuffer, 1) * CVPixelBufferGetHeightOfPlane(pixelBuffer,1);
            void* baseAddress = CVPixelBufferGetBaseAddressOfPlane(pixelBuffer,1);
            memcpy(s_UnityPixelBuffers.pUVPixelBytes, baseAddress, numBytes);
        }
        
        CVPixelBufferUnlockBaseAddress(pixelBuffer, 0);
    }
    
    id<MTLTexture> textureY = nil;
    id<MTLTexture> textureCbCr = nil;

    // textureY
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 0);
        const size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 0);
        MTLPixelFormat pixelFormat = MTLPixelFormatR8Unorm;
        
        
        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, pixelBuffer, NULL, pixelFormat, width, height, 0, &texture);
        if(status == kCVReturnSuccess)
        {
            textureY = CVMetalTextureGetTexture(texture);
            CFRelease(texture);
        }
    }

    // textureCbCr
    {
        const size_t width = CVPixelBufferGetWidthOfPlane(pixelBuffer, 1);
        const size_t height = CVPixelBufferGetHeightOfPlane(pixelBuffer, 1);
        MTLPixelFormat pixelFormat = MTLPixelFormatRG8Unorm;

        CVMetalTextureRef texture = NULL;
        CVReturn status = CVMetalTextureCacheCreateTextureFromImage(NULL, _textureCache, pixelBuffer, NULL, pixelFormat, width, height, 1, &texture);
        if(status == kCVReturnSuccess)
        {
            textureCbCr = CVMetalTextureGetTexture(texture);
            CFRelease(texture);
        }
    }

    if (textureY != nil && textureCbCr != nil) {
        dispatch_async(dispatch_get_main_queue(), ^{
            // always assign the textures atomic
            s_CapturedImageTextureY = textureY;
            s_CapturedImageTextureCbCr = textureCbCr;
        });
    }
}

- (void)session:(ARSession *)session didFailWithError:(NSError *)error
{
    if (_arSessionFailedCallback != NULL)
    {
        _arSessionFailedCallback(static_cast<const void*>([[error localizedDescription] UTF8String]));
    }
}

- (void)session:(ARSession *)session didAddAnchors:(NSArray<ARAnchor*>*)anchors
{
    [self sendAnchorAddedEventToUnity:anchors];
}

- (void)session:(ARSession *)session didUpdateAnchors:(NSArray<ARAnchor*>*)anchors
{
   [self sendAnchorUpdatedEventToUnity:anchors];
}

- (void)session:(ARSession *)session didRemoveAnchors:(NSArray<ARAnchor*>*)anchors
{
   [self sendAnchorRemovedEventToUnity:anchors];
}

- (void) sendAnchorAddedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorAddedEvent:anchorPtr];
    }
}

- (void)session:(ARSession *)session cameraDidChangeTrackingState:(ARCamera *)camera
{
    if (_arSessionTrackingChanged != NULL)
    {
        UnityARCamera unityCamera;
        GetUnityARCameraDataFromCamera(unityCamera, camera, _getPointCloudData);
        _arSessionTrackingChanged(unityCamera);
    }
}

- (void)sessionWasInterrupted:(ARSession *)session
{
    if (_arSessionInterrupted != NULL)
    {
        _arSessionInterrupted();

    }
}

- (void)sessionInterruptionEnded:(ARSession *)session
{
    if (_arSessionInterruptionEnded != NULL)
    {
        _arSessionInterruptionEnded();
    }
}

- (void) sendAnchorRemovedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorRemovedEvent:anchorPtr];
    }
}

- (void) sendAnchorUpdatedEventToUnity:(NSArray<ARAnchor*>*)anchors
{
    for (ARAnchor* anchorPtr in anchors)
    {
        id<UnityARAnchorEventDispatcher> dispatcher = [_classToCallbackMap objectForKey:[anchorPtr class]];
        [dispatcher sendAnchorUpdatedEvent:anchorPtr];
    }
}

@end

/// Create the native mirror to the C# ARSession object

extern "C" void* unity_CreateNativeARSession()
{
    UnityARSession *nativeSession = [[UnityARSession alloc] init];
    nativeSession->_session = [ARSession new];
    nativeSession->_session.delegate = nativeSession;
    unityCameraNearZ = .01;
    unityCameraFarZ = 30;
    s_UnityPixelBuffers.bEnable = false;
    return (__bridge_retained void*)nativeSession;
}

extern "C" void session_SetSessionCallbacks(const void* session, UNITY_AR_FRAME_CALLBACK frameCallback,
                                            UNITY_AR_SESSION_FAILED_CALLBACK sessionFailed,
                                            UNITY_AR_SESSION_VOID_CALLBACK sessionInterrupted,
                                            UNITY_AR_SESSION_VOID_CALLBACK sessionInterruptionEnded,
                                            UNITY_AR_SESSION_TRACKING_CHANGED trackingChanged)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    nativeSession->_frameCallback = frameCallback; 
    nativeSession->_arSessionFailedCallback = sessionFailed;
    nativeSession->_arSessionInterrupted = sessionInterrupted;
    nativeSession->_arSessionInterruptionEnded = sessionInterruptionEnded;
    nativeSession->_arSessionTrackingChanged = trackingChanged;
}

extern "C" void session_SetPlaneAnchorCallbacks(const void* session, UNITY_AR_ANCHOR_CALLBACK anchorAddedCallback, 
                                            UNITY_AR_ANCHOR_CALLBACK anchorUpdatedCallback, 
                                            UNITY_AR_ANCHOR_CALLBACK anchorRemovedCallback)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    UnityARAnchorCallbackWrapper* anchorCallbacks = [[UnityARAnchorCallbackWrapper alloc] init];
    anchorCallbacks->_anchorAddedCallback = anchorAddedCallback;
    anchorCallbacks->_anchorUpdatedCallback = anchorUpdatedCallback;
    anchorCallbacks->_anchorRemovedCallback = anchorRemovedCallback;
    [nativeSession->_classToCallbackMap setObject:anchorCallbacks forKey:[ARPlaneAnchor class]];
}

extern "C" void session_SetUserAnchorCallbacks(const void* session, UNITY_AR_USER_ANCHOR_CALLBACK userAnchorAddedCallback, 
                                            UNITY_AR_USER_ANCHOR_CALLBACK userAnchorUpdatedCallback, 
                                            UNITY_AR_USER_ANCHOR_CALLBACK userAnchorRemovedCallback)
{
    UnityARSession* nativeSession = (__bridge UnityARSession*)session;
    UnityARUserAnchorCallbackWrapper* userAnchorCallbacks = [[UnityARUserAnchorCallbackWrapper alloc] init];
    userAnchorCallbacks->_anchorAddedCallback = userAnchorAddedCallback;
    userAnchorCallbacks->_anchorUpdatedCallback = userAnchorUpdatedCallback;
    userAnchorCallbacks->_anchorRemovedCallback = userAnchorRemovedCallback;
    [nativeSession->_classToCallbackMap setObject:userAnchorCallbacks forKey:[ARAnchor class]];
}

extern "C" void StartWorldTrackingSessionWithOptions(void* nativeSession, ARKitWorldTrackingSessionConfiguration unityConfig, UnityARSessionRunOptions runOptions)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARWorldTrackingConfiguration* config = [ARWorldTrackingConfiguration new];
    ARSessionRunOptions runOpts = GetARSessionRunOptionsFromUnityARSessionRunOptions(runOptions);
    GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config options:runOpts ];
    [session setupMetal];
}



extern "C" void StartWorldTrackingSession(void* nativeSession, ARKitWorldTrackingSessionConfiguration unityConfig)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARWorldTrackingConfiguration* config = [ARWorldTrackingConfiguration new];
    GetARSessionConfigurationFromARKitWorldTrackingSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config];
    [session setupMetal];
}

extern "C" void StartSessionWithOptions(void* nativeSession, ARKitSessionConfiguration unityConfig, UnityARSessionRunOptions runOptions)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARConfiguration* config = [AROrientationTrackingConfiguration new];
    ARSessionRunOptions runOpts = GetARSessionRunOptionsFromUnityARSessionRunOptions(runOptions);
    GetARSessionConfigurationFromARKitSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config options:runOpts ];
    [session setupMetal];
}



extern "C" void StartSession(void* nativeSession, ARKitSessionConfiguration unityConfig)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARConfiguration* config = [AROrientationTrackingConfiguration new];
    GetARSessionConfigurationFromARKitSessionConfiguration(unityConfig, config);
    session->_getPointCloudData = (BOOL) unityConfig.getPointCloudData;
    [session->_session runWithConfiguration:config];
    [session setupMetal];
}

extern "C" void PauseSession(void* nativeSession)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    [session->_session pause];
}

extern "C" void StopSession(void* nativeSession)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    [session teardownMetal];
}

extern "C" UnityARUserAnchorData SessionAddUserAnchor(void* nativeSession, UnityARUserAnchorData anchorData)
{
    // create a native ARAnchor and add it to the session
    // then return the data back to the user that they will
    // need in case they want to remove it
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    ARAnchor *newAnchor = [[ARAnchor alloc] initWithTransform:matrix_identity_float4x4];
    
    [session->_session addAnchor:newAnchor];
    UnityARUserAnchorData returnAnchorData;
    UnityARUserAnchorDataFromARAnchorPtr(returnAnchorData, newAnchor);
    return returnAnchorData;
}

extern "C" void SessionRemoveUserAnchor(void* nativeSession, const char * anchorIdentifier)
{
    // go through anchors and find the right one
    // then remove it from the session
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    for (ARAnchor* a in session->_session.currentFrame.anchors)
    {
        if ([[a.identifier UUIDString] isEqualToString:[NSString stringWithUTF8String:anchorIdentifier]])
        {
            [session->_session removeAnchor:a];
            return;
        }
    }
}

extern "C" void SetCameraNearFar (float nearZ, float farZ)
{
    unityCameraNearZ = nearZ;
    unityCameraFarZ = farZ;
}

extern "C" void CapturePixelData (uint32_t enable, void* pYPixelBytes, void *pUVPixelBytes)
{
    s_UnityPixelBuffers.bEnable = (BOOL) enable;
    if (s_UnityPixelBuffers.bEnable)
    {
        s_UnityPixelBuffers.pYPixelBytes = pYPixelBytes;
        s_UnityPixelBuffers.pUVPixelBytes = pUVPixelBytes;
    } else {
        s_UnityPixelBuffers.pYPixelBytes = NULL;
        s_UnityPixelBuffers.pUVPixelBytes = NULL;
    }
}

extern "C" struct HitTestResult
{
    void* ptr;
    int count;
};

// Must match ARHitTestResult in ARHitTestResult.cs
extern "C" struct UnityARHitTestResult
{
    ARHitTestResultType type;
    double distance;
    UnityARMatrix4x4 localTransform;
    UnityARMatrix4x4 worldTransform;
    void* anchorPtr;
    bool isValid;
};

// Must match ARTextureHandles in UnityARSession.cs
extern "C" struct UnityARTextureHandles
{
    void* textureY;
    void* textureCbCr;
};

// Cache results locally
static NSArray<ARHitTestResult *>* s_LastHitTestResults;

// Returns the number of hits and caches the results internally
extern "C" int HitTest(void* nativeSession, CGPoint point, ARHitTestResultType types)
{
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    point = CGPointApplyAffineTransform(CGPointMake(point.x, 1.0f - point.y), CGAffineTransformInvert(CGAffineTransformInvert(s_CurAffineTransform)));
    s_LastHitTestResults = [session->_session.currentFrame hitTest:point types:types];

    return (int)[s_LastHitTestResults count];
}
extern "C" UnityARHitTestResult GetLastHitTestResult(int index)
{
    UnityARHitTestResult unityResult;
    memset(&unityResult, 0, sizeof(UnityARHitTestResult));

    if (s_LastHitTestResults != nil && index >= 0 && index < [s_LastHitTestResults count])
    {
        ARHitTestResult* hitResult = s_LastHitTestResults[index];
        unityResult.type = hitResult.type;
        unityResult.distance = hitResult.distance;
        ARKitMatrixToUnityARMatrix4x4(hitResult.localTransform, &unityResult.localTransform);
        ARKitMatrixToUnityARMatrix4x4(hitResult.worldTransform, &unityResult.worldTransform);
        unityResult.anchorPtr = (void*)[hitResult.anchor.identifier.UUIDString UTF8String];
        unityResult.isValid = true;
    }

    return unityResult;
}

extern "C" UnityARTextureHandles GetVideoTextureHandles()
{
    UnityARTextureHandles handles;
    handles.textureY = (__bridge_retained void*)s_CapturedImageTextureY;
    handles.textureCbCr = (__bridge_retained void*)s_CapturedImageTextureCbCr;

    return handles;
}

extern "C" bool GetARPointCloud(float** verts, unsigned int* vertLength)
{
    *verts = (float*)s_PointCloud;
    *vertLength = (unsigned int)s_PointCloudSize * 4;
    return YES;
}

extern "C" UnityARMatrix4x4 GetCameraProjectionMatrix()
{
    return s_CameraProjectionMatrix;
}

extern "C" float GetAmbientIntensity()
{
    return s_AmbientIntensity;
}

extern "C" int GetTrackingQuality()
{
    return s_TrackingQuality;
}

extern "C" bool IsARKitWorldTrackingSessionConfigurationSupported()
{
    return ARWorldTrackingConfiguration.isSupported;
}

extern "C" bool IsARKitSessionConfigurationSupported()
{
    return AROrientationTrackingConfiguration.isSupported;
}
