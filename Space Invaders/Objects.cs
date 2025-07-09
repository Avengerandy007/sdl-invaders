/*
	A few words on how collision detection works, because it is kinda retarded and doesn't make that much sense:

	In every case where I need collision detection I take the objects I am checking from position, find out it's "other sides coordinates" by adding its width and height.
	I do the same for the object I try to collide with and find out if that objects coordinates are in between this ones.

*/

using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;
using System.Numerics;
using System.Timers;


static class ObjectLogic{
	public static List<Projectile> projectiles = new List<Projectile>();
	public static List<Projectile> queuedProjectiles = new List<Projectile>();
	public static Enemy[] enemies = new Enemy[20];
	public static UI[] UIelements = {Program.level, Program.lives, Program.score};

	//Setup all needed sdl services and enemy logic
	public static void Setup(){
		IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
		TTF_Init();
		Enemy.MoveTimerStart();
	}

	static bool playerDied = false;

	public static void RenderObjects(){

		
		Program.player.Render();

		#region UI Elements
		Program.lives.variableToDisplay = Program.player.lives;
		Program.level.variableToDisplay = LevelLogic.currentLevel;
		Program.score.variableToDisplay = Program.player.score;
		#endregion

		if (!playerDied) foreach(var projectile in projectiles){

			if (projectile.sender is null){} 
			else if (!projectile.sender.exists) continue;
			projectile.Loop();

			if (projectile.HitPlayer()){
				if (Program.player.lives == 0){
					playerDied = true;
					PlayerDied();
					Program.player.lives = 3;
					Program.player.score = 0;
					ClearEnemies();
					break;
				}else{
					Program.player.lives--;
					ClearProjectiles();
					Program.player.position = Program.player.spawnPosition;
					Program.player.rect.x = Program.player.position;
					break;
				}
			}

		}else{
			ClearProjectiles();
			LevelLogic.Cycle();
			queuedProjectiles.Clear();
			playerDied = false;
		}

		foreach(var enemy in enemies){
			if (enemy is null) continue;
			enemy.Render();
			if (enemy.WasHit()){
				Program.player.amountOfKills++;
				ClearEnemies();
				LevelLogic.CheckIfAllKilled();
				Program.player.score += enemy.scoreFactor;
				break;
			}
		}

		queuedProjectiles.RemoveAll(projectile => !projectile.exists);

		projectiles.AddRange(queuedProjectiles);
		queuedProjectiles.Clear();

		projectiles.RemoveAll(projectile => !projectile.exists);

		foreach (var element in UIelements) element.Render();

	}

	//foreach enemy position in the current level, create a new enemy
	public static void AddEnemies(Enemy[] levelEnemies){
		enemies = levelEnemies;
	}

	public static void ClearEnemies(){
		enemies = Array.FindAll(enemies, (enemy => enemy.exists)).ToArray();
	}

	public static void ClearProjectiles(){
		foreach(var projectile in projectiles) projectile.exists = false;
	}

	static void PlayerDied(){
		foreach (var enemy in enemies) enemy.StopFiring();
		queuedProjectiles.Clear();
		foreach (var projectile in projectiles) projectile.exists = false;
		LevelLogic.currentLevel = 0;
		Program.player.amountOfKills = 0;
		foreach(var enemy in enemies) enemy.exists = false;
		Program.player.position = Program.player.spawnPosition;
		Program.player.rect.x = Program.player.position;
		LevelLogic.Cycle();

	}

	public static void CleanObjects(){
		Enemy.DestroyTimer();
		Program.player.CleanUp();
		Enemy.CleanUp();
		foreach (var element in UIelements) element.CleanUp();
		IMG_Quit();
		TTF_Quit();
	}
}

//Describes the player object
class Player : IObjects{
	
	#region Display
	IntPtr surface;
	IntPtr texture;
	public SDL_Rect rect;

	public int position; //Describes the players position along the X axis
	public int spawnPosition;
	public int lives = 3;
	public int score = 0;
	
	//Loads all the SDL necities to display the player sprite
	public void Setup(){

		surface = IMG_Load("Dependencies/Player.png");
		if (surface == IntPtr.Zero) Console.WriteLine($"Player surface is null: {SDL_GetError()}");
		texture = SDL_CreateTextureFromSurface(Window.renderer, surface);
		if (texture == IntPtr.Zero) Console.WriteLine($"Player texture is null: {SDL_GetError()}");
		SDL_FreeSurface(surface);

		int winW;
		int winH;
		SDL_GetWindowSize(Window.window, out winW, out winH);

		spawnPosition = ((winW - 100) / 2);
		
		//Set the players size and coordinates
		rect = new SDL_Rect{
			x = spawnPosition,
			y = winH - 100,
			w = 50,
			h = 25 
		};

		position = rect.x;
	}

