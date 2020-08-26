   $UnityPath = ${Env:${{ parameters.UnityVersion }}}
   # Find unity.exe as Start-UnityEditor currently doesn't support arbitrary parameters
   $editor = Get-ChildItem $UnityPath -Filter 'Unity.exe' -Recurse | Select-Object -First 1 -ExpandProperty FullName
   
   # The build output goes to a unique combination of Platform + Arch + Scene to ensure that
   # each build will have a fresh destination folder.
   $outDir = "$(Build.ArtifactStagingDirectory)\build\${{ parameters.UnityVersion }}\${{ parameters.Platform }}_${{ parameters.Arch }}_${{ parameters.Scene }}"
   $logFile = New-Item -Path "$outDir\build\build.log" -ItemType File -Force
   $logDirectory = "$outDir\logs"
   $projectPath = $(Get-Location)
   $sceneList = ''
   if ("${{ parameters.Scene }}" -eq "HandInteractionExamples")
   {
       $sceneList += 'Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandInteractionExamples.unity'
   }
   elseif ("${{ parameters.Scene }}" -eq "SolverExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Solvers\Scenes\SolverExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "AudioLoFi")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Audio\Scenes\AudioLoFiEffectDemo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "AudioOcclusion")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Audio\Scenes\AudioOcclusionDemo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "LeapMotion")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\LeapMotionHandTrackingExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "SpatialAwareness")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\SpatialAwareness\Scenes\SpatialAwarenessMeshDemo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "GLTF")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Gltf\Scenes\Gltf-Loading-Demo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "GLB")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Gltf\Scenes\Glb-Loading-Demo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "BoundaryVisualization")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Boundary\Scenes\BoundaryVisualization.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "ClippingExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\ClippingExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "MaterialGallery")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\MaterialGallery.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "StandardMaterialComparison")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\StandardMaterialComparison.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "StandardMaterials")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\StandardMaterials.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "MixedRealityCapabilityDemo")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Utilities\Scenes\MixedRealityCapabilityDemo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "SurfaceMagnetismSA")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Solvers\Scenes\SurfaceMagnetismSpatialAwarenessExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "TapToPlace")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Solvers\Scenes\TapToPlaceExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "HIGestureEvents")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandInteractionGestureEventsExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "HIRecordAHPose")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandInteractionRecordArticulatedHandPose.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "HandMenuExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandMenuExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "NearMenuExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\NearMenuExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "DiagnosticsDemo")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Diagnostics\Scenes\DiagnosticsDemo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "InputActionsExample")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\InputActions\InputActionsExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "DisablePointersExample")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\DisablePointers\DisablePointersExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "Dictation")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\Dictation\Dictation.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "InputData")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\InputData\InputDataExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "PointerResult")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\PointerResult\PointerResultExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "PrimaryPointer")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\PrimaryPointer\PrimaryPointerExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "SpeechInput")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\Speech\SpeechInputExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXBoundsControlExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\BoundsControl\Scenes\BoundsControlExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXBoundsControlRuntime")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\BoundsControl\Scenes\BoundsControlRuntimeExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXColorPicker")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\ColorPicker\ColorPickerScene.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXDialog")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Dialog\DialogExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXDock")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Dock\DockExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXDwell")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Dwell\DwellScene.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXElastic")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Elastic\Scenes\ElasticDemo.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXExamplesHub")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\ExamplesHub\Scenes\MRTKExamplesHub.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXHandCoach")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\HandCoach\Scenes\HandCoachExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXHandMenuLayout")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\HandMenuLayout\Scenes\HandMenuLayoutExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXJoystick")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Joystick\Scenes\JoystickExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXMRKeyboard")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\MixedRealityKeyboard\Scenes\MixedRealityKeyboardExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXNNKeyboard")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\NonNativeKeyboard\Scenes\NonNativeKeyboardExample.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXPulseShader")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\PulseShader\Scenes\PulseShaderExamples.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXRiggedHandVisual")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\RiggedHand\Scenes\RiggedHandVisualizer.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXScrollObject")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\ScrollingObjectCollection\Scenes\ScrollingObjectCollection.unity"
   }
   elseif ("${{ parameters.Scene }}" -eq "EXSolvers")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Solvers\DirectionalIndicatorExample.unity"
   }

   $extraArgs = ''
   if ("${{ parameters.Platform }}" -eq "UWP")
   {
       $extraArgs += '-buildTarget WSAPlayer -buildAppx'
   }
   elseif ("${{ parameters.Platform }}" -eq "Standalone")
   {
       $extraArgs += "-buildTarget StandaloneWindows"
   }
   if ("${{ parameters.UnityArgs }}" -ne "none")
   {
       $extraArgs += " ${{ parameters.UnityArgs }}"
   }
   if ("${{ parameters.ScriptingBackend }}" -eq ".NET")
   {
       $extraArgs += " -scriptingBackend 2"
   }

   $proc = ''
   if ("${{ parameters.Scene }}" -eq "LeapMotion")
   {
       Write-Output 'Coming soon importing Leap Motion core Unity package. This just builds an audio example scene.'
       Start-Process -FilePath "$editor" -ArgumentList "-projectPath $projectPath -executeMethod Microsoft.MixedReality.Toolkit.Build.Editor.UnityPlayerBuildTools.StartCommandLineBuild -sceneList Assets\MRTK\Examples\Demos\Audio\Scenes\AudioLoFiEffectDemo.unity -logFile $($logFile.FullName) -batchMode -${{ parameters.Arch }} -buildOutput $outDir $extraArgs -CacheServerIPAddress $(Unity.CacheServer.Address) -logDirectory $logDirectory" -PassThru
   }
   else
   {
       Start-Process -FilePath "$editor" -ArgumentList "-projectPath $projectPath -executeMethod Microsoft.MixedReality.Toolkit.Build.Editor.UnityPlayerBuildTools.StartCommandLineBuild -sceneList $sceneList -logFile $($logFile.FullName) -batchMode -${{ parameters.Arch }} -buildOutput $outDir $extraArgs -CacheServerIPAddress $(Unity.CacheServer.Address) -logDirectory $logDirectory" -PassThru
   }

   $ljob = Start-Job -ScriptBlock { param($log) Get-Content "$log" -Wait } -ArgumentList $logFile.FullName
   
   while (-not $proc.HasExited -and $ljob.HasMoreData)
   {
       Receive-Job $ljob
       Start-Sleep -Milliseconds 200
   }
   Receive-Job $ljob
   
   Stop-Job $ljob
   
   Remove-Job $ljob
   Stop-Process $proc
   Write-Output '====================================================='
   Write-Output '           Unity Build Player Finished               '
   Write-Output '====================================================='
   if (Test-Path $logFile.FullName)
   {
      Write-Output '====================================================='
      Write-Output '           Begin Unity Player Log                    '
      Write-Output '====================================================='
      Get-Content $logFile.FullName
      Write-Output '====================================================='
      Write-Output '           End Unity Player Log                      '
      Write-Output '====================================================='
   }
   else
   {
      Write-Output 'Unity Player Log Missing!'
   }
   # The NuGet and AppX logs are only relevant for UWP builds.
   if ("${{ parameters.Platform }}" -eq "UWP")
   {
        $nugetRestoreLogFileName = "$logDirectory\nugetRestore.log"
        if (Test-Path $nugetRestoreLogFileName)
        {
            Write-Output '====================================================='
            Write-Output '           Begin NuGet Restore Log                   '
            Write-Output '====================================================='
            Get-Content $nugetRestoreLogFileName
            Write-Output '====================================================='
            Write-Output '           End NuGet Restore Log                     '
            Write-Output '====================================================='
        }
        else
        {
            Write-Output "NuGet Restore Log Missing $nugetRestoreLogFileName!"
        }
        $appxBuildLogFileName = "$logDirectory\buildAppx.log"
        if (Test-Path $appxBuildLogFileName)
        {
            Write-Output '====================================================='
            Write-Output '           Begin AppX Build Log                      '
            Write-Output '====================================================='
            Get-Content $appxBuildLogFileName
            Write-Output '====================================================='
            Write-Output '           End AppX Build Log                        '
            Write-Output '====================================================='
        }
        else
        {
            Write-Output "AppX Build Log Missing $appxBuildLogFileName!"
        }
    # if the Publish Project Parameter is set to yes, this if statement will stage the project files for publishing to build artifacts. 
    if ("${{ parameters.PublishProject }}" -eq "yes")
    {
        Write-Output '***** Preparing project files for publishing *****'
        Copy-Item -Path '$projectPath' -Destination "$(Build.ArtifactStagingDirectory)\build\${{ parameters.UnityVersion }}\${{ parameters.Platform }}_${{ parameters.Arch }}_${{ parameters.Scene }}\ProjectFiles\"
    }
    elseif ("${{ parameters.PublishProject }}" -eq "no")
    {
        Write-Output '***** Project files already published *****'
    }
   }
   if ($proc.ExitCode -ne 0)
   {
       exit $proc.ExitCode
   }
  displayName: "Build ${{ parameters.Scene }} ${{ parameters.Arch }} ${{ parameters.Scene }}"
