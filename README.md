# Lockless rendering of procedural quads on the GPU

This is a small showcase of how to do lockless rendering on the GPU through the use of Compute shaders.

width and height needs to be set before entering playmode.
There is no culling or depth sorting, everything is drawn in the order they are in the buffer

Probably not very useful for most situations, more of a benchmark to see how hard it is possible to push the GPU.