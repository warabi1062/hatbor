using System;
using UniRx;
using UnityEngine;

namespace Hatbor.Config
{
    [Serializable, ConfigGroup("Avatar")]
    public sealed class AvatarConfig : IConfigurable
    {
        static readonly string DefaultPath = System.IO.Path.Combine(Application.streamingAssetsPath, "avatar.vrm");

        public string PersistentKey => "AvatarConfig";

        [SerializeField] StringReactiveProperty path = new(DefaultPath);
        [SerializeField] Vector3ReactiveProperty position = new(new UnityEngine.Vector3(0f, -1f, -2f));

        [FilePathConfigProperty("Choose Avatar", "vrm")]
        public ReactiveProperty<string> Path => path;
        [ConfigProperty("Position")]
        public ReactiveProperty<UnityEngine.Vector3> Position => position;
    }
}