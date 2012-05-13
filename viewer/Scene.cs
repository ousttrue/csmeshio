using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using SlimDX;
using SlimDX.Direct3D9;

namespace SlimDXViewer
{
    public class Scene
    {
        private ResourceManager _resourceManager=new ResourceManager();
        private List<IDrawable> _drawables=new List<IDrawable>();            
        private List<Light> _lights=new List<Light>();

        private View _view = new View();
        public View View
        {
            get{ return _view; }
        }

        public Scene()
        {
            _lights.Add(
                    new Light()
                    {
                    Type = LightType.Directional,
                    Diffuse = Color.White,
                    Ambient = Color.GhostWhite,
                    Direction = new SlimDX.Vector3(0.0f, -1.0f, 0.0f)
                    });
        }

        ~Scene()
        {
            Dispose();
        }

        /// <summary>
        /// D3DPOOL_DEFAULTフラグで作成したリソースを開放するべし
        /// </summary>
        public void ClearDefaultResources()
        {
            _resourceManager.ClearDefaultPoolResources();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _resourceManager.Release();
        }

        /// <summary>
        /// pmxモデルを追加
        /// </summary>
        public void Load(meshio.Pmx.Model pmx)
        {
            var model=new PmxModel(pmx);
            _drawables.Add(model);
            _resourceManager.Add(model);
        }

        /// <summary>
        /// teapotモデルを追加
        /// </summary>
        public void LoadTeapot()
        {
            var model=new Teapot();
            _drawables.Add(model);
            _resourceManager.Add(model);
        }

        /// <summary>
        /// 三角形モデルを追加
        /// </summary>
        public void LoadTriangle()
        {
            var model=new Triangle();
            _drawables.Add(model);
            _resourceManager.Add(model);
        }

        /// <summary>
        /// ビューポートサイズの変更
        /// </summary>
        public void OnResize(int w, int h)
        {
            _view.OnResize(w, h);
        }

        /// <summary>
        /// シーンを更新
        /// </summary>
        public void Update(Device device, long delta)
        {
            // リソース生成
            _resourceManager.Update(device);
        }

        /// <summary>
        /// シーンを描画
        /// </summary>
        public void Render(Device device)
        {
            device.Clear(ClearFlags.All, Color.Black, 1, 0); 
            _view.Render(device);

            //ライト設定
            device.SetRenderState(RenderState.Lighting, true);
            foreach(var item in _lights.Select((l, i)=>new {l, i})){
                device.SetLight(item.i, item.l);
                device.EnableLight(item.i, true);
            }

            _drawables.ForEach(d => d.Draw(device));
        }
    }

}

