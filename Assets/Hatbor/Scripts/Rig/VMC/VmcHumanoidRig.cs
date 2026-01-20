using Hatbor.Config;
using Hatbor.VMC;
using UniHumanoid;
using UnityEngine;
using UniVRM10;
using VContainer;

namespace Hatbor.Rig.VMC
{
    public sealed class VmcHumanoidRig : IHumanoidRig
    {
        const float SmoothFactor = 0.05f;

        readonly VmcServer vmcServer;
        readonly FixedCameraConfig cameraConfig;

        [Inject]
        public VmcHumanoidRig(VmcServer vmcServer, FixedCameraConfig cameraConfig)
        {
            this.vmcServer = vmcServer;
            this.cameraConfig = cameraConfig;
        }

        void IHumanoidRig.Update(Vrm10Instance instance)
        {
            vmcServer.ProcessRead();
            Update(instance.Humanoid);
        }

        void Update(Humanoid humanoid)
        {
            var boneLocalPoses = vmcServer.BoneLocalPoses;
            foreach (var (bone, pose) in boneLocalPoses)
            {
                var t = humanoid.GetBoneTransform(bone);
                if (t == null) continue;
                if (bone == HumanBodyBones.Hips)
                {
                    t.localRotation = Quaternion.Slerp(t.localRotation, pose.rotation, SmoothFactor);
                    ApplyFaceToCamera(t);
                }
                else
                {
                    t.localRotation = Quaternion.Slerp(t.localRotation, pose.rotation, SmoothFactor);
                }
            }
        }

        void ApplyFaceToCamera(Transform hips)
        {
            var cameraPos = cameraConfig.CameraPosition.Value;
            var hipsPos = hips.position;

            var direction = new Vector3(cameraPos.x - hipsPos.x, 0f, cameraPos.z - hipsPos.z);
            if (direction.sqrMagnitude < 0.001f) return;

            var targetYRotation = Quaternion.LookRotation(direction).eulerAngles.y;
            var currentRotation = hips.rotation.eulerAngles;
            hips.rotation = Quaternion.Euler(currentRotation.x, targetYRotation, currentRotation.z);
        }
    }
}
