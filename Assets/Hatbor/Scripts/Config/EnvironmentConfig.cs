using System;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Hatbor.Config
{
    [Serializable, ConfigGroup("Environment")]
    public sealed class EnvironmentConfig : IConfigurable
    {
        public string PersistentKey => "EnvironmentConfig";

        [SerializeField] ReactiveProperty<TonemappingMode> tonemapping = new(UnityEngine.Rendering.Universal.TonemappingMode.Neutral);
        [SerializeField] BoolReactiveProperty bloomEnabled = new(false);
        [SerializeField] FloatReactiveProperty bloomThreshold = new(1.1f);
        [SerializeField] FloatReactiveProperty bloomIntensity = new(1f);
        [SerializeField] FloatReactiveProperty bloomScatter = new(0.7f);
        [SerializeField] BoolReactiveProperty vignetteEnabled = new(false);
        [SerializeField] FloatReactiveProperty vignetteIntensity = new(0.3f);
        [SerializeField] FloatReactiveProperty vignetteSmoothness = new(0.3f);

        [ConfigProperty("Tonemapping")]
        public ReactiveProperty<TonemappingMode> Tonemapping => tonemapping;
        [ConfigProperty("Bloom")]
        public ReactiveProperty<bool> BloomEnabled => bloomEnabled;
        [SliderConfigProperty("Bloom Threshold", 1.1f, 0f, 2f)]
        public ReactiveProperty<float> BloomThreshold => bloomThreshold;
        [SliderConfigProperty("Bloom Intensity", 1f, 0f, 5f)]
        public ReactiveProperty<float> BloomIntensity => bloomIntensity;
        [SliderConfigProperty("Bloom Scatter", 0.7f, 0f, 1f)]
        public ReactiveProperty<float> BloomScatter => bloomScatter;
        [ConfigProperty("Vignette")]
        public ReactiveProperty<bool> VignetteEnabled => vignetteEnabled;
        [SliderConfigProperty("Vignette Intensity", 0.3f, 0f, 1f)]
        public ReactiveProperty<float> VignetteIntensity => vignetteIntensity;
        [SliderConfigProperty("Vignette Smoothness", 0.3f, 0.01f, 1f)]
        public ReactiveProperty<float> VignetteSmoothness => vignetteSmoothness;
    }
}
