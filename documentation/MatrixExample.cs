using System;
using System.Drawing; //We need this to use colors
using AshConsoleGraphics;

namespace MatrixExample{

	class Program
	{
		static void Main()
		{
			//Using AshConsoleGraphics doesn't need to clear the console
	
			List<AshConsoleGraphics.GuiElement> MatrixElements = new List<AshConsoleGraphics.GuiElement>(); //We declare a List of GuiElements so it's easier to add elements. 
			AshConsoleGraphics.GuiScreen MatrixScreen; //We declare the GuiScreen that will be used
	
			while (true) //Forever loop. Press Ctrl+C in the console to get out
			{
	
				int width = Console.WindowWidth; 
				int heigth = Console.WindowHeight;	//We create this variables so if the Console size changes in the middle of the execution, it doesn't crash
				
				for (int j = 0; j < width; j++)
				{
					for (int i = 0; i < heigth; i++) //Nested for loops to loop trough every character
					{
						MatrixElements.Add(new AshConsoleGraphics.GuiLabel(RandomCharacter(), j, i, new AshConsoleGraphics.Col(0, 255, 0)));
						//We add a new GuiLabel that is a random charachter, in the correct postition(j and i), with a green color
					}
				}
				MatrixScreen = new AshConsoleGraphics.GuiScreen(MatrixElements, width, heigth, new AshConsoleGraphics.Col(255, 255, 255)); //We initialize the GuiScreen using the elemnts, the size(width and height), and the default color, that here isn't used
				MatrixScreen.doPrint(); //We print the GuiScreen
				MatrixElements.Clear(); //We clear the Elements list
			}
		}
		
		static string RandomCharacter(){ //Little function to get a random charachter. 3 in 4 chance of being an space
			Random random = new Random();
			if(random.Next(4) == 0) {
				return ((char)random.Next(33, 127)).ToString();
			} else {
				return ' '.ToString();
			}
		}
	}
}