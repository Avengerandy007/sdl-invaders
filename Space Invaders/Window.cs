using static SDL2.SDL;

//Static class that describes the game window
public static class Window{
	
	static bool running; //If true then continue loop

	public static IntPtr window;
	public static IntPtr renderer;

	//Intialise all the needed components for window
	public static void Setup(){

		running = true;

		//Check if SDL is working properly
		if (SDL_Init(SDL_INIT_VIDEO) < 0){
			Console.WriteLine($"There was a problem starting SDL: {SDL_GetError()}");
		}

		//Initialise the window
		window = SDL_CreateWindow("Shitty clone of Space Invaders", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 840, 640, SDL_WindowFlags.SDL_WINDOW_SHOWN);

		//Check if the window is working
		if (window == IntPtr.Zero) Console.WriteLine($"There was a problem creating the window: {SDL_GetError()}");

		//Initialise the renderer
		renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

		//Check if the renderer is working properly
		if (renderer == IntPtr.Zero) Console.WriteLine($"There was a problem creating the renderer: {SDL_GetError()}");
	}
	
	//Run as long as running is true and clean up afterwards
	public static void MainLoop(){
		while (running){
			Render();
			PollEvents();
		}
		CleanUp();
	}

	//Render the current objects on screen
	static void Render(){
		SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
		SDL_RenderClear(renderer);
		ObjectLogic.RenderObjects();
		SDL_RenderPresent(renderer);
	}

	//Listen for events
	static void PollEvents(){
		while (SDL_PollEvent(out SDL_Event e) == 1){
			switch (e.type){
				case SDL_EventType.SDL_QUIT:
					running = false;
				break;
			}
		}
	}

	//Destroy everything once running is false
	static void CleanUp(){
		SDL_DestroyWindow(window);
		SDL_DestroyRenderer(renderer);
		ObjectLogic.CleanObjects();
		SDL_Quit();
		return;
	}
}
