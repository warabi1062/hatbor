using System;
using Hatbor.Config;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer;
using VContainer.Unity;

namespace Hatbor.Camera
{
    public sealed class PostProcessingController : IStartable, IDisposable
    {
        readonly EnvironmentConfig environmentConfig;
        readonly CompositeDisposable disposables = new();

        Volume volume;
        Tonemapping tonemapping;
        Bloom bloom;
        Vignette vignette;

        [Inject]
        public PostProcessingController(EnvironmentConfig environmentConfig)
        {
            this.environmentConfig = environmentConfig;
        }

        void IStartable.Start()
        {
            volume = UnityEngine.Object.FindFirstObjectByType<Volume>();
            if (volume == null)
            {
                Debug.LogWarning("Volume not found");
                return;
            }

            if (volume.profile.TryGet(out tonemapping))
            {
                environmentConfig.Tonemapping
                    .Subscribe(mode => tonemapping.mode.Override(mode))
                    .AddTo(disposables);
            }

            if (volume.profile.TryGet(out bloom))
            {
                environmentConfig.BloomEnabled
                    .Subscribe(enabled => bloom.active = enabled)
                    .AddTo(disposables);

                environmentConfig.BloomThreshold
                    .Subscribe(threshold => bloom.threshold.Override(threshold))
                    .AddTo(disposables);

                environmentConfig.BloomIntensity
                    .Subscribe(intensity => bloom.intensity.Override(intensity))
                    .AddTo(disposables);

                environmentConfig.BloomScatter
                    .Subscribe(scatter => bloom.scatter.Override(scatter))
                    .AddTo(disposables);
            }

            if (volume.profile.TryGet(out vignette))
            {
                environmentConfig.VignetteEnabled
                    .Subscribe(enabled => vignette.active = enabled)
                    .AddTo(disposables);

                environmentConfig.VignetteIntensity
                    .Subscribe(intensity => vignette.intensity.Override(intensity))
                    .AddTo(disposables);

                environmentConfig.VignetteSmoothness
                    .Subscribe(smoothness => vignette.smoothness.Override(smoothness))
                    .AddTo(disposables);
            }
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }
    }
}
