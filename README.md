# TopDownArenaShooter
- New player input used

-Player
-player movement done with CharacterController - PlayerController
-PlayerCombat
	- has auto reload when magazine empty
	- shooting bullets from bullet pool
	- has basic lineRenderer to help show aim
-Weapons
	- each weapon can be defined in WeaponData
	- in WeaponInventory owned weapons and their WeaponState (ammo in magazine and ammo in reserve) are stored and managed.
	- there is logic for equipping and adding weapon, but for this demo theres only one defined gun and no collectable weapons or input for switching, just left it scalable

- Health has IDamageable and its used by enemies and player
- Player can pickup collectables with Pickup script on them

Pickups
- implemented because there is health and ammo defined, so health and ammo are two possible collectables
- PickupData - defines them, has amount and also weapon type, again for scalability in the future when theres different weapons - different ammo
- when player picks up health it heals itself - also when its max health, player cant pick up health collectable
- when player picks up ammo, its stored on reserved unless its already max capacity then it cant pick up
- collectables are dropped when enemy is killed with defined probability on EnemyData
- pickups are addressables

Enemy
- has EnemyBehaviour and EnemyData
- using navmesh simplest form of chase and attack, has no patrol (didn't think it was needed here)
- EnemyData has reference to prefab, dropchance for collectables, maxhealth, detection and attack range, damage, chase speed and attack cooldown. few of those stats are modified with DifficultyProfile (few for demo purposes, all can be modified)

Levels
- each level is defined by LevelData which is addressable (for the possibility of having bunch of LevelDatas)
- on LevelData theres spawn rate in which the enemies are spawned on entry points, spawn step is how many at one rate and max alive enemies at a time
- EntyPoints are on the scene and used for only for position value
- LevelData holds a reference to scene, although in this demo same scene is used just implemented levels like "waves" but it can be changed
- LevelData has EnemyEntry array that has reference to enemy prefab and count how many will be spawned of those enemies
- for difficulty DifficultyProfile with animation curve is used, so that depending on a level number and max difficulty level, between two min and max multiplier values each level can get corresponding multiplier for enemy stats
- LevelCatalog holds all leveldata
- LevelLoader - used for loading levels both next and from index for level selection
- level selection buttons are made based on level catalog size and call level loader with index
- GameController handles spawning enemies and monitors if all enemies are dead to request loading next level, also here when spawning enemies is where difficulty modifers are calculated

- language selection and audio volume stored on PlayerPrefs
- AddressableMusicPlayer handles loading and playing addressable audio like background music (sfx are not addressable as they are small enough)
