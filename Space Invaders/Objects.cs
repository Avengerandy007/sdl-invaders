using static SDL2.SDL;
using static SDL2.SDL_image;
using System.Numerics;
using System.Timers;

//Necesary logic for all object
static class ObjectLogic{
	
	public static List<Projectile> projectiles = new List<Projectile>();
	public static List<Enemy> enemies = new List<Enemy>();

	public static void Setup(){
		IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
		Enemy.Setup();
	}

	public static void RenderObjects(){
		Program.player.Render();
		foreach(var projectile in projectiles){
			projectile.Loop();

			//Remove the projectile when not in view anymore
			if (!projectile.exists){
				projectiles.Remove(projectile);
				break;
			}

			if (projectile.HitPlayer()){
				foreach (var enemy in enemies) enemy.StopFiring(projectile);
				LevelLogic.currentLevel = 0;
				Program.player.amountOfKills = 0;
				projectiles.Clear();
				enemies.Clear();
				LevelLogic.Cycle();
				Program.player.position = Program.player.spawnPosition;
				Program.player.rect.x = Program.player.position;
				break;
			}
		}

		foreach(var enemy in enemies){
			enemy.Render();
			if (enemy.WasHit()){
				Program.player.amountOfKills++;
				LevelLogic.CheckIfAllKilled();
				enemies.Remove(enemy);
				break;
			}
		}
	}

	public static void CleanObjects(){
		Program.player.CleanUp();
		Enemy.CleanUp();
		IMG_Quit();
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
		ObjectLogic.projectiles.Add(new Projectile(true, new Vector2(0, 0)));
	}
}

class Projectile{

	public bool exists;
	bool firedFromplayer;
	
	public SDL_Rect rect;

	Vector2 spawnPosition;

	System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
	
	//Parameter is to know in which direction to go and what position to spawn at
	public Projectile(bool isActivatedbyPlayer, Vector2 enemyPosition){
		exists = true;
		firedFromplayer = isActivatedbyPlayer; 
		stopwatch.Start();
		if (firedFromplayer){
			spawnPosition = new Vector2(Program.player.position + 23, Program.player.rect.y - 50);
			Console.WriteLine("Projectile fired");
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

	void Move(){
		if (stopwatch.Elapsed.TotalMilliseconds >= 5){
			if (firedFromplayer){
				rect.y -= 5;
				stopwatch.Restart();
			}else {
				rect.y += 5;
				stopwatch.Restart();
			}
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

	public void Loop(){
		if (!exists) return;
		Render();
		Move();
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

	static IntPtr surface;
	static IntPtr texture;
	SDL_Rect rect;


	Vector2 position;

	System.Timers.Timer fireProjectileTimer;
	Random timeBetweenShots;

	public Enemy(Vector2 inPos){
		position = inPos;
		timeBetweenShots = new Random();
		fireProjectileTimer = new System.Timers.Timer(timeBetweenShots.Next(2, 5) * 1000);
		fireProjectileTimer.Elapsed += FireProjectile;
		fireProjectileTimer.Start();
		Console.WriteLine($"Hello from {position}");
		rect = new SDL_Rect{
			x = (int)position.X,
			y = (int)position.Y,
			w = 50,
			h = 25 
		};
	}

	void FireProjectile(Object? source, ElapsedEventArgs e){
		ObjectLogic.projectiles.Add(new Projectile(false, position));
	}

	public static void Setup(){
		surface = IMG_Load("Dependencies/Crab.png");
		if (surface == IntPtr.Zero) Console.WriteLine($"There was a problem creating the enemy surface: {SDL_GetError()}");
		texture = SDL_CreateTextureFromSurface(Window.renderer, surface);
		if (texture == IntPtr.Zero) Console.WriteLine($"There was a problem creating the enemy texture: {SDL_GetError()}");
		SDL_FreeSurface(surface);
	}

	public bool WasHit(){

		int enemyXbegin = (int)position.X;
		int enemyXend = enemyXbegin + rect.w;

		int enemyYbegin = (int)position.Y;
		int enemyYend = enemyYbegin + rect.h;

		foreach(var projectile in ObjectLogic.projectiles){
			int Xbegin = projectile.rect.x;
			int Xend = Xbegin + projectile.rect.w;

			int Ybegin = projectile.rect.y;

			if (Xbegin >= enemyXbegin && Xend <= enemyXend && Ybegin <= enemyYend && Ybegin >= enemyYbegin ){
				StopFiring(projectile);
				return true;
			}

		}

		return false;
	}

	public void StopFiring(Projectile projectile){
		ObjectLogic.projectiles.Remove(projectile);
		fireProjectileTimer.Elapsed -= FireProjectile;
		fireProjectileTimer.Stop();
		fireProjectileTimer.Dispose();

	}

	public void Render(){
		if (texture == IntPtr.Zero) Console.WriteLine($"There was a problem maintaining the texture: {SDL_GetError()}");
		SDL_RenderCopy(Window.renderer, texture, IntPtr.Zero, ref rect);
	}

	public static void CleanUp(){
		SDL_DestroyTexture(texture);
	}
}
