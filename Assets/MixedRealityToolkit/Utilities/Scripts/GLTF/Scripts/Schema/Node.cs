using UnityEngine;
using System.Collections.Generic;
using GLTF.JsonExtensions;
using Newtonsoft.Json;

namespace GLTF
{
    /// <summary>
    /// A node in the node hierarchy.
    /// When the node contains `skin`, all `mesh.primitives` must contain `JOINT`
    /// and `WEIGHT` attributes.  A node can have either a `matrix` or any combination
    /// of `translation`/`rotation`/`scale` (TRS) properties.
    /// TRS properties are converted to matrices and postmultiplied in
    /// the `T * R * S` order to compose the transformation matrix;
    /// first the scale is applied to the vertices, then the rotation, and then
    /// the translation. If none are provided, the transform is the identity.
    /// When a node is targeted for animation
    /// (referenced by an animation.channel.target), only TRS properties may be present;
    /// `matrix` will not be present.
    /// </summary>
    public class Node : GLTFChildOfRootProperty
    {

        private bool _useTRS;

        /// <summary>
        /// The index of the camera referenced by this node.
        /// </summary>
        public CameraId Camera;

        /// <summary>
        /// The indices of this node's children.
        /// </summary>
        public List<NodeId> Children;

        /// <summary>
        /// The index of the skin referenced by this node.
        /// </summary>
        public SkinId Skin;

        /// <summary>
        /// A floating-point 4x4 transformation matrix stored in column-major order.
        /// </summary>
        public Matrix4x4 Matrix = Matrix4x4.identity;

        /// <summary>
        /// The index of the mesh in this node.
        /// </summary>
        public MeshId Mesh;

        /// <summary>
        /// The node's unit quaternion rotation in the order (x, y, z, w),
        /// where w is the scalar.
        /// </summary>
        public Quaternion Rotation = new Quaternion(0, 0, 0, 1);

        /// <summary>
        /// The node's non-uniform scale.
        /// </summary>
        public Vector3 Scale = Vector3.one;

        /// <summary>
        /// The node's translation.
        /// </summary>
        public Vector3 Translation = Vector3.zero;

        /// <summary>
        /// The weights of the instantiated Morph Target.
        /// Number of elements must match number of Morph Targets of used mesh.
        /// </summary>
        public List<double> Weights;

        public void GetUnityTRSProperties(out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            Vector3 localPosition, localScale;
            Quaternion localRotation;

            if (!_useTRS)
            {
                GetTRSProperties(Matrix, out localPosition, out localRotation, out localScale);
            }
            else
            {
                localPosition = Translation;
                localRotation = Rotation;
                localScale = Scale;
            }

            position = new Vector3(localPosition.x, localPosition.y, -localPosition.z);
            rotation = new Quaternion(-localRotation.x, -localRotation.y, localRotation.z, localRotation.w);
            scale = new Vector3(localScale.x, localScale.y, localScale.z);
        }

        public void SetUnityTransform(Transform transform)
        {
            Translation.Set(transform.localPosition.x, transform.localPosition.y, -transform.localPosition.z);
            Rotation.Set(-transform.localRotation.x, -transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
            Scale.Set(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        private void GetTRSProperties(Matrix4x4 mat, out Vector3 position, out Quaternion rotation, out Vector3 scale)
        {
            position = mat.GetColumn(3);

            scale = new Vector3(
                mat.GetColumn(0).magnitude,
                mat.GetColumn(1).magnitude,
                mat.GetColumn(2).magnitude
            );

#if UNITY_2017_2_OR_NEWER
            rotation = mat.rotation;
#else
            rotation = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
#endif
        }

        public static Node Deserialize(GLTFRoot root, JsonReader reader)
        {
            var node = new Node();

            while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
            {
                var curProp = reader.Value.ToString();

                switch (curProp)
                {
                    case "camera":
                        node.Camera = CameraId.Deserialize(root, reader);
                        break;
                    case "children":
                        node.Children = NodeId.ReadList(root, reader);
                        break;
                    case "skin":
                        node.Skin = SkinId.Deserialize(root, reader);
                        break;
                    case "matrix":
                        var list = reader.ReadDoubleList();
                        var mat = new Matrix4x4();
                        for (var i = 0; i < 16; i++)
                        {
                            mat[i] = (float)list[i];
                        }
                        node.Matrix = mat;
                        break;
                    case "mesh":
                        node.Mesh = MeshId.Deserialize(root, reader);
                        break;
                    case "rotation":
                        node._useTRS = true;
                        node.Rotation = reader.ReadAsQuaternion();
                        break;
                    case "scale":
                        node._useTRS = true;
                        node.Scale = reader.ReadAsVector3();
                        break;
                    case "translation":
                        node._useTRS = true;
                        node.Translation = reader.ReadAsVector3();
                        break;
                    case "weights":
                        node.Weights = reader.ReadDoubleList();
                        break;
                    default:
                        node.DefaultPropertyDeserializer(root, reader);
                        break;
                }
            }

            return node;
        }

        public override void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (Camera != null)
            {
                writer.WritePropertyName("camera");
                writer.WriteValue(Camera.Id);
            }

            if (Children != null && Children.Count > 0)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (var child in Children)
                {
                    writer.WriteValue(child.Id);
                }
                writer.WriteEndArray();
            }

            if (Skin != null)
            {
                writer.WritePropertyName("skin");
                writer.WriteValue(Skin.Id);
            }

            if (Matrix != Matrix4x4.identity)
            {
                writer.WritePropertyName("matrix");
                writer.WriteStartArray();
                for (var i = 0; i < 16; i++)
                {
                    writer.WriteValue(Matrix[i]);
                }
                writer.WriteEndArray();
            }

            if (Mesh != null)
            {
                writer.WritePropertyName("mesh");
                writer.WriteValue(Mesh.Id);
            }

            if (Rotation != Quaternion.identity)
            {
                writer.WritePropertyName("rotation");
                writer.WriteStartArray();
                writer.WriteValue(Rotation.x);
                writer.WriteValue(Rotation.y);
                writer.WriteValue(Rotation.z);
                writer.WriteValue(Rotation.w);
                writer.WriteEndArray();
            }

            if (Scale != Vector3.one)
            {
                writer.WritePropertyName("scale");
                writer.WriteStartArray();
                writer.WriteValue(Scale.x);
                writer.WriteValue(Scale.y);
                writer.WriteValue(Scale.z);
                writer.WriteEndArray();
            }

            if (Translation != Vector3.zero)
            {
                writer.WritePropertyName("translation");
                writer.WriteStartArray();
                writer.WriteValue(Translation.x);
                writer.WriteValue(Translation.y);
                writer.WriteValue(Translation.z);
                writer.WriteEndArray();
            }

            if (Weights != null && Weights.Count > 0)
            {
                writer.WritePropertyName("weights");
                writer.WriteStartArray();
                foreach (var weight in Weights)
                {
                    writer.WriteValue(weight);
                }
                writer.WriteEndArray();
            }

            base.Serialize(writer);

            writer.WriteEndObject();
        }
    }
}
