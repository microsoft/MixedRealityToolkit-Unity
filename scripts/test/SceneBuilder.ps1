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
   If ("${{ parameters.Scene }}" -eq "HandInteractionExamples")
   {
       $sceneList += 'Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandInteractionExamples.unity'
   }
   ElseIf ("${{ parameters.Scene }}" -eq "SolverExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Solvers\Scenes\SolverExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "AudioLoFi")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Audio\Scenes\AudioLoFiEffectDemo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "AudioOcclusion")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Audio\Scenes\AudioOcclusionDemo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "LeapMotion")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\LeapMotionHandTrackingExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "SpatialAwareness")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\SpatialAwareness\Scenes\SpatialAwarenessMeshDemo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "GLTF")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Gltf\Scenes\Gltf-Loading-Demo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "GLB")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Gltf\Scenes\Glb-Loading-Demo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "BoundaryVisualization")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Boundary\Scenes\BoundaryVisualization.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "ClippingExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\ClippingExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "MaterialGallery")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\MaterialGallery.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "StandardMaterialComparison")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\StandardMaterialComparison.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "StandardMaterials")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\StandardShader\Scenes\StandardMaterials.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "MixedRealityCapabilityDemo")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Utilities\Scenes\MixedRealityCapabilityDemo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "SurfaceMagnetismSA")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Solvers\Scenes\SurfaceMagnetismSpatialAwarenessExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "TapToPlace")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Solvers\Scenes\TapToPlaceExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "HIGestureEvents")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandInteractionGestureEventsExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "HIRecordAHPose")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandInteractionRecordArticulatedHandPose.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "HandMenuExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandMenuExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "NearMenuExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\HandTracking\Scenes\NearMenuExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "DiagnosticsDemo")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Diagnostics\Scenes\DiagnosticsDemo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "InputActionsExample")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\InputActions\InputActionsExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "DisablePointersExample")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\DisablePointers\DisablePointersExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "Dictation")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\Dictation\Dictation.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "InputData")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\InputData\InputDataExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "PointerResult")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\PointerResult\PointerResultExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "PrimaryPointer")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\PrimaryPointer\PrimaryPointerExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "SpeechInput")
   {
       $sceneList += "Assets\MRTK\Examples\Demos\Input\Scenes\Speech\SpeechInputExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXBoundsControlExamples")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\BoundsControl\Scenes\BoundsControlExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXBoundsControlRuntime")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\BoundsControl\Scenes\BoundsControlRuntimeExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXColorPicker")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\ColorPicker\ColorPickerScene.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXDialog")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Dialog\DialogExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXDock")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Dock\DockExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXDwell")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Dwell\DwellScene.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXElastic")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Elastic\Scenes\ElasticDemo.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXExamplesHub")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\ExamplesHub\Scenes\MRTKExamplesHub.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXHandCoach")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\HandCoach\Scenes\HandCoachExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXHandMenuLayout")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\HandMenuLayout\Scenes\HandMenuLayoutExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXJoystick")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Joystick\Scenes\JoystickExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXMRKeyboard")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\MixedRealityKeyboard\Scenes\MixedRealityKeyboardExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXNNKeyboard")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\NonNativeKeyboard\Scenes\NonNativeKeyboardExample.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXPulseShader")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\PulseShader\Scenes\PulseShaderExamples.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXRiggedHandVisual")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\RiggedHand\Scenes\RiggedHandVisualizer.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXScrollObject")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\ScrollingObjectCollection\Scenes\ScrollingObjectCollection.unity"
   }
   ElseIf ("${{ parameters.Scene }}" -eq "EXSolvers")
   {
       $sceneList += "Assets\MRTK\Examples\Experimental\Solvers\DirectionalIndicatorExample.unity"
   }

   $extraArgs = ''
   If ("${{ parameters.Platform }}" -eq "UWP")
   {
       $extraArgs += '-buildTarget WSAPlayer -buildAppx'
   }
   ElseIf ("${{ parameters.Platform }}" -eq "Standalone")
   {
       $extraArgs += "-buildTarget StandaloneWindows"
   }
   If ("${{ parameters.UnityArgs }}" -ne "none")
   {
       $extraArgs += " ${{ parameters.UnityArgs }}"
   }
   If ("${{ parameters.ScriptingBackend }}" -eq ".NET")
   {
       $extraArgs += " -scriptingBackend 2"
   }

   $proc = ''
   If ("${{ parameters.Scene }}" -eq "LeapMotion")
   {
       Write-Output 'Coming soon importing Leap Motion core Unity package. This just builds an audio example scene.'
       Start-Process -FilePath "$editor" -ArgumentList "-projectPath $projectPath -executeMethod Microsoft.MixedReality.Toolkit.Build.Editor.UnityPlayerBuildTools.StartCommandLineBuild -sceneList Assets\MRTK\Examples\Demos\Audio\Scenes\AudioLoFiEffectDemo.unity -logFile $($logFile.FullName) -batchMode -${{ parameters.Arch }} -buildOutput $outDir $extraArgs -CacheServerIPAddress $(Unity.CacheServer.Address) -logDirectory $logDirectory" -PassThru
   }
   Else
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
   If (Test-Path $logFile.FullName)
   {
      Write-Output '====================================================='
      Write-Output '           Begin Unity Player Log                    '
      Write-Output '====================================================='
      Get-Content $logFile.FullName
      Write-Output '====================================================='
      Write-Output '           End Unity Player Log                      '
      Write-Output '====================================================='
   }
   Else
   {
      Write-Output 'Unity Player Log Missing!'
   }
   # The NuGet and AppX logs are only relevant for UWP builds.
   If ("${{ parameters.Platform }}" -eq "UWP")
   {
        $nugetRestoreLogFileName = "$logDirectory\nugetRestore.log"
        If (Test-Path $nugetRestoreLogFileName)
        {
            Write-Output '====================================================='
            Write-Output '           Begin NuGet Restore Log                   '
            Write-Output '====================================================='
            Get-Content $nugetRestoreLogFileName
            Write-Output '====================================================='
            Write-Output '           End NuGet Restore Log                     '
            Write-Output '====================================================='
        }
        Else
        {
            Write-Output "NuGet Restore Log Missing $nugetRestoreLogFileName!"
        }
        $appxBuildLogFileName = "$logDirectory\buildAppx.log"
        If (Test-Path $appxBuildLogFileName)
        {
            Write-Output '====================================================='
            Write-Output '           Begin AppX Build Log                      '
            Write-Output '====================================================='
            Get-Content $appxBuildLogFileName
            Write-Output '====================================================='
            Write-Output '           End AppX Build Log                        '
            Write-Output '====================================================='
        }
        Else
        {
            Write-Output "AppX Build Log Missing $appxBuildLogFileName!"
        }
    # If the Publish Project Parameter is set to yes, this if statement will stage the project files for publishing to build artifacts. 
    If ("${{ parameters.PublishProject }}" -eq "yes")
    {
        Write-Output '***** Preparing project files for publishing *****'
        Copy-Item -Path '$projectPath' -Destination "$(Build.ArtifactStagingDirectory)\build\${{ parameters.UnityVersion }}\${{ parameters.Platform }}_${{ parameters.Arch }}_${{ parameters.Scene }}\ProjectFiles\"
    }
    ElseIf ("${{ parameters.PublishProject }}" -eq "no")
    {
        Write-Output '***** Project files already published *****'
    }
   }
   If ($proc.ExitCode -ne 0)
   {
       exit $proc.ExitCode
   }
  displayName: "Build ${{ parameters.Scene }} ${{ parameters.Arch }} ${{ parameters.Scene }}"
