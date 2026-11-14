using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Unity.Scripting.LifecycleManagement;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    /// <summary>
    /// Covers the session start flow that replaced the deprecated availability API
    /// (<c>ARSessionManager.CheckAvailability</c>, <c>SessionAvailability</c>, and <c>supportsInstall</c>).
    /// <see cref="ARSessionManager"/> now starts its subsystem with
    /// <see cref="XRSubsystem{TSubsystem, TSubsystemDescriptor, TProvider}.TryStartAsync"/> and
    /// <see cref="XRSubsystemStartOptions.InstallSoftwareIfNeeded"/>, and checks the result with
    /// <see cref="XRResultStatus.IsSuccess"/>.
    /// </summary>
    class ARSessionManagerTestFixture
    {
        const string k_DefaultSessionId = nameof(ARSessionManagerTestFixture) + ".DefaultSession";
        const string k_MockSessionId = nameof(ARSessionManagerTestFixture) + ".MockSession";

        // Matches the options ARSessionManager passes when it starts the session subsystem.
        const XRSubsystemStartOptions k_SessionManagerStartOptions = XRSubsystemStartOptions.InstallSoftwareIfNeeded;

        class MockProvider : XRSessionSubsystem.Provider { }

        /// <summary>
        /// Emulates a platform session subsystem that may be unsupported, or may require software
        /// to be installed before it can start.
        /// </summary>
        class MockSessionSubsystem : XRSessionSubsystem
        {
            public bool isSupported { get; set; }

            public bool isInstalled { get; set; }

            public bool canInstall { get; set; }

            public override Awaitable<XRResultStatus> TryStartAsync(
                CancellationToken token, XRSubsystemStartOptions options = XRSubsystemStartOptions.None)
            {
                var installRequested = (options & XRSubsystemStartOptions.InstallSoftwareIfNeeded) != 0;
                if (isSupported && !isInstalled && canInstall && installRequested)
                    isInstalled = true;

                if (!isSupported || !isInstalled)
                {
                    return AwaitableUtils<XRResultStatus>.FromResult(
                        new XRResultStatus(XRResultStatus.StatusCode.Unsupported));
                }

                Start();
                return AwaitableUtils<XRResultStatus>.FromResult(XRResultStatus.unqualifiedSuccess);
            }
        }

        [NoAutoStaticsCleanup]
        static readonly List<XRSessionSubsystemDescriptor> k_Descriptors = new();

        XRSessionSubsystem m_Subsystem;

        [OneTimeSetUp]
        public void RegisterTestDescriptors()
        {
            XRSessionSubsystemDescriptor.Register(new XRSessionSubsystemDescriptor.Cinfo
            {
                id = k_DefaultSessionId,
                providerType = typeof(MockProvider),
            });

            XRSessionSubsystemDescriptor.Register(new XRSessionSubsystemDescriptor.Cinfo
            {
                id = k_MockSessionId,
                providerType = typeof(MockProvider),
                subsystemTypeOverride = typeof(MockSessionSubsystem),
            });
        }

        [TearDown]
        public void DestroySubsystem()
        {
            m_Subsystem?.Destroy();
            m_Subsystem = null;
        }

        static XRSessionSubsystem CreateSubsystem(string id)
        {
            SubsystemManager.GetSubsystemDescriptors(k_Descriptors);
            foreach (var descriptor in k_Descriptors)
            {
                if (descriptor.id == id)
                    return descriptor.Create();
            }

            Assert.Fail($"No {nameof(XRSessionSubsystemDescriptor)} registered with id '{id}'.");
            return null;
        }

        MockSessionSubsystem CreateMockSubsystem(bool isSupported, bool isInstalled, bool canInstall)
        {
            var subsystem = (MockSessionSubsystem)CreateSubsystem(k_MockSessionId);
            subsystem.isSupported = isSupported;
            subsystem.isInstalled = isInstalled;
            subsystem.canInstall = canInstall;
            m_Subsystem = subsystem;
            return subsystem;
        }

        static XRResultStatus TryStartSync(XRSessionSubsystem subsystem, XRSubsystemStartOptions options)
        {
            var awaiter = subsystem.TryStartAsync(CancellationToken.None, options).GetAwaiter();
            Assert.IsTrue(awaiter.IsCompleted, "Expected TryStartAsync to complete synchronously.");
            return awaiter.GetResult();
        }

        [Test]
        public void DefaultTryStartAsyncStartsSessionAndReportsSuccess()
        {
            m_Subsystem = CreateSubsystem(k_DefaultSessionId);

            var result = TryStartSync(m_Subsystem, k_SessionManagerStartOptions);

            Assert.IsTrue(result.IsSuccess());
            Assert.IsTrue(m_Subsystem.running);
        }

        // Each case replaces one of the deprecated SessionAvailability / ARSessionState checks:
        // Supported | Installed                    -> Ready
        // None                                     -> Unsupported
        // Supported, install not possible          -> Unsupported
        // Supported, install possible              -> NeedsInstall (now installed by TryStartAsync)
        [TestCase(true, true, false, ExpectedResult = true, TestName = "SessionStartsWhenSupportedAndInstalled")]
        [TestCase(false, false, false, ExpectedResult = false, TestName = "SessionFailsToStartWhenUnsupported")]
        [TestCase(true, false, false, ExpectedResult = false, TestName = "SessionFailsToStartWhenNotInstalledAndInstallNotPossible")]
        [TestCase(true, false, true, ExpectedResult = true, TestName = "SessionInstallsAndStartsWhenNotInstalledAndInstallIsPossible")]
        public bool TryStartAsyncWithInstallSoftwareIfNeededReportsResult(bool isSupported, bool isInstalled, bool canInstall)
        {
            var subsystem = CreateMockSubsystem(isSupported, isInstalled, canInstall);

            var result = TryStartSync(subsystem, k_SessionManagerStartOptions);

            Assert.AreEqual(result.IsSuccess(), subsystem.running);
            return result.IsSuccess();
        }

        [Test]
        public void SessionDoesNotInstallWithoutInstallSoftwareIfNeeded()
        {
            var subsystem = CreateMockSubsystem(isSupported: true, isInstalled: false, canInstall: true);

            var result = TryStartSync(subsystem, XRSubsystemStartOptions.None);

            Assert.IsTrue(result.IsError());
            Assert.IsFalse(subsystem.isInstalled);
            Assert.IsFalse(subsystem.running);
        }
    }
}
