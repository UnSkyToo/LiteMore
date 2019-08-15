using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace LiteFramework.Extend.Debug
{
    internal abstract class DebuggerInfo
    {
        internal class SystemItem : ScrollableDebuggerDrawItem
        {
            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>System Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Device Unique ID:", SystemInfo.deviceUniqueIdentifier);
                    DrawItem("Device Name:", SystemInfo.deviceName);
                    DrawItem("Device Type:", SystemInfo.deviceType.ToString());
                    DrawItem("Device Model:", SystemInfo.deviceModel);
                    DrawItem("Processor Type:", SystemInfo.processorType);
                    DrawItem("Processor Count:", SystemInfo.processorCount.ToString());
                    DrawItem("Processor Frequency:", $"{SystemInfo.processorFrequency} MHz");
                    DrawItem("System Memory Size:", $"{SystemInfo.systemMemorySize} MB");
#if UNITY_5_5_OR_NEWER
                    DrawItem("Operating System Family:", SystemInfo.operatingSystemFamily.ToString());
#endif
                    DrawItem("Operating System:", SystemInfo.operatingSystem);
#if UNITY_5_6_OR_NEWER
                    DrawItem("Battery Status:", SystemInfo.batteryStatus.ToString());
                    DrawItem("Battery Level:", $"{SystemInfo.batteryLevel:P1}");
#endif
#if UNITY_5_4_OR_NEWER
                    DrawItem("Supports Audio:", SystemInfo.supportsAudio.ToString());
#endif
                    DrawItem("Supports Location Service:", SystemInfo.supportsLocationService.ToString());
                    DrawItem("Supports Accelerometer:", SystemInfo.supportsAccelerometer.ToString());
                    DrawItem("Supports Gyroscope:", SystemInfo.supportsGyroscope.ToString());
                    DrawItem("Supports Vibration:", SystemInfo.supportsVibration.ToString());
                    DrawItem("Genuine:", Application.genuine.ToString());
                    DrawItem("Genuine Check Available:", Application.genuineCheckAvailable.ToString());
                }
                GUILayout.EndVertical();
            }
        }

        internal class EnvironmentItem : ScrollableDebuggerDrawItem
        {
            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>Environment Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Product Name:", Application.productName);
                    DrawItem("Company Name:", Application.companyName);
#if UNITY_5_6_OR_NEWER
                    DrawItem("Game Identifier:", Application.identifier);
#else
                DrawItem("Game Identifier:", Application.bundleIdentifier);
#endif
                    DrawItem("Lite Framework Version:", LiteConfigure.LiteFrameworkVersion);
                    DrawItem("Application Version:", Application.version);
                    DrawItem("Unity Version:", Application.unityVersion);
                    DrawItem("Platform:", Application.platform.ToString());
                    DrawItem("System Language:", Application.systemLanguage.ToString());
                    DrawItem("Cloud Project Id:", Application.cloudProjectId);
#if UNITY_5_6_OR_NEWER
                    DrawItem("Build Guid:", Application.buildGUID);
#endif
                    DrawItem("Target Frame Rate:", Application.targetFrameRate.ToString());
                    DrawItem("Internet Reachability:", Application.internetReachability.ToString());
                    DrawItem("Background Loading Priority:", Application.backgroundLoadingPriority.ToString());
                    DrawItem("Is Playing:", Application.isPlaying.ToString());
#if UNITY_5_5_OR_NEWER
                    DrawItem("Splash Screen Is Finished:", SplashScreen.isFinished.ToString());
#else
                DrawItem("Is Showing Splash Screen:", Application.isShowingSplashScreen.ToString());
#endif
                    DrawItem("Run In Background:", Application.runInBackground.ToString());
#if UNITY_5_5_OR_NEWER
                    DrawItem("Install Name:", Application.installerName);
#endif
                    DrawItem("Install Mode:", Application.installMode.ToString());
                    DrawItem("Sandbox Type:", Application.sandboxType.ToString());
                    DrawItem("Is Mobile Platform:", Application.isMobilePlatform.ToString());
                    DrawItem("Is Console Platform:", Application.isConsolePlatform.ToString());
                    DrawItem("Is Editor:", Application.isEditor.ToString());
#if UNITY_5_6_OR_NEWER
                    DrawItem("Is Focused:", Application.isFocused.ToString());
#endif
#if UNITY_2018_2_OR_NEWER
                    DrawItem("Is Batch Mode:", Application.isBatchMode.ToString());
#endif
#if UNITY_5_3
                DrawItem("Stack Trace Log Type:", Application.stackTraceLogType.ToString());
#endif
                }
                GUILayout.EndVertical();
            }
        }

        internal class ScreenItem : ScrollableDebuggerDrawItem
        {
            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>Screen Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Current Resolution", GetResolutionString(Screen.currentResolution));
                    DrawItem("Screen Size", $"{Screen.width} x {Screen.height}");
                    DrawItem("Screen DPI", Screen.dpi.ToString("F2"));
                    DrawItem("Screen Orientation", Screen.orientation.ToString());
                    DrawItem("Is Full Screen", Screen.fullScreen.ToString());
#if UNITY_2018_1_OR_NEWER
                    DrawItem("Full Screen Mode", Screen.fullScreenMode.ToString());
#endif
                    DrawItem("Sleep Timeout", GetSleepTimeoutDescription(Screen.sleepTimeout));
                    DrawItem("Cursor Visible", Cursor.visible.ToString());
                    DrawItem("Cursor Lock State", Cursor.lockState.ToString());
                    DrawItem("Auto Landscape Left", Screen.autorotateToLandscapeLeft.ToString());
                    DrawItem("Auto Landscape Right", Screen.autorotateToLandscapeRight.ToString());
                    DrawItem("Auto Portrait", Screen.autorotateToPortrait.ToString());
                    DrawItem("Auto Portrait Upside Down", Screen.autorotateToPortraitUpsideDown.ToString());
#if UNITY_2017_2_OR_NEWER && !UNITY_2017_2_0
                    DrawItem("Safe Area", Screen.safeArea.ToString());
#endif
                    DrawItem("Support Resolutions", GetResolutionsString(Screen.resolutions));
                }
                GUILayout.EndVertical();
            }

            private static string GetSleepTimeoutDescription(int Timeout)
            {
                if (Timeout == SleepTimeout.NeverSleep)
                {
                    return "Never Sleep";
                }

                if (Timeout == SleepTimeout.SystemSetting)
                {
                    return "System Setting";
                }

                return Timeout.ToString();
            }

            private static string GetResolutionString(Resolution Re)
            {
                return $"{Re.width} x {Re.height} @ {Re.refreshRate}Hz";
            }

            private static string GetResolutionsString(Resolution[] Res)
            {
                var Results = new string[Res.Length];
                for (var Index = 0; Index < Res.Length; ++Index)
                {
                    Results[Index] = GetResolutionString(Res[Index]);
                }

                return string.Join("; ", Results);
            }
        }

        internal class GraphicsItem : ScrollableDebuggerDrawItem
        {
            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>Graphics Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Device ID:", SystemInfo.graphicsDeviceID.ToString());
                    DrawItem("Device Name:", SystemInfo.graphicsDeviceName);
                    DrawItem("Device Vendor ID:", SystemInfo.graphicsDeviceVendorID.ToString());
                    DrawItem("Device Vendor:", SystemInfo.graphicsDeviceVendor);
                    DrawItem("Device Type:", SystemInfo.graphicsDeviceType.ToString());
                    DrawItem("Device Version:", SystemInfo.graphicsDeviceVersion);
                    DrawItem("Memory Size:", $"{SystemInfo.graphicsMemorySize} MB");
                    DrawItem("Multi Threaded:", SystemInfo.graphicsMultiThreaded.ToString());
                    DrawItem("Shader Level:", GetShaderLevelString(SystemInfo.graphicsShaderLevel));
                    DrawItem("Global Maximum LOD:", Shader.globalMaximumLOD.ToString());
#if UNITY_5_5_OR_NEWER
                    DrawItem("Active Tier", Graphics.activeTier.ToString());
#endif
#if UNITY_2017_2_OR_NEWER
                    DrawItem("Active Color Gamut", Graphics.activeColorGamut.ToString());
#endif
                    DrawItem("NPOT Support:", SystemInfo.npotSupport.ToString());
                    DrawItem("Max Texture Size:", SystemInfo.maxTextureSize.ToString());
                    DrawItem("Supported Render Target Count:", SystemInfo.supportedRenderTargetCount.ToString());
#if UNITY_5_4_OR_NEWER
                    DrawItem("Copy Texture Support:", SystemInfo.copyTextureSupport.ToString());
#endif
#if UNITY_5_5_OR_NEWER
                    DrawItem("Uses Reversed ZBuffer:", SystemInfo.usesReversedZBuffer.ToString());
#endif
#if UNITY_5_6_OR_NEWER
                    DrawItem("Max Cubemap Size:", SystemInfo.maxCubemapSize.ToString());
                    DrawItem("Graphics UV Starts At Top:", SystemInfo.graphicsUVStartsAtTop.ToString());
#endif
#if UNITY_2019_1_OR_NEWER
                    DrawItem("Min Constant Buffer Offset Alignment:", SystemInfo.minConstantBufferOffsetAlignment.ToString());
#endif
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Has Hidden Surface Removal On GPU:", SystemInfo.hasHiddenSurfaceRemovalOnGPU.ToString());
                    DrawItem("Has Dynamic Uniform Array Indexing In Fragment Shaders:",
                        SystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders.ToString());
#endif
#if UNITY_5_3 || UNITY_5_4
                    DrawItem("Supports Stencil:", SystemInfo.supportsStencil.ToString());
                    DrawItem("Supports Render Textures:", SystemInfo.supportsRenderTextures.ToString());
#endif
                    DrawItem("Supports Sparse Textures:", SystemInfo.supportsSparseTextures.ToString());
                    DrawItem("Supports 3D Textures:", SystemInfo.supports3DTextures.ToString());
                    DrawItem("Supports Shadows:", SystemInfo.supportsShadows.ToString());
                    DrawItem("Supports Raw Shadow Depth Sampling:",
                        SystemInfo.supportsRawShadowDepthSampling.ToString());
#if !UNITY_2019_1_OR_NEWER
                    DrawItem("Supports Render To Cubemap:", SystemInfo.supportsRenderToCubemap.ToString());
#endif
                    DrawItem("Supports Compute Shader:", SystemInfo.supportsComputeShaders.ToString());
                    DrawItem("Supports Instancing:", SystemInfo.supportsInstancing.ToString());
#if !UNITY_2019_1_OR_NEWER
                    DrawItem("Supports Image Effects:", SystemInfo.supportsImageEffects.ToString());
#endif
#if UNITY_5_4_OR_NEWER
                    DrawItem("Supports 2D Array Textures:", SystemInfo.supports2DArrayTextures.ToString());
                    DrawItem("Supports Motion Vectors:", SystemInfo.supportsMotionVectors.ToString());
#endif
#if UNITY_5_5_OR_NEWER
                    DrawItem("Supports Cubemap Array Textures:", SystemInfo.supportsCubemapArrayTextures.ToString());
#endif
#if UNITY_5_6_OR_NEWER
                    DrawItem("Supports 3D Render Textures:", SystemInfo.supports3DRenderTextures.ToString());
#endif
#if UNITY_2017_2_OR_NEWER && !UNITY_2017_2_0 || UNITY_2017_1_4
                    DrawItem("Supports Texture Wrap Mirror Once", SystemInfo.supportsTextureWrapMirrorOnce.ToString());
#endif
#if UNITY_2019_1_OR_NEWER
                DrawItem("Supports Graphics Fence", SystemInfo.supportsGraphicsFence.ToString());
#elif UNITY_2017_3_OR_NEWER
                    DrawItem("Supports GPU Fence", SystemInfo.supportsGPUFence.ToString());
#endif
#if UNITY_2017_3_OR_NEWER
                    DrawItem("Supports Async Compute", SystemInfo.supportsAsyncCompute.ToString());
                    DrawItem("Supports Multisampled Textures", SystemInfo.supportsMultisampledTextures.ToString());
#endif
#if UNITY_2018_1_OR_NEWER
                    DrawItem("Supports Async GPU Readback", SystemInfo.supportsAsyncGPUReadback.ToString());
                    DrawItem("Supports 32bits Index Buffer", SystemInfo.supports32bitsIndexBuffer.ToString());
                    DrawItem("Supports Hardware Quad Topology", SystemInfo.supportsHardwareQuadTopology.ToString());
#endif
#if UNITY_2018_2_OR_NEWER
                    DrawItem("Supports Mip Streaming", SystemInfo.supportsMipStreaming.ToString());
                    DrawItem("Supports Multisample Auto Resolve", SystemInfo.supportsMultisampleAutoResolve.ToString());
#endif
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Supports Separated Render Targets Blend:",
                        SystemInfo.supportsSeparatedRenderTargetsBlend.ToString());
