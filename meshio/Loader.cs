using System;


namespace meshio {

    public struct Vector2
    {
        public float X;
        public float Y;
    };

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;
    };

    public struct Vector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
    };

    public class ByteReader
    {
        byte[] _buf;
        int _index=0;

        public ByteReader(byte[] buf)
        {
            _buf=buf;
        }

        public byte GetByte()
        {
            byte n=_buf[_index];
            _index++;
            return n;
        }

        public ushort GetUShort()
        {
            ushort n=BitConverter.ToUInt16(_buf, _index);
            _index+=2;
            return n;
        }

        public int GetInt()
        {
            int n=BitConverter.ToInt32(_buf, _index);
            _index+=4;
            return n;
        }

        public uint GetUInt()
        {
            uint n=BitConverter.ToUInt32(_buf, _index);
            _index+=4;
            return n;
        }

        public float GetFloat()
        {
            float n=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            return n;
        }

        public Vector2 GetVector2()
        {
            Vector2 v;
            v.X=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            v.Y=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            return v;
        }

        public Vector3 GetVector3()
        {
            Vector3 v;
            v.X=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            v.Y=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            v.Z=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            return v;
        }

        public Vector4 GetVector4()
        {
            Vector4 v;
            v.X=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            v.Y=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            v.Z=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            v.W=BitConverter.ToSingle(_buf, _index);
            _index+=4;
            return v;
        }

		public void GetBytes(ref byte[] bytes)
		{
			for(int i=0; i<bytes.Length; ++i){
				bytes[i]=_buf[_index];
				++_index;
			}
		}

        public string GetAscii(int len)
        {
			int i=0;
			for(; i<len; ++i){
				if(_buf[_index+i]=='\0'){
					break;
				}
			}
            string s=System.Text.Encoding.ASCII.GetString(_buf, _index, i);
            _index+=len;
            return s;
        }

        public string GetSJIS(int len)
        {
			int i=0;
			for(; i<len; ++i){
				if(_buf[_index+i]=='\0'){
					break;
				}
			}
            string s=System.Text.Encoding.GetEncoding(932).GetString(_buf, _index, i);
            _index+=len;
            return s;
        }

		///
		/// PMX様式(int byte length + bytes)
		///
        public string GetUnicodeText()
        {
            int len=GetInt();
            string s=System.Text.Encoding.Unicode.GetString(_buf, _index, len);
            _index+=len;
            return s;
        }

		///
		/// PMX様式(int byte length + bytes)
		///
        public string GetUtf8Text()
        {
            int len=GetInt();
            string s=System.Text.Encoding.UTF8.GetString(_buf, _index, len);
            _index+=len;
            return s;
        }

    }
}

