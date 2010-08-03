using System.Drawing;
using Nexus;
using Satis.ModelViewer.Framework.Rendering.Effects;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering.Decorators
{
	/// <summary>
	/// // With thanks to Kyle Hayward's shadow mapping demo at
	/// http://graphicsrunner.blogspot.com/
	/// </summary>
	public class BlurComponent
	{
		//gaussian offsets and weights
		private readonly Vector4D[] _sampleOffsetsHoriz;
		private readonly Vector4D[] _sampleOffsetsVert;

		private readonly float[] _sampleWeightsHoriz;
		private readonly float[] _sampleWeightsVert;

		//this will slightly blur the depth maps. If you want a larger blur change the sample_count
		//to a higher ODD number. You also need to change the sample_count in the GuassianBlur.fx
		//shader file to the same number also
		private const int SampleCount = 5;

		private readonly Device _graphics;
		private readonly GaussianBlurEffect _effect;

		private readonly Texture _intermediateTexture;

		private Sprite _sprite;

		public int Dims;
		public Format Format;

		public Texture InputTexture;
		public Texture OutputTexture;

		public BlurComponent(Device graphics, int size)
		{
			_graphics = graphics;

			Dims = size;
			Format = Format.A8R8G8B8;

			_sampleOffsetsHoriz = new Vector4D[SampleCount];
			_sampleOffsetsVert = new Vector4D[SampleCount];

			_sampleWeightsHoriz = new float[SampleCount];
			_sampleWeightsVert = new float[SampleCount];

			int width = Dims - 5;
			int height = Dims - 5;

			SetBlurEffectParameters(1.0f / width, 0, ref _sampleOffsetsHoriz, ref _sampleWeightsHoriz);
			SetBlurEffectParameters(0, 1.0f / height, ref _sampleOffsetsVert, ref _sampleWeightsVert);

			_effect = new GaussianBlurEffect(_graphics);

			OutputTexture = new Texture(_graphics, Dims, Dims, 1, Usage.RenderTarget, Format, Pool.Default);
			_intermediateTexture = new Texture(_graphics, Dims, Dims, 1, Usage.RenderTarget, Format, Pool.Default);

			_sprite = new Sprite(_graphics);
		}

		public void Draw()
		{
			//get the scene texture
			_effect.Texture = InputTexture;

			//set the horizontal offsets and weights
			_effect.Scale = new Vector2D(1.0f / (Dims * 0.5f * 0.25f), 0.0f);		// Bluring horinzontaly

			//pass 1: blur the texture horizontally
			DrawFullscreenQuad(InputTexture, OutputTexture);

			//set the horizontal blur texture
			_effect.Texture = OutputTexture;

			//set the vertical offests and weights
			_effect.Scale = new Vector2D(0.0f, 1.0f / (Dims * 0.5f));		

			//pass 2: blur the texture vertically
			DrawFullscreenQuad(OutputTexture, _intermediateTexture);
			OutputTexture = _intermediateTexture;
		}

		private void DrawFullscreenQuad(Texture texture, Texture renderTarget)
		{
			//set the render target. 
			Surface savedRenderTarget = _graphics.GetRenderTarget(0);
			_graphics.SetRenderTarget(0, renderTarget.GetSurfaceLevel(0));
			DrawFullscreenQuad(texture);
			_graphics.SetRenderTarget(0, savedRenderTarget);
		}

		private void DrawFullscreenQuad(Texture texture)
		{
			_sprite.Begin(SpriteFlags.None);

			//begin the shader effect, only need 1 pass
			if (_effect != null)
			{
				_effect.Begin();
				_effect.BeginPass(0);
			}

			_sprite.Draw(texture, Colors.White.ToColor4());

			/*Rectangle dest = new Rectangle(0, 0, Dims, Dims);
			VertexTransformedPositionTexture[] vertices = new VertexTransformedPositionTexture[4];
			vertices[0] = new VertexTransformedPositionTexture(new Point4D(dest.Left - 0.5f, dest.Top - 0.5f, 0, 1), new Point2D(0, 0));
			vertices[1] = new VertexTransformedPositionTexture(new Point4D(dest.Right - 0.5f, dest.Top - 0.5f, 0, 1), new Point2D(1, 0));
			vertices[2] = new VertexTransformedPositionTexture(new Point4D(dest.Right - 0.5f, dest.Bottom - 0.5f, 0, 1), new Point2D(0, 1));
			vertices[3] = new VertexTransformedPositionTexture(new Point4D(dest.Right - 0.5f, dest.Bottom - 0.5f, 0, 1), new Point2D(1, 1));
			_graphics.DrawIndexedUserPrimitives(PrimitiveType.TriangleStrip, 0, 4, 2, new[] { 0, 1, 2, 3},
				Format.Index32, vertices, VertexTransformedPositionTexture.SizeInBytes);*/

			if (_effect != null)
			{
				_effect.EndPass();
				_effect.End();
			}

			_sprite.End();
		}

		/// <summary>
		/// Computes sample weightings and texture coordinate offsets
		/// for one pass of a separable gaussian blur filter.
		/// </summary>
		private static void SetBlurEffectParameters(float dx, float dy, ref Vector4D[] offsets, ref float[] weights)
		{
			// The first sample always has a zero offset.
			weights[0] = ComputeGaussian(0);
			offsets[0] = new Vector4D();

			// Maintain a sum of all the weighting values.
			float totalWeights = weights[0];

			// Add pairs of additional sample taps, positioned
			// along a line in both directions from the center.
			for (int i = 0; i < SampleCount / 2; i++)
			{
				// Store weights for the positive and negative taps.
				float weight = ComputeGaussian(i + 1);

				weights[i * 2 + 1] = weight;
				weights[i * 2 + 2] = weight;

				totalWeights += weight * 2;

				// To get the maximum amount of blurring from a limited number of
				// pixel shader samples, we take advantage of the bilinear filtering
				// hardware inside the texture fetch unit. If we position our texture
				// coordinates exactly halfway between two texels, the filtering unit
				// will average them for us, giving two samples for the price of one.
				// This allows us to step in units of two texels per sample, rather
				// than just one at a time. The 1.5 offset kicks things off by
				// positioning us nicely in between two texels.
				float sampleOffset = i * 2 + 1.5f;

				Vector4D delta = new Vector4D(dx, dy, 1.0f, 1.0f) * sampleOffset;

				// Store texture coordinate offsets for the positive and negative taps.
				offsets[i * 2 + 1] = delta;
				offsets[i * 2 + 2] = -delta;
			}

			// Normalize the list of sample weightings, so they will always sum to one.
			for (int i = 0; i < SampleCount; i++)
			{
				weights[i] /= totalWeights;
			}
		}

		/// <summary>
		/// Evaluates a single point on the gaussian falloff curve.
		/// Used for setting up the blur filter weightings.
		/// </summary>
		private static float ComputeGaussian(float n)
		{
			//theta = the blur amount
			const float theta = 4.0f;

			return ((1.0f / MathUtility.Sqrt(2 * MathUtility.PI * theta)) *
										 MathUtility.Exp(-(n * n) / (2 * theta * theta)));
		}
	}
}