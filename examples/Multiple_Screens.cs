using System;
using System.Text;
using AshLib; //For Colors
using AshLib.Formatting; //For CharFormat
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

class Program{
	
	static Random rand = new Random();
	
	static void Main(){

		Console.CursorVisible = false;

		string[] words = { //Some words for random generation
			"hello", "never", "no", "yes", "bacon", "pig", "gloria", "order", "emperor", "hierarchy", "path", "file", "folder",
			"project", "hub", "ship", "space", "king", "rifle", "ammunition", "soldier", "war", "krieg", "windows", "linux",
			"version", "failed", "corrupted", "cancelled", "affirmative", "propose", "answer", "client", "connect", "server",
			"slave", "worker", "state", "viktor", "century", "millisecond", "double", "broker", "tsoy", "exception", "lord",
			"medieval", "partition", "resistance", "adder", "assembler", "type", "first", "second", "third", "fourth", "fifth"
		};
		
		//Declarations needed because the actions use them
		TuiScreenInteractive LeftScreen = null;
		TuiScreenInteractive RightScreen = null;
		TuiScreenInteractive AlternateScreen = null;
		MultipleTuiScreenInteractive BigScreen = null;
		
		CharFormat selected = new CharFormat(null, null, 1, null, null, false, null, false); //underlined
		
		TuiSelectable[,] LeftElements = null;
		LeftElements = new TuiSelectable[,]{
			{new TuiFramedTextBox("", 16, Placement.Center, 0, -3, null, null, null, new CharFormat(null, null, 1, null, Color3.Yellow, false, null, false), new CharFormat(Color3.Yellow))},
			{new TuiButton("Set color", Placement.Center, 0, 1, null, selected, new CharFormat(Color3.Yellow)).SetAction(setColor)},
			{new TuiButton("Change screen", Placement.Center, 0, 4, null, selected, new CharFormat(Color3.Yellow)).SetAction(goToOther2)}
		};
		
		TuiSelectable[,] RightElements = null;
		RightElements = new TuiSelectable[,]{
			{new TuiButton("Change screen", Placement.Center, 0, 1, null, selected, new CharFormat(Color3.Yellow)).SetAction(goToOther)},
			{new TuiButton("Change to another", Placement.Center, 0, 2, null, selected, new CharFormat(Color3.Yellow)).SetAction(goToAlternate)},
			{new TuiButton("Useless button that does nothing", Placement.Center, 0, 4, null, selected, new CharFormat(Color3.Yellow)).SetAction(joke)},
			{new TuiFramedCheckBox(' ', 'X', false, Placement.Center, 8, 7, null, null, null, new CharFormat(null, null, 1, null, Color3.Yellow, false, null, false), new CharFormat(Color3.Yellow))},
			{new TuiFramedRadio(' ', 'X', "Left: ", "Right: ", Placement.Center, 0, 10, null, null, null, new CharFormat(null, null, 1, null, Color3.Yellow, false, null, false), null, null, new CharFormat(Color3.Yellow))},
		};
		
		TuiSelectable[,] AlternateElements = null;
		AlternateElements = new TuiSelectable[13, 12];
		
		TuiButton change = new TuiButton("Change to alternate", Placement.TopCenter, 0, 1, null, selected, new CharFormat(Color3.Yellow)).SetAction(goBack);
		
		//The entire top row
		for(int i = 0; i < 12; i++){
			AlternateElements[0, i] = change;
		}
		
		//Populate the matrix
		for(int i = 0; i < 143; i++){
			AlternateElements[1 + i / 12, i % 12] = new TuiFramedCheckBox(' ', 'X', rand.Next(7) != 1, Placement.TopLeft, 6 * (i % 12), 20 + 4 * (i / 12), null, null, null, new CharFormat(null, null, 1, null, Color3.Yellow, false, null, false), new CharFormat(Color3.Yellow));
		}
		
		LeftScreen = new TuiScreenInteractive(50,20, LeftElements, 0, 1, new CharFormat(new Color3(255, 100, 0), new Color3(30, 0, 0)), new TuiLabel("Enter color:", Placement.Center, -2, -5, null));
		RightScreen = new TuiScreenInteractive(50,20, RightElements, 0, 0, Placement.TopRight, 0, 0, new CharFormat(new Color3(150, 0, 255), new Color3(0, 0, 30)),
												new TuiLog(48, 9, Placement.TopCenter, 0, 1, new CharFormat(Color3.White, Color3.Black)),
												new TuiLabel("Stop generating:", Placement.Center, -2, 7, null));
		AlternateScreen = new TuiScrollingScreenInteractive(50,20, AlternateElements, 0, 0, Placement.TopRight, 0, 0, new CharFormat(new Color3(0, 255, 0), new Color3(0, 30, 0)));
		
		BigScreen = new MultipleTuiScreenInteractive(101, 20, new TuiScreenInteractive[]{RightScreen, LeftScreen}, null, null, new TuiVerticalLine(20, '│', Placement.TopCenter, 0, 0, new CharFormat(Color3.Yellow)));
		
		//Need to set the first because default is none
		BigScreen.SelectedScreen = LeftScreen;
		
		BigScreen.AutoResize = true;
		//Resize logic is most elements resize accordingly
		BigScreen.OnResize += (s, args) => {
			Console.CursorVisible = false;
			
			LeftScreen.Xsize = args.X / 2 - 1;
			LeftScreen.Ysize = args.Y;
			
			RightScreen.Xsize = args.X / 2 - 1;
			RightScreen.Ysize = args.Y;
			
			AlternateScreen.Xsize = args.X / 2 - 1;
			AlternateScreen.Ysize = args.Y;
			
			((TuiLog) RightScreen.Elements[0]).Xsize = RightScreen.Xsize - 4;
			((TuiLog) RightScreen.Elements[0]).Ysize = RightScreen.Ysize / 2 - 1;
			
			((TuiVerticalLine) BigScreen.Elements[0]).Ysize = BigScreen.Ysize;
		};
		
		BigScreen.SubKeyEvent(ConsoleKey.W, (s, ck) => {
			((TuiLog) RightScreen.Elements[0]).Scroll++;
		});
		
		BigScreen.SubKeyEvent(ConsoleKey.S, (s, ck) => {
			((TuiLog) RightScreen.Elements[0]).Scroll--;
		});
		
		int j = 0;
		
		int c = 0;
		
		//Add words to the log
		BigScreen.FinishPlayCycleEvent = (sb) => {
			if(((TuiFramedCheckBox) RightElements[3,0]).Checked){
				return;
			}
			
			j++;
			if(j == 10){
				j = 0;
				((TuiLog) RightScreen.Elements[0]).Append(generateContent(rand));
			}
			
			if(c == 143){
				return; //Do it only once
			}
			
			//Count how many are checked
			for(int i = 0; i < 143; i++){
				if(((TuiFramedCheckBox)AlternateElements[1 + i / 12, i % 12]).Checked){
					c++;
				}
			}
			
			if(c == 143){
				((TuiWritable) LeftElements[0,0]).Text = "Good job!";
			}else{
				c = 0;
			}
		};
		
		BigScreen.Play();
		
		void setColor(TuiSelectable e, ConsoleKeyInfo ck){
			string t = ((TuiWritable) LeftElements[0,0]).Text;
			dynamic g = e;
			if(t == "random"){
				g.TextFormat = new CharFormat(new Color3((byte) rand.Next(256), (byte) rand.Next(256), (byte) rand.Next(256)));
				g.SelectedTextFormat = g.TextFormat;
			}else if(Color3.TryParse(t, out Color3 c)){
				g.TextFormat = new CharFormat(c);
				g.SelectedTextFormat = g.TextFormat;
			}
		}
		
		void joke(TuiSelectable e, ConsoleKeyInfo ck){
			((TuiWritable) LeftElements[0,0]).Text = "SIKE!";
		}
		
		void goToOther(TuiSelectable e, ConsoleKeyInfo cki){
			BigScreen.SelectedScreen = LeftScreen;
		}
		
		void goToOther2(TuiSelectable e, ConsoleKeyInfo ck){
			BigScreen.SelectedScreen = RightScreen;
		}
		
		void goToAlternate(TuiSelectable e, ConsoleKeyInfo cki){
			//Remove the other one
			BigScreen.ScreenList.Remove(RightScreen);
			BigScreen.Elements.Remove(RightScreen);
			
			//Add the new one
			BigScreen.ScreenList.Add(AlternateScreen);
			BigScreen.Elements.Add(AlternateScreen);
			
			BigScreen.SelectedScreen = AlternateScreen;
		}
		
		void goBack(TuiSelectable e, ConsoleKeyInfo cki){
			BigScreen.ScreenList.Remove(AlternateScreen);
			BigScreen.Elements.Remove(AlternateScreen);
			BigScreen.ScreenList.Add(RightScreen);
			BigScreen.Elements.Add(RightScreen);
			BigScreen.SelectedScreen = LeftScreen;
		}
		
		string generateContent(Random ran){
			StringBuilder sb = new StringBuilder();
			
			int m = 1;
			for(int i = 0; i < m; i++){
				sb.Append(words[ran.Next(words.Length)]);
				int h = ran.Next(18);
				if(h == 0){
					sb.Append(Environment.NewLine);
					continue;
				}else if(h == 1){
					sb.Append(", ");
					continue;
				}else if(h == 2){
					sb.Append(". ");
					continue;
				}
				else if(h == 3){
					sb.Append("." + Environment.NewLine);
					continue;
				}
				sb.Append(" ");
			}
			
			return sb.ToString();
		}
		
		Color3 generateColor(Random ran){
			return new Color3((byte) ran.Next(256), (byte) ran.Next(256), (byte) ran.Next(256));
		}
	}
}