using GLTF.JsonExtensions;
using Newtonsoft.Json;

namespace GLTF
{
	public enum InterpolationType
	{
		LINEAR,
		STEP
	}

	/// <summary>
	/// Combines input and output accessors with an interpolation algorithm to define a keyframe graph (but not its target).
	/// </summary>
	public class AnimationSampler : GLTFProperty
	{
		/// <summary>
		/// The index of an accessor containing keyframe input values, e.g., time.
		/// That accessor must have componentType `FLOAT`. The values represent time in
		/// seconds with `time[0] >= 0.0`, and strictly increasing values,
		/// i.e., `time[n + 1] > time[n]`
		/// </summary>
		public AccessorId Input;

		/// <summary>
		/// Interpolation algorithm. When an animation targets a node's rotation,
		/// and the animation's interpolation is `\"LINEAR\"`, spherical linear
		/// interpolation (slerp) should be used to interpolate quaternions. When
		/// interpolation is `\"STEP\"`, animated value remains constant to the value
		/// of the first point of the time-frame, until the next time-frame.
		/// </summary>
		public InterpolationType Interpolation;

		/// <summary>
		/// The index of an accessor, containing keyframe output values. Output and input
		/// accessors must have the same `count`. When sampler is used with TRS target,
		/// output accessor's componentType must be `FLOAT`.
		/// </summary>
		public AccessorId Output;

		public static AnimationSampler Deserialize(GLTFRoot root, JsonReader reader)
		{
			var animationSampler = new AnimationSampler();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "input":
						animationSampler.Input = AccessorId.Deserialize(root, reader);
						break;
					case "interpolation":
						animationSampler.Interpolation = reader.ReadStringEnum<InterpolationType>();
						break;
					case "output":
						animationSampler.Output = AccessorId.Deserialize(root, reader);
						break;
					default:
						animationSampler.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return animationSampler;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName("input");
			writer.WriteValue(Input.Id);

			if (Interpolation != InterpolationType.LINEAR)
			{
				writer.WritePropertyName("interpolation");
				writer.WriteValue(Interpolation.ToString());
			}

			writer.WritePropertyName("output");
			writer.WriteValue(Output.Id);

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
