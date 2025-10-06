# Game Concept
Two different AI strongholds will spawn AI units, who will then charge across the battlefield, attempting to murder the other units. The AI will have to plan out whether to attack other units, protecting their strongholds, or go after the enemy strongholds, winning the game. The player will “bet” on which faction they think will win. If they guess correctly, then that faction will start the next round with slightly less resources, as they paid back the player’s investment. If they lose, then they get a boost in starting resources for the next round, as they keep the player’s betting money.
# High-level description of the different AI systems and how you will use them in the game
Main AI is the one that keeps track of the stronghold hp and resources to spawn units.
3 different types of AI units, the Barbarian, the Cleric, and the Ranger. 
The Barbarian will have melee attacks and go in the front to attack the closest enemy to them. 
Barbarians will also only be able to see in a 5m circle around them
The Cleric will heal the lowest and closest unit.
Clerics will be able to see any friendly unit on the map, but will not know the locations of enemy units
The Ranger will mainly attack enemies closest to the enemy stronghold or the stronghold. However if an enemy comes within its field of view then it will attack them. 
Rangers will have a conical field of view, with the cone stretching out to 10m