#endif
#if UNITY_2019_1_OR_NEWER
                DrawItem("Supports Set Constant Buffer:", SystemInfo.supportsSetConstantBuffer.ToString());
#endif
                }
                GUILayout.EndVertical();
            }

            private static string GetShaderLevelString(int ShaderLevel)
            {
                return $"Shader Model {ShaderLevel / 10}.{ShaderLevel % 10}";
            }
        }

        internal class InputItem : ScrollableDebuggerDrawItem
        {
            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>Input Acceleration Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Acceleration:", Input.acceleration.ToString());
                    DrawItem("Acceleration Event Count:", Input.accelerationEventCount.ToString());
                    DrawItem("Acceleration Events:", GetAccelerationEventsString(Input.accelerationEvents));
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Input Compass Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                        {
                            Input.compass.enabled = true;
                        }

                        if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                        {
                            Input.compass.enabled = false;
                        }
                    }
                    GUILayout.EndHorizontal();

                    DrawItem("Enabled:", Input.compass.enabled.ToString());
                    DrawItem("Heading Accuracy:", Input.compass.headingAccuracy.ToString());
                    DrawItem("Magnetic Heading:", Input.compass.magneticHeading.ToString());
                    DrawItem("Raw Vector:", Input.compass.rawVector.ToString());
                    DrawItem("Timestamp:", Input.compass.timestamp.ToString());
                    DrawItem("True Heading:", Input.compass.trueHeading.ToString());
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Input Gyroscope Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                        {
                            Input.gyro.enabled = true;
                        }

                        if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                        {
                            Input.gyro.enabled = false;
                        }
                    }
                    GUILayout.EndHorizontal();

                    DrawItem("Enabled:", Input.gyro.enabled.ToString());
                    DrawItem("Update Interval:", Input.gyro.updateInterval.ToString());
                    DrawItem("Attitude:", Input.gyro.attitude.eulerAngles.ToString());
                    DrawItem("Gravity:", Input.gyro.gravity.ToString());
                    DrawItem("Rotation Rate:", Input.gyro.rotationRate.ToString());
                    DrawItem("Rotation Rate Unbiased:", Input.gyro.rotationRateUnbiased.ToString());
                    DrawItem("User Acceleration:", Input.gyro.userAcceleration.ToString());
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Input Location Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Enable", GUILayout.Height(30f)))
                        {
                            Input.location.Start();
                        }

                        if (GUILayout.Button("Disable", GUILayout.Height(30f)))
                        {
                            Input.location.Stop();
                        }
                    }
                    GUILayout.EndHorizontal();

                    DrawItem("Is Enabled By User:", Input.location.isEnabledByUser.ToString());
                    DrawItem("Status:", Input.location.status.ToString());
                    DrawItem("Horizontal Accuracy:", Input.location.lastData.horizontalAccuracy.ToString());
                    DrawItem("Vertical Accuracy:", Input.location.lastData.verticalAccuracy.ToString());
                    DrawItem("Longitude:", Input.location.lastData.longitude.ToString());
                    DrawItem("Latitude:", Input.location.lastData.latitude.ToString());
                    DrawItem("Altitude:", Input.location.lastData.altitude.ToString());
                    DrawItem("Timestamp:", Input.location.lastData.timestamp.ToString());
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Input Summary Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Back Button Leaves App:", Input.backButtonLeavesApp.ToString());
                    DrawItem("Device Orientation:", Input.deviceOrientation.ToString());
                    DrawItem("Mouse Present:", Input.mousePresent.ToString());
                    DrawItem("Mouse Position:", Input.mousePosition.ToString());
                    DrawItem("Mouse Scroll Delta:", Input.mouseScrollDelta.ToString());
                    DrawItem("Any Key:", Input.anyKey.ToString());
                    DrawItem("Any Key Down:", Input.anyKeyDown.ToString());
                    DrawItem("Input String:", Input.inputString);
                    DrawItem("IME Is Selected:", Input.imeIsSelected.ToString());
                    DrawItem("IME Composition Mode:", Input.imeCompositionMode.ToString());
                    DrawItem("Compensate Sensors:", Input.compensateSensors.ToString());
                    DrawItem("Composition Cursor Position:", Input.compositionCursorPos.ToString());
                    DrawItem("Composition String:", Input.compositionString);
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Input Touch Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Touch Supported:", Input.touchSupported.ToString());
                    DrawItem("Touch Pressure Supported:", Input.touchPressureSupported.ToString());
                    DrawItem("Stylus Touch Supported:", Input.stylusTouchSupported.ToString());
                    DrawItem("Simulate Mouse With Touches:", Input.simulateMouseWithTouches.ToString());
                    DrawItem("Multi Touch Enabled:", Input.multiTouchEnabled.ToString());
                    DrawItem("Touch Count:", Input.touchCount.ToString());
                    DrawItem("Touches:", GetTouchesString(Input.touches));
                }
                GUILayout.EndVertical();
            }

            private static string GetAccelerationEventString(AccelerationEvent Evt)
            {
                return $"{Evt.acceleration}, {Evt.deltaTime}";
            }

            private static string GetAccelerationEventsString(AccelerationEvent[] Evts)
            {
                var Results = new string[Evts.Length];
                for (var Index = 0; Index < Evts.Length; ++Index)
                {
                    Results[Index] = GetAccelerationEventString(Evts[Index]);
                }

                return string.Join("; ", Results);
            }

            private static string GetTouchString(Touch T)
            {
                return $"{T.position}, {T.deltaPosition}, {T.rawPosition}, {T.pressure}, {T.phase}";
            }

            private static string GetTouchesString(Touch[] TS)
            {
                var Results = new string[TS.Length];
                for (var Index = 0; Index < TS.Length; ++Index)
                {
                    Results[Index] = GetTouchString(TS[Index]);
                }

                return string.Join("; ", Results);
            }
        }

        internal class OtherItem : ScrollableDebuggerDrawItem
        {
            private static bool ApplyExpensiveChanges_ = false;

            protected override void OnDrawScrollable()
            {
                GUILayout.Label("<b>Scene Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Scene Count:", SceneManager.sceneCount.ToString());
                    DrawItem("Scene Count In Build Settings:", SceneManager.sceneCountInBuildSettings.ToString());

                    Scene activeScene = SceneManager.GetActiveScene();
                    DrawItem("Active Scene Name:", activeScene.name);
                    DrawItem("Active Scene Path:", activeScene.path);
                    DrawItem("Active Scene Build Index:", activeScene.buildIndex.ToString());
                    DrawItem("Active Scene Is Dirty:", activeScene.isDirty.ToString());
                    DrawItem("Active Scene Is Loaded:", activeScene.isLoaded.ToString());
                    DrawItem("Active Scene Is Valid:", activeScene.IsValid().ToString());
                    DrawItem("Active Scene Root Count:", activeScene.rootCount.ToString());
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Path Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Data Path:", Application.dataPath);
                    DrawItem("Persistent Data Path:", Application.persistentDataPath);
                    DrawItem("Streaming Assets Path:", Application.streamingAssetsPath);
                    DrawItem("Temporary Cache Path:", Application.temporaryCachePath);
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Console Log Path:", Application.consoleLogPath);
#endif
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Time Information</b>");
                GUILayout.BeginVertical("box");
                {
                    DrawItem("Time Scale",
                        $"{LiteManager.TimeScale} [{GetTimeScaleDescription(LiteManager.TimeScale)}]");
                    DrawItem("Realtime Since Startup", Time.realtimeSinceStartup.ToString());
                    DrawItem("Time Since Level Load", Time.timeSinceLevelLoad.ToString());
                    DrawItem("Time", Time.time.ToString());
                    DrawItem("Fixed Time", Time.fixedTime.ToString());
                    DrawItem("Unscaled Time", Time.unscaledTime.ToString());
#if UNITY_5_6_OR_NEWER
                    DrawItem("Fixed Unscaled Time", Time.fixedUnscaledTime.ToString());
#endif
                    DrawItem("Delta Time", Time.deltaTime.ToString());
                    DrawItem("Fixed Delta Time", Time.fixedDeltaTime.ToString());
                    DrawItem("Unscaled Delta Time", Time.unscaledDeltaTime.ToString());
#if UNITY_5_6_OR_NEWER
                    DrawItem("Fixed Unscaled Delta Time", Time.fixedUnscaledDeltaTime.ToString());
#endif
                    DrawItem("Smooth Delta Time", Time.smoothDeltaTime.ToString());
                    DrawItem("Maximum Delta Time", Time.maximumDeltaTime.ToString());
#if UNITY_5_5_OR_NEWER
                    DrawItem("Maximum Particle Delta Time", Time.maximumParticleDeltaTime.ToString());
#endif
                    DrawItem("Frame Count", Time.frameCount.ToString());
                    DrawItem("Rendered Frame Count", Time.renderedFrameCount.ToString());
                    DrawItem("Capture Framerate", Time.captureFramerate.ToString());
#if UNITY_5_6_OR_NEWER
                    DrawItem("In Fixed Time Step", Time.inFixedTimeStep.ToString());
#endif
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Quality Level</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    int currentQualityLevel = QualitySettings.GetQualityLevel();

                    DrawItem("Current Quality Level:", QualitySettings.names[currentQualityLevel]);
                    ApplyExpensiveChanges_ = GUILayout.Toggle(ApplyExpensiveChanges_,
                        "Apply expensive changes on quality level change.");

                    int newQualityLevel =
                        GUILayout.SelectionGrid(currentQualityLevel, QualitySettings.names, 3, "toggle");
                    if (newQualityLevel != currentQualityLevel)
                    {
                        QualitySettings.SetQualityLevel(newQualityLevel, ApplyExpensiveChanges_);
                    }
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Rendering Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawItem("Active Color Space:", QualitySettings.activeColorSpace.ToString());
                    DrawItem("Desired Color Space:", QualitySettings.desiredColorSpace.ToString());
                    DrawItem("Max Queued Frames:", QualitySettings.maxQueuedFrames.ToString());
                    DrawItem("Pixel Light Count:", QualitySettings.pixelLightCount.ToString());
                    DrawItem("Master Texture Limit:", QualitySettings.masterTextureLimit.ToString());
                    DrawItem("Anisotropic Filtering:", QualitySettings.anisotropicFiltering.ToString());
                    DrawItem("Anti Aliasing:", QualitySettings.antiAliasing.ToString());
#if UNITY_5_5_OR_NEWER
                    DrawItem("Soft Particles:", QualitySettings.softParticles.ToString());
#endif
                    DrawItem("Soft Vegetation:", QualitySettings.softVegetation.ToString());
                    DrawItem("Realtime Reflection Probes:", QualitySettings.realtimeReflectionProbes.ToString());
                    DrawItem("Billboards Face Camera Position:",
                        QualitySettings.billboardsFaceCameraPosition.ToString());
#if UNITY_2017_1_OR_NEWER
                    DrawItem("Resolution Scaling Fixed DPI Factor:",
                        QualitySettings.resolutionScalingFixedDPIFactor.ToString());
#endif
#if UNITY_2018_2_OR_NEWER
                    DrawItem("Texture Streaming Enabled", QualitySettings.streamingMipmapsActive.ToString());
                    DrawItem("Texture Streaming Add All Cameras",
                        QualitySettings.streamingMipmapsAddAllCameras.ToString());
                    DrawItem("Texture Streaming Memory Budget",
                        QualitySettings.streamingMipmapsMemoryBudget.ToString());
                    DrawItem("Texture Streaming Renderers Per Frame",
                        QualitySettings.streamingMipmapsRenderersPerFrame.ToString());
                    DrawItem("Texture Streaming Max Level Reduction",
                        QualitySettings.streamingMipmapsMaxLevelReduction.ToString());
                    DrawItem("Texture Streaming Max File IO Requests",
                        QualitySettings.streamingMipmapsMaxFileIORequests.ToString());
#endif
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Shadows Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
#if UNITY_2017_1_OR_NEWER
                    DrawItem("Shadowmask Mode:", QualitySettings.shadowmaskMode.ToString());
#endif
#if UNITY_5_5_OR_NEWER
                    DrawItem("Shadow Quality:", QualitySettings.shadows.ToString());
#endif
#if UNITY_5_4_OR_NEWER
                    DrawItem("Shadow Resolution:", QualitySettings.shadowResolution.ToString());
#endif
                    DrawItem("Shadow Projection:", QualitySettings.shadowProjection.ToString());
                    DrawItem("Shadow Distance:", QualitySettings.shadowDistance.ToString());
                    DrawItem("Shadow Near Plane Offset:", QualitySettings.shadowNearPlaneOffset.ToString());
                    DrawItem("Shadow Cascades:", QualitySettings.shadowCascades.ToString());
                    DrawItem("Shadow Cascade 2 Split:", QualitySettings.shadowCascade2Split.ToString());
                    DrawItem("Shadow Cascade 4 Split:", QualitySettings.shadowCascade4Split.ToString());
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Other Information</b>");
                GUILayout.BeginVertical("box");
                {
#if UNITY_2019_1_OR_NEWER
                    DrawItem("Skin Weights:", QualitySettings.skinWeights.ToString());
#else
                    DrawItem("Blend Weights:", QualitySettings.blendWeights.ToString());
#endif
                    DrawItem("VSync Count:", QualitySettings.vSyncCount.ToString());
                    DrawItem("LOD Bias:", QualitySettings.lodBias.ToString());
                    DrawItem("Maximum LOD Level:", QualitySettings.maximumLODLevel.ToString());
                    DrawItem("Particle Raycast Budget:", QualitySettings.particleRaycastBudget.ToString());
                    DrawItem("Async Upload Time Slice:", $"{QualitySettings.asyncUploadTimeSlice} ms");
                    DrawItem("Async Upload Buffer Size:", $"{QualitySettings.asyncUploadBufferSize} MB");
#if UNITY_2018_3_OR_NEWER
                    DrawItem("Async Upload Persistent Buffer:", QualitySettings.asyncUploadPersistentBuffer.ToString());
#endif
                }
                GUILayout.EndVertical();

                GUILayout.Label("<b>Web Player Information</b>");
                GUILayout.BeginVertical(GUI.skin.box);
                {
#if !UNITY_2017_2_OR_NEWER
                DrawItem("Is Web Player:", Application.isWebPlayer.ToString());
#endif
                    DrawItem("Absolute URL:", Application.absoluteURL);
#if !UNITY_2017_2_OR_NEWER
                DrawItem("Source Value:", Application.srcValue);
#endif
#if !UNITY_2018_2_OR_NEWER
                DrawItem("Streamed Bytes:", Application.streamedBytes.ToString());
#endif
#if UNITY_5_3 || UNITY_5_4
                DrawItem("Web Security Enabled:", Application.webSecurityEnabled.ToString());
                DrawItem("Web Security Host URL:", Application.webSecurityHostUrl.ToString());
#endif
                }
                GUILayout.EndVertical();
            }

            private static string GetTimeScaleDescription(float TimeScale)
            {
                if (TimeScale <= 0f)
                {
                    return "Pause";
                }

                if (TimeScale < 1f)
                {
                    return "Slower";
                }

                if (TimeScale > 1f)
                {
                    return "Faster";
                }

                return "Normal";
            }
        }
    }
}