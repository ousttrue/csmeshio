using System.Collections.Generic;
using System.Linq;
using SlimDX.Direct3D9;

namespace SlimDXViewer
{
    interface IResource
    {
        /// <summary>
        /// リソースを生成する
        /// </summary>
        bool Initialize(Device device);

        /// <summary>
        /// リソースを開放する
        /// </summary>
        void Release();

        /// <summary>
        /// DeviceのReset前に開放されるべきか？
        /// </summary>
        bool IsDefaultPool();
    }

    /// ToDo: WeakRef
    class ResourceManager
    {
        Queue<IResource> _released=new Queue<IResource>();
        List<IResource> _initialized=new List<IResource>();

        public ResourceManager()
        {
        }

        public void Add(IResource resource)
        {
            _released.Enqueue(resource);
        }

        /// <summary>
        /// リソースをひとつ生成する
        /// </summary>
        public void Update(Device device)
        {
            if(_released.Count()==0){
                return;
            }
            var r=_released.Dequeue();
            if(r.Initialize(device)){
                // 初期化済み
                _initialized.Add(r);
            }
            else{
                // 後ろに回す
                _released.Enqueue(r);
            }
        }

        /// <summary>
        /// すべてのリソースを開放する
        /// </summary>
        public void Release()
        {
            foreach(var r in _initialized){
                r.Release();
                _released.Enqueue(r);
            }
            _initialized.Clear();
        }

        /// <summary>
        /// すべてのD3DPOOL_DEFAULTで作られたリソースを開放する
        /// </summary>
        public void ClearDefaultPoolResources()
        {
            foreach(var r in _initialized.Where(r => r.IsDefaultPool())){
                _initialized.Remove(r);
                r.Release();
                _released.Enqueue(r);
            }
        }
    }
}

