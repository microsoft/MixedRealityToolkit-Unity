// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

package com.Microsoft.MixedReality.Toolkit;

import com.unity3d.player.UnityPlayerActivity;

import android.Manifest;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.hardware.display.DisplayManager;
import android.hardware.display.VirtualDisplay;
import android.media.CamcorderProfile;
import android.media.MediaRecorder;
import android.media.projection.*;
import android.os.Bundle;
import android.util.Log;

import java.io.File;

enum ScreenRecorderActivityState {
    NO_PERMISSIONS,
    READY,
    INITIALIZING,
    INITIALIZED,
    RECORDING
}

public class ScreenRecorderActivity extends UnityPlayerActivity {
    final int PERMISSIONS_REQUEST_CODE = 99998;
    final int SCREEN_CAPTURE_REQUEST_CODE = 99999;
    final String TAG = "ScreenRecorderActivity";
    final String VIRTUAL_DISPLAY_NAME = "ScreenRecorderActivityVirtualDisplay";
    final String NO_ERROR_MESSAGE = "No errors";
    final String DIRECTORY_NAME = "/sdcard/";

    private String[] permissions;
    private String fileName = "";
    private String lastErrorMessage = NO_ERROR_MESSAGE;

    private MediaProjectionManager manager;
    private MediaProjection projection;
    private MediaRecorder recorder;
    private VirtualDisplay display;
    private ScreenRecorderActivityState state;
    boolean recordingCaptured = false;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Log.d(TAG, "Activity created");

        permissions = new String[]{
                Manifest.permission.RECORD_AUDIO,
                Manifest.permission.WRITE_EXTERNAL_STORAGE
        };
        requestPermissions(permissions, PERMISSIONS_REQUEST_CODE);

