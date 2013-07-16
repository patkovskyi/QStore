QStore
======

Compact sequence storage library.

My initial goal was to develop language-agnostic spellchecking library — QSpell.
The approach I chose required implementing dictionary as deterministic acyclic finite state automaton.

Later I found out that this subtask can have it's own value. 
It allows storing large sets/maps with keys that are sequences of some elements 
(not only strings, that are sequences of characters) in a very space-efficient way. 
For example, it can store russian language lexicon that takes ~20mb in ASCII clear-text file in less than 2 mb of memory.
It's also easily serializable with binary serialization or protobuf.
Besides saving memory, it allows fast lookup and fast search by prefix.
So I separated this project and called it QStore.

QSpell was implemented as my final project for master's degree in CS, 
but I believe it needs a lot of improvements before I can make it open-source without blushing.

QStore is pretty much polished, optimized and tested, so you're welcome to try it out!
This project is under MIT license.
