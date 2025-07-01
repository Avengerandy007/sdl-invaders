using static SDL2.SDL;
using static SDL2.SDL_image;

//Necesary logic for all object
static class ObjectLogic{
	public static void Setup(){
		IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
	}

	public static void RenderObjects(){
		Program.player.Render();
	}

	public static void CleanObjects(){
		Program.player.CleanUp();
		IMG_Quit();
	}
}

//Describes the player object
class Player : IObjects{
	
	#region Display
	IntPtr surface;
	IntPtr texture;
	SDL_Rect rect;

	int position; //Describes the players position along the X axis
	
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
		
		//Set the players size and coordinates
		rect = new SDL_Rect{
			x = ((winW - 100) / 2),
			y = 530,
			w = 100,
			h = 50
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


	//If is called with true then move left otherwise right
	public void Move(bool left){
		if (left){
			position -= 10;
		}else{
			position += 10;
		}
		rect.x = position;
	}
}

class Enemy : IObjects{

	public void Setup(){

	}

	public void Render(){

	}

	public void CleanUp(){

	}
}
