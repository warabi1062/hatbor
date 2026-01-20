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
        const float SmoothFactor = 0.5f;

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

            // First pass: apply all VMC rotations
            foreach (var (bone, pose) in boneLocalPoses)
            {
                var t = humanoid.GetBoneTransform(bone);
                if (t == null) continue;
                t.localRotation = Quaternion.Slerp(t.localRotation, pose.rotation, SmoothFactor);
            }

            // Second pass: adjust hips to face camera with head influence
            var hips = humanoid.GetBoneTransform(HumanBodyBones.Hips);
            var head = humanoid.GetBoneTransform(HumanBodyBones.Head);
            if (hips != null)
            {
                ApplyFaceToCameraWithHeadInfluence(hips, head);
            }
        }

        void ApplyFaceToCameraWithHeadInfluence(Transform hips, Transform head)
        {
            const float headInfluence = 0.4f;

            var cameraPos = cameraConfig.CameraPosition.Value;
            var hipsPos = hips.position;

            var directionToCamera = new Vector3(cameraPos.x - hipsPos.x, 0f, cameraPos.z - hipsPos.z);
            if (directionToCamera.sqrMagnitude < 0.001f) return;

            var baseCameraYRotation = Quaternion.LookRotation(directionToCamera).eulerAngles.y;

            // Get head's Y rotation relative to hips (local rotation in hierarchy)
            var headYOffset = 0f;
            if (head != null)
            {
                // Calculate head's local Y rotation relative to hips
                var hipsForward = hips.forward;
                hipsForward.y = 0f;
                var headForward = head.forward;
                headForward.y = 0f;

                if (hipsForward.sqrMagnitude > 0.001f && headForward.sqrMagnitude > 0.001f)
                {
                    var hipsYRotation = Quaternion.LookRotation(hipsForward).eulerAngles.y;
                    var headYRotation = Quaternion.LookRotation(headForward).eulerAngles.y;
                    // Head rotation relative to hips
                    headYOffset = Mathf.DeltaAngle(hipsYRotation, headYRotation) * headInfluence;
                }
            }

            var currentRotation = hips.rotation.eulerAngles;
            hips.rotation = Quaternion.Euler(currentRotation.x, baseCameraYRotation + headYOffset, currentRotation.z);
        }
    }
}
