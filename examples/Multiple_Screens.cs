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
			"slave", "worker", "state", "washer", "century", "millisecond", "double", "broker", "crypto", "exception", "lord",
			"medieval", "partition", "resistance", "adder", "assembler", "type", "first", "second", "third", "fourth", "fifth"
		};
		
		//Declarations needed because the actions use them
		TuiScreenInteractive LeftScreen = null;
		TuiScreenInteractive RightScreen = null;
		MultipleTuiScreenInteractive BigScreen = null;
		
		TuiSelectable[,] LeftElements = null;
		LeftElements = new TuiSelectable[,]{
			{new TuiFramedTextBox("", 16, Placement.Center, 0, -3, new CharFormat(new Color3(255, 100, 0)), new CharFormat(new Color3(255, 100, 0)), new CharFormat(new Color3(255, 100, 0)), new CharFormat(Color3.Yellow), new CharFormat(Color3.Yellow))},
			{new TuiButton("Set color", Placement.Center, 0, 1, new CharFormat(new Color3(255, 100, 0)), new CharFormat(Color3.Yellow)).SetAction(setColor)},
			{new TuiButton("Change screen", Placement.Center, 0, 4, new CharFormat(new Color3(255, 100, 0)), new CharFormat(Color3.Yellow)).SetAction(goToOther2)}
		};
		
		TuiSelectable[,] RightElements = null;
		RightElements = new TuiSelectable[,]{
			{new TuiButton("Change screen", Placement.Center, 0, 1, new CharFormat(new Color3(150, 0, 255)), new CharFormat(Color3.Yellow)).SetAction(goToOther)},
			{new TuiButton("Useless button that does nothing", Placement.Center, 0, 4, new CharFormat(new Color3(150, 0, 255)), new CharFormat(Color3.Yellow)).SetAction(joke)},
			{new TuiFramedCheckBox(' ', 'X', false, Placement.Center, 9, 7, new CharFormat(new Color3(150, 0, 255)), new CharFormat(new Color3(150, 0, 255)), new CharFormat(new Color3(150, 0, 255)), new CharFormat(Color3.Yellow), new CharFormat(Color3.Yellow))},
		};
		
		LeftScreen = new TuiScreenInteractive(50,20, LeftElements, 0, 1, new CharFormat(null, new Color3(30, 0, 0)), new TuiLabel("Enter color:", Placement.Center, -2, -5, new CharFormat(new Color3(255, 100, 0))));
		RightScreen = new TuiScreenInteractive(50,20, RightElements, 0, 0, Placement.TopRight, 0, 0, new CharFormat(null, new Color3(0, 0, 30)),
												new TuiLog(48, 9, Placement.TopCenter, 0, 1, new CharFormat(Color3.White, Color3.Black)),
												new TuiLabel("Stop generating:", Placement.Center, -2, 7, new CharFormat(new Color3(150, 0, 255))));
		
		BigScreen = new MultipleTuiScreenInteractive(101, 20, new TuiScreenInteractive[]{RightScreen, LeftScreen}, null, null, new TuiVerticalLine(20, '│', Placement.TopCenter, 0, 0, new CharFormat(Color3.Yellow)));
		
		//Need to set the first because default is none
		BigScreen.SelectedScreen = LeftScreen;
		
		BigScreen.AutoResize = true;
		//Resize logic is most elements resize accordingly
		BigScreen.OnResize = (gs) => {
			Console.CursorVisible = false;
			
			LeftScreen.Xsize = BigScreen.Xsize / 2 - 1;
			LeftScreen.Ysize = BigScreen.Ysize;
			
			RightScreen.Xsize = BigScreen.Xsize / 2 - 1;
			RightScreen.Ysize = BigScreen.Ysize;
			
			((TuiLog) RightScreen.Elements[0]).Xsize = RightScreen.Xsize - 2;
			((TuiLog) RightScreen.Elements[0]).Ysize = RightScreen.Ysize / 2 - 1;
			
			((TuiVerticalLine) BigScreen.Elements[0]).Ysize = BigScreen.Ysize;
		};
		
		int j = 0;
		
		//Add words to the log
		BigScreen.FinishPlayCycleEvent = (sb) => {
			if(((TuiFramedCheckBox) RightElements[2,0]).Checked){
				return;
			}
			
			j++;
			if(j == 10){
				j = 0;
				((TuiLog) RightScreen.Elements[0]).Append(generateContent(rand));
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
		
		string generateContent(Random ran){
			StringBuilder sb = new StringBuilder();
			
			int m = 1;
			for(int i = 0; i < m; i++){
				sb.Append(words[ran.Next(words.Length)]);
				int h = ran.Next(24);
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
	}
}