	public void Render(){
		SDL_RenderCopy(Window.renderer, texture, IntPtr.Zero, ref rect);
	}

	public void CleanUp(){
		SDL_DestroyTexture(texture);
	} 
	#endregion

	public int amountOfKills;

	//If is called with true then move left otherwise right
	public void Move(bool left){
		if (left){
			position -= 10;
		}else{
			position += 10;
		}
		rect.x = position;
	}

	public void FireProjectile(){
		ObjectLogic.projectiles.Add(new Projectile(true, new Vector2(0, 0), null)); 
	}
}

class Projectile{

	public bool exists;
	public bool firedFromplayer;
	
	public SDL_Rect rect;

	Vector2 spawnPosition;
	public Enemy? sender;

	//Parameter is to know in which direction to go and what position to spawn at
	public Projectile(bool isActivatedbyPlayer, Vector2 enemyPosition, Enemy? inSender){
		exists = true;
		firedFromplayer = isActivatedbyPlayer; 
		sender = inSender;
		if (firedFromplayer){
			spawnPosition = new Vector2(Program.player.position + 23, Program.player.rect.y - 50);
		}else{
			spawnPosition = new Vector2(enemyPosition.X + 20, enemyPosition.Y + 30);
		}

		rect = new SDL_Rect{
			x = (int)spawnPosition.X,
			y = (int)spawnPosition.Y,
			w = 5,
			h = 50
		};
	}

	//Change the position every frame depending if was shot from player
	void Move(){
		if (firedFromplayer){
			rect.y -= 5;
		}else {
			rect.y += 5;
		}
	}

	public bool HitPlayer(){
		int Xbegin = (int)spawnPosition.X;
		int Xend = Xbegin + rect.w;

		int Ybegin = rect.y;
		int Yend = Ybegin + rect.h;

		int pXbegin = Program.player.rect.x;
		int pXend = pXbegin + Program.player.rect.w;

		int pYbegin = Program.player.rect.y;

		if (firedFromplayer){
			return false;
		}else{
			if (Xbegin >= pXbegin && Xend <= pXend && Yend >= pYbegin) return true;
		}

		return false;
	}

	//Render and move each frame
	public void Loop(){
		if (!exists) return;
		Render();
		Move();
		
		//If is out of the screen bounds then destroy this
		if (rect.y < 0 || rect.y > 640){
			exists = false;
		}
	}

	void Render(){
		SDL_SetRenderDrawColor(Window.renderer, 255, 255, 255, 255);
		SDL_RenderDrawRect(Window.renderer, ref rect);
		SDL_RenderFillRect(Window.renderer, ref rect);
		SDL_SetRenderDrawColor(Window.renderer, 0, 0, 0, 255);
	}
}

class Enemy{

	IntPtr surface;
	IntPtr texture;
	SDL_Rect rect;


	public Vector2 position;
	public bool exists;

	public int scoreFactor; //The amount of score the player earns when this is killed

	System.Timers.Timer fireProjectileTimer;
	Random timeBetweenShots; //A random factor for each enemies time in between shots

	static System.Timers.Timer moveTimer = new System.Timers.Timer(6000);
	
	int enemyXbegin;
	int enemyXend;

	int enemyYbegin;
	int enemyYend;


	//Initialise the timers logic
	public static void MoveTimerStart(){
		moveTimer.Elapsed += Move;
		moveTimer.Start();
	}

	//Move every enemy 70 pixel to the right, when further than 730, move 100 pixels up(down)
	static void Move(Object? source, ElapsedEventArgs e){
		foreach(var enemy in ObjectLogic.enemies){
			enemy.position.X += 70;
			if (enemy.position.X > 730){
				enemy.position.Y += 100;
				enemy.position.X = 100;
			}
		}

		//Check if this is below a certain treshold and then reset completely the player
		if (ObjectLogic.enemies.Last().position.Y >= 450){
			Program.player.position = Program.player.spawnPosition;
			Program.player.rect.x = Program.player.position;
			LevelLogic.currentLevel = 0;
			foreach(var enemy in ObjectLogic.enemies) enemy.exists = false;
			ObjectLogic.ClearProjectiles();
			ObjectLogic.queuedProjectiles.Clear();
			LevelLogic.Cycle();
			Program.player.lives = 3;
		} 
	}


	public static void DestroyTimer(){
		moveTimer.Elapsed -= Move;
		moveTimer.Stop();
		moveTimer.Dispose();
	}

	//Parameter name starts with in because I have another variable with the same name
	public Enemy(){
		exists = true;

		UpdateDataCoordinates();

		timeBetweenShots = new Random();
		fireProjectileTimer = new System.Timers.Timer(timeBetweenShots.Next(5, 20) * 1000);
		
		rect = new SDL_Rect{
			x = (int)position.X,
			y = (int)position.Y,
			w = 50,
			h = 25 
		};

	}

	public void UpdateDataCoordinates(){
		
		enemyXbegin = (int)position.X;
		enemyXend = enemyXbegin + rect.w;

		enemyYbegin = (int)position.Y;
		enemyYend = enemyYbegin + rect.h;

	}

	//Fire projectile when timer reaches 0
	void FireProjectile(Object? source, ElapsedEventArgs e){
		if (!exists) return;
		ObjectLogic.queuedProjectiles.Add(new Projectile(false, position, this)); 
	}

	//Load all necesary assets for displaying to screen
	public virtual void Setup(){
		if (surface == IntPtr.Zero) Console.WriteLine($"There was a problem creating the enemy surface: {SDL_GetError()}");
		texture = SDL_CreateTextureFromSurface(Window.renderer, surface);
		if (texture == IntPtr.Zero) Console.WriteLine($"There was a problem creating the enemy texture: {SDL_GetError()}");
		SDL_FreeSurface(surface);
	}

	
	//Check if was hit by player projectile
	public bool WasHit(){
		
		UpdateDataCoordinates();
		foreach(var projectile in ObjectLogic.projectiles){
			if (!exists || !projectile.firedFromplayer) continue;

			int Xbegin = projectile.rect.x;
			int Xend = Xbegin + projectile.rect.w;

			int Ybegin = projectile.rect.y;
			int Yend = Ybegin + projectile.rect.h;

			int disX = Xend - enemyXbegin;
			int disY = Yend - enemyYbegin;
			double distance = Math.Sqrt(Math.Pow(disX, 2) + Math.Pow(disY, 2));

			if (distance > 80) continue;
			
			//If the incoming projectile is in between the X and Y coordinates of the enemy
			if (Xbegin >= enemyXbegin && Xend <= enemyXend && Ybegin <= enemyYend && Ybegin >= enemyYbegin){
				StopFiring();
				exists = false;
				KillProjectile(projectile);
				return true;
			}

		}

		return false;
	}

	//Destroy the projectile that killed this
	void KillProjectile(Projectile projectile){
		projectile.exists = false;
	}

	//Dispose of the firing timers logic
	public void StopFiring(){
		fireProjectileTimer.Elapsed -= FireProjectile;
		fireProjectileTimer.Stop();
		fireProjectileTimer.Dispose();
	}

	//Render the enemy to the screen each frame and change its position acordingly
	public void Render(){
		rect.x = (int)position.X;
		rect.y = (int)position.Y;
		if (texture == IntPtr.Zero) Console.WriteLine($"There was a problem maintaining the texture: {SDL_GetError()}");
		SDL_RenderCopy(Window.renderer, texture, IntPtr.Zero, ref rect);
	}

	public static void CleanUp(){
		foreach (var enemy in ObjectLogic.enemies){
			SDL_DestroyTexture(enemy.texture);
			enemy.StopFiring();
		}
	}

	public class Type : Enemy{
		public class Crab : Type{
			public Crab(){
				scoreFactor = 3;
				fireProjectileTimer.Elapsed += FireProjectile;
				fireProjectileTimer.Start();
				Setup();

			}

        		public override void Setup()
        		{
				surface = IMG_Load("Dependencies/Crab.png");
            			base.Setup();
        		}

		}

		public class Squid : Type{
			public Squid(){
				scoreFactor = 1;
				fireProjectileTimer.Elapsed += FireProjectile;
				fireProjectileTimer.Start();
				Setup();
			}

        		public override void Setup()
        		{
				surface = IMG_Load("Dependencies/Squid.png");
            			base.Setup();
        		}
    
		}
	}

	
}

class UI : IObjects {

	IntPtr font;
	IntPtr surface;
	IntPtr texture;
	SDL_Rect rect;
	SDL_Color white;

	public int variableToDisplay; //Updated from while loop manually for all UI components
	string? textToDisplay;
	Vector2 position;

	public UI(string text, Vector2 location){
		textToDisplay = text;
		position = location;
		Setup();
	}
	
	public void Setup(){
		font = TTF_OpenFont("Dependencies/BitcountGridDouble-Regular.ttf", 40);
		rect = new SDL_Rect{
			x = (int)position.X,
			y = (int)position.Y,
			w = 100,
			h = 50
		};
		white = new SDL_Color{r = 255, g = 255, b = 255, a = 255};
	}

	public void Render(){
		SDL_DestroyTexture(texture);
		string display = textToDisplay + variableToDisplay;
		surface = TTF_RenderText_Solid(font, display, white);
		texture = SDL_CreateTextureFromSurface(Window.renderer, surface);
		SDL_FreeSurface(surface);
		SDL_RenderCopy(Window.renderer, texture, IntPtr.Zero, ref rect);
	}

	public void CleanUp(){
		SDL_DestroyTexture(texture);
	}
}
