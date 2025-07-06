using System.Numerics;

class LevelLogic{
	
	public static int currentLevel = 0;

	//Describe all the levels
	public static Level[] levels = {
		
		new Level{
			index = 0,
			amountOfenemies = 16,
			enemyPositions = [
				new Vector2(100, 100),
				new Vector2(170, 100),
				new Vector2(240, 100),
				new Vector2(310, 100),
				new Vector2(520, 100),
				new Vector2(590, 100),
				new Vector2(660, 100),
				new Vector2(730, 100),
				new Vector2(100, 200),
				new Vector2(170, 200),
				new Vector2(240, 200),
				new Vector2(310, 200),
				new Vector2(520, 200),
				new Vector2(590, 200),
				new Vector2(660, 200),
				new Vector2(730, 200)
			]
		},

		new Level{
			index = 1,
			amountOfenemies = 18,
			enemyPositions = [
				new Vector2(100, 100),
				new Vector2(170, 100),
				new Vector2(240, 100),
				new Vector2(310, 100),
				new Vector2(380, 100),
				new Vector2(450, 100),
				new Vector2(520, 100),
				new Vector2(590, 100),
				new Vector2(660, 100),
				new Vector2(730, 100),
				new Vector2(100, 200),
				new Vector2(170, 200),
				new Vector2(240, 200),
				new Vector2(310, 200),
				new Vector2(520, 200),
				new Vector2(590, 200),
				new Vector2(660, 200),
				new Vector2(730, 200)

			]
		},

		new Level{
			index = 2,
			amountOfenemies = 20,
			enemyPositions = [
				new Vector2(100, 100),
				new Vector2(170, 100),
				new Vector2(240, 100),
				new Vector2(310, 100),
				new Vector2(380, 100),
				new Vector2(450, 100),
				new Vector2(520, 100),
				new Vector2(590, 100),
				new Vector2(660, 100),
				new Vector2(730, 100),
				new Vector2(100, 200),
				new Vector2(170, 200),
				new Vector2(240, 200),
				new Vector2(310, 200),
				new Vector2(380, 200),
				new Vector2(450, 200),
				new Vector2(520, 200),
				new Vector2(590, 200),
				new Vector2(660, 200),
				new Vector2(730, 200)
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
	public Vector2[] enemyPositions; //An array containing the positions of all the enemies in the level, these increase by 70 on the X axis foreach one as to leave a little space in between them but not keep them too far appart, however if you want you can put as much space in between them as you wish

	public Level(){
		this.enemyPositions = new Vector2[amountOfenemies];
	}

	//Load all the enemies at the specified coordonates
	public void Start(){
		foreach(Vector2 position in enemyPositions){
			ObjectLogic.enemies.Add(new Enemy(position));
		}
	}
}
