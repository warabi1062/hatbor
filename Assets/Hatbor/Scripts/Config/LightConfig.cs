using System;
using UniRx;
using UnityEngine;

namespace Hatbor.Config
{
    [Serializable, ConfigGroup("Lighting")]
    public sealed class LightConfig : IConfigurable
    {
        public string PersistentKey => "LightConfig";

        [SerializeField] Vector3ReactiveProperty direction = new(new Vector3(50, 180, 0));
        [SerializeField] FloatReactiveProperty temperature = new(6500f);
        [SerializeField] FloatReactiveProperty intensity = new(1f);

        [Vector3ConfigProperty("Direction", 50f, 180f, 0f)]
        public ReactiveProperty<Vector3> Direction => direction;
        [TemperatureConfigProperty("Temperature")]
        public ReactiveProperty<float> Temperature => temperature;
        [SliderConfigProperty("Intensity", 1f, 0f, 10f)]
        public ReactiveProperty<float> Intensity => intensity;


    }
}