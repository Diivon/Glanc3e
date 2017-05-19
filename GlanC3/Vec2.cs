namespace Glc
{
	public class Vec2
	{
		public float x, y;
		public Vec2(float x = 0, float y = 0)
		{
			this.x = x;
			this.y = y;
		}
		/// <summary>Returns cpp code which create gc::Vec2</summary>
		/// <returns>::gc::Vec2(x, y)</returns>
		public string GetCppCtor()
		{
			return "::gc::Vec2(" + x + ", " + y + ')';
		}
	}
}
