using System;
using System.Collections.Generic;
using NUnit.Framework;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARFoundation.Tests
{
#if UNITY_6000_5_OR_NEWER
    partial class ARSessionTestFixture
#else
    class ARSessionTestFixture
#endif
    {
        // This fixture intentionally exercises the deprecated availability/install flow
        // (CheckAvailability, GetAvailabilityAsync, SessionAvailability, supportsInstall, and the
        // NeedsInstall/Installing states) to keep regression coverage until those APIs are removed.
        // CS0618: use of obsolete members. CS0672: overriding an obsolete member (MockProvider).
#pragma warning disable CS0618, CS0672

        enum SupportsInstall
        {
            No,
            Yes
        }

        class MockProvider : XRSessionSubsystem.Provider
        {
#if UNITY_6000_5_OR_NEWER
            [NoAutoStaticsCleanup]
#endif
            public static SessionAvailability availability;

            public override Promise<SessionAvailability> GetAvailabilityAsync()
            {
                return Promise<SessionAvailability>.CreateResolvedPromise(availability);
            }
        }

        class MockSessionSubsystem : XRSessionSubsystem { }

        class SubsystemThatSupportsInstall : MockSessionSubsystem { }

        class SubsystemThatDoesNotSupportInstall : MockSessionSubsystem { }

        static string GetSubsystemName(SupportsInstall supportsInstall) => $"SessionThatSupportsInstall{supportsInstall.ToString()}";

#if UNITY_6000_5_OR_NEWER
        [AutoStaticsCleanup]
#endif
        static XRGeneralSettings s_OldInstance;

        static XRSessionSubsystemDescriptor.Cinfo GetDescriptorCinfo(SupportsInstall supportsInstall)
        {
            var canBeInstalled = supportsInstall == SupportsInstall.Yes;
            var type = canBeInstalled
                ? typeof(SubsystemThatSupportsInstall)
                : typeof(SubsystemThatDoesNotSupportInstall);

            return new XRSessionSubsystemDescriptor.Cinfo
            {
                id = GetSubsystemName(supportsInstall),
                providerType = typeof(MockProvider),
                subsystemTypeOverride = type,
                supportsInstall = canBeInstalled,
            };
        }

        [OneTimeSetUp]
        public void RegisterTestDescriptor()
        {
            XRSessionSubsystemDescriptor.Register(GetDescriptorCinfo(SupportsInstall.No));
            XRSessionSubsystemDescriptor.Register(GetDescriptorCinfo(SupportsInstall.Yes));
        }

        class MockLoader : XRLoaderHelper
        {
#if UNITY_6000_5_OR_NEWER
            [NoAutoStaticsCleanup]
#endif
            public static SupportsInstall supportsInstall;

#if UNITY_6000_5_OR_NEWER
            [NoAutoStaticsCleanup]
#endif
            static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new();

            XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();

            public override bool Initialize()
            {
                CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, GetSubsystemName(supportsInstall));
                return sessionSubsystem != null;
            }
        }

        static void CheckAvailabilitySync()
        {
            var enumerator = ARSession.CheckAvailability();
            while (enumerator.MoveNext()) { }
        }

        static void InitMock(SupportsInstall supportsInstall, SessionAvailability availability)
        {
            MockLoader.supportsInstall = supportsInstall;
            MockProvider.availability = availability;
            var xrManager = ScriptableObject.CreateInstance<XRManagerSettings>();
            xrManager.name = $"{nameof(ARSessionTestFixture)} XR Manager Settings";
            var mockLoader = ScriptableObject.CreateInstance<MockLoader>();
            mockLoader.name = $"{nameof(ARSessionTestFixture)} Mock Loader";
            xrManager.loaders.Add(mockLoader);
            xrManager.InitializeLoaderSync();
#if UNITY_EDITOR
            s_OldInstance = XRGeneralSettings.Instance;
            var xrGeneralSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
            XRGeneralSettings.Instance = xrGeneralSettings;
            xrGeneralSettings.name = $"{nameof(ARSessionTestFixture)} XR General Settings";
            XRGeneralSettings.Instance.Manager = xrManager;
#else
            throw new InvalidOperationException("This test is only valid on the Editor platform.");
#endif
        }

        static void DeinitMock()
        {
            var xrManager = XRGeneralSettings.Instance.Manager;
            xrManager.DeinitializeLoader();
            xrManager.loaders.Clear();
            ScriptableObject.Destroy(xrManager);
            ScriptableObject.Destroy(XRGeneralSettings.Instance);
#if UNITY_EDITOR
            XRGeneralSettings.Instance = s_OldInstance;
#else
            throw new InvalidOperationException("This test is only valid on the Editor platform.");
#endif
        }

        static void RunAvailabilityCheck(SupportsInstall supportsInstall, SessionAvailability availability)
        {
            ARSession.s_State = ARSessionState.None;
            InitMock(supportsInstall, availability);
            CheckAvailabilitySync();
            DeinitMock();
        }

        [Test]
        public void DoesNotThrowWhenNoSubsystemPresent()
        {
            CheckAvailabilitySync();
        }

        [Test]
        public void ReportsReady()
        {
            RunAvailabilityCheck(SupportsInstall.No, SessionAvailability.Supported | SessionAvailability.Installed);
            Assert.AreEqual(ARSessionState.Ready, ARSession.state);
        }

        [Test]
        public void ReportsUnsupported()
        {
            RunAvailabilityCheck(SupportsInstall.No, SessionAvailability.None);
            Assert.AreEqual(ARSessionState.Unsupported, ARSession.state);
        }

        [Test]
        public void ReportsUnsupportedWhenSupportedButNotInstalledAndInstallNotPossible()
        {
            RunAvailabilityCheck(SupportsInstall.No, SessionAvailability.Supported);
            Assert.AreEqual(ARSessionState.Unsupported, ARSession.state);
        }

        [Test]
        public void ReportsNeedsInstallWhenSupportedButNotInstalledAndInstallIsPossible()
        {
            RunAvailabilityCheck(SupportsInstall.Yes, SessionAvailability.Supported);
            Assert.AreEqual(ARSessionState.NeedsInstall, ARSession.state);
        }
    }
}
