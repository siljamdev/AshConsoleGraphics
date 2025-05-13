using System;
using AshLib; //For colors
using AshLib.Formatting; //For CharFormat
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

//This example might be really slow, depending on the size of the console window. It could also probably be otimized
class Program{
	static Random random = new Random(); //Used for generating the random chars
	
	static void Main(){
		//We declare the Screen that will be used with null colors and no elements
		TuiScreenInteractive MatrixScreen = new TuiScreenInteractive(0, 0, null, 0, 0, null, null);
		
		//We set the screen to resize to the window size
		MatrixScreen.AutoResize = true;
		
		//On resize, generate all elements again
		MatrixScreen.OnResize = ((gs) => {
			MatrixScreen.Elements.Clear();
		});
		
		//Each loop, move each charachter down and if some pass, delete them. Then, add a new row at the top
		MatrixScreen.FinishPlayCycleEvent = ((gs) => {
			for(int i = 0; i < MatrixScreen.Elements.Count; i++){
				TuiLabel l = (TuiLabel) MatrixScreen.Elements[i];
				l.OffsetY++;
				if(l.OffsetY >= MatrixScreen.Ysize){
					MatrixScreen.Elements.RemoveAt(i);
					i--;
				}
			}
			
			for(int i = 0; i < MatrixScreen.Xsize; i++){
				if(random.Next(4) == 0){
					MatrixScreen.Elements.Add(new TuiLabel(RandomChar(), Placement.TopLeft, i, 0, new CharFormat(new Color3(0, (byte) (100 + random.Next(155)), 0))));
				}
			}
		});
		
		//Play the screen, this is for allowing escape
		MatrixScreen.Play();
	}
	
	static string RandomChar(){ //Little function to get a random charachter. 3 in 4 chance of being an space
		return ((char)random.Next(33, 127)).ToString();
	}
}