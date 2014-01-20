![Sphere SFML Logo](http://radnen.tengudev.com/images/spheresfml.png)

A .NET Sphere implementation using SFML for graphics, input, etc.

**Alpha Downloads:**
 - v0.80: http://radnen.tengudev.com/spheresfml/sphere-sfml-0.80.zip
 - older: http://radnen.tengudev.com/spheresfml/

Parity with Sphere v1.5
=======================
**Complete**
 - Images: 100%
 - Input: 100%
 - Sounds: 100%
 - Fonts: 100%
 - WindowStyles: 100%
 - Spritesets: 100%
 - Files: 100% (minus raw files)

**Incomplete**
 - General: 89%
 - Persons: 83%
 - Surfaces: 69%
 - Primitives: 65%
 - Map Engine: 55%

**To Do**
 - Networking: 0%
 - Raw files: 0%

**Future**
 - Particle Engine
 - Sound Effect Object
 - Isometric maps
 - 'Infinite' maps

New Features
============
I took the liberty of adding some new things to Sphere.
 - Binary Heap: new BinaryHeap(compare_func): 100%
 - XML Serialization: new XMLDocument(filename): 50%

How To
======

You can use the following command line args with this engine:
 - **-games:** a list of the games in the /games directory
 - **-game "dir":** play the Sphere game (the game.sgm file or containing folder).
 - **-conformance:** see what Sphere version parity it's at.
 - **-help:** see this help message.
 - **"dir":** play the sphere game (ending in .sgm) by dragging and dropping it over this executable.

**Example**:
```
Engine.exe -game "C:/Path/to/game.sgm"

# or:

Engine.exe -game "C:/path/to/game
```

Credits
=======
 - Andrew "Radnen" Helenius
