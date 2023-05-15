# n-dimensional chess

The `Hello World` of object orientated programming!

Ok not really, it's just an attempt at a chess implementation scalable in all directions (quite literally).

### I made a fancy title screen for this game:

https://user-images.githubusercontent.com/50182007/181196062-011c88f9-8bdd-4526-a894-afa5dd40b4aa.mov

### Alpha test in 6D:

https://github.com/YeeticusFinch/Fancy-Chess/assets/50182007/57c369a1-dd1a-42a6-9464-70d5a1c7314a

### Alpha test in 3D:

https://user-images.githubusercontent.com/50182007/181196374-cf6b1dd4-2370-4c9b-96fb-e2c5eb31179f.mov


### Alpha test in 2D:

https://user-images.githubusercontent.com/50182007/181196389-1a2262a0-f142-4e0e-9a2c-99b29de48df2.mov


## Planned Features:

### ----------------------------- Predefined Game Modes -----------------------------
#### Regular Chess
8x8x1 board, 16 pieces per side
#### Ocean Man
8x16x3 board, 16 pieces per side
Pieces start on top, they can go “under water”, and it’s a long way to get to the other side
#### Bowling
8x8x1, 16 pieces per side
To consume another piece, you must knock it off of the board
Pieces can knock into eachother, you can knock your pieces into others as well
#### 21st Century


### ----------------------------- Some Custom Pieces --------------------------
#### Princess
Can slide like a bishop, and jump like a knight
#### Empress
Can slide like a rook, and jump like a knight
#### Donald Trump
Moves like a king.
Can only move to a spot that’s guarded by another piece (another piece can’t move away to break this).
Places a pawn on the previous square during a move (building a wall)
#### Pope
Ferz, Alpil, and Knight combined
#### Zebra
Long jumping knight (3 forward, 2 sideways)
#### Camel
Long jumping knight (3 forward, 1 sideways)
#### Alpil
Jumps 2 spaces diagonally
#### Ferz
Can move one space diagonally
#### Dabbaba
Jumps 2 spaces laterally
#### Buffalo
Knight, Camel, and Zebra combined
#### Elephant
Can jump 1 or 2 spaces diagonally (Ferz and Alpil combined)
#### Unicorn
Can jump 2 spaces laterally, or 1 space diagonally
#### Evil Wolf
Moves like a king, but with no backwards movement
#### Falcon
Moves forward as a bishop, and backwards as a rook
#### Faro
Can consume like a rook, but must jump over a piece for a non consuming move
#### Flying Cock (this is legit from fairy chess, I’m not joking)
Moves 1 square diagonally forward, or one square sideways
#### Friend
Moves like any friendly piece that is guarding it
#### Girlscout
A rook that zigzags
#### Hippo
A knight that can only move to capture
#### Mao Zedong
Moves like a knight by sliding, cannot jump
#### Pyramid
Can’t be moved or taken, just blocks a square
#### Twin Tower
Slides one square diagonally forward then any number of squares forward like a rook.
Can do the same thing backwards too
#### War Machine
Can jump 1 or 2 spaces laterally
#### Zero
Jumps and lands on the same square, allows for a turn skip
#### Crab
Moves forward like a knight.
Moves backwards like 2 backwards 2 sideways)
#### Dragon
Knight and Pawn combined
#### Drunkern Rook
Rook that skips squares
#### Landlord
Can teleport to any unoccupied square on the board, can’t eat anything
#### Bigot
Can consume friendly pieces
#### Soviet
Can swap places with any friendly piece on the board
#### Stealth Bomber
Can put the king in check in secret without announcing ‘check’
#### Medusa
A queen that can’t capture, anybody in a straight line is frozen, including friendlies
#### Epstein
Moves like a king, can drag an adjacent piece along the same movement vector (as long as there is space for the adjacent piece)
#### Dalek
Moves like a king, can consume like a rook, but without actually moving
#### Alchemist
Copies move/capture of opponent’s last piece moved. If opponent’s last piece moved was the Queen. Alchemist copies movement but can only capture as a standard pawn.
#### Storm King
Moves/Captures one square as a rook and backwards as a Knight. S.K. has the option to WILD ATTACK capturing all adjacent friendly and enemy pieces.
