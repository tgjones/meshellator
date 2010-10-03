using System;

namespace Meshellator.Importers.LightwaveObj.Objects
{
	public class Vertex
	{
		public float X {get;set;}
		public float Y { get; set; }
		public float Z { get; set; }

		public Vertex()
		{

		}

		public Vertex(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vertex(int i, int j)
		{
			X = i;
			Y = j;
		}

		public Vertex(Vertex position)
		{
			X = position.X;
			Y = position.Y;
			Z = position.Z;
		}

		public float Length()
		{
			return (float) Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
		}

		public void Normalize()
		{
			float length = Length();

			X /= length;
			Y /= length;
			Z /= length;
		}

		/*public double distanceFrom(Vertex to)
		{
			return Math.sqrt(this.getX() * to.getX() + this.getY() + to.getY() + this.getZ() * to.getZ());
		}
		public Vertex rotateZ(double angle)
		{
			float savedX = getX();
			x = (float) (x * Math.cos(angle) + y * Math.sin(angle));
			y = (float) (savedX * -Math.sin(angle) + y * Math.cos(angle));

			return this;

		}

		public Vertex rotateX(double angle)
		{

			// Rotation matrix on X is
			//		1		0		0
			//		0	 cos(x)	sin(x)
			//		0	-sin(x) cos(x)
			float savedY = y;
			y = (float) (y * Math.cos(angle) + z * -Math.sin(angle));
			z = (float) (savedY * Math.sin(angle) + z * Math.cos(angle));
			return this;

		}

		public Vertex copyAndRotateZ(float angle)
		{
			float newX = (float) (x * Math.cos(angle) + y * Math.sin(angle));
			float newY = (float) (x * -Math.sin(angle) + y * Math.cos(angle));

			return new Vertex(newX, newY, this.z);

		}

		public void add(Vertex offSet)
		{
			x += offSet.getX();
			y += offSet.getY();
			z += offSet.getZ();
		}


		public Vertex copyAndAdd(Vertex offSet)
		{
			return new Vertex(getX() + offSet.getX(), getY() + offSet.getY(), getZ() + offSet.getZ());
		}


		public Vertex mult(Vertex offSet)
		{
			return new Vertex(getX() * offSet.getX(), getY() * offSet.getY(), getZ() * offSet.getZ());
		}

		public Vertex mult(double factor)
		{
			return mult((float) factor);
		}

		public Vertex mult(float factor)
		{
			return new Vertex(getX() * factor, getY() * factor, getZ() * factor);
		}
		public Vertex copyAndSub(Vertex v)
		{
			// TODO Auto-generated method stub
			return new Vertex(getX() - v.getX(), getY() - v.getY(), getZ() - v.getZ());
		}

		public Vertex copyAndMult(float coef)
		{
			// TODO Auto-generated method stub
			return new Vertex(getX() * coef, getY() * coef, getZ() * coef);
		}

		public float dot(Vertex v)
		{
			return v.x * x + v.y * y;//+ v.z * z;
		}

		public float perpDot(Vertex v)
		{
			return x * v.y - y * v.x;
		}
		public void subFrom(Vertex position)
		{
			setX(position.getX() - getX());
			setY(position.getY() - getY());
			setZ(position.getZ() - getZ());
		}

		public String toString()
		{
			return "x=" + x + ",y=" + y + ",z=" + z;
		}*/
	}
}