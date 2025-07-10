using System.Numerics;
using Enemies;
using Enemies.Types;
class LevelLogic{
	
	public static int currentLevel = 0;

	//Describe all the levels
	public static Level[] levels = {
		
		new Level{
			index = 0,
			amountOfenemies = 16,
			GenerateEnemies = () => new Enemy[]{
				new Crab{position = new Vector2(100, 100)},
				new Crab{position = new Vector2(170, 100)},
				new Squid{position = new Vector2(240, 100)},
				new Squid{position = new Vector2(310, 100)},
				new Crab{position = new Vector2(520, 100)},
				new Squid{position = new Vector2(590, 100)},
				new Squid{position = new Vector2(660, 100)},
				new Crab{position = new Vector2(730, 100)},
				new Crab{position = new Vector2(100, 200)},
				new Squid{position = new Vector2(170, 200)},
				new Squid{position = new Vector2(240, 200)},
				new Crab{position = new Vector2(310, 200)},
				new Squid{position = new Vector2(520, 200)},
				new Squid{position = new Vector2(590, 200)},
				new Crab{position = new Vector2(660, 200)},
				new Crab{position = new Vector2(730, 200)},
			}
		},

		new Level{
			index = 1,
			amountOfenemies = 18,
			GenerateEnemies = () => new Enemy[]{
				new Crab{position = new Vector2(100, 100)},
				new Crab{position = new Vector2(170, 100)},
				new Squid{position = new Vector2(240, 100)},
				new Squid{position = new Vector2(310, 100)},
				new Crab{position = new Vector2(380, 100)},
				new Crab{position = new Vector2(450, 100)},
				new Squid{position = new Vector2(520, 100)},
				new Squid{position = new Vector2(590, 100)},
				new Crab{position = new Vector2(660, 100)},
				new Crab{position = new Vector2(730, 100)},
				new Squid{position = new Vector2(100, 200)},
				new Squid{position = new Vector2(170, 200)},
				new Crab{position = new Vector2(240, 200)},
				new Crab{position = new Vector2(310, 200)},
				new Squid{position = new Vector2(520, 200)},
				new Crab{position = new Vector2(590, 200)},
				new Crab{position = new Vector2(660, 200)},
				new Squid{position = new Vector2(730, 200)},
			}
		},

		new Level{
			index = 2,
			amountOfenemies = 20,
			GenerateEnemies = () => new Enemy[]{
				new Crab{position = new Vector2(100, 100)},
				new Crab{position = new Vector2(170, 100)},
				new Squid{position = new Vector2(240, 100)},
				new Squid{position = new Vector2(310, 100)},
				new Crab{position = new Vector2(380, 100)},
				new Crab{position = new Vector2(450, 100)},
				new Squid{position = new Vector2(520, 100)},
				new Squid{position = new Vector2(590, 100)},
				new Crab{position = new Vector2(660, 100)},
				new Crab{position = new Vector2(730, 100)},
				new Squid{position = new Vector2(100, 200)},
				new Squid{position = new Vector2(170, 200)},
				new Crab{position = new Vector2(240, 200)},
				new Crab{position = new Vector2(310, 200)},
				new Squid{position = new Vector2(380, 200)},
				new Squid{position = new Vector2(450, 200)},
				new Crab{position = new Vector2(520, 200)},
				new Crab{position = new Vector2(590, 200)},
				new Squid{position = new Vector2(660, 200)},
				new Squid{position = new Vector2(730, 200)},
			}
		}
	};

	//In the case that all of the enemies in the level are killed we move on to the next level
	public static void CheckIfAllKilled(){
		if (levels[currentLevel].amountOfenemies == Program.player.amountOfKills){
			Program.player.amountOfKills = 0;
			currentLevel++;
			Cycle();
		}
	}

	//Start the next level
	public static void Cycle(){
		ObjectLogic.ClearEnemies();
		ObjectLogic.ClearProjectiles();
		if (currentLevel < levels.Length) levels[currentLevel].Start();
		else {
			currentLevel = 0;
			levels[currentLevel].Start();
		}
	}

}

class Level{
	public int index;
	public int amountOfenemies;

	Enemy[] enemies;

	public Func<Enemy[]> GenerateEnemies;

	public Level(){
		enemies = new Enemy[20];
		GenerateEnemies = () => Array.Empty<Enemy>();
	}

	//Load all the enemies at the specified coordonates
	public void Start(){
		enemies = GenerateEnemies();
		ObjectLogic.AddEnemies(enemies);
	}
}
