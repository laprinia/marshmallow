# Marshmallow

![main](https://user-images.githubusercontent.com/51471463/166144766-7cd72247-5c27-4ba1-b181-6fe358fe8169.png)


Collaborative side-scroller, story driven 2D game. The game is based mostly on visuals and story, visuals which are highly inspired by Hayao Miyazaki’s work.


### 📷Progress so far:



* **Movement**:


https://user-images.githubusercontent.com/51471463/166144799-49d1f0e2-c56f-4c21-80b5-282fec8a5f6f.mp4


 The flow of the game is from left to right.
 
 Animations were hand drawn and adapt to the movement speed.
 
 Added later was a movement function that varies the speed of Marshmallow's walk.
> sin( time * f1 + p1 )*a1  +  sin( time * f2 + p2 )*a2 +  baseSpeed



* **Bezier Curves**:

![bezier](https://user-images.githubusercontent.com/51471463/166144876-65b39d96-ea12-43a0-b3b7-f50fac9f6997.png)


In the tutorial area, a butterfly will guide Marshmallow through the basic controls.

The path is traced using Bezier curves.



* **Parallax Effect**:



 For an immersive experience, elements in the background, as well as objects in the foreground, move along with the character.


* **Interaction System**:


https://user-images.githubusercontent.com/51471463/166145164-4d58dbaf-d07a-4bfa-8fe6-02f6ab141c44.mp4


Achieved using a Fixed Joint component.

Objects attach/detach to the character on pressing the "E" key.

The character needs to push objects around to solve puzzles.

* **Save/Load System**:

Uses checkpoints to save automatically.

Uses binary files.

* **Dialogue System**:

![dialogue](https://user-images.githubusercontent.com/51471463/166144954-ce28bab5-786e-4774-b532-5d43ecd18e21.png)

Freezes any movement from the the main character.

Composed from a Dialogue Manager and a Dialogue Sender.

Once the player is in the dialogue area, the conversation is displayed.

* **Quest System**:

![quest-manager](https://user-images.githubusercontent.com/51471463/166145020-68bba8c6-aca0-457a-a410-d8098c56660c.png)


Composed from a Quest Manager and Quest-type items that contain all the info.


### Multiplayer

Up to three players can play along and choose any of the three dogs. The multiplayer layer is added using [Mirror](https://mirror-networking.com/).


* **Network Manager**

![network-manager](https://user-images.githubusercontent.com/51471463/166145034-0427ab2a-61c8-4b25-aca4-e922f864039f.png)

It acts as a state manager, and needs a reference to the offline/online Scene and the Prefab of the player.

* **Network Identity**

![network-identity](https://user-images.githubusercontent.com/51471463/166145189-df18af0c-c565-45ac-ba7a-a6b1af6280bd.png)

The Network Animator assures that the animations are synced. Can animate locally or on server.
The Network Transform syncs the Transform component at a given time.
The Network Start Position can have multiple positions passed, but it's up to the manager on how it instantiates the players.
