using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A manager for <see cref="ARTrackedImage"/>s. Uses the <c>XRImageTrackingSubsystem</c>
    /// to recognize and track 2D images in the physical environment.
    /// </summary>
    /// <remarks>
    /// Related information: <a href="xref:arfoundation-image-tracking">AR Tracked Image Manager component</a>
    /// </remarks>
    [DefaultExecutionOrder(ARUpdateOrder.k_TrackedImageManager)]
    [RequireComponent(typeof(XROrigin))]
    [AddComponentMenu("XR/AR Foundation/AR Tracked Image Manager")]
    [HelpUrl("features/image-tracking")]
    [Icon(Constants.arvrIconPath)]
    public sealed class ARTrackedImageManager : ARTrackableManager<
        XRImageTrackingSubsystem,
        XRImageTrackingSubsystemDescriptor,
        XRImageTrackingSubsystem.Provider,
        XRTrackedImage,
        ARTrackedImage>
    {
        Dictionary<Guid, XRReferenceImage> m_ReferenceImages = new();

        [SerializeField]
        [FormerlySerializedAs("m_ReferenceLibrary")]
        [Tooltip("The library of images which will be detected and/or tracked in the physical environment.")]
        XRReferenceImageLibrary m_SerializedLibrary;

        /// <summary>
        /// Get or set the reference image library (that is, the set of images to search for in the physical environment).
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the <see cref="referenceLibrary"/> is set to <c>null</c> while image tracking is enabled.</exception>
        public IReferenceImageLibrary referenceLibrary
        {
            get
            {
                if (subsystem != null)
                    return subsystem.imageLibrary;

                return m_SerializedLibrary;
            }
            [Obsolete("The referenceLibrary setter is deprecated in AR Foundation 6.7. Use TrySetReferenceLibraryAsync or TrySetReferenceLibrary instead.")]
            set
            {
                switch (value)
                {
                    case null when subsystem != null && subsystem.running:
                        throw new InvalidOperationException(
                            "Can't set a null reference library while image tracking is enabled.");
                    case XRReferenceImageLibrary serializedLibrary:
                    {
                        m_SerializedLibrary = serializedLibrary;
                        if (subsystem != null)
                            subsystem.imageLibrary = subsystem.CreateRuntimeLibrary(serializedLibrary);
                        break;
                    }
                    case RuntimeReferenceImageLibrary runtimeLibrary:
                    {
                        m_SerializedLibrary = null;
                        EnsureSubsystemInstanceSet();

                        if (subsystem != null)
                            subsystem.imageLibrary = runtimeLibrary;
                        break;
                    }
                }

                if (subsystem != null)
                    UpdateReferenceImages(subsystem.imageLibrary);
            }
        }

        /// <summary>
        /// Attempts to create a runtime reference image library from <paramref name="serializedLibrary"/>, activate it,
        /// and assign it to this manager.
        /// </summary>
        /// <remarks>
        /// After this operation completes successfully, <see cref="referenceLibrary"/> returns the newly created
        /// runtime library.
        ///
        /// Only one asynchronous operation from this method may be in progress at a time.
        /// Use the `cancellationToken` to cancel a previous call to this method if needed,
        /// otherwise subsequent calls will fail while an operation is in progress.
        /// </remarks>
        /// <param name="serializedLibrary">The serialized library to convert to a runtime library and assign, or
        /// <c>null</c> to assign an empty library.</param>
        /// <param name="cancellationToken">A cancellation token, which you can use to cancel the operation in progress.</param>
        /// <returns>The result of the async operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="serializedLibrary"/> is <c>null</c>.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled, either by
        /// <paramref name="cancellationToken"/> or because this component was destroyed.</exception>
        public async Awaitable<Result<RuntimeReferenceImageLibrary>> TrySetReferenceLibraryAsync(
            XRReferenceImageLibrary serializedLibrary, CancellationToken cancellationToken = default)
        {
            EnsureSubsystemInstanceSet();
            if (subsystem == null)
            {
                return new(new XRResultStatus(XRResultStatus.StatusCode.ProviderUninitialized), null);
            }

            CancellationTokenSource cts = null;
            CancellationToken token;
            if (cancellationToken.CanBeCanceled)
            {
                cts =
                    CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, cancellationToken);
                token = cts.Token;
            }
            else
            {
                token = destroyCancellationToken;
            }

            try
            {
                var result = await subsystem.TryCreateRuntimeLibraryAsync(serializedLibrary, token);

                if (!result.status.IsSuccess())
                {
                    return result;
                }

                m_SerializedLibrary = serializedLibrary;
                subsystem.imageLibrary = result.value;
                UpdateReferenceImages(subsystem.imageLibrary);
                return result;
            }
            finally
            {
                cts?.Dispose();
            }
        }

        /// <summary>
        /// Attempts to assign <paramref name="runtimeLibrary"/> to this manager.
        /// </summary>
        /// <example>
        /// <code>
        ///     var status = manager.TrySetReferenceLibrary(runtimeLibrary);
        ///     if (status.IsSuccess())
        ///         DoSomething();
        /// </code>
        /// </example>
        /// <param name="runtimeLibrary">The runtime library to assign.</param>
        /// <returns>The status of the operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="runtimeLibrary"/> is <c>null</c>.</exception>
        public XRResultStatus TrySetReferenceLibrary(RuntimeReferenceImageLibrary runtimeLibrary)
        {
            if (runtimeLibrary == null)
            {
                return new XRResultStatus(XRResultStatus.StatusCode.ValidationFailure);
            }

            EnsureSubsystemInstanceSet();
            if (subsystem == null)
            {
                return new XRResultStatus(XRResultStatus.StatusCode.ProviderUninitialized);
            }

            m_SerializedLibrary = null;
            subsystem.imageLibrary = runtimeLibrary;
            UpdateReferenceImages(subsystem.imageLibrary);

            return XRResultStatus.unqualifiedSuccess;
        }

        /// <summary>
        /// Creates a <c>UnityEngine.XR.ARSubsystems.RuntimeReferenceImageLibrary</c> from an existing
        /// <c>UnityEngine.XR.ARSubsystems.XRReferenceImageLibrary</c>
        /// or an empty library if <paramref name="serializedLibrary"/> is <c>null</c>.
        /// Use this to construct reference image libraries at runtime. If the library is of type
        /// <c>MutableRuntimeReferenceImageLibrary</c>, it is modifiable at runtime.
        /// </summary>
        /// <param name="serializedLibrary">An existing <c>XRReferenceImageLibrary</c>, or <c>null</c> to create an empty image library.</param>
        /// <returns>A new <c>RuntimeReferenceImageLibrary</c> representing the deserialized version of <paramref name="serializedLibrary"/> or an empty library if <paramref name="serializedLibrary"/> is <c>null</c>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if there is no subsystem. This usually means image tracking is not supported.</exception>
        [Obsolete("CreateRuntimeLibrary is deprecated in AR Foundation 6.7. Use TryCreateRuntimeLibraryAsync instead.")]
        public RuntimeReferenceImageLibrary CreateRuntimeLibrary(XRReferenceImageLibrary serializedLibrary = null)
        {
            EnsureSubsystemInstanceSet();

            if (subsystem == null)
                throw new NotSupportedException("No image tracking subsystem found. This usually means image tracking is not supported.");

            return subsystem.CreateRuntimeLibrary(serializedLibrary);
        }

        /// <summary>
        /// Attempts to create a <c>RuntimeReferenceImageLibrary</c> from an existing <c>XRReferenceImageLibrary</c>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="referenceImageLibrary"/> is <c>null</c>, the new library is empty. You can only add
        /// images to the resulting library if the platform supports mutable libraries; refer to
        /// <see cref="XRImageTrackingSubsystemDescriptor.supportsMutableLibrary"/>.
        /// </remarks>
        /// <param name="referenceImageLibrary">An existing <c>XRReferenceImageLibrary</c>, or <c>null</c> to create an empty image library.</param>
        /// <param name="cancellationToken">A cancellation token, which you can use to cancel the operation in progress.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the provider doesn't implement asynchronous
        /// library creation.</exception>
        /// <exception cref="System.OperationCanceledException">Thrown if the operation is canceled by
        /// <paramref name="cancellationToken"/>.</exception>
        public Awaitable<Result<RuntimeReferenceImageLibrary>> TryCreateRuntimeLibraryAsync(
            XRReferenceImageLibrary referenceImageLibrary, CancellationToken cancellationToken = default)
        {
            EnsureSubsystemInstanceSet();

            if (subsystem == null)
            {
                return AwaitableUtils<Result<RuntimeReferenceImageLibrary>>.FromResult(
                    new Result<RuntimeReferenceImageLibrary>(
                        new XRResultStatus(XRResultStatus.StatusCode.ProviderUninitialized), null));
            }

            return subsystem.TryCreateRuntimeLibraryAsync(referenceImageLibrary, cancellationToken);
        }

        [SerializeField]
        [Tooltip("The maximum number of moving images to track in realtime. Not all implementations support this feature.")]
        int m_MaxNumberOfMovingImages;

        bool supportsMovingImages => descriptor?.supportsMovingImages == true;

        /// <summary>
        /// The requested maximum number of moving images to track in real time.
        /// </summary>
        /// <remarks>
        /// Support can vary between devices and providers. Check for support at runtime with
        /// <see cref="SubsystemLifecycleManager{TSubsystem,TSubsystemDescriptor,TProvider}.descriptor"/>'s
        /// `supportsMovingImages` property. If `supportsMovingImages` is `false`, setting this property has no effect.
        /// The reverse isn't guaranteed: a provider can support moving images without supporting a configurable maximum
        /// number of them, in which case `supportsMovingImages` will be true, but setting this property has no effect.
        /// </remarks>
        public int requestedMaxNumberOfMovingImages
        {
            get => supportsMovingImages ? subsystem.requestedMaxNumberOfMovingImages : m_MaxNumberOfMovingImages;
            set
            {
                m_MaxNumberOfMovingImages = value;
                if (enabled && (descriptor?.supportsMovingImages == true))
                {
                    subsystem.requestedMaxNumberOfMovingImages = value;
                }
            }
        }

        /// <summary>
        /// Get the maximum number of moving images to track in real time that is currently in use by the subsystem.
        /// </summary>
        public int currentMaxNumberOfMovingImages => supportsMovingImages ? subsystem.currentMaxNumberOfMovingImages : 0;

        [SerializeField]
        [Tooltip("If not null, instantiates this prefab for each detected image.")]
        GameObject m_TrackedImagePrefab;

        /// <summary>
        /// If not null, instantiates this Prefab for each detected image.
        /// </summary>
        /// <remarks>
        /// The purpose of this property is to extend the functionality of <see cref="ARTrackedImage"/>s.
        /// It is not the recommended way to instantiate content associated with an <see cref="ARTrackedImage"/>.
        /// </remarks>
        public GameObject trackedImagePrefab
        {
            get => m_TrackedImagePrefab;
            set => m_TrackedImagePrefab = value;
        }

        /// <summary>
        /// Get the Prefab that will be instantiated for each <see cref="ARTrackedImage"/>.
        /// </summary>
        /// <returns>The Prefab that will be instantiated for each <see cref="ARTrackedImage"/>.</returns>
        protected override GameObject GetPrefab() => m_TrackedImagePrefab;

        /// <summary>
        /// Invoked once per frame with information about the <see cref="ARTrackedImage"/>s that have changed (that is, been added, updated, or removed).
        /// This happens just before <see cref="ARTrackedImage"/>s are destroyed, so you can set <c>ARTrackedImage.destroyOnRemoval</c> to <c>false</c>
        /// from this event to suppress this behavior.
        /// </summary>
        [Obsolete("trackedImagesChanged has been deprecated in AR Foundation version 6.0. Use trackablesChanged instead.", false)]
        public event Action<ARTrackedImagesChangedEventArgs> trackedImagesChanged;

        /// <summary>
        /// The name to be used for the <c>GameObject</c> whenever a new image is detected.
        /// </summary>
        protected override string gameObjectName => nameof(ARTrackedImage);

        /// <summary>
        /// Sets the image library on the subsystem before Start() is called on the <c>XRImageTrackingSubsystem</c>.
        /// </summary>
        protected override void OnBeforeStart()
        {
            if (subsystem.imageLibrary == null && m_SerializedLibrary != null)
            {
                CreateLibraryFromSerializedAsync();
                return;
            }

            UpdateReferenceImages(subsystem.imageLibrary);
            if (supportsMovingImages)
            {
                subsystem.requestedMaxNumberOfMovingImages = m_MaxNumberOfMovingImages;
            }

            enabled = (subsystem.imageLibrary != null);
#if !UNITY_6000_6_OR_NEWER || UNITY_ENABLE_CHECKS
#if !UNITY_6000_6_OR_NEWER
            if (Debug.isDebugBuild)
#endif
            {
                if (subsystem.imageLibrary == null)
                {
                    Debug.LogWarning($"{nameof(ARTrackedImageManager)} '{name}' was enabled but no reference image library is specified. To enable, set a valid reference image library and then re-enable this component.");
                }
            }
#endif
        }

        async void CreateLibraryFromSerializedAsync()
        {
            try
            {
                var result = await TrySetReferenceLibraryAsync(m_SerializedLibrary, destroyCancellationToken);
                if (!result.status.IsSuccess())
                {
                    enabled = false;
                    Debug.LogError(
                        $"{nameof(ARTrackedImageManager)} '{name}' failed to create a runtime reference image library from its serialized library: {result.status}. The component was disabled. Assign a valid reference image library and enable this component to retry.",
                        this);
                }
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        bool FindReferenceImage(Guid guid, out XRReferenceImage referenceImage)
        {
            if (m_ReferenceImages.TryGetValue(guid, out referenceImage))
                return true;

            // If we are using a mutable library, then it's possible an image
            // has been added that we don't yet know about, so search the library.
            if (referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                foreach (var candidateImage in mutableLibrary)
                {
                    if (candidateImage.guid.Equals(guid))
                    {
                        referenceImage = candidateImage;
                        m_ReferenceImages.Add(referenceImage.guid, referenceImage);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Invoked just after updating each <see cref="ARTrackedImage"/>. Used to update the <see cref="ARTrackedImage.referenceImage"/>.
        /// </summary>
        /// <param name="image">The tracked image being updated.</param>
        /// <param name="sessionRelativeData">New data associated with the tracked image. Spatial data is
        /// relative to the <see cref="XROrigin"/>.</param>
        protected override void OnAfterSetSessionRelativeData(
            ARTrackedImage image,
            XRTrackedImage sessionRelativeData)
        {
            if (FindReferenceImage(sessionRelativeData.sourceImageId, out XRReferenceImage referenceImage))
            {
                image.referenceImage = referenceImage;
            }
#if !UNITY_6000_6_OR_NEWER || UNITY_ENABLE_CHECKS
            else
            {
#if !UNITY_6000_6_OR_NEWER
                if (Debug.isDebugBuild)
#endif
                Debug.LogError($"Could not find reference image with guid {sessionRelativeData.sourceImageId}");
            }
#endif
        }

        /// <summary>
        /// Invokes the <see cref="trackedImagesChanged"/> event.
        /// </summary>
        /// <param name="added">A list of images added this frame.</param>
        /// <param name="updated">A list of images updated this frame.</param>
        /// <param name="removed">A list of images removed this frame.</param>
        [Obsolete("OnTrackablesChanged() has been deprecated in AR Foundation version 6.0.", false)]
        protected override void OnTrackablesChanged(
            List<ARTrackedImage> added, List<ARTrackedImage> updated, List<ARTrackedImage> removed)
        {
            if (trackedImagesChanged != null)
            {
                using (new ScopedProfiler("OnTrackedImagesChanged"))
                    trackedImagesChanged?.Invoke(
                        new ARTrackedImagesChangedEventArgs(
                            added,
                            updated,
                            removed));
            }
        }

        void UpdateReferenceImages(RuntimeReferenceImageLibrary library)
        {
            m_ReferenceImages.Clear();

            if (library == null)
                return;

            int count = library.count;
            for (int i = 0; i < count; ++i)
            {
                var referenceImage = library[i];
                m_ReferenceImages[referenceImage.guid] = referenceImage;
            }
        }
    }
}
