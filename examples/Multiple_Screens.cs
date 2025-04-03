using System;
using System.Text;
using AshLib;
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

class Program{
	
	Random rand = new Random();
	
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
			{new TuiFramedTextBox("", 16, Placement.Center, 0, -3, new Color3(255, 100, 0), null, new Color3(255, 100, 0), null, new Color3(255, 100, 0), null, Color3.Yellow, null, Color3.Yellow, null)},
			{new TuiButton("Set color", Placement.Center, 0, 1, new Color3(255, 100, 0), null, Color3.Yellow, null).SetAction(setColor)},
			{new TuiButton("Change screen", Placement.Center, 0, 4, new Color3(255, 100, 0), null, Color3.Yellow, null).SetAction(goToOther2)}
		};
		
		TuiSelectable[,] RightElements = null;
		RightElements = new TuiSelectable[,]{
			{new TuiButton("Change screen", Placement.Center, 0, 1, new Color3(150, 0, 255), null, Color3.Yellow, null).SetAction(goToOther)},
			{new TuiButton("Useless button that does nothing", Placement.Center, 0, 4, new Color3(150, 0, 255), null, Color3.Yellow, null).SetAction(joke)},
			{new TuiFramedCheckBox(' ', 'X', false, Placement.Center, 9, 7, new Color3(150, 0, 255), null, new Color3(150, 0, 255), null, new Color3(150, 0, 255), null, Color3.Yellow, null, Color3.Yellow, null)},
		};
		
		LeftScreen = new TuiScreenInteractive(50,20, LeftElements, 0, 1, null, new Color3(30, 0, 0), new TuiLabel("Enter color:", Placement.Center, -2, -5, new Color3(255, 100, 0), null));
		RightScreen = new TuiScreenInteractive(50,20, RightElements, 0, 0, Placement.TopRight, 0, 0, null, new Color3(0, 0, 30),
												new TuiLog(48, 9, Placement.TopCenter, 0, 1, Color3.White, Color3.Black),
												new TuiLabel("Stop generating:", Placement.Center, -2, 7, new Color3(150, 0, 255), null));
		
		BigScreen = new MultipleTuiScreenInteractive(101, 20, new TuiScreenInteractive[]{RightScreen, LeftScreen}, null, null, new TuiVerticalLine(20, '│', Placement.TopCenter, 0, 0, Color3.Yellow, null));
		
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
				g.FgTextColor = new Color3((byte) rand.Next(256), (byte) rand.Next(256), (byte) rand.Next(256));
				g.FgTextSelectedColor = g.FgTextColor;
			}else if(Color3.TryParse(t, out Color3 c)){
				g.FgTextColor = c;
				g.FgTextSelectedColor = g.FgTextColor;
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