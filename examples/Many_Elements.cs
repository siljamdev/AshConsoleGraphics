using System;
using AshLib;
using AshLib.Time; //For DeltaHelper
using AshLib.Formatting; //For CharFormat
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

class Program{
	
	static Random rand = new Random();

	static void Main(){
		
		TuiElement[] Elements = {
			new TuiLabel("FPS: 0", Placement.TopLeft, 0, 0, new CharFormat(Color3.Yellow)), //Fps counter
			new TuiLabel("aaa", Placement.BottomCenter, 0, 0, new CharFormat(new Color3(255, 120, 0))),
			new TuiButton("Hello!!", Placement.TopLeft, 30, 2, new CharFormat(new Color3(23, 70, 255))),
			new TuiLabel("TestTestTest", Placement.TopLeft, 30, 13),
			new TuiFrame(20, 10, Placement.TopLeft, 50, 7, new CharFormat(new Color3(0, 255, 0))),
			new TuiFramedTextBox("subi", 5, Placement.TopLeft, 80, 10, new CharFormat(Color3.Cyan), new CharFormat(Color3.Magenta)),
			new TuiHorizontalLine(20, 'ñ', Placement.TopLeft, 5, 15, null) //Proof that charachters outside of ascii work as well
		};
		
		AshConsoleGraphics.TuiScreen SmallScreen = new AshConsoleGraphics.TuiScreen(100, 30, new CharFormat(null,  Color3.Black), Elements);
		AshConsoleGraphics.TuiScreen BigScreen = new AshConsoleGraphics.TuiScreen(100, 20, null, null, SmallScreen);
		
		DeltaHelper dh = new DeltaHelper(); //AshLib utility, used for calculating times. Here is used for FPS
		dh.Start();
		
		while(true){
			BigScreen.Print();
			((TuiLabel) Elements[0]).Text = "FPS: " + dh.stableFps; //Update FPS
			
			if(Console.KeyAvailable){
				ConsoleKey k = Console.ReadKey(true).Key;
				if(k == ConsoleKey.A){
					SmallScreen.Xsize = (1 + rand.Next(90)); //Randomize internal screen size
					SmallScreen.Ysize = (1 + rand.Next(16));
					//Add another element
					BigScreen.Elements.Add(new AshConsoleGraphics.TuiLabel("bbb", Placement.TopLeft, rand.Next(100), 1 + rand.Next(19), new CharFormat(new Color3((byte) rand.Next(256), (byte) rand.Next(256), (byte) rand.Next(256)))));
				}else if(k == ConsoleKey.Escape){
					break;
				}
			}
			
			dh.Frame();
		}
	}
}