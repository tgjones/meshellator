using System;
using System.Collections;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;

namespace Satis.Viewer.Xna
{
	/// <summary>
	/// Summary description for Renderer.
	/// </summary>
	public class Renderer
	{
		#region Variables

		private GraphicsDevice m_pDevice;
		private BasicEffect m_pBasicEffect;
		private int m_nRotationX;
		private int m_nRotationY;
		private int m_nRotationZ;
		private int m_nMovement;
		private ArrayList m_pMeshes;

		private GraphicsArcBall m_pArcBall;
		private Matrix m_pObjectMatrix;

		#endregion

		#region Properties

		public GraphicsDevice Device
		{
			get { return m_pDevice; }
		}

		#endregion

		#region Constructor

		public Renderer(Control pControl)
		{
			// set device parameters
			PresentationParameters pPresentParams = new PresentationParameters();
			pPresentParams.DeviceWindowHandle = pControl.Handle;
			pPresentParams.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;
			pPresentParams.EnableAutoDepthStencil = true;
			pPresentParams.PresentationInterval = PresentInterval.Immediate;
			pPresentParams.SwapEffect = SwapEffect.Discard;
			pPresentParams.IsFullScreen = false;

			// create device
			m_pDevice = new GraphicsDevice(
								GraphicsAdapter.DefaultAdapter,
				DeviceType.Hardware,
				pControl.Handle,
				pPresentParams);

			m_pBasicEffect = new BasicEffect(m_pDevice, null);

			// set projection and view transforms
			m_pBasicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
	(float) (Math.PI / 4),
	pControl.Width / (float) pControl.Height,
	1.0f, 6000.0f);
			m_pBasicEffect.View = Matrix.CreateLookAt(
	new Vector3(0, 800.0f, 1500.0f),
	new Vector3(0.0f, 0.0f, 0.0f),
	new Vector3(0.0f, 1.0f, 0.0f));

			m_pDevice.RenderState.AlphaBlendEnable = true;
			m_pDevice.RenderState.SourceBlend = Blend.SourceAlpha;
			m_pDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

			m_pDevice.RenderState.DepthBufferEnable = true;

			m_pDevice.RenderState.CullMode = CullMode.None;
			m_pDevice.RenderState.FillMode = FillMode.Solid;

			/*// set up a white, directional light
			m_pBasicEffect.LightingEnabled = true;
			m_pBasicEffect.DirectionalLight0.DiffuseColor = Microsoft.Xna.Framework.Graphics.Color.White.ToVector3();
			m_pBasicEffect.DirectionalLight0.Direction = new Vector3(-2.0f, -2.0f, -2.0f);
			m_pBasicEffect.DirectionalLight0.Enabled = true;*/
			m_pBasicEffect.EnableDefaultLighting();

			m_pBasicEffect.PreferPerPixelLighting = true;

			// turn on some ambient light
			//m_pBasicEffect.AmbientLightColor = Microsoft.Xna.Framework.Graphics.Color.Gray.ToVector3();

			m_pMeshes = new ArrayList();

			m_pArcBall = new GraphicsArcBall(pControl);

			m_pArcBall.SetWindow(m_pDevice.PresentationParameters.BackBufferWidth,
				m_pDevice.PresentationParameters.BackBufferHeight, 2.0f);
			m_pArcBall.Radius = 50.0f;
		}

		#endregion

		#region Methods

		public void AddMesh(IRenderable pItem)
		{
			m_pMeshes.Add(pItem);
		}

		public void Render(bool bSolid, bool bLeft, bool bRight, bool bUp, bool bDown,
				bool bRLeft, bool bRRight, bool bZoomIn, bool bZoomOut)
		{
			// Setup viewing postion from ArcBall
			m_pObjectMatrix = m_pArcBall.RotationMatrix;
			m_pObjectMatrix = m_pObjectMatrix * m_pArcBall.TranslationMatrix;

			int nMovement = 3;
			if (bLeft) m_nRotationY -= nMovement;
			if (bRight) m_nRotationY += nMovement;
			if (bUp) m_nRotationX -= nMovement;
			if (bDown) m_nRotationX += nMovement;
			if (bRLeft) m_nRotationZ -= nMovement;
			if (bRRight) m_nRotationZ += nMovement;
			if (bZoomIn) m_nMovement += nMovement;
			if (bZoomOut) m_nMovement -= nMovement;
			Matrix tWorldMatrix = Matrix.Identity;
			tWorldMatrix *= Matrix.CreateRotationX(m_nRotationX * (float) Math.PI / 180.0f);
			tWorldMatrix *= Matrix.CreateRotationY(m_nRotationY * (float) Math.PI / 180.0f);
			tWorldMatrix *= Matrix.CreateRotationZ(m_nRotationZ * (float) Math.PI / 180.0f);
			tWorldMatrix *= Matrix.CreateTranslation(m_nMovement, 0.0f, m_nMovement);
			m_pBasicEffect.World = m_pObjectMatrix;

			m_pDevice.RenderState.FillMode = (bSolid) ? FillMode.Solid : FillMode.WireFrame;

			m_pDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Microsoft.Xna.Framework.Graphics.Color.White,
											1.0f, 0);

			foreach (IRenderable pMesh in m_pMeshes)
			{
				pMesh.Render(m_pBasicEffect);
			}

			m_pDevice.Present();
		}

		#endregion
	}

	public interface IRenderable
	{
		void Render(BasicEffect basicEffect);
	}
}
