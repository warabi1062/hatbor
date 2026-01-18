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
        [SerializeField] FloatReactiveProperty intensity = new(2f);

        [ConfigProperty("Direction")]
        public ReactiveProperty<Vector3> Direction => direction;
        [TemperatureConfigProperty("Temperature")]
        public ReactiveProperty<float> Temperature => temperature;
        [ConfigProperty("Intensity")]
        public ReactiveProperty<float> Intensity => intensity;


    }
}