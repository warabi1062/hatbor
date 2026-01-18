using Hatbor.VMC;
using UnityEngine;
using UniVRM10;
using VContainer;

namespace Hatbor.Rig.VMC
{
    public sealed class VmcRootTransformRig : IRootTransformRig
    {
        const float SmoothFactor = 0.03f;

        readonly VmcServer vmcServer;

        [Inject]
        public VmcRootTransformRig(VmcServer vmcServer)
        {
            this.vmcServer = vmcServer;
        }

        void IRootTransformRig.Update(Vrm10Instance instance)
        {
            vmcServer.ProcessRead();
            var rootPose = vmcServer.RootPose;
            var t = instance.transform;
            var smoothedRotation = Quaternion.Slerp(t.localRotation, rootPose.rotation, SmoothFactor);
            t.localRotation = smoothedRotation;
        }
    }
}