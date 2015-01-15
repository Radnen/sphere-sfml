![Sphere SFML Logo](http://radnen.tengudev.com/images/spheresfml.png)

A .NET Sphere implementation using SFML for graphics, input, etc.

**Alpha Downloads:**
 - v0.90: http://radnen.tengudev.com/spheresfml/sphere-sfml-0.90.zip
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
 - Networking: 100%
 - Files: 100%
 - Other: 86.1%

**Future**
 - Particle Engine
 - Sound Effect Object
 - Isometric maps
 - 'Infinite' maps

New Features
============
I took the liberty of adding some new things to Sphere.

Binary Heap: 100% Complete
```
var heap = new BinaryHeap(compare_func);
heap.add(value);
heap.pop();
heap.shift();
heap.remove(); 
heap.resort();
heap.clear();
heap.size();
```

XML Serialization: 50% Complete
```
var doc = new XMLDocument(filename);
doc.write(object);
doc.close();
```

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
