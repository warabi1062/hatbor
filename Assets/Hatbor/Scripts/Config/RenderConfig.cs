using System;
using UniRx;
using UnityEngine;

namespace Hatbor.Config
{
    [Serializable, ConfigGroup("Rendering")]
    public sealed class RenderConfig : IConfigurable
    {
        public string PersistentKey => "RenderConfig";

        [SerializeField] Vector2IntReactiveProperty size = new(new Vector2Int(1920, 1080));
        [SerializeField] IntReactiveProperty targetFrameRate = new(60);
        [SerializeField] BoolReactiveProperty enabledSharingTexture = new(true);
        [SerializeField] BoolReactiveProperty transparentBackground = new(true);
        [SerializeField] BoolReactiveProperty mirrorPreview = new(true);

        [Vector2IntConfigProperty("Size", 1920, 1080)]
        public ReactiveProperty<Vector2Int> Size => size;
        [ConfigProperty("Target FPS", IsDelayed = true)]
        public ReactiveProperty<int> TargetFrameRate => targetFrameRate;
#if UNITY_STANDALONE_OSX
        [ConfigProperty("Syphon")]
#elif UNITY_STANDALONE_WIN
        [ConfigProperty("Spout2")]
#endif
        public ReactiveProperty<bool> EnabledSharingTexture => enabledSharingTexture;
        [ConfigProperty("TransparentBG")]
        public ReactiveProperty<bool> TransparentBackground => transparentBackground;
        [ConfigProperty("Mirror Preview")]
        public ReactiveProperty<bool> MirrorPreview => mirrorPreview;
    }
}