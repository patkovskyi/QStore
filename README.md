QSpell
======

Compact sequence storage &amp; spellchecking library.
Based on my master's degree project.

The initial goal was to develop language-agnostic spellchecking library — QSpell.
But to do this I had to implement dictionary as deterministic acyclic finite state automaton.

Later I found out that this sub-task can have it's own value. 
It allows storing large sets/maps with keys that are sequences of some elements (not only strings — sequences of chars) 
in a very space-efficient way. This project is called QStore. 

QStore allows fast lookup and fast search by prefix.
For example, it can store russian language lexicon that takes ~20mb in ASCII clear-text file in less than 2 mb of memory.
It's also easily serializable with binary serialization or protobuf.

QSpell is very far from production-ready state, so I don't recommend anyone using it now.
But QStore is pretty much polished, optimized and tested, so you're welcome to try it out!
This project is under MIT license.
