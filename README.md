# LCOS-Mods

This repository is maintained by a dedicated team through the Rensselaer Center for Open Source (RCOS). We develop mods for Lethal Company, aiming to enhance and diversify the gaming experience.

## Repository Setup

This repository is comprised of one large `.sln`, which holds many `.csproj` files which are either mods or modding tools. The `Postbuild` folder holds a helper script which will simplify the process of setting up and testing mods. This folder is referenced by the `.csproj` of `Template`, which serves as a base for mod creation. This folder can be copied to create new mods, but these copies must be placed in the root of this repository.

To create Unity asset bundles, use the provided unity project. Create a folder for your mod's assets, assign each of your assets to its own bundle, and then run `Assets/Create AssetBundles`. The bundles will be saved to the `UnityProject/AssetBundles` folder, where they can be copied into your mod folders.

## Community and Support

For discussions, support, and sharing ideas about our mods:
- Join our [Discord](https://discord.gg/3h3nC54PGE).

## Reporting Issues

For bug reports or feature suggestions, please use the [Issues](https://github.com/LCOS-Lethal-Company-Open-Source/LCOS-Mods/issues) section of this repository.

We are excited to engage with the community and look forward to your feedback and contributions!
