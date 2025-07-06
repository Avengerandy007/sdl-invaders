using System.Numerics;

class Program{
	
	public static Player player = new Player();
	public static UI lives = new UI("Lives: ", new Vector2(370, 600));
	public static UI level = new UI("Level: ", new Vector2(20, 10));
	public static UI score = new UI("Score: ", new Vector2(150, 10));

	public static void Main(){
		Window.Setup();
		ObjectLogic.Setup();
		player.Setup();
		LevelLogic.levels[LevelLogic.currentLevel].Start();
		Window.MainLoop();
	}
}


