using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Nexus;
using Satis.Generators;
using Satis.ModelViewer.Framework.Rendering.Effects;
using SlimDX;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering.Decorators
{
	internal class NormalsDecorator : DecoratorBase
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct VertexPositionColor
		{
			public Point3D Position;
			public Vector3D Color;

			public VertexPositionColor(Point3D position, Vector3D color)
			{
				Position = position;
				Color = color;
			}

			public static VertexElement[] VertexElements
			{
				get
				{
					return new[]
					{
						new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
						new VertexElement(0, Vector3D.SizeInBytes, DeclarationType.Float3, DeclarationMethod.Default,
							DeclarationUsage.Color, 0),
						VertexElement.VertexDeclarationEnd
					};
				}
			}

			public static int SizeInBytes
			{
				get { return Point3D.SizeInBytes + Vector3D.SizeInBytes; }
			}
		}

		private readonly Device _device;
		private readonly VertexDeclaration _lineVertexDeclaration;
		private readonly LineEffect _lineEffect;

		private readonly Dictionary<ModelMesh, NormalBuffers> _normals;

		public NormalsDecorator(Device device)
		{
			_device = device;

			_lineVertexDeclaration = new VertexDeclaration(_device, VertexPositionColor.VertexElements);

			_lineEffect = new LineEffect(device);

			_normals = new Dictionary<ModelMesh, NormalBuffers>();
		}

		private NormalBuffers GetNormalBuffers(ModelMesh mesh)
		{
			if (!_normals.ContainsKey(mesh))
			{
				NormalBuffers normalBuffers = new NormalBuffers();

				Line3D[] normalLines = NormalLinesGenerator.Generate(mesh.SourceMesh);
				normalBuffers.PrimitiveCount = normalLines.Length;
				normalBuffers.VertexCount = normalLines.Length * 2;

				VertexBuffer vertexBuffer = new VertexBuffer(_device,
					normalBuffers.VertexCount * VertexPositionColor.SizeInBytes,
					Usage.WriteOnly, VertexFormat.None, Pool.Default);
				DataStream vertexDataStream = vertexBuffer.Lock(0,
					normalBuffers.VertexCount * VertexPositionColor.SizeInBytes,
					LockFlags.None);
				VertexPositionColor[] vertices = new VertexPositionColor[normalBuffers.VertexCount];
				int counter = 0;
				for (int i = 0; i < normalLines.Length; ++i)
				{
					Vector3D normalColor = Vector3D.Normalize(normalLines[i].Direction);
					normalColor += Vector3D.One;
					normalColor *= 0.5f;
					vertices[counter++] = new VertexPositionColor(normalLines[i].Start, normalColor);
					vertices[counter++] = new VertexPositionColor(normalLines[i].End, normalColor);
				}
				vertexDataStream.WriteRange(vertices);
				vertexBuffer.Unlock();
				normalBuffers.Vertices = vertexBuffer;

				IndexBuffer indexBuffer = new IndexBuffer(_device, normalBuffers.VertexCount * sizeof(int),
					Usage.WriteOnly, Pool.Default, false);
				DataStream indexDataStream = indexBuffer.Lock(0, normalBuffers.VertexCount * sizeof(int), LockFlags.None);
				indexDataStream.WriteRange(Enumerable.Range(0, normalBuffers.VertexCount).ToArray());
				indexBuffer.Unlock();
				normalBuffers.Indices = indexBuffer;

				_normals.Add(mesh, normalBuffers);
			}
			return _normals[mesh];
		}

		public override void OnEndDrawMesh(ModelMesh mesh, RenderSettings renderSettings)
		{
			NormalBuffers normalBuffers = GetNormalBuffers(mesh);

			_device.VertexDeclaration = _lineVertexDeclaration;
			_device.SetStreamSource(0, normalBuffers.Vertices, 0, VertexPositionColor.SizeInBytes);
			_device.Indices = normalBuffers.Indices;

			_lineEffect.WorldViewProjection = renderSettings.ViewMatrix * renderSettings.ProjectionMatrix;

			int passes = _lineEffect.Begin();
			for (int pass = 0; pass < passes; ++pass)
			{
				_lineEffect.BeginPass(pass);
				_device.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
					normalBuffers.VertexCount, 0, normalBuffers.PrimitiveCount);
				_lineEffect.EndPass();
			}
		}

		public override bool IsActive(RenderSettings settings)
		{
			return settings.Parameters.ShowNormals;
		}

		private class NormalBuffers
		{
			public VertexBuffer Vertices;
			public int VertexCount;
			public IndexBuffer Indices;
			public int PrimitiveCount;
		}
	}
}