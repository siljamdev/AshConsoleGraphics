using System;
using AshLib; //For Colors
using AshLib.Formatting; //For CharFormat
using AshConsoleGraphics;

class Program{
	static void Main(){
		Console.CursorVisible = false;
		
		//Example of use of connected lines screen
		TuiConnectedLinesScreen lines = new TuiConnectedLinesScreen(20, 20,
			new ILineElement[]{
				new TuiHorizontalLine(10, 'a', Placement.Center, 0, 1), //In all of these, the char does not really matter
				new TuiHorizontalLine(12, 'a', Placement.TopLeft, 3, 9),
				new TuiHorizontalLine(17, 'a', Placement.Center, 0, -5),
				new TuiVerticalLine(13, 'a', Placement.Center, 0, -6),
				new TuiVerticalLine(8, 'a', Placement.TopRight, 0, 0),
				new TuiFrame(13, 14, Placement.Center, 0, 0)},
			null, null);
		
		TuiScreen screen = new TuiScreen(100, 20, new CharFormat(null, new Color3(30, 0, 0)), lines);
		
		screen.Print();
		
		Console.ReadKey(); //Dont exit immediately
	}
}