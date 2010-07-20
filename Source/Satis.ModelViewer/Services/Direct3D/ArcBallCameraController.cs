using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Nexus;
using Nexus.Graphics.Transforms;

namespace Satis.ModelViewer.Services.Direct3D
{
	/// <summary>
	/// With thanks to http://viewport3d.com/trackball.htm.
	/// An arcball camera controller translates 2D mouse movements into 3D rotations. This is done by projecting
	/// the mouse position on to an imaginary sphere behind the viewport. As the mouse is moved, the camera is rotated
	/// to keep the same point on the sphere underneath the mouse pointer.
	/// 
	/// Also used Graphics Gems IV, Section III Part 1 - Arcball Rotation Control by Ken Shoemake.
	/// Google Books is your friend :)
	/// 
	/// http://www.tecgraf.puc-rio.br/~mgattass/fcg/material/shoemake92.pdf is probably the best free
	/// description I found.
	/// 
	/// http://www.autodeskresearch.com/publications/viewcube
	///  descibes Autodesk's ViewCube implementation.
	/// 
	/// TODO: Implement "ViewCube" by using Cursor.SetPosition and hiding cursor, then using relative
	/// movement to have infinite arcball rotation.
	/// </summary>
	public class ArcBallCameraController
	{
		private readonly RenderWindow _renderWindow;
		private readonly FrameworkElement _eventSource;
		private readonly AxisAngleRotation _rotation;
		private Quaternion _persistentOrientation;
		private Quaternion _currentOrientation;
		private Point3D _initialPosition3D = new Point3D(0, 0, 1);
		private bool _mouseDown;

		public event EventHandler CameraTransformed;

		private void OnCameraTransformed(EventArgs e)
		{
			EventHandler handler = CameraTransformed;
			if (handler != null) handler(this, e);
		}

		public ArcBallCameraController(RenderWindow renderWindow, FrameworkElement eventSource)
		{
			_renderWindow = renderWindow;
			_eventSource = eventSource;
			eventSource.MouseDown += OnMouseDown;
			eventSource.MouseUp += OnMouseUp;
			eventSource.MouseMove += OnMouseMove;

			_rotation = new AxisAngleRotation();
			Transform = new RotateTransform { Rotation = _rotation };

			_persistentOrientation = Quaternion.Identity;
		}

		public Transform3D Transform { get; private set; }

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
#if !SILVERLIGHT
			Trace.WriteLine("Mouse Down");
#endif

			_mouseDown = true;

			Mouse.Capture(_eventSource, CaptureMode.Element);
			Point2D initialPosition2D = ToApolloPoint(e.GetPosition(_eventSource));
			_initialPosition3D = ProjectToArcBall(initialPosition2D);
			_currentOrientation = _persistentOrientation;
		}

		private static Point2D ToApolloPoint(Point point)
		{
			return new Point2D((float) point.X, (float) point.Y);
		}

		/// <summary>
		/// On each mouse move event, we need to calculate a rotation to keep the same point under the mouse pointer.
		/// There are two steps:
		/// (1) Figure out what point on the sphere is under the mouse pointer.
		/// (2) Compute the rotation required to transform the old point onto the new point.
		/// </summary>
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			//Trace.WriteLine("Mouse Move; MouseDown = " + _mouseDown);

			if (!_mouseDown)
				return;

			Point2D currentPosition2D = ToApolloPoint(e.GetPosition(_eventSource));
			Track(currentPosition2D);

			OnCameraTransformed(EventArgs.Empty);
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			Trace.WriteLine("Mouse Up");

			_mouseDown = false;

			_persistentOrientation = _currentOrientation;
			Mouse.Capture(_eventSource, CaptureMode.None);
		}

		/// <summary>
		/// In order to find the point on the sphere under the mouse pointer, we need to project the 2D point on to the sphere
		/// inscribed in the viewport.
		/// </summary>
		private Point3D ProjectToArcBall(Point2D screenPoint)
		{
			// Scale bounds to [0,0] - [2,2]
			float x = screenPoint.X / (_renderWindow.Width / 2.0f);
			float y = screenPoint.Y / (_renderWindow.Height / 2.0f);

			// Translate [0,0] to the centre.
			x = x - 1;

			// Flip so +y is up instead of down.
			y = 1 - y;

			// Now that we've found our x and y position on the sphere, we can find z. Since our imaginary sphere is
			// of radius 1 we know that Pythagoras equation is going to evaluate to 1.
			float z2 = 1 - (x * x) - (y * y);
			float z = (z2 > 0) ? MathUtility.Sqrt(z2) : 0;

			// We now have the (x,y,z) coordinates of the point on the sphere beneath the mouse pointer.
			Vector3D p = new Vector3D(x, y, z);
			p.Normalize();

			return (Point3D) p;
		}

		/// <summary>
		/// On each mouse move, we want to construct a rotation that will keep the same point on the sphere underneath
		/// the mouse pointer. We do this by remembering the initial point on the sphere from when the mouse started
		/// dragging, and constructing a rotation that will transform it to the point currently under the mouse
		/// pointer.
		/// 
		/// We will need two things:
		/// (1) The axis of rotation.
		/// (2) The angle of rotation (theta).
		/// </summary>
		private void Track(Point2D currentPosition2D)
		{
			Vector3D currentPosition3D = (Vector3D) ProjectToArcBall(currentPosition2D);

			// Because our sphere is centered at the origin, we may interpret our points as vectors. Doing so it is trivial
			// to find the axis and angle of rotation using the cross product dot product respectively.
			Vector3D axis = Vector3D.Cross((Vector3D) _initialPosition3D, currentPosition3D);
			float theta = Vector3D.AngleBetween((Vector3D) _initialPosition3D, currentPosition3D);

			// Once we have the axis and angle, all that remains is to apply the new rotation to the current orientation.

			// We negate the angle because we are rotating the camera (if we were rotating the scene, we wouldn't do this).
			Quaternion delta = new Quaternion(axis, -theta);

			// Get the initial orientation.
			Quaternion q = _persistentOrientation;

			// Compose the delta with the initial orientation.
			q *= delta;

			// Store the new orientation.
			_rotation.Axis = q.Axis;
			_rotation.Angle = q.Angle;

			_currentOrientation = q;

			Trace.WriteLine("ArcBall Angle = " + q.Angle + "; Axis = " + q.Axis);
		}
	}
}