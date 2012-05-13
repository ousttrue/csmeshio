using System;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace SlimDXViewer
{
    interface IDrawable
    {
        /// <summary>
        /// 描画する
        /// </summary>
        void Draw(Device device);
    }

    class Teapot: IResource, IDrawable
    {
        private Mesh _mesh;

        bool IResource.Initialize(Device device)
        {
            _mesh = Mesh.CreateTeapot(device);
            return true;
        }

        bool IResource.IsDefaultPool()
        {
            return false;
        }

        void IResource.Release()
        {
        }

        void IDrawable.Draw(Device device)
        {
            if(_mesh==null){
                return;
            }
            //マテリアル設定
            device.Material = new Material() { Diffuse = new Color4(Color.GhostWhite) };
            //描画
            _mesh.DrawSubset(0);
        }
    }

}

