using System.Numerics;

class Program{
	
	public static Player player = new Player();
	public static UI lives = new UI(ref player.lives, "Lives: ", new Vector2(370, 600));

	public static void Main(){
		Window.Setup();
		ObjectLogic.Setup();
		player.Setup();
		LevelLogic.levels[LevelLogic.currentLevel].Start();
		Window.MainLoop();
	}
}


