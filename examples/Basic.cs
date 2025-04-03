using System;
using AshLib; //For colors
using AshConsoleGraphics;

class Program{
	static void Main(){
		Console.CursorVisible = false; //That way we wont see the cursor flashing
		
		TuiScreen screen = new TuiScreen(100, 20, null, null, new TuiLabel("Hello, World!", Placement.Center, 0, 0, Color3.Yellow, null));
		
		screen.AutoResize = true; //If the console window resizes, so will the screen to fit all
		
		while(true){
			screen.Print();
		}
	}
}