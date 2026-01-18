using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hatbor.Config;
using Hatbor.Rig;
using UniGLTF;
using UniRx;
using UniVRM10;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Hatbor.Avatar
{
    public sealed class Avatar : IAsyncStartable, ITickable, IDisposable
    {
        readonly string path;
        readonly AvatarRig rig;
        readonly AvatarConfig config;
        readonly CompositeDisposable disposables = new();

        Vrm10Instance instance;

        [Inject]
        public Avatar(string path, AvatarRig rig, AvatarConfig config)
        {
            this.path = path;
            this.rig = rig;
            this.config = config;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            instance = await LoadAsync(path, cancellation);
            Setup(instance.GetComponent<RuntimeGltfInstance>());
            rig.Initialize(instance);

            config.Position
                .Subscribe(pos => instance.transform.position = pos)
                .AddTo(disposables);
        }

        void ITickable.Tick()
        {
            if (instance == null) return;
            rig.Update(instance);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
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