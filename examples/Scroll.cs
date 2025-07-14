using System;
using AshLib; //For colors
using AshLib.Formatting; //For CharFormats
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

class Program{
	static void Main(){
		Console.CursorVisible = false; //That way we wont see the cursor flashing
		
		TuiSelectable[,] temp = new TuiSelectable[100, 1]; //Pre-allocate the selectable matrix
		
		for(int i = 0; i < 100; i++){ //Populate it
			temp[i, 0] = new TuiButton(i.ToString(), Placement.TopLeft, 0, i);
		}
		
		TuiScrollingScreenInteractive screen = new TuiScrollingScreenInteractive(100, 20, temp, 0, 0, null, new TuiLabel("Hello", Placement.Center, 0, 0));
		
		screen.AutoResize = true; //If the console window resizes, so will the screen to fit all
		
		screen.Play();
	}
}