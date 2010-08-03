using System.Collections.Generic;
using Nexus;
using SlimDX.Direct3D9;

namespace Satis.ModelViewer.Framework.Rendering.Decorators
{
	internal class ShadowDecorator : IDecorator
	{
		private const int ShadowMapSize = 1024;

		private readonly Device _device;
		private Matrix3D _lightViewProjection;
		private readonly BoundingFrustum _cameraFrustum;

		private bool _creatingShadowMap;

		private Surface _savedDepthBuffer;
		private Surface _savedBackBuffer;
		private Texture _shadowRenderTarget;
		private readonly Surface _shadowDepthBuffer;

		private readonly Sprite _shadowMapSprite;

		private readonly BlurComponent _blur;

		public ShadowDecorator(Device device)
		{
			_device = device;
			_cameraFrustum = new BoundingFrustum(Matrix3D.Identity);

			// Create new floating point render target
			_shadowRenderTarget = new Texture(device, ShadowMapSize, ShadowMapSize, 1,
				Usage.RenderTarget, Format.G32R32F, Pool.Default);

			// Create depth buffer to use when rendering to the shadow map
			_shadowDepthBuffer = Surface.CreateDepthStencil(_device, ShadowMapSize, ShadowMapSize,
				Format.D24X8, MultisampleType.None, 0, true);

			_shadowMapSprite = new Sprite(device);

			_blur = new BlurComponent(_device, ShadowMapSize);
		}

		public bool IsActive(RenderSettings settings)
		{
			return settings.Parameters.ShowShadows;
		}

		public void OnBeginDrawModel(Model model, RenderSettings renderSettings, IEnumerable<IDecorator> decorators)
		{
			if (_creatingShadowMap)
				return;

			_creatingShadowMap = true;

			_cameraFrustum.Matrix = renderSettings.ViewMatrix * renderSettings.ProjectionMatrix;

			// Update the lights ViewProjection matrix based on the 
			// current camera frustum
			_lightViewProjection = CreateLightViewProjectionMatrix(model, renderSettings.Parameters.LightDirection);

			// Save the current back buffer.
			_savedBackBuffer = _device.GetRenderTarget(0);
			// Set our render target to our floating point render target
			_device.SetRenderTarget(0, _shadowRenderTarget.GetSurfaceLevel(0));
			// Save the current stencil buffer
			_savedDepthBuffer = _device.DepthStencilSurface;
			// Set the graphics device to use the shadow depth stencil buffer
			_device.DepthStencilSurface = _shadowDepthBuffer;

			// Clear the render target to white or all 1's
			// We set the clear to white since that represents the 
			// furthest the object could be away
			_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new SlimDX.Color4(1, 1, 1), 1, 0);

			model.DrawInternal(renderSettings, decorators);

			_creatingShadowMap = false;

			// Now blur shadow map.
			//_blur.InputTexture = _shadowRenderTarget;
			//_blur.Draw();
			//_shadowRenderTarget = _blur.OutputTexture;

			// Set render target back to the back buffer
			_device.SetRenderTarget(0, _savedBackBuffer);
			// Reset the depth buffer
			_device.DepthStencilSurface = _savedDepthBuffer;

			//_shadowRenderTarget.GenerateMipSublevels();
		}

		/// <summary>
		/// Creates the WorldViewProjection matrix from the perspective of the 
		/// light using the cameras bounding frustum to determine what is visible 
		/// in the scene.
		/// </summary>
		/// <returns>The WorldViewProjection for the light</returns>
		private static Matrix3D CreateLightViewProjectionMatrix(Model model, Vector3D lightDirection)
		{
			// Matrix with that will rotate in points the direction of the light
			Matrix3D lightRotation = Matrix3D.CreateLookAt(Point3D.Zero, -lightDirection, Vector3D.Up);

			/*// Get the corners of the frustum
			Point3D[] frustumCorners = _cameraFrustum.GetCorners();

			// Transform the positions of the corners into the direction of the light
			for (int i = 0; i < frustumCorners.Length; i++)
				frustumCorners[i] = Point3D.Transform(frustumCorners[i], lightRotation);*/

			// Find the smallest box around the points
			//AxisAlignedBoundingBox lightBox = new AxisAlignedBoundingBox(frustumCorners);
			AxisAlignedBoundingBox lightBox = model.SourceScene.Bounds;
			lightBox.Transform(lightRotation);

			//lightBox = lightBox.Transform(Matrix3D.CreateScale(2f));

			Vector3D boxSize = lightBox.Max - lightBox.Min;
			Vector3D halfBoxSize = boxSize * 0.5f;

			// The position of the light should be in the center of the back
			// pannel of the box. 
			Point3D lightPosition = lightBox.Min + halfBoxSize;
			lightPosition.Z = lightBox.Min.Z;

			// We need the position back in world coordinates so we transform 
			// the light position by the inverse of the lights rotation
			lightPosition = Point3D.Transform(lightPosition, Matrix3D.Invert(lightRotation));

			// Create the view matrix for the light
			Matrix3D lightView = Matrix3D.CreateLookAt(lightPosition, -lightDirection, Vector3D.Up);

			// Create the projection matrix for the light
			// The projection is orthographic since we are using a directional light
			Matrix3D lightProjection = Matrix3D.CreateOrthographic(boxSize.X * 2, boxSize.Y * 2, -boxSize.Z, boxSize.Z);

			return lightView * lightProjection;
		}

		public void OnEndDrawModel(Model model, RenderSettings renderSettings)
		{
			//DrawShadowMapToScreen();
		}

		public void OnBeginDrawMesh(ModelMesh mesh, RenderSettings renderSettings)
		{
			mesh.Effect.LightViewProjection = _lightViewProjection;
			if (_creatingShadowMap)
			{
				mesh.Effect.CurrentTechnique = "RenderShadowMap";
			}
			else
			{
				mesh.Effect.CurrentTechnique = "RenderScene";
				mesh.Effect.ShadowsEnabled = true;
				mesh.Effect.ShadowMap = _shadowRenderTarget;
				mesh.Effect.ShadowMapSize = ShadowMapSize;
			}
		}

		public void OnEndDrawMesh(ModelMesh mesh, RenderSettings renderSettings)
		{
			if (!_creatingShadowMap)
			{
				mesh.Effect.ShadowsEnabled = false;
				mesh.Effect.ShadowMap = null;
			}
		}

		/// <summary>
		/// Render the shadow map texture to the screen
		/// </summary>
		private void DrawShadowMapToScreen()
		{
			const float scale = 0.3f;
			_shadowMapSprite.Transform = SlimDX.Matrix.Scaling(scale, scale, scale);
			_shadowMapSprite.Begin(SpriteFlags.None);
			_shadowMapSprite.Draw(_shadowRenderTarget, new SlimDX.Color4(1, 1, 1, 1));
			_shadowMapSprite.End();

			//BaseTexture.ToFile(_shadowRenderTarget, "SavedShadowMap.png", ImageFileFormat.Png);
		}
	}
}