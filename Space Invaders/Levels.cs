using System.Numerics;

class LevelLogic{
	
	Level[] levels = {
		 new Level{
			index = 0,
			ammountOfenemies = 2,
			enemyPositions = [
				new Vector2(300, 300),
				new Vector2(400, 300)
			]
		}
	};

}

class Level{
	public int index;
	public int ammountOfenemies;
	public Vector2[] enemyPositions; //An array containing the positions of all the enemies in the level

	public Level(){
		this.enemyPositions = new Vector2[ammountOfenemies];
	}

	void Start(){
		foreach(Vector2 position in enemyPositions){
			//TODO: Create a new enemy at this position
		}
	}
}
