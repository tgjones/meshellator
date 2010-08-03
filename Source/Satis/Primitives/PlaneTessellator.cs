using Nexus;

namespace Satis.Primitives
{
	public class PlaneTessellator : BasicPrimitiveTessellator
	{
		private readonly int _width;
		private readonly int _length;

		protected override Vector3D PositionOffset
		{
			get { return new Vector3D(-_width / 2f, 0, -_length / 2f); }
		}

		public override PrimitiveTopology PrimitiveTopology
		{
			get { return PrimitiveTopology.TriangleStrip; }
		}

		public PlaneTessellator(int width, int length)
		{
			_width = width;
			_length = length;
		}

		public override void Tessellate()
		{
			// Create vertices.
			Vector3D normal = new Vector3D(0, 1, 0);
			for (int z = 0; z < _length; ++z)
				for (int x = 0; x < _width; ++x)
					AddVertex(new Point3D(x, 0, (_length - 1) - z), normal); // Invert z so that winding order is correct.

			// Create indices.
			for (int z = 0; z < _length; ++z)
				for (int x = 0; x < _width; ++x)
				{
					// Create vertex for degenerate triangle.
					if (x == 0 && z > 0)
						AddIndex(((z + 0) * _width) + x);

					AddIndex(((z + 0) * _width) + x);
					AddIndex(((z + 1) * _width) + x);

					// Create vertex for degenerate triangle.
					if (x == _width - 1 && z < _length - 2)
						AddIndex(((z + 1) * _width) + x);
				}
		}
	}
}