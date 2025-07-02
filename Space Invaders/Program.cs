using System;

class Program{
	
	public static Player player = new Player();

	public static void Main(){
		Window.Setup();
		player.Setup();
		LevelLogic.levels[LevelLogic.currentLevel].Start();
		Window.MainLoop();
	}
}


