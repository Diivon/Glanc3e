using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glc.Component
{
	public abstract partial class GraphicalComponent : Component
	{
		public enum AnimationType
		{
			Single,
			Cyclic,
			PingPong
		}
		internal static string _AnimationTypeToString(AnimationType t)
		{
			switch (t)
			{
				case AnimationType.Single:
					return "::gc::AnimationType::Single";
				case AnimationType.Cyclic:
					return "::gc::AnimationType::Cyclic";
				case AnimationType.PingPong:
					return "::gc::AnimationType::PingPong";
				default:
					return "an error was occured in _AnimationTypeToString()";
			}
		}
	}
}