        state = ScreenRecorderActivityState.NO_PERMISSIONS;
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == PERMISSIONS_REQUEST_CODE)
        {
            Log.d(TAG, "Handling record audio permission request result");
            if (permissions != null &&
                permissions.length == grantResults.length) {
                boolean obtainedPermissions = true;
                for (int i = 0; i < grantResults.length; i++) {
                    if (grantResults[i] != PackageManager.PERMISSION_GRANTED) {
                        obtainedPermissions = false;
                        break;
                    }
                }

                if (obtainedPermissions) {
                    state = ScreenRecorderActivityState.READY;
                    return;
                }
            }

            Log.d(TAG, "Failed to obtain record audio permission");
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == SCREEN_CAPTURE_REQUEST_CODE) {
            Log.d(TAG, "Handling screen capture request result");
            HandleScreenCaptureResult(resultCode, data);
        }
    }

    public boolean Initialize(String fileName) {
        if (state == ScreenRecorderActivityState.READY)
        {
            state = ScreenRecorderActivityState.INITIALIZING;

            this.fileName = fileName;
            Context context = this.getApplicationContext();
            manager = (MediaProjectionManager) context.getSystemService(Context.MEDIA_PROJECTION_SERVICE);
            Intent intent = manager.createScreenCaptureIntent();
            this.startActivityForResult(manager.createScreenCaptureIntent(), SCREEN_CAPTURE_REQUEST_CODE);
            return true;
        }

        return false;
    }

    public boolean IsInitialized(){
        return (state == ScreenRecorderActivityState.INITIALIZED);
    }

    public boolean StartRecording() {
        if (!IsReadyToRecord()) {
            return false;
        }

        if (state == ScreenRecorderActivityState.INITIALIZED) {
            recorder.start();
            Log.d(TAG, "Started recording: " + fileName);
            state = ScreenRecorderActivityState.RECORDING;
            return true;
        } else {
            Log.d(TAG, "StartRecording called when already recording");
            return false;
        }
    }

    public boolean StopRecording() {
        if (!IsReadyToRecord()) {
            return false;
        }

        if (state == ScreenRecorderActivityState.RECORDING) {
            recorder.stop();
            state = ScreenRecorderActivityState.READY;
            recordingCaptured = true;
            Log.d(TAG, "Stopped recording: " + fileName);

            CleanUp();
            return true;
        } else {
            Log.d(TAG, "StopRecording called when not recording");
            return false;
        }
    }

    public boolean IsRecordingAvailable(){
        return (state == ScreenRecorderActivityState.READY) && recordingCaptured;
    }

    public boolean ShowRecording() {
        if (IsRecordingAvailable())
        {
            Intent showRecordingIntent = new Intent(Intent.ACTION_VIEW);
            showRecordingIntent.setType("video/*");
            this.startActivity(showRecordingIntent);
            return true;
        }

        return false;
    }


    public String GetLastErrorMessage() {
        return lastErrorMessage;
    }

     private void CleanUp() {
        if (manager != null) {
            manager = null;
        }

        if (display != null)
        {
            display.release();
            display = null;
        }

        if (recorder != null){
            if (state == ScreenRecorderActivityState.RECORDING)
                recorder.stop();

            recorder.release();
            recorder = null;
        }

        if (projection != null) {
            projection.stop();
            projection = null;
        }

        state = ScreenRecorderActivityState.READY;
    }

    private boolean IsReadyToRecord() {
        if (state == ScreenRecorderActivityState.NO_PERMISSIONS)
        {
            Log.d(TAG, "Failed to obtain necessary permissions when starting app");
            return false;
        }

        if (state == ScreenRecorderActivityState.READY || state == ScreenRecorderActivityState.INITIALIZING) {
            Log.d(TAG, "ScreenRecorderActivity is not yet initialized");
            return false;
        }

        if (recorder == null)
        {
            Log.d(TAG, "MediaRecorder has not yet been created");
            return false;
        }

        return true;
    }


    private void HandleScreenCaptureResult(int resultCode, Intent data) {
        if (manager == null)
        {
            Log.d(TAG, "MediaProjectionManager was null when attempting to initialize");
            return;
        }

        projection = (MediaProjection) manager.getMediaProjection(resultCode, data);
        if (projection == null)
        {
            Log.d(TAG, "Failed to obtain media projection");
            return;
        }

        DisplayMetrics metrics = getResources().getDisplayMetrics();
        CamcorderProfile profile = CamcorderProfile.get(CamcorderProfile.QUALITY_HIGH);
        profile.videoFrameWidth = metrics.widthPixels;
        profile.videoFrameHeight = metrics.heightPixels;

        try
        {
            recorder = new MediaRecorder();
            recorder.setAudioSource(MediaRecorder.AudioSource.MIC);
            recorder.setVideoSource(MediaRecorder.VideoSource.SURFACE);
            recorder.setProfile(profile);
            recorder.setOutputFile(DIRECTORY_NAME + fileName);
        } catch (Exception e)
        {
            Log.d(TAG, "Failed to create MediaRecorder");
            lastErrorMessage = e.getMessage();
            CleanUp();
            return;
        }

        try {
            recorder.prepare();
        } catch (Exception e) {
            Log.d(TAG, "Failed to prepare MediaRecorder");
            lastErrorMessage = e.getMessage();
            CleanUp();
            return;
        }

        try {
            display = projection.createVirtualDisplay(
                    VIRTUAL_DISPLAY_NAME,
                    profile.videoFrameWidth,
                    profile.videoFrameHeight,
                    metrics.densityDpi,
                    DisplayManager.VIRTUAL_DISPLAY_FLAG_AUTO_MIRROR,
                    recorder.getSurface(),
                    null,
                    null);
        } catch (Exception e){
            Log.d(TAG, "Failed to create VirtualDisplay");
            lastErrorMessage = e.getMessage();
            CleanUp();
            return;
        }

        Log.d(TAG, "ScreenRecorderActivity was successfully initialized");
        state = ScreenRecorderActivityState.INITIALIZED;
    }
}
