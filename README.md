# Welding - GameDevTv 2023 Game Jam Version
The entire project along with source code, models, shaders, etc.

I'm planning to add a separate repository for the echolocation system exclusively with a better tutorial on how to use it.

Feel free to grab anything you want from the repository.

## Echolocation System
This requires Unity's post-processing stack (V2), and only works for the built-in render pipeline for now.

- [*Assets/Resources/Shaders*](Assets/Resources/Shaders): EcholocationPostProcess and HiddenNormalsTexture shaders
- [*Assets/Scripts/Effects/Echolocation*](Assets/Scripts/Effects/Echolocation): C# scripts for handling the post-processing effect, and building the sound system around the shader
