using System;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Gemini.Modules.Output;
using Nexus;
using Nexus.Graphics.Transforms;

namespace Meshellator.Viewer.Modules.ModelEditor.Views
{
	/// <summary>
	/// /// Copied from 3D Tools, 3dtools.codeplex.com
	///     Trackball is a utility class which observes the mouse events
	///     on a specified FrameworkElement and produces a Transform3D
	///     with the resultant rotation and scale.
	/// 
	///     Example Usage:
	/// 
	///         Trackball trackball = new Trackball();
	///         trackball.EventSource = myElement;
	///         myViewport3D.Camera.Transform = trackball.Transform;
	/// 
	///     Because Viewport3Ds only raise events when the mouse is over the
	///     rendered 3D geometry (as opposed to not when the mouse is within
	///     the layout bounds) you usually want to use another element as 
	///     your EventSource.  For example, a transparent border placed on
	///     top of your Viewport3D works well:
	///     
	///         <Grid>
	///           <ColumnDefinition />
	///           <RowDefinition />
	///           <Viewport3D Name="myViewport" ClipToBounds="True" Grid.Row="0" Grid.Column="0" />
	///           <Border Name="myElement" Background="Transparent" Grid.Row="0" Grid.Column="0" />
	///         </Grid>
	///     
	///     NOTE: The Transform property may be shared by multiple Cameras
	///           if you want to have auxilary views following the trackball.
	/// 
	///           It can also be useful to share the Transform property with
	///           models in the scene that you want to move with the camera.
	///           (For example, the Trackport3D's headlight is implemented
	///           this way.)
	/// 
	///           You may also use a Transform3DGroup to combine the
	///           Transform property with additional Transforms.
	/// </summary> 
	public class Trackball
	{
		private FrameworkElement _eventSource;
		private Point _previousPosition2D;
		private Vector3D _initialPosition3D = new Vector3D(0, 0, 1);

		private Transform3DGroup _transform;
		private ScaleTransform3D _scale = new ScaleTransform3D();
		private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();

		private Quaternion _currentOrientation,_persistentOrientation;

		private bool _mouseDown;

		public Trackball()
		{
			_transform = new Transform3DGroup();
			_transform.Children.Add(_scale);
			_transform.Children.Add(new RotateTransform3D(_rotation));

			_currentOrientation = Quaternion.Identity;
			_persistentOrientation = Quaternion.Identity;

			IoC.Get<IPropertyGrid>().SelectedObject = _rotation;
		}

		/// <summary>
		///     A transform to move the camera or scene to the trackball's
		///     current orientation and scale.
		/// </summary>
		public Transform3D Transform
		{
			get { return _transform; }
		}

		#region Event Handling

		/// <summary>
		///     The FrameworkElement we listen to for mouse events.
		/// </summary>
		public FrameworkElement EventSource
		{
			get { return _eventSource; }

			set
			{
				if (_eventSource != null)
				{
					_eventSource.MouseDown -= this.OnMouseDown;
					_eventSource.MouseUp -= this.OnMouseUp;
					_eventSource.MouseMove -= this.OnMouseMove;
				}

				_eventSource = value;

				_eventSource.MouseDown += this.OnMouseDown;
				_eventSource.MouseUp += this.OnMouseUp;
				_eventSource.MouseMove += this.OnMouseMove;
			}
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			IoC.Get<IOutput>().Append("Mouse down");

			Mouse.Capture(EventSource, CaptureMode.Element);
			_previousPosition2D = e.GetPosition(EventSource);
			_initialPosition3D = ProjectToTrackball(
					EventSource.ActualWidth,
					EventSource.ActualHeight,
					_previousPosition2D);
			_currentOrientation = _persistentOrientation;

			_mouseDown = true;
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			IoC.Get<IOutput>().Append("Mouse up");
			_mouseDown = false;
			_persistentOrientation = _currentOrientation;
			Mouse.Capture(EventSource, CaptureMode.None);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			Point currentPosition = e.GetPosition(EventSource);

			if (_mouseDown)
			{
				IoC.Get<IOutput>().Append("Mouse drag");

				// Prefer tracking to zooming if both buttons are pressed.
				if (e.LeftButton == MouseButtonState.Pressed)
					Track(currentPosition);
				else if (e.RightButton == MouseButtonState.Pressed)
					Zoom(currentPosition);

				OnTransformUpdated(EventArgs.Empty);
			}

			_previousPosition2D = currentPosition;
		}

		#endregion Event Handling

		private void Track(Point currentPosition)
		{
			Vector3D currentPosition3D = ProjectToTrackball(EventSource.ActualWidth, EventSource.ActualHeight, currentPosition);

			IoC.Get<IOutput>().Append("New trackball position: " + currentPosition3D);

			// Because our sphere is centered at the origin, we may interpret our points as vectors. Doing so it is trivial
			// to find the axis and angle of rotation using the cross product dot product respectively.
			Vector3D axis = Vector3D.Cross(_initialPosition3D, currentPosition3D);
			float angle = Vector3D.AngleBetween(_initialPosition3D, currentPosition3D);

			// We negate the angle because we are rotating the camera (if we were rotating the scene, we wouldn't do this).
			Quaternion delta = new Quaternion(axis, -angle);

			// Get the initial orientation.
			Quaternion q = _persistentOrientation;

			// Compose the delta with the previous orientation
			q *= delta;

			// Store the new orientation.
			_rotation.Axis = q.Axis;
			_rotation.Angle = q.Angle;

			_currentOrientation = q;

			//_previousPosition3D = currentPosition3D;
		}

		private static Vector3D ProjectToTrackball(double width, double height, Point point)
		{
			double x = point.X / (width / 2);    // Scale so bounds map to [0,0] - [2,2]
			double y = point.Y / (height / 2);

			x = x - 1;                           // Translate 0,0 to the center
			y = 1 - y;                           // Flip so +Y is up instead of down

			double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
			double z = z2 > 0 ? Math.Sqrt(z2) : 0;

			return new Vector3D((float) x, (float) y, (float) z);
		}

		private void Zoom(Point currentPosition)
		{
			double yDelta = currentPosition.Y - _previousPosition2D.Y;

			double scale = Math.Exp(yDelta / 100);    // e^(yDelta/100) is fairly arbitrary.

			_scale.ScaleX *= (float) scale;
			_scale.ScaleY *= (float) scale;
			_scale.ScaleZ *= (float) scale;
		}

		public event EventHandler TransformUpdated;

		protected void OnTransformUpdated(EventArgs e)
		{
			EventHandler handler = TransformUpdated;
			if (handler != null) handler(this, e);
		}
	}
}