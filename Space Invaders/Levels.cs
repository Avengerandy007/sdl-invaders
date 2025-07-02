using System.Numerics;

class LevelLogic{
	
	public static int currentLevel = 0;

	public static Level[] levels = {
		
		new Level{
			index = 0,
			amountOfenemies = 3,
			enemyPositions = [
				new Vector2(100, 100),
				new Vector2(200, 200),
				new Vector2(500, 500)
			]
		},

		new Level{
			index = 1,
			amountOfenemies = 2,
			enemyPositions = [
				new Vector2(300, 300),
				new Vector2(400, 300)
			]
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
	public Vector2[] enemyPositions; //An array containing the positions of all the enemies in the level

	public Level(){
		this.enemyPositions = new Vector2[amountOfenemies];
	}

	//Load all the enemies at the specified coordonates
	public void Start(){
		foreach(Vector2 position in enemyPositions){
			new Enemy(position);
		}
	}
}
