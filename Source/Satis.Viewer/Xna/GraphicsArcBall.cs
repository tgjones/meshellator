using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Satis.Viewer.Xna
{
	/// <summary>
	/// An arc ball class
	/// </summary>
	public class GraphicsArcBall
	{
		private int internalWidth;   // ArcBall's window width
		private int internalHeight;  // ArcBall's window height
		private float internalradius;  // ArcBall's radius in screen coords
		private float internalradiusTranslation; // ArcBall's radius for translating the target

		private Quaternion internaldownQuat;               // Quaternion before button down
		private Quaternion internalnowQuat;                // Composite quaternion for current drag
		private Matrix internalrotationMatrix;         // Matrix for arcball's orientation
		private Matrix internalrotationDelta;    // Matrix for arcball's orientation
		private Matrix internaltranslationMatrix;      // Matrix for arcball's position
		private Matrix internaltranslationDelta; // Matrix for arcball's position
		private bool internaldragging;               // Whether user is dragging arcball
		private bool internaluseRightHanded;        // Whether to use RH coordinate system
		private int saveMouseX = 0;      // Saved mouse position
		private int saveMouseY = 0;
		private Vector3 internalvectorDown;         // Button down vector
		System.Windows.Forms.Control parent; // parent




		/// <summary>
		/// Constructor
		/// </summary>
		public GraphicsArcBall(System.Windows.Forms.Control p)
		{
			internaldownQuat = Quaternion.Identity;
			internalnowQuat = Quaternion.Identity;
			internalrotationMatrix = Matrix.Identity;
			internalrotationDelta = Matrix.Identity;
			internaltranslationMatrix = Matrix.Identity;
			internaltranslationDelta = Matrix.Identity;
			internaldragging = false;
			internalradiusTranslation = 1.0f;
			internaluseRightHanded = false;

			parent = p;
			// Hook the events 
			p.MouseDown += new MouseEventHandler(this.OnContainerMouseDown);
			p.MouseUp += new MouseEventHandler(this.OnContainerMouseUp);
			p.MouseMove += new MouseEventHandler(this.OnContainerMouseMove);
		}




		/// <summary>
		/// Set the window dimensions
		/// </summary>
		public void SetWindow(int width, int height, float radius)
		{
			// Set ArcBall info
			internalWidth = width;
			internalHeight = height;
			internalradius = radius;
		}




		/// <summary>
		/// Screen coords to a vector
		/// </summary>
		private Vector3 ScreenToVector(int xpos, int ypos)
		{
			// Scale to screen
			float x = -(xpos - internalWidth / 2) / (internalradius * internalWidth / 2);
			float y = (ypos - internalHeight / 2) / (internalradius * internalHeight / 2);

			if (internaluseRightHanded)
			{
				x = -x;
				y = -y;
			}

			float z = 0.0f;
			float mag = x * x + y * y;

			if (mag > 1.0f)
			{
				float scale = 1.0f / (float) Math.Sqrt(mag);
				x *= scale;
				y *= scale;
			}
			else
				z = (float) Math.Sqrt(1.0f - mag);

			// Return vector
			return new Vector3(x, y, z);
		}




		/// <summary>
		/// Fired when the containers mouse button is down
		/// </summary>
		private void OnContainerMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Store off the position of the cursor when the button is pressed
			saveMouseX = e.X;
			saveMouseY = e.Y;

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				// Start drag mode
				internaldragging = true;
				internalvectorDown = ScreenToVector(e.X, e.Y);
				internaldownQuat = internalnowQuat;
			}
		}




		/// <summary>
		/// Fired when the containers mouse button has been released
		/// </summary>
		private void OnContainerMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				// End drag mode
				internaldragging = false;
			}
		}




		/// <summary>
		/// Fired when the containers mouse is moving
		/// </summary>
		private void OnContainerMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (internaldragging)
				{
					// recompute nowQuat
					Vector3 vCur = ScreenToVector(e.X, e.Y);
					Quaternion qAxisToAxis = D3DXQuaternionAxisToAxis(internalvectorDown, vCur);
					internalnowQuat = internaldownQuat;
					internalnowQuat = Quaternion.Multiply(internalnowQuat, qAxisToAxis);
					internalrotationDelta = Matrix.CreateFromQuaternion(qAxisToAxis);
				}
				else
					internalrotationDelta = Matrix.Identity;

				internalrotationMatrix = Matrix.CreateFromQuaternion(internalnowQuat);
				internaldragging = true;
			}

			if ((e.Button == System.Windows.Forms.MouseButtons.Right) || (e.Button == System.Windows.Forms.MouseButtons.Middle))
			{
				// Normalize based on size of window and bounding sphere radius
				float fDeltaX = (saveMouseX - e.X) * internalradiusTranslation / internalWidth;
				float fDeltaY = (saveMouseY - e.Y) * internalradiusTranslation / internalHeight;

				if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					internaltranslationDelta = Matrix.CreateTranslation(-2 * fDeltaX, 2 * fDeltaY, 0.0f);
					internaltranslationMatrix = Matrix.Multiply(internaltranslationMatrix, internaltranslationDelta);
				}
				if (e.Button == System.Windows.Forms.MouseButtons.Middle)
				{
					internaltranslationDelta = Matrix.CreateTranslation(0.0f, 0.0f, 5 * fDeltaY);
					internaltranslationMatrix = Matrix.Multiply(internaltranslationMatrix, internaltranslationDelta);
				}

				// Store mouse coordinate
				saveMouseX = e.X;
				saveMouseY = e.Y;
			}
		}

		#region Various properties of the class
		public float Radius
		{
			set
			{ internalradiusTranslation = value; }
		}
		public bool RightHanded
		{
			get { return internaluseRightHanded; }
			set { internaluseRightHanded = value; }
		}
		public Matrix RotationMatrix
		{
			get { return internalrotationMatrix; }
		}
		public Matrix RotationDeltaMatrix
		{
			get { return internalrotationDelta; }
		}
		public Matrix TranslationMatrix
		{
			get { return internaltranslationMatrix; }
		}
		public Matrix TranslationDeltaMatrix
		{
			get { return internaltranslationDelta; }
		}
		public bool IsBeingDragged
		{
			get { return internaldragging; }
		}
		#endregion

		/// <summary>
		/// Axis to axis quaternion double angle (no normalization)
		/// Takes two points on unit sphere an angle THETA apart, returns
		/// quaternion that represents a rotation around cross product by 2*THETA.
		/// </summary>
		public static Quaternion D3DXQuaternionUnitAxisToUnitAxis2(Vector3 fromVector, Vector3 toVector)
		{
			Vector3 axis = Vector3.Cross(fromVector, toVector);    // proportional to sin(theta)
			return new Quaternion(axis.X, axis.Y, axis.Z, Vector3.Dot(fromVector, toVector));
		}




		/// <summary>
		/// Axis to axis quaternion 
		/// Takes two points on unit sphere an angle THETA apart, returns
		/// quaternion that represents a rotation around cross product by theta.
		/// </summary>
		public static Quaternion D3DXQuaternionAxisToAxis(Vector3 fromVector, Vector3 toVector)
		{
			Vector3 vA = Vector3.Normalize(fromVector), vB = Vector3.Normalize(toVector);
			Vector3 vHalf = Vector3.Add(vA, vB);
			vHalf = Vector3.Normalize(vHalf);
			return D3DXQuaternionUnitAxisToUnitAxis2(vA, vHalf);
		}
	}

}
