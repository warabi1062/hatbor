using System;
using UniRx;
using UnityEngine;

namespace Hatbor.Config
{
    [Serializable, ConfigGroup("Camera")]
    public sealed class FixedCameraConfig : IConfigurable
    {
        public string PersistentKey => "HidCameraConfig";

        [SerializeField]
        Vector3ReactiveProperty cameraPosition = new (Vector3.zero);
        [SerializeField]
        Vector3ReactiveProperty cameraRotation = new (new Vector3(0f, 180f, 0f));
        [SerializeField]
        FloatReactiveProperty fieldOfView = new (30f);

        [Vector3ConfigProperty("Position", 0f, 0f, 0f)]
        public ReactiveProperty<Vector3> CameraPosition => cameraPosition;
        [Vector3ConfigProperty("Rotation", 0f, 180f, 0f)]
        public ReactiveProperty<Vector3> CameraRotation => cameraRotation;
        [SliderConfigProperty("Field of View", 30f, 1f, 120f)]
        public ReactiveProperty<float> FieldOfView => fieldOfView;
    }
}