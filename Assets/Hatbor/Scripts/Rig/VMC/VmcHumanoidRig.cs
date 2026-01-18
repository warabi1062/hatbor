using Hatbor.VMC;
using UniHumanoid;
using UnityEngine;
using UniVRM10;
using VContainer;

namespace Hatbor.Rig.VMC
{
    public sealed class VmcHumanoidRig : IHumanoidRig
    {
        const float SmoothFactor = 0.03f;

        readonly VmcServer vmcServer;

        [Inject]
        public VmcHumanoidRig(VmcServer vmcServer)
        {
            this.vmcServer = vmcServer;
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
                    var smoothedPosition = Vector3.Lerp(t.localPosition, pose.position, SmoothFactor);
                    var smoothedRotation = Quaternion.Slerp(t.localRotation, pose.rotation, SmoothFactor);
                    t.SetLocalPositionAndRotation(smoothedPosition, smoothedRotation);
                }
                else
                {
                    t.localRotation = Quaternion.Slerp(t.localRotation, pose.rotation, SmoothFactor);
                }
            }
        }
    }
}