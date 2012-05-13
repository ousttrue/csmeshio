using System;
using System.IO;
using System.Linq;


namespace meshio.Pmx {

    class PmxException : System.ApplicationException
    {
        public PmxException(string msg):base("PmxException: " + msg)
        {
        }
    }

    enum TEXT_ENCODING
    {
        UTF16,
        UTF8,
    };

    public enum VERTEX_DEFORM
    {
        BDEF1,
        BDEF2,
        BDEF4,
        SDEF,
    };

    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        // ToDo: additional_uv
        public VERTEX_DEFORM DeformType;
        public int[] BoneIndices;
        public float[] BoneWeights;
        public float EdgeFactor;
    }

	public enum SPHERE_MODE
	{
		SPHERE_NONE,
		SPHERE_SPH,
		SPHERE_ADD,
	}

	public class Material
	{
		public string Name;
		public string EnglishName;
		public Vector4 Diffuse;
		public Vector3 Specular;
		public float SpecularFactor;
		public Vector3 Ambient;
		public byte Flag;
		public Vector4 EdgeColor;
		public float EdgeSize;
		public int TextureIndex;
		public int SphereTextureIndex;
		public SPHERE_MODE SphereMode;
		public bool UseSharedToon;
		public int ToonTextureIndex;
		public string Memo;
		public int IndexCount;
	}

	public class IKLink
	{
		public int BoneIndex;
		public bool IsLimited;
		public Vector3 MinEulerRadians;
		public Vector3 MaxEulerRadians;
	}

	public class IKSolver
	{
		public int TargetIndex;
		public int Iterations;
		public float UnitRadian;
		public  IKLink[] Chains;
	}

	[Flags]
	public enum BONEFLAG
	{
		// 0-3
		HAS_TAILBONE=0x0001,
		CAN_ROTATE=0x0002,
		CAN_TRANSLATE=0x0004,
		// 4-7
		CAN_MANIPULATE=0x0010,
		HAS_IK=0x0020,
		// 8-11
		ROTATION_EFFECTED=0x0100,
		TRANSLATION_EFFECTED=0x0200,
		HAS_FIXEDAXIS=0x0400,
		HAS_LOCALAXIS=0x0800,
		// 12-15
		DEFORM_AFTER_PHYSICS=0x1000,
		DEFORM_EXTERNAL_PARENT=0x2000,
	}
	public class Bone
	{
		public string Name;
		public string EnglishName;
		public Vector3 Position;
		public int ParentIndex;
		public int Layer;
		public BONEFLAG Flags;
		public Vector3 TailOffset;
		public int TailBoneIndex;
		public int EffectIndex;
		public float EffectFactor;
		public Vector3 FixedAxis;
		public Vector3 LocalAxisX;
		public Vector3 LocalAxisZ;
		public int ExternalParentKey;
		public IKSolver IKSolver;

		public bool HasFlag(BONEFLAG flag)
		{
			return (Flags & flag)==flag;
		}
	};

    public class Model
    {
        public string Name;
        public string EnglishName;
        public string Comment;
        public string EnglishComment;

        public Vertex[] Vertices;
        public int[] Indices;
		public string[] Textures;
		public Material[] Materials;
		public Bone[] Bones;
    }

    public class Loader
    {
        private readonly Func<ByteReader, string> GetText;
        private readonly Func<ByteReader, int> GetVertexIndex;
        private readonly Func<ByteReader, int> GetTextureIndex;
        private readonly Func<ByteReader, int> GetBoneIndex;

        private Loader(
                Func<ByteReader, string> GetText,
                Func<ByteReader, int> GetVertexIndex,
                Func<ByteReader, int> GetTextureIndex,
                Func<ByteReader, int> GetBoneIndex)
        {
            this.GetText=GetText;
            this.GetVertexIndex=GetVertexIndex;
            this.GetTextureIndex=GetTextureIndex;
            this.GetBoneIndex=GetBoneIndex;
        }

        private Vertex GetVertex(ByteReader io)
        {
            Vertex vertex;
            vertex.Position=io.GetVector3();
            vertex.Normal=io.GetVector3();
            vertex.UV=io.GetVector2();
            vertex.DeformType=(VERTEX_DEFORM)io.GetByte();
            vertex.BoneIndices=new int[4];
            vertex.BoneWeights=new float[4];
            switch(vertex.DeformType)
            {
                case VERTEX_DEFORM.BDEF1:
                    vertex.BoneIndices[0]=GetBoneIndex(io);
                    break;
                case VERTEX_DEFORM.BDEF2:
                    vertex.BoneIndices[0]=GetBoneIndex(io);
                    vertex.BoneIndices[1]=GetBoneIndex(io);
                    vertex.BoneWeights[0]=io.GetFloat();
                    break;
                case VERTEX_DEFORM.BDEF4:
                    vertex.BoneIndices[0]=GetBoneIndex(io);
                    vertex.BoneIndices[1]=GetBoneIndex(io);
                    vertex.BoneIndices[2]=GetBoneIndex(io);
                    vertex.BoneIndices[3]=GetBoneIndex(io);
                    vertex.BoneWeights[0]=io.GetFloat();
                    vertex.BoneWeights[1]=io.GetFloat();
                    vertex.BoneWeights[2]=io.GetFloat();
                    vertex.BoneWeights[3]=io.GetFloat();
                    break;
                case VERTEX_DEFORM.SDEF:
                    throw new PmxException("not implemented");
            }
            vertex.EdgeFactor=io.GetFloat();
            return vertex;
        }

        private Material GetMaterial(ByteReader io)
        {
			var material=new Material();
			material.Name=GetText(io);
			material.EnglishName=GetText(io);
			material.Diffuse=io.GetVector4();
			material.Specular=io.GetVector3();
			material.SpecularFactor=io.GetFloat();
			material.Ambient=io.GetVector3();
			material.Flag=io.GetByte();
			material.EdgeColor=io.GetVector4();
			material.EdgeSize=io.GetFloat();
			material.TextureIndex=GetTextureIndex(io);
			material.SphereTextureIndex=GetTextureIndex(io);
			material.SphereMode=(SPHERE_MODE)io.GetByte();
			material.UseSharedToon=io.GetByte()==0 ? false : true;
			material.ToonTextureIndex=GetTextureIndex(io);
			material.Memo=GetText(io);
			material.IndexCount=io.GetInt();
            return material;
        }

        private Bone GetBone(ByteReader io)
        {
			var bone=new Bone();
			bone.Name=GetText(io);
			bone.EnglishName=GetText(io);
			bone.Position=io.GetVector3();
			bone.ParentIndex=GetBoneIndex(io);
			bone.Layer=io.GetInt();
			bone.Flags=(BONEFLAG)io.GetUShort();
			if(bone.HasFlag(BONEFLAG.HAS_TAILBONE)){
				bone.TailBoneIndex=GetBoneIndex(io);
			}
			else{
				bone.TailOffset=io.GetVector3();
			}
			if(bone.HasFlag(BONEFLAG.ROTATION_EFFECTED) 
					|| bone.HasFlag(BONEFLAG.TRANSLATION_EFFECTED)){
				bone.EffectIndex=GetBoneIndex(io);
				bone.EffectFactor=io.GetFloat();
			}
			if(bone.HasFlag(BONEFLAG.HAS_FIXEDAXIS)){
				bone.FixedAxis=io.GetVector3();
			}
			if(bone.HasFlag(BONEFLAG.HAS_LOCALAXIS)){
				bone.LocalAxisX=io.GetVector3();
				bone.LocalAxisZ=io.GetVector3();
			}
			if(bone.HasFlag(BONEFLAG.DEFORM_EXTERNAL_PARENT)){
				bone.ExternalParentKey=io.GetInt();
			}
			if(bone.HasFlag(BONEFLAG.HAS_IK)){
				var ik=new IKSolver();
				bone.IKSolver=ik;
				ik.TargetIndex=GetBoneIndex(io);
				ik.Iterations=io.GetInt();
				ik.UnitRadian=io.GetFloat();
				int Count=io.GetInt();
				ik.Chains=Enumerable.Range(1, Count).Select(_
						=>{
						var link=new IKLink();
						link.BoneIndex=GetBoneIndex(io);
						link.IsLimited=io.GetByte()==0 ? false : true;
						if(link.IsLimited){
						link.MinEulerRadians=io.GetVector3();
						link.MaxEulerRadians=io.GetVector3();
						}
						return link;
						}).ToArray();
			}
            return bone;
        }

        public Model load(ByteReader io)
        {
            var model=new Model();

            // モデル情報
            model.Name=GetText(io);
            model.EnglishName=GetText(io);
            model.Comment=GetText(io);
            model.EnglishComment=GetText(io);

			{
				// 頂点
				int Count=io.GetInt();
				model.Vertices=Enumerable.Range(1, Count).Select(_
						=>GetVertex(io)).ToArray();
			}

			{
				// 面
				int Count=io.GetInt();
				model.Indices=Enumerable.Range(1, Count).Select(_
						=>GetVertexIndex(io)).ToArray();
			}

			{
				// テクスチャ
				int Count=io.GetInt();
				model.Textures=Enumerable.Range(1, Count).Select(_
						=>GetText(io)).ToArray();
			}

			{
				// 材質
				int Count=io.GetInt();
				model.Materials=Enumerable.Range(1, Count).Select(_
						=>GetMaterial(io)).ToArray();
			}

			{
				// ボーン
				int Count=io.GetInt();
				model.Bones=Enumerable.Range(1, Count).Select(_
						=>GetBone(io)).ToArray();
			}

            return model;
        }

        private static Func<ByteReader, string> GetTextFunc(TEXT_ENCODING encoding)
        {
            switch(encoding)
            {
                case TEXT_ENCODING.UTF16:
                    return (ByteReader io)=>{
                        return io.GetUnicodeText();
                    };

                case TEXT_ENCODING.UTF8:
                    return (ByteReader io)=>{
                        return io.GetUtf8Text();
                    };

                default:
                    throw new PmxException("unknown encoding");
            }
        }

        private static Func<ByteReader, int> GetIndexFunc(int bytes)
        {
            switch(bytes)
            {
                case 1:
                    return (ByteReader io)=>{
                        return io.GetByte();
                    };

                case 2:
                    return (ByteReader io)=>{
                        return io.GetUShort();
                    };

                case 4:
                    return (ByteReader io)=>{
                        return io.GetInt();
                    };

                default:
                    throw new PmxException("invalid bytes");
            }
        }

        public static Model loadFromPath(string path)
        {
            var io=new ByteReader(File.ReadAllBytes(path));
            // pmx header
            var magic=io.GetAscii(4);
            if(magic!="PMX "){
                throw new PmxException("invalid magic");
            }
            var version=io.GetFloat();
            if(version!=2.0f){
                throw new PmxException("invalid version");
            }
            // flags
            int flags=io.GetByte();
            if(flags!=8){
                throw new PmxException("invalid byte");
            }
            TEXT_ENCODING encoding=(TEXT_ENCODING)io.GetByte();
            byte additional_uv=io.GetByte();
            byte vertex_index_bytes=io.GetByte();
            byte texture_index_bytes=io.GetByte();
            byte material_index_bytes=io.GetByte();
            byte bone_index_bytes=io.GetByte();
            byte morph_index_bytes=io.GetByte();
            byte rigidbody_index_bytes=io.GetByte();

            var loader=new Loader(
                    GetTextFunc(encoding),
                    GetIndexFunc(vertex_index_bytes),
                    GetIndexFunc(texture_index_bytes),
                    GetIndexFunc(bone_index_bytes)
                    );

            return loader.load(io);
        }
    }
}

