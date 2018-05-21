using GLTF.Schema;
using UnityEngine;

namespace UnityGLTF.Extensions
{
    public static class SchemaExtensions
    {
        public static void GetUnityTRSProperties(this Node node, out Vector3 position, out Quaternion rotation,
            out Vector3 scale)
        {
            Vector3 localPosition, localScale;
            Quaternion localRotation;

            if (!node.UseTRS)
            {
                GetTRSProperties(node.Matrix, out localPosition, out localRotation, out localScale);
            }
            else
            {
                localPosition = node.Translation.ToUnityVector3();
                localRotation = node.Rotation.ToUnityQuaternion();
                localScale = node.Scale.ToUnityVector3();
            }

            position = new Vector3(localPosition.x, localPosition.y, -localPosition.z);
            rotation = new Quaternion(-localRotation.x, -localRotation.y, localRotation.z, localRotation.w);
            scale = new Vector3(localScale.x, localScale.y, localScale.z);
            // normally you would flip scale.z here too, but that's done in Accessor
        }

        public static void SetUnityTransform(this Node node, Transform transform)
        {
            node.Translation = new GLTF.Math.Vector3(transform.localPosition.x, transform.localPosition.y,
                -transform.localPosition.z);
            node.Rotation = new GLTF.Math.Quaternion(-transform.localRotation.x, -transform.localRotation.y,
                transform.localRotation.z, transform.localRotation.w);
            node.Scale = new GLTF.Math.Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        // todo: move to utility class
        public static void GetTRSProperties(GLTF.Math.Matrix4x4 mat, out Vector3 position, out Quaternion rotation,
            out Vector3 scale)
        {
            position = mat.GetColumn(3);

            scale = new Vector3(
                mat.GetColumn(0).magnitude,
                mat.GetColumn(1).magnitude,
                mat.GetColumn(2).magnitude
            );

            rotation = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
        }

#if false
        public static SamplerId GetSamplerId(this GLTFRoot root, UnityEngine.Texture textureObj)
        {
            for (var i = 0; i < root.Samplers.Count; i++)
            {
                bool filterIsNearest = root.Samplers[i].MinFilter == MinFilterMode.Nearest
                    || root.Samplers[i].MinFilter == MinFilterMode.NearestMipmapNearest
                    || root.Samplers[i].MinFilter == MinFilterMode.LinearMipmapNearest;

                bool filterIsLinear = root.Samplers[i].MinFilter == MinFilterMode.Linear
                    || root.Samplers[i].MinFilter == MinFilterMode.NearestMipmapLinear;

                bool filterMatched = textureObj.filterMode == FilterMode.Point && filterIsNearest
                    || textureObj.filterMode == FilterMode.Bilinear && filterIsLinear
                    || textureObj.filterMode == FilterMode.Trilinear && root.Samplers[i].MinFilter == MinFilterMode.LinearMipmapLinear;

                bool wrapMatched =
textureObj.wrapMode == TextureWrapMode.Clamp && root.Samplers[i].WrapS == GLTFSerialization.WrapMode.ClampToEdge
                    || textureObj.wrapMode == TextureWrapMode.Repeat && root.Samplers[i].WrapS != GLTFSerialization.WrapMode.ClampToEdge;

                if(filterMatched && wrapMatched)
                {
                    return new SamplerId
                    {
                        Id = i,
                        Root = root
                    };
                }
            }

            return null;
        }

        //todo blgross unity
        public static ImageId GetImageId(this GLTFRoot root, UnityEngine.Texture textureObj)
        {
            for (var i = 0; i < Images.Count; i++)
            {
                if (Images[i].Contents == textureObj)
                {
                    return new ImageId
                    {
                        Id = i,
                        Root = this
                    };
                }
            }

            return null;
        }
#endif

        public static Vector3 GetColumn(this GLTF.Math.Matrix4x4 mat, uint columnNum)
        {
            switch (columnNum)
            {
                case 0:
                    {
                        return new Vector3(mat.M11, mat.M21, mat.M31);
                    }
                case 1:
                    {
                        return new Vector3(mat.M12, mat.M22, mat.M32);
                    }
                case 2:
                    {
                        return new Vector3(mat.M13, mat.M23, mat.M33);
                    }
                case 3:
                    {
                        return new Vector3(mat.M14, mat.M24, mat.M34);
                    }
                default:
                    throw new System.Exception("column num is out of bounds");
            }
        }

        public static Vector2 ToUnityVector2(this GLTF.Math.Vector2 vec3)
        {
            return new Vector2(vec3.X, vec3.Y);
        }

        public static Vector2[] ToUnityVector2(this GLTF.Math.Vector2[] inVecArr)
        {
            Vector2[] outVecArr = new Vector2[inVecArr.Length];
            for (int i = 0; i < inVecArr.Length; ++i)
            {
                outVecArr[i] = inVecArr[i].ToUnityVector2();
            }
            return outVecArr;
        }

        public static Vector3 ToUnityVector3(this GLTF.Math.Vector3 vec3)
        {
            return new Vector3(vec3.X, vec3.Y, vec3.Z);
        }

        public static Vector3[] ToUnityVector3(this GLTF.Math.Vector3[] inVecArr)
        {
            Vector3[] outVecArr = new Vector3[inVecArr.Length];
            for (int i = 0; i < inVecArr.Length; ++i)
            {
                outVecArr[i] = inVecArr[i].ToUnityVector3();
            }
            return outVecArr;
        }

        public static Vector4 ToUnityVector4(this GLTF.Math.Vector4 vec4)
        {
            return new Vector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
        }

        public static Vector4[] ToUnityVector4(this GLTF.Math.Vector4[] inVecArr)
        {
            Vector4[] outVecArr = new Vector4[inVecArr.Length];
            for (int i = 0; i < inVecArr.Length; ++i)
            {
                outVecArr[i] = inVecArr[i].ToUnityVector4();
            }
            return outVecArr;
        }

        public static UnityEngine.Color ToUnityColor(this GLTF.Math.Color color)
        {
            return new UnityEngine.Color(color.R, color.G, color.B, color.A);
        }

        public static GLTF.Math.Color ToNumericsColor(this UnityEngine.Color color)
        {
            return new GLTF.Math.Color(color.r, color.g, color.b, color.a);
        }

        public static UnityEngine.Color[] ToUnityColor(this GLTF.Math.Color[] inColorArr)
        {
            UnityEngine.Color[] outColorArr = new UnityEngine.Color[inColorArr.Length];
            for (int i = 0; i < inColorArr.Length; ++i)
            {
                outColorArr[i] = inColorArr[i].ToUnityColor();
            }
            return outColorArr;
        }

        public static Quaternion ToUnityQuaternion(this GLTF.Math.Quaternion quaternion)
        {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }
    }
}
