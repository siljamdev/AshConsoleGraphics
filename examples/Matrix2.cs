using System;
using AshLib; //For colors
using AshLib.Time; //For DeltaHelper
using AshLib.Formatting; //For CharFormat
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

//This example might be really slow, depending on the size of the console window. It could also probably be otimized
class Program{
	static Random random = new Random(); //Used for generating the random chars
	
	static DeltaHelper dh = new DeltaHelper();
	
	static void Main(){
		//We declare the Screen that will be used with null colors and no elements
		TuiScreen MatrixScreen = new TuiScreen(0, 0, null);
		
		//We set the screen to resize to the window size
		MatrixScreen.AutoResize = true;
		
		List<TuiLabel> front = new();
		List<TuiLabel> middle = new();
		List<TuiLabel> back = new();
		
		//On resize, generate all elements again
		MatrixScreen.OnResize += ((s, args) => {
			MatrixScreen.Elements.Clear();
			front.Clear();
			middle.Clear();
			back.Clear();
		});
		
		dh.Start();
		
		//counter
		int j = 0;
		
		while(true){
			MatrixScreen.Print();
			
			//Each loop, move each charachter down and if some pass, delete them. Then, add a new row at the top
			for(int i = 0; i < front.Count; i++){
				TuiLabel l = front[i];
				l.OffsetY++;
				if(l.OffsetY >= MatrixScreen.Ysize){
					MatrixScreen.Elements.Remove(l);
					front.RemoveAt(i);
					i--;
				}
			}
			
			if(j % 3 == 0){
				for(int i = 0; i < middle.Count; i++){
					TuiLabel l = middle[i];
					l.OffsetY++;
					if(l.OffsetY >= MatrixScreen.Ysize){
						MatrixScreen.Elements.Remove(l);
						middle.RemoveAt(i);
						i--;
					}
				}
			}
			
			if(j % 5 == 0){
				for(int i = 0; i < back.Count; i++){
					TuiLabel l = back[i];
					l.OffsetY++;
					if(l.OffsetY >= MatrixScreen.Ysize){
						MatrixScreen.Elements.Remove(l);
						back.RemoveAt(i);
						i--;
					}
				}
			}
			
			//Generate new falling leters
			for(int i = 0; i < MatrixScreen.Xsize; i++){
				switch(random.Next(500)){
					case 0:
					case 1:
					string c = RandomChar();
					TuiLabel l = new TuiLabel(c, Placement.TopLeft, i, 0, new CharFormat(new Color3(0, 230, 0)));
					MatrixScreen.Elements.Add(l);
					front.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -1, new CharFormat(new Color3(0, 160, 0)));
					MatrixScreen.Elements.Add(l);
					front.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -2, new CharFormat(new Color3(0, 90, 0)));
					MatrixScreen.Elements.Add(l);
					front.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -3, new CharFormat(new Color3(0, 40, 0)));
					MatrixScreen.Elements.Add(l);
					front.Add(l);
					break;
					
					case 2:
					c = RandomChar();
					l = new TuiLabel(c, Placement.TopLeft, i, 0, new CharFormat(new Color3(0, 200, 0)));
					MatrixScreen.Elements.Add(l);
					middle.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -1, new CharFormat(new Color3(0, 130, 0)));
					MatrixScreen.Elements.Add(l);
					middle.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -2, new CharFormat(new Color3(0, 80, 0)));
					MatrixScreen.Elements.Add(l);
					middle.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -3, new CharFormat(new Color3(0, 30, 0)));
					MatrixScreen.Elements.Add(l);
					middle.Add(l);
					break;
					
					case 3:
					c = RandomChar();
					l = new TuiLabel(c, Placement.TopLeft, i, 0, new CharFormat(new Color3(0, 180, 0)));
					MatrixScreen.Elements.Add(l);
					back.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -1, new CharFormat(new Color3(0, 120, 0)));
					MatrixScreen.Elements.Add(l);
					back.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -2, new CharFormat(new Color3(0, 70, 0)));
					MatrixScreen.Elements.Add(l);
					back.Add(l);
					
					l = new TuiLabel(c, Placement.TopLeft, i, -3, new CharFormat(new Color3(0, 30, 0)));
					MatrixScreen.Elements.Add(l);
					back.Add(l);
					break;
				}
			}
			
			j++;
			
			dh.TargetLazy(20);
			dh.Frame();
		}
	}
	
	static string RandomChar(){ //Little function to get a random charachter
		return ((char)random.Next(33, 127)).ToString();
	}
}