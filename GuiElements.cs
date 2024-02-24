using System;
using System.Drawing;
using Pastel;
using System.Text;

namespace AshConsoleGraphics
{

public abstract class GuiElement
    {
        protected int X;
        protected int Y;
        protected int Xsize;
        protected int Ysize;
        protected char[,] draw;
        protected Col?[,] dColor;

        public int getX()
        {
            return X;
        }

        public int getY()
        {
            return Y;
        }

        public int getXsize()
        {
            return Xsize;
        }

        public int getYsize()
        {
            return Ysize;
        }
		
		public void checkForExceptions()
		{
			if(this.X < 0 || this.Y < 0){
				throw new Exception("GuiElements position must be non-negative, and it is X:"+this.X+" Y:"+this.Y);
			}
			if(this.Xsize < 1 || this.Ysize < 1){
				throw new Exception("GuiElements size must be non-negative and bigger than 0, and it is X:"+this.Xsize+" Y:"+this.Ysize);
			}
		}

        public char[,] print()
        {
            return this.draw;
        }

        public Col?[,] pCol()
        {
            return this.dColor;
        }
    }

    public class GuiLabel : GuiElement
    {
        public GuiLabel(string text, int X, int Y, Col? c)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = text.Length;
            this.Ysize = 1;
            this.draw = new char[1, text.Length];
            this.dColor = new Col?[1, text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                this.draw[0, i] = text[i];
				dColor[0, i] = c;
            }
			this.checkForExceptions();
        }
		
		public GuiLabel(string text, int X, int Y) : this(text, X, Y, null){}
        
    }
    public class GuiRectangle : GuiElement
    {
        public GuiRectangle(char t, int X, int Y, int Xsize, int Ysize, Col? c)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = Xsize;
            this.Ysize = Ysize;
            this.draw = new char[Ysize, Xsize];
            this.dColor = new Col?[Ysize, Xsize];
            for (int i = 0; i < Ysize; i++)
            {
                for (int j = 0; j < Xsize; j++)
                {
                    this.draw[i, j] = t;
					dColor[i, j] = c;
                }
            }
			this.checkForExceptions();
        }
		
		public GuiRectangle(char t, int X, int Y, int Xsize, int Ysize) : this(t, X, Y, Xsize, Ysize, null){}
		
    }
	
	public class GuiSquare : GuiRectangle
	{
		public GuiSquare(char t, int X, int Y, int size, Col? c) : base(t, X, Y, size, size, c){
			
		}
		public GuiSquare(char t, int X, int Y, int size) : this(t, X, Y, size, null){
			
		}
	}
	
	public class GuiFrame : GuiElement
    {

        public GuiFrame(char t, char u, int X, int Y, int Xsize, int Ysize, Col? C)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = Xsize;
            this.Ysize = Ysize;
			
			this.draw = new char[Ysize, Xsize];
			this.dColor = new Col?[Ysize, Xsize];
			
			for (int j = 0; j < Ysize; j++)
            {
                for (int k = 0; k < Xsize; k++)
                {
                    dColor[j, k] = C;
					draw[j, k] = ' ';
                }
            }
			
			this.draw[0, 0] = u;
            for (int i = 1; i < Xsize - 1; i++)
            {
                this.draw[0, i] = t;
            }
            this.draw[0, Xsize - 1] = u;
			
			for (int i=0; i < Ysize-1; i++){
				this.draw[i+1, 0] = t;
				this.draw[i+1, Xsize-1] = t;
			}
			
			this.draw[Ysize-1, 0] = u;
            for (int i = 1; i < Xsize - 1; i++)
            {
                this.draw[Ysize-1, i] = t;
            }
            this.draw[Ysize-1, Xsize - 1] = u;
			this.checkForExceptions();
        }
		
		public GuiFrame(char t, char u, int X, int Y, int Xsize, int Ysize) : this(t, u, X, Y, Xsize, Ysize, null){}
    }
	
	public class GuiLinedFrame : GuiElement
    {

        public GuiLinedFrame(int X, int Y, int Xsize, int Ysize, Col? C)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = Xsize;
            this.Ysize = Ysize;
			
			this.draw = new char[Ysize, Xsize];
			this.dColor = new Col?[Ysize, Xsize];
			
			for (int j = 0; j < Ysize; j++)
            {
                for (int k = 0; k < Xsize; k++)
                {
                    dColor[j, k] = C;
					draw[j, k] = ' ';
                }
            }
			
			this.draw[0, 0] = '┌';
            for (int i = 1; i < Xsize - 1; i++)
            {
                this.draw[0, i] = '─';
            }
            this.draw[0, Xsize - 1] = '┐';
			
			for (int i=0; i < Ysize-1; i++){
				this.draw[i+1, 0] = '│';
				this.draw[i+1, Xsize-1] = '│';
			}
			
			this.draw[Ysize-1, 0] = '└';
            for (int i = 1; i < Xsize - 1; i++)
            {
                this.draw[Ysize-1, i] = '─';
            }
            this.draw[Ysize-1, Xsize - 1] = '┘';
			this.checkForExceptions();
        }
		
		public GuiLinedFrame(int X, int Y, int Xsize, int Ysize) : this(X, Y, Xsize, Ysize, null){}
		
    }
	
}