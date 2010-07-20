using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satis.Importers.Autodesk3ds;
using Satis.Viewer.Xna;
using Model = Satis.Viewer.Xna.Model;

namespace Satis.Viewer
{
	public partial class Form1 : Form
	{
		private Renderer m_pRenderer;
		private bool m_bSolid = true;
		private bool m_bLeft, m_bRight;
		private bool m_bUp, m_bDown;
		private bool m_bRLeft, m_bRRight;
		private bool m_bZoomIn, m_bZoomOut;

		public Form1()
		{
			InitializeComponent();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
			{
				string sFileName = openFileDialog1.FileName;

				FileStream fileStream = File.OpenRead(sFileName);
				Autodesk3dsImporter importer = new Autodesk3dsImporter();
				Scene scene = importer.ImportFile(fileStream, null);
				fileStream.Close();
				m_pRenderer = new Renderer(panel1);
				ConvertToModels(scene);
				timer1.Enabled = true;
			}
		}

		private void ConvertToModels(Scene scene)
		{
			foreach (Mesh mesh in scene.Meshes)
			{
				VertexBuffer vertexBuffer = new VertexBuffer(m_pRenderer.Device, typeof(VertexPositionNormalTexture), mesh.Positions.Count, BufferUsage.WriteOnly);
				VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[mesh.Positions.Count];
				for (int i = 0; i < vertices.Length; ++i)
					vertices[i] = new VertexPositionNormalTexture(mesh.Positions[i].ToVector3(), Vector3.Up, Vector2.Zero);
				//vertices[i] = new VertexPositionNormalTexture(mesh.Positions[i].ToVector3(), Vector3.Up, mesh.TextureCoordinates[i].ToVector2());
				vertexBuffer.SetData(vertices);

				VertexDeclaration vertexDeclaration = new VertexDeclaration(m_pRenderer.Device, VertexPositionNormalTexture.VertexElements);

				IndexBuffer indexBuffer = new IndexBuffer(m_pRenderer.Device, typeof(int), mesh.Indices.Count, BufferUsage.WriteOnly);
				indexBuffer.SetData(mesh.Indices.ToArray());

				Subset subset = new Subset();
				subset.FaceCount = mesh.Indices.Count / 3;
				subset.FaceStart = 0;
				subset.Material = scene.Materials.Single(m => m.Name == mesh.MaterialName);

				m_pRenderer.AddMesh(new Model(m_pRenderer.Device, vertexBuffer, vertices.Length, vertexDeclaration, indexBuffer, new [] { subset }));
			}
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if (m_pRenderer != null)
			{
				m_pRenderer.Render(m_bSolid, m_bLeft, m_bRight, m_bUp, m_bDown,
					m_bRLeft, m_bRRight, m_bZoomIn, m_bZoomOut);
				Application.DoEvents();
			}
		}

		private void solidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_bSolid = true;
			solidToolStripMenuItem.Checked = true;
			wireframeToolStripMenuItem.Checked = false;
		}

		private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			m_bSolid = false;
			solidToolStripMenuItem.Checked = false;
			wireframeToolStripMenuItem.Checked = true;
		}
	}
}
