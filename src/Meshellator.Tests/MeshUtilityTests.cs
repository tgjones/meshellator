using Nexus;
using NUnit.Framework;

namespace Meshellator.Tests
{
	[TestFixture]
	public class MeshUtilityTests
	{
		[Test]
		public void CanConvertTriangleStripToTriangleList()
		{
			// Arrange.
			Int32Collection indices = new Int32Collection(new[] { 0, 3, 1, 4, 2, 5 });

			// Act.
			Int32Collection newIndices = MeshUtility.ConvertTriangleStripToTriangleList(indices);

			// Assert.
			CollectionAssert.AreEqual(new[] { 0, 3, 1, 1, 3, 4, 1, 4, 2, 2, 4, 5 }, newIndices);
		}
	}
}