using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hatbor.Rig;
using UniGLTF;
using UniVRM10;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Hatbor.Avatar
{
    public sealed class Avatar : IAsyncStartable, ITickable, IDisposable
    {
        static readonly Vector3 Position = new(0f, -1f, -2f);

        readonly string path;
        readonly AvatarRig rig;

        Vrm10Instance instance;

        [Inject]
        public Avatar(string path, AvatarRig rig)
        {
            this.path = path;
            this.rig = rig;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            instance = await LoadAsync(path, cancellation);
            Setup(instance.GetComponent<RuntimeGltfInstance>());
            rig.Initialize(instance);

            instance.transform.position = Position;
        }

        void ITickable.Tick()
        {
            if (instance == null) return;
            rig.Update(instance);
        }

        void IDisposable.Dispose()
        {
            if (instance == null) return;
            Object.Destroy(instance.gameObject);
            instance = null;
        }

        static async UniTask<Vrm10Instance> LoadAsync(string path, CancellationToken ctx)
        {
            var instance = await Vrm10.LoadPathAsync(path,
                controlRigGenerationOption: ControlRigGenerationOption.None,
                materialGenerator: new UrpVrm10MaterialDescriptorGenerator(),
                ct: ctx);
            return instance;
        }

        static void Setup(RuntimeGltfInstance instance)
        {
            instance.EnableUpdateWhenOffscreen();
        }
    }
}