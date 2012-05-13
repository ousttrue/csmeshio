using System;
using System.IO;
using System.Linq;


namespace meshio.Vmd {

    class VmdException : System.ApplicationException
    {
        public VmdException(string msg):base("VmdException: " + msg)
        {
        }
    }

	public struct BezierParam
	{
		public byte x1;
		public byte y1;
		public byte x2;
		public byte y2;
	}

	public struct BoneMotion
	{
		public string BoneName;
		public uint FrameNum;
		public Vector3 Position;
		public Vector4 Rotation;

		public BezierParam InterpolateX;
		public BezierParam InterpolateY;
		public BezierParam InterpolateZ;
		public BezierParam InterpolateW;
	}

	public class Model
	{
		public string Magic;
		public string ModelName;

		public BoneMotion[] BoneMotions;
	}

    public class Loader
    {
		private static BoneMotion GetBoneMotion(ByteReader io)
		{
			// ?
			var motion=new BoneMotion();

			motion.BoneName=io.GetSJIS(15);
			motion.FrameNum=io.GetUInt();
			motion.Position=io.GetVector3();
			motion.Rotation=io.GetVector4();

			var bezierParams=new byte[64];
			io.GetBytes(ref bezierParams);

			return motion;
		}

        public static Model loadFromPath(string path)
        {
            var io=new ByteReader(File.ReadAllBytes(path));

			var model=new Model();

			model.Magic=io.GetAscii(30);
			model.ModelName=io.GetSJIS(20);

			int boneMotionCount=io.GetInt();
            model.BoneMotions=Enumerable.Range(1, boneMotionCount).Select(_
                    =>GetBoneMotion(io)).ToArray();

			return model;
        }
    }
}

