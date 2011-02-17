# Meshellator

### What is this?

3D asset import library for .NET 4.0 and Silverlight 4. Supported formats are Autodesk 3DS and Lightwave OBJ.

### How do I use it?

Meshellator is designed to be extremely simple to use. The following line will import a model into a `Scene` object:

	Scene scene = MeshellatorLoader.ImportFromFile(@"Models\3ds\85-nissan-fairlady.3ds");

You can then iterate over the materials and meshes within the scene.