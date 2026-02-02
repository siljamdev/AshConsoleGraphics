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
		
		TuiSelectable.LeftSelector = '[';
		TuiSelectable.RightSelector = ']';

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
			{new TuiFramedTextBox("", 16, Placement.Center, 0, -3, null, null, null, new CharFormat(null, null, 0, null, Color3.Yellow, false, null, false), new CharFormat(Color3.Yellow), new CharFormat(Color3.Yellow))},
			{new TuiButton("Set color", Placement.Center, 0, 1, null, selected, new CharFormat(Color3.Yellow)).SetAction(setColor)},
			{new TuiButton("Change screen", Placement.Center, 0, 4, null, selected, new CharFormat(Color3.Yellow)).SetAction(goToOther2)}
		};
		
		TuiSelectable[,] RightElements = null;
		RightElements = new TuiSelectable[,]{
			{new TuiButton("Change screen", Placement.Center, 0, 1, null, selected, new CharFormat(Color3.Yellow)).SetAction(goToOther)},
			{new TuiButton("Change to another", Placement.Center, 0, 2, null, selected, new CharFormat(Color3.Yellow)).SetAction(goToAlternate)},
			{new TuiButton("Useless button that does nothing", Placement.Center, 0, 4, null, selected, new CharFormat(Color3.Yellow)).SetAction(joke)},
			{new TuiFramedCheckBox(' ', 'X', false, Placement.Center, 8, 7, null, null, null, new CharFormat(null, null, 0, null, Color3.Yellow, false, null, false), new CharFormat(Color3.Yellow))},
			{new TuiSlider(16, '—', '@', 0.04f, 0.5f, Placement.Center, 0, 11, null, null, null, new CharFormat(null, null, 0, null, Color3.Yellow, false, null, false), new CharFormat(Color3.Yellow))},
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
			AlternateElements[1 + i / 12, i % 12] = new TuiFramedCheckBox(' ', 'X', rand.Next(7) != 1, Placement.TopLeft, 6 * (i % 12), 20 + 4 * (i / 12), null, null, null, new CharFormat(null, null, 0, null, Color3.Yellow, false, null, false), new CharFormat(Color3.Yellow));
		}
		
		LeftScreen = new TuiScreenInteractive(50,20, LeftElements, 0, 1, new CharFormat(new Color3(255, 100, 0), new Color3(30, 0, 0)),
			new TuiLabel("Enter color:", Placement.Center, -2, -5, null)
		);
		RightScreen = new TuiScreenInteractive(50,20, RightElements, 0, 0, Placement.TopRight, 0, 0, new CharFormat(new Color3(150, 0, 255), new Color3(0, 0, 30)),
			new TuiFormatLog(48, 9, Placement.TopCenter, 0, 1, new CharFormat(Color3.White, Color3.Black)),
			new TuiLabel("Stop generating:", Placement.Center, -2, 7, null),
			new TuiLabel("Generation speed:", Placement.Center, 0, 10, null)
		);
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
			
			((TuiFormatLog) RightScreen.Elements[0]).Xsize = RightScreen.Xsize - 4;
			((TuiFormatLog) RightScreen.Elements[0]).Ysize = RightScreen.Ysize / 2 - 1;
			
			((TuiVerticalLine) BigScreen.Elements[0]).Ysize = BigScreen.Ysize;
		};
		
		RightScreen.SubKeyEvent(ConsoleKey.W, (s, ck) => {
			((TuiFormatLog) RightScreen.Elements[0]).Scroll++;
		});
		
		RightScreen.SubKeyEvent(ConsoleKey.S, (s, ck) => {
			((TuiFormatLog) RightScreen.Elements[0]).Scroll--;
		});
		
		RightScreen.SubKeyEvent(ConsoleKey.Spacebar, (s, ck) => {
			((TuiFormatLog) RightScreen.Elements[0]).ScrollToTop();
		});
		
		int j = 0;
		
		int d = 0;
		
		int c = 0;
		
		//Add words to the log
		BigScreen.OnFinishPlayCycle = (sb, a) => {
			//Blinking cursor effect
			d++;
			if(d >= 100){
				if(TuiWritable.Cursor == '_'){
					TuiWritable.Cursor = ' ';
				}else{
					TuiWritable.Cursor = '_';
				}
				d = 0;
			}
			
			if(c != 143){ //Do it only once
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
			}
			
			//Dont generate
			if(((TuiFramedCheckBox) RightElements[3,0]).Checked){
				return;
			}
			
			//Limit speed of generation
			j++;
			if(j >= (int) (150f - 145f*((TuiSlider) RightElements[4,0]).Filled)){
				j = 0;
				((TuiFormatLog) RightScreen.Elements[0]).Append(generateContent(rand), new CharFormat(new Color3((byte) rand.Next(255), (byte) rand.Next(255), (byte) rand.Next(255))));
			}
		};
		
		BigScreen.Play();
		
		void setColor(TuiSelectable e, ConsoleKeyInfo ck){
			string t = ((TuiWritable) LeftElements[0,0]).Text;
			dynamic g = e;
			if(t == "random"){
				Color3 coll = new Color3((byte) rand.Next(256), (byte) rand.Next(256), (byte) rand.Next(256));
				g.TextFormat = new CharFormat(coll);
				g.SelectedTextFormat = new CharFormat(null, null, 1, null, coll, false, null, false);
			}else if(Color3.TryParse(t, out Color3 c)){
				g.TextFormat = new CharFormat(c);
				g.SelectedTextFormat = new CharFormat(null, null, 1, null, c, false, null, false);
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
				int h = ran.Next(20);
				if(h == 0){
					sb.Append(Environment.NewLine);
					continue;
				}else if(h == 1){
					sb.Append(", ");
					continue;
				}else if(h == 2){
					sb.Append(". ");
					continue;
				}else if(h == 3){
					sb.Append("." + Environment.NewLine);
					continue;
				}else if(h == 4){
					sb.Append("." + Environment.NewLine + Environment.NewLine);
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