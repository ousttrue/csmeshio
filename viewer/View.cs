using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

namespace SlimDXViewer
{

    public class View
    {
        // projection
        private float _fovy=(float)(Math.PI / 4);
        private float _aspectRatio=1.0f;
        private float _near=0.1f;
        private float _far=1000.0f;
        private Matrix _projection;

        // view
        private float _head=0;
        private float _pitch=0;
        private float _roll=0;
        private float _distance=5.0f;
        private float _shiftX=0;
        private float _shiftY=0;
        private const float SHIFT_FACTOR=0.01f;
        private Matrix _view;

        public View()
        {
        }

        public void OnResize(int w, int h)
        {
            _aspectRatio=(float)w / (float)h;
            CalcMatrix();
        }

        private float ToRadian(float degree)
        {
            return (float)(Math.PI  * (degree/180));
        }

        private void CalcMatrix()
        {
            _projection=Matrix.PerspectiveFovLH(_fovy, 
                    _aspectRatio,
                    _near, _far);

            _view=
                Matrix.RotationY(ToRadian(_head)) 
                * Matrix.RotationX(ToRadian(_pitch))
                * Matrix.RotationZ(ToRadian(_roll));
            _view.M41=_shiftX;
            _view.M42=_shiftY;
            _view.M43=_distance;
            /*
            _view=Matrix.LookAtLH(
                    new Vector3(3.0f, 2.0f, -3.0f),
                    Vector3.Zero, _up);
                    */
        }

        public void Head(float d)
        {
            _head+=d;
            CalcMatrix();
        }

        public void Pitch(float d)
        {
            _pitch+=d;
            CalcMatrix();
        }

        public void ShiftX(float d)
        {
            _shiftX+=d * SHIFT_FACTOR;
            CalcMatrix();
        }

        public void ShiftY(float d)
        {
            _shiftY+=d * SHIFT_FACTOR;
            CalcMatrix();
        }

        public void Dolly(int d)
        {
            if(d<0){
                _distance*=1.1f;
            }
            else if(d>0){
                _distance*=0.9f;
            }
            CalcMatrix();
        }

        public void Render(Device device)
        {
            device.SetTransform(TransformState.Projection, _projection);
            device.SetTransform(TransformState.View, _view);
        }
    };

}
