The purpose of this project was personal education. I wanted to remind myself of how to work with an Alpha-Beta pruning algorithm.

This implementation of Checkers has some major flaws:

1) The huristic that assigns a value to a possible game state assigns the same value to kinged and non-kinged pieces. Thus, the AI is not always smart enough to make a move that would lead to having a kinged piece.

2) The black playing pieces can be hard to see.

3) The AI is surprisingly slow.

I imagine that all of these would be easy to fix, but there are plenty of functioning Checkers AIs available for free. For the moment this project has served its purpose of helping me understand Alpha-Beta pruning better.