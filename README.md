# Lockless rendering of procedural quads on the GPU with Unity

[![Video](https://user-images.githubusercontent.com/4514574/91391266-e8848400-e839-11ea-8404-d1d96f3d04fc.png)](https://streamable.com/e/7acmup)  
Click image above to open video

**This is a small showcase of how to do lockless rendering on the GPU through the use of Compute shaders.**  
**16 million quads (100 million vertices generated per frame) @ 60FPS on a RTX 2070**

Pretty much how GPU Particles are done, but they can be much more versatile.  
Probably not very useful as it stands, more of a benchmark to see how hard it is possible to push the GPU.

**Important:**  
should work on all render pipelines (Built-In, URP and HDRP)
width and height needs to be set before entering playmode.  
There is no culling or depth sorting, everything is drawn in the order they are in the buffer  
