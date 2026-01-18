using System;
using UniRx;
using UnityEngine;

namespace Hatbor.Config
{
    [Serializable, ConfigGroup("Debug")]
    public sealed class DebugConfig : IConfigurable
    {
        public string PersistentKey => "DebugConfig";

        [SerializeField] BoolReactiveProperty vmcDebugEnabled = new(false);

        [ConfigProperty("VMC Debug Enabled")]
        public ReactiveProperty<bool> VmcDebugEnabled => vmcDebugEnabled;
    }
}
