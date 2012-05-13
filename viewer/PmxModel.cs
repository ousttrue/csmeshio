using System;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace SlimDXViewer
{
    /// <summary>
    /// Represents a vertex with a position, normal and uv.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PmxVertex : IEquatable<PmxVertex> {
        public Vector3 Position {
            get;
            set;
        }

        public Vector3 Normal {
            get;
            set;
        }

        public Vector2 UV {
            get;
            set;
        }

        public PmxVertex(Vector3 position, Vector3 normal, Vector2 uv)
            : this() {
                Position = position;
                Normal = normal;
                UV = uv;
            }

        public static bool operator ==(PmxVertex left, PmxVertex right) {
            return left.Equals(right);
        }

        public static bool operator !=(PmxVertex left, PmxVertex right) {
            return !(left == right);
        }

        public override int GetHashCode() {
            return Position.GetHashCode() + Normal.GetHashCode() + UV.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return Equals((PmxVertex)obj);
        }

        public bool Equals(PmxVertex other) {
            return (Position == other.Position 
                    && Normal == other.Normal
                    && UV == other.UV);
        }
    }

    class PmxModel: IResource, IDrawable
    {
        meshio.Pmx.Model _pmx;
        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        VertexDeclaration _vertexDecl;

        public PmxModel(meshio.Pmx.Model pmx)
        {
            _pmx=pmx;
        }

        bool IResource.Initialize(Device device)
        {
            if(_vertexBuffer!=null){
                return true;
            }

            // VertexBuffer
            _vertexBuffer = new VertexBuffer(
                    device,
                    _pmx.Vertices.Length * Marshal.SizeOf( typeof( PmxVertex ) ),
                    Usage.WriteOnly,
                    VertexFormat.None,
                    Pool.Managed
                    );
            {
                var stream = _vertexBuffer.Lock( 0, 0, LockFlags.None );
                foreach(var v in _pmx.Vertices){
                    stream.Write(
                            new PmxVertex( 
                                new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
                                new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z),
                                new Vector2(v.UV.X, v.UV.Y)
                                ));
                }
                _vertexBuffer.Unlock();
            }

            // IndexBuffer
            _indexBuffer = new IndexBuffer(
                    device,
                    _pmx.Indices.Length * 4,
                    Usage.WriteOnly,
                    Pool.Managed,
                    false);
            {
                var stream = _indexBuffer.Lock(0, 0, LockFlags.None);
                stream.WriteRange(
                        _pmx.Indices, 0, _pmx.Indices.Length
                        );
                _indexBuffer.Unlock();
            }

            // VertexDeclaration
            _vertexDecl = new VertexDeclaration(device, new[] {
                    new VertexElement(0, 0, 
                        DeclarationType.Float3, 
                        DeclarationMethod.Default, DeclarationUsage.Position, 0), 
                    new VertexElement(0, 12, 
                        DeclarationType.Float3, 
                        DeclarationMethod.Default, DeclarationUsage.Normal, 0), 
                    new VertexElement(0, 24, 
                        DeclarationType.Float2, 
                        DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0), 
                    VertexElement.VertexDeclarationEnd
                    });

            return true;
        }

        bool IResource.IsDefaultPool()
        {
            return false;
        }

        void IResource.Release()
        {
            if(_vertexBuffer==null){
                return;
            }

            _vertexBuffer.Dispose();    
            _vertexBuffer=null;
            _indexBuffer.Dispose();
            _indexBuffer=null;
            _vertexDecl.Dispose();
            _vertexDecl=null;
        }

        void IDrawable.Draw(Device device)
        {
            if(_vertexBuffer==null){
                return;
            }
            //マテリアル設定
            device.Material = new Material() { Diffuse = new Color4(Color.GhostWhite) };

            // vertices
            device.SetStreamSource( 0, _vertexBuffer, 0, 
                    Marshal.SizeOf( typeof( PmxVertex ) ) );
            device.VertexDeclaration = _vertexDecl;

            // indices
            device.Indices = _indexBuffer;
            device.DrawIndexedPrimitives( PrimitiveType.TriangleList, 0, 0,
                    _pmx.Vertices.Length,
                    0, _pmx.Indices.Length/3 );
        }
    }
}

