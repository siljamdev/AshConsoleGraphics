using System;
using System.Diagnostics;
using System.IO;
using AshLib; //For colors
using AshLib.Formatting; //For CharFormat
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

class Program{
	static void Main(){
		Console.CursorVisible = false; //That way we wont see the cursor flashing
		
		//The input box
		TuiFramedScrollingTextBox input = new TuiFramedScrollingTextBox("", 256, 32, Placement.BottomCenter, 0, 1, new CharFormat(new Color3(255, 100, 0)), new CharFormat(new Color3(255, 100, 0)), new CharFormat(new Color3(255, 100, 0)), new CharFormat(Color3.Yellow), new CharFormat(Color3.Yellow));
		
		//The output log
		TuiLog output = new TuiLog(98, 9, Placement.TopCenter, 0, 1, new CharFormat(new Color3(255, 100, 0), Color3.Black));
		
		//The matrix, that only has one element
		TuiSelectable[,] elements = new TuiSelectable[,]{
			{input}
		};
		
		//The interactive screen
		TuiScreenInteractive screen = new TuiScreenInteractive(100, 20, elements, 0, 0, new CharFormat(null, new Color3(30, 0, 0)), output);
		
		screen.AutoResize = true; //If the console window resizes, so will the screen to fit all
		
		//This will be called on resize
		screen.OnResize = (gs) => {
			Console.CursorVisible = false; //When window resizes, cursor pops back up
			
			//Adjust sizes
			
			input.BoxXsize = gs.Xsize - 4;
			
			output.Xsize = gs.Xsize - 6;
			output.Ysize = gs.Ysize - 6;
		};
		
		//The cmd process
		Process commandLine = new Process{
			StartInfo = new ProcessStartInfo{
				FileName = "cmd.exe",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};
		
		//Redirect output to the log
		commandLine.OutputDataReceived += (s, e) => {
			if(e.Data != null){
				output.AppendLine(e.Data);	
			}else{
				output.AppendLine("");
			}
		};
		
		//Redirect error output to the log
		commandLine.ErrorDataReceived += (s, e) => {
			if(e.Data != null){
				output.AppendLine(e.Data);
			}else{
				output.AppendLine("");
			}
		};
		
		//Start the process
		commandLine.Start();
		//Input writer
		StreamWriter inputWriter = commandLine.StandardInput;
		commandLine.BeginOutputReadLine();
		commandLine.BeginErrorReadLine();
		
		//Enter command with enter
		input.SubKeyEvent(ConsoleKey.Enter, (s, ck) => {
			inputWriter.WriteLine(input.Text);
			inputWriter.Flush();
			input.Text = "";
		});
		
		commandLine.EnableRaisingEvents = true;
		
		//Stop the screen(thus exiting the program) when the process exits
		commandLine.Exited += (s, e) => {
			screen.Stop();
		};
		
		//Play the screen
		screen.Play();
	}
}