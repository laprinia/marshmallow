# Marshmallow

![main character](/screenshots/start2.png "Marshmallow the Corgi")



Collaborative side-scroller, story driven 2D game. The game is based mostly on visuals and story, visuals which are highly inspired by Hayao Miyazaki’s work.

![game development](/screenshots/animation.png "Marshmallow frame by frame animation")
### 📷Progress so far:



* **Movement**:

![game development](/screenshots/movement.gif "Marshmallow movement")
 
 The flow of the game is from left to right.
 
 Animations were hand drawn and adapt to the movement speed.
 
 Added later was a movement function that varies the speed of Marshmallow's walk.
> sin( time * f1 + p1 )*a1  +  sin( time * f2 + p2 )*a2 +  baseSpeed



* **Bezier Curves**:

![game development](/screenshots/bezier.png "Butterfly Bezier curves")

In the tutorial area, a butterfly will guide Marshmallow through the basic controls.

The path is traced using Bezier curves.



* **Parallax Effect**:

![game development](/screenshots/parallax.png "Parallax camera effect")

 For an immersive experience, elements in the background, as well as objects in the foreground, move along with the character.




* **Interaction System**:

![game development](/screenshots/interaction.gif "Fixed Joint interaction")

Achieved using a Fixed Joint component.

Objects attach/detach to the character on pressing the "E" key.

The character needs to push objects around to solve puzzles.

* **Save/Load System**:

Uses checkpoints to save automatically.

Uses binary files.
