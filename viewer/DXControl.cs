using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using SlimDX;
using SlimDX.Direct3D9;

namespace SlimDXViewer
{

    public class DXControl : UserControl
    {
        private Direct3D _direct3d;
        private PresentParameters _presentParam;
        private Device _device;
        private bool _deviceLost = false;
        private Stopwatch _sw = new Stopwatch(); 

        private Scene _scene = new Scene();
        public Scene Scene
        {
            get{ return _scene; }
        }

        public DXControl()
        {
            // start timer
            _sw.Start();

            // initialize d3d device
            _direct3d = new Direct3D();
            _presentParam = new PresentParameters()
            {
                BackBufferWidth = ClientSize.Width,
                BackBufferHeight = ClientSize.Height,
                Windowed = true,
                BackBufferFormat = Format.X8R8G8B8,
                BackBufferCount = 1,
                SwapEffect = SwapEffect.Discard,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D24S8,
            };
            _device = new Device(_direct3d, 0, 
                    DeviceType.Hardware, Handle, 
                    CreateFlags.HardwareVertexProcessing, _presentParam);

            // event handlers
            Resize += OnResize;
        }

        /// <summary>
        /// クライアント領域のサイズが変更された
        /// </summary>
        private void OnResize(object sender, EventArgs e)
        {
            _presentParam.BackBufferWidth = ClientSize.Width;
            _presentParam.BackBufferHeight = ClientSize.Height;
            Reset();
            _scene.OnResize(ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、
        /// 破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing){
                _scene.Dispose();
                _device.Dispose();
                _direct3d.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// デバイスリセット
        /// </summary>
        public void Reset()
        {
            _scene.ClearDefaultResources();
            _device.Reset(_presentParam);
            _deviceLost = false;
        }

        /// <summary>
        /// ループ１回分
        /// </summary>
        public void ProcessFrame()
        {
            // 更新
            long delta=_sw.ElapsedMilliseconds;
            _sw.Reset();
            _scene.Update(_device, delta);

            if (_deviceLost) {
                //デバイスロスト時の対応
                if (_device.TestCooperativeLevel() == ResultCode.DeviceNotReset)
                {
                    Reset();
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                    return;
                }
            }

            try
            {
                _device.BeginScene();

                _scene.Render(_device);

                _device.EndScene();
                _device.Present(); 
            }
            catch (Direct3D9Exception e)
            {
                //デバイスロストかどうか判定
                if (e.ResultCode == ResultCode.DeviceLost)
                {
                    _deviceLost = true;
                }
                else
                {
                    throw;
                }
            }
        }

    }
}

