using System;
using System.Drawing;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace SlimDXViewer
{
    class Triangle: IResource, IDrawable
    {
        VertexBuffer _vertexBuffer;
        VertexDeclaration _vertexDecl;

        /// <summary>
        /// Represents a vertex with a position and a color.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ColoredVertex : IEquatable<ColoredVertex> {
            /// <summary>
            /// Gets or sets the position of the vertex.
            /// </summary>
            public Vector3 Position {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the color of the vertex.
            /// </summary>
            public int Color {
                get;
                set;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ColoredVertex"/> struct.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="color">The color.</param>
            public ColoredVertex(Vector3 position, int color)
                : this() {
                    Position = position;
                    Color = color;
                }

            /// <summary>
            /// Implements operator ==.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(ColoredVertex left, ColoredVertex right) {
                return left.Equals(right);
            }

            /// <summary>
            /// Implements operator !=.
            /// </summary>
            /// <param name="left">The left side of the operator.</param>
            /// <param name="right">The right side of the operator.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(ColoredVertex left, ColoredVertex right) {
                return !(left == right);
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            public override int GetHashCode() {
                return Position.GetHashCode() + Color.GetHashCode();
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="obj">Another object to compare to.</param>
            /// <returns>
            /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
            /// </returns>
            public override bool Equals(object obj) {
                if (obj == null)
                    return false;

                if (GetType() != obj.GetType())
                    return false;

                return Equals((ColoredVertex)obj);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            public bool Equals(ColoredVertex other) {
                return (Position == other.Position && Color == other.Color);
            }
        }

        bool IResource.Initialize(Device device)
        {
            // VertexBuffer
            _vertexBuffer = new VertexBuffer(
                    device,
                    3 * Marshal.SizeOf( typeof( ColoredVertex ) ),
                    Usage.WriteOnly,
                    VertexFormat.None,
                    Pool.Managed
                    );

            var stream = _vertexBuffer.Lock( 0, 0, LockFlags.None );
            stream.WriteRange( new[] {
                    new ColoredVertex( new Vector3(0.0f, 0.5f, 0), Color.Red.ToArgb() ),
                    new ColoredVertex( new Vector3(0.5f, -0.5f, 0), Color.Blue.ToArgb() ),
                    new ColoredVertex( new Vector3(-0.5f, -0.5f, 0), Color.Green.ToArgb() ),
                    } );

            _vertexBuffer.Unlock();

            // VertexDeclaration
            _vertexDecl = new VertexDeclaration(device, new[] {
                    new VertexElement(0, 0, 
                        DeclarationType.Float3, 
                        DeclarationMethod.Default, DeclarationUsage.Position, 0), 
                    new VertexElement(0, 12, 
                        DeclarationType.Color, 
                        DeclarationMethod.Default, DeclarationUsage.Color, 0), 
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
            _vertexBuffer.Dispose();    
            _vertexDecl.Dispose();
        }

        void IDrawable.Draw(Device device)
        {
            device.SetRenderState( RenderState.Lighting, false );

            device.SetStreamSource( 0, _vertexBuffer, 0, 
                    Marshal.SizeOf( typeof( ColoredVertex ) ) );
            device.VertexDeclaration = _vertexDecl;
            device.DrawPrimitives( PrimitiveType.TriangleList, 0, 1 );
        }
    }
}

