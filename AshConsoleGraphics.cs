using System;
using System.Drawing;
using Pastel;
using System.Text;

namespace AshConsoleGraphics
{
	
	public static class FastConsole
{
    static readonly BufferedStream str;

    static FastConsole()
    {
        Console.OutputEncoding = Encoding.Unicode;

        str = new BufferedStream(Console.OpenStandardOutput(), 0x15000);
    }

    public static void Write(String s)
    {
        var rgb = new byte[s.Length << 1];
        Encoding.Unicode.GetBytes(s, 0, s.Length, rgb, 0);

        lock (str)   // (optional, can omit if appropriate)
            str.Write(rgb, 0, rgb.Length);
    }

    public static void Flush() { lock (str) str.Flush(); }
};

    public class GuiScreen
    {
		protected List<GuiElement> elements = new List<GuiElement>();
        protected char[,] draw;
        protected Color[,] dColor;
        int length;
        int height;
        protected Color defCol;

        public GuiScreen(List<GuiElement> e, int l, int h, Color d)
        {
            this.elements = e;
            this.length = l;
            this.height = h;
            this.defCol = d;
        }

        public void preparePrint()
        {
            this.draw = new char[height, length];
            this.dColor = new Color[height, length];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    draw[i, j] = ' ';
                    dColor[i, j] = defCol;
                }
            }
            for (int i = 0; i < this.elements.Count; i++)
            {
                GuiElement e = this.elements[i];
				
                for (int j = 0; j < e.getYsize(); j++)
                {
                    for (int k = 0; k < e.getXsize(); k++)
                    {
                        this.draw[e.getY() + j, e.getX() + k] = e.print()[j, k];
                        this.dColor[e.getY() + j, e.getX() + k] = e.pCol()[j, k];
                    }
                }
            }
        }

        public void doPrint()
        {
            preparePrint();
            StringBuilder final = new StringBuilder();
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.length; j++)
                {
                    if (this.draw[i, j] == ' '){
						final.Append(this.draw[i, j].ToString());
					} else {
						final.Append(this.draw[i, j].ToString().Pastel(this.dColor[i, j]));
					}
					
                }
                final.AppendLine("");
            }
            Console.SetCursorPosition(0, 0);
			FastConsole.Write(final.ToString());
            //Console.Out.WriteLine(final.ToString());
			FastConsole.Flush();
        }

        public void setElem(int index, GuiElement e)
        {
            elements[index] = e;
        }
		
		public GuiElement getElem(int index){
			return this.elements[index];
		}
    }

    public class GuiScreenB : GuiScreen{
		protected int bSelected = 0;
		protected int[,] bNav;
		protected int bX;
		protected int bY;
		protected Dictionary<int, Action> bFunctions = new Dictionary<int, Action>();
		
		public GuiScreenB(List<GuiElement> e, int l, int h, int[,] bn, int startX, int startY, Color c) : base(e,l,h, c){
			this.bNav = bn;
			this.bX = startX;
			this.bY = startY;
			if (this.elements[getIndexOfB(bNav[this.bX,this.bY])] is GuiSelectable){
				this.bSelected = getIndexOfB(bNav[this.bX,this.bY]);
				setSel(true);
			}
		}
		
		
		public void subBevent(int bIndex, Action bFunction)
		{
			bFunctions[bIndex] = bFunction;
		}
		
		public void bPressed(int bIndex)
		{
			if (bFunctions.ContainsKey(bIndex))
			{
				Action bFunction = bFunctions[bIndex];
				bFunction.Invoke();
			}
		}
		
		public void move(int dir){
			if (dir==0){
				if(this.bX+1 < bNav.GetLength(1)){
					setSel(false);
					this.bX++;
					this.bSelected = getIndexOfB(bNav[this.bY,this.bX]);
					setSel(true);
				}
			} else if (dir==1){
				if(this.bX != 0){
					setSel(false);
					this.bX--;
					this.bSelected = getIndexOfB(bNav[this.bY,this.bX]);
					setSel(true);
				}
			} else if (dir==3){
				if(this.bY+1 < bNav.GetLength(0)){
					setSel(false);
					this.bY++;
					this.bSelected = getIndexOfB(bNav[this.bY,this.bX]);
					setSel(true);
				}
			} else if (dir==2){
				if(this.bY != 0){
					setSel(false);
					this.bY--;
					this.bSelected = getIndexOfB(bNav[this.bY,this.bX]);
					setSel(true);
				}
			}
		}
		
		public int bGetInd(){
			if (this.elements[bSelected] is GuiSelectable b){
				return b.getIndex();
			}
			return 0;
		}
		
		public void setSel(bool bn){
			if (this.elements[bSelected] is GuiSelectable b){
				//dynamic b = this.elements[bSelected];
				b.setSel(bn);
			}
		}
		
		public int getIndexOfB(int n){
			for (int i = 0; i < this.elements.Count; i++){
				if(this.elements[i] is GuiSelectable b){
					if(b.getIndex() == n){
						return i;
					}
				} else {
					continue;
				}
			}
			return 0;
		}
		
		public void tbWrite(char c){
			if (this.elements[bSelected] is GuiFramedTextBox f){
				if (f.getText().Length == f.getXsize()-4){
					return;
				}
				f.setText(f.getText()+c);
				f.updateText();
			}
		}
		
		public void tbDel(){
			if (this.elements[bSelected] is GuiFramedTextBox f){
				if (f.getText().Length == 0){
					return;
				}
				f.setText(f.getText().Substring(0,f.getText().Length-1));
				f.updateText();
			}
		}
		
		public void play() {
			bool controller = true;
			while(controller){
				this.doPrint();
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);
	
				switch (keyInfo.Key)
				{
					case ConsoleKey.UpArrow:
						this.move(2);
						break;
					case ConsoleKey.DownArrow:
						this.move(3);
						break;
					case ConsoleKey.LeftArrow:
						this.move(1);
						break;
					case ConsoleKey.RightArrow:
						this.move(0);
						break;	
					case ConsoleKey.Enter:
						this.bPressed(this.bGetInd());
						break;
					case ConsoleKey.Escape:
						return;
						break;
					case ConsoleKey.Backspace:
						this.tbDel();
						break;
					default:
						if (Char.IsLetterOrDigit(keyInfo.KeyChar) || Char.IsPunctuation(keyInfo.KeyChar) || keyInfo.KeyChar == ' '){
							this.tbWrite(keyInfo.KeyChar);
						}
						break;
				}
			}
			return;
		}
	}
	
    public class GuiElement
    {
        protected int X;
        protected int Y;
        protected int Xsize;
        protected int Ysize;
        protected char[,] draw;
        protected Color[,] dColor;

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
				throw new Exception("GuiElements position must be non-negative");
			}
			if(this.Xsize < 1 || this.Ysize < 1){
				throw new Exception("GuiElements size must be non-negative and bigger than 0");
			}
		}

        public char[,] print()
        {
            return this.draw;
        }

        public Color[,] pCol()
        {
            return this.dColor;
        }
    }

    public class GuiLabel : GuiElement
    {
        public GuiLabel(string text, int X, int Y, Color c)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = text.Length;
            this.Ysize = 1;
            this.draw = new char[1, text.Length];
            this.dColor = new Color[1, text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                this.draw[0, i] = text[i];
            }
            for (int i = 0; i < Xsize; i++)
            {
                dColor[0, i] = c;
            }
			this.checkForExceptions();
        }
    }
    public class GuiRectangle : GuiElement
    {
        public GuiRectangle(char t, int X, int Y, int Xsize, int Ysize, Color c)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = Xsize;
            this.Ysize = Ysize;
            this.draw = new char[Ysize, Xsize];
            this.dColor = new Color[Ysize, Xsize];
            for (int i = 0; i < Ysize; i++)
            {
                for (int j = 0; j < Xsize; j++)
                {
                    this.draw[i, j] = t;
                }
            }
            for (int i = 0; i < Ysize; i++)
            {
                for (int j = 0; j < Xsize; j++)
                {
                    dColor[i, j] = c;
                }
            }
			this.checkForExceptions();
        }
    }
	
	public class GuiSquare : GuiRectangle
	{
		public GuiSquare(char t, int X, int Y, int size, Color c) : base(t,X,Y,size,size,c){
			
		}
	}
	
	public class GuiFrame : GuiElement
    {

        public GuiFrame(char t,char u,int X, int Y, int Xsize, int Ysize,Color C)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = Xsize;
            this.Ysize = Ysize;
			
			this.draw = new char[Ysize, Xsize];
			this.dColor = new Color[Ysize, Xsize];
			
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
    }
	
	public class GuiLinedFrame : GuiElement
    {

        public GuiLinedFrame(int X, int Y, int Xsize, int Ysize,Color C)
        {
            this.X = X;
            this.Y = Y;
            this.Xsize = Xsize;
            this.Ysize = Ysize;
			
			this.draw = new char[Ysize, Xsize];
			this.dColor = new Color[Ysize, Xsize];
			
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
    }

    public abstract class GuiSelectable : GuiElement
    {
        protected int index;
        protected bool selected = false;
        protected Color col;

        public GuiSelectable(int ind, int X, int Y, Color C)
        {
            this.index = ind;
            this.X = X;
            this.Y = Y;
            this.col = C;
        }

        public void setSel(bool s)
        {
			this.selected = s;
            updateDraw();
        }

        public bool getSel()
        {
            return this.selected;
        }

        public int getIndex()
        {
            return this.index;
        }

        virtual public void updateDraw()
        {

        }
    }

    public class GuiButton : GuiSelectable
    {

        protected string text;

        public GuiButton(int ind, string text, int X, int Y, Color C) : base(ind, X, Y, C)
        {
            this.text = text;
            this.Xsize = text.Length;
            this.Ysize = 1;
			updateText();
			this.checkForExceptions();
        }

        override public void updateDraw()
        {
            if (this.selected)
            {
                this.Xsize += 2;
                this.X--;
                updateText();
            }
            else
            {
                this.Xsize -= 2;
                this.X++;
                updateText();
            }
        }
		
		public void updateText()
        {
            if (this.selected)
            {
                this.draw = new char[1, Xsize];
                this.dColor = new Color[1, Xsize];

                this.draw[0, 0] = '>';
                for (int i = 1; i < text.Length + 1; i++)
                {
                    this.draw[0, i] = text[i-1];
                }
                this.draw[0, text.Length+1] = '<';
                for (int i = 0; i < Xsize; i++)
                {
                    dColor[0, i] = this.col;
                }
            }
            else
            {
                this.draw = new char[1, Xsize];
                this.dColor = new Color[1, Xsize];

                for (int i = 0; i < text.Length; i++)
                {
                    this.draw[0, i] = text[i];
                }
                for (int i = 0; i < Xsize; i++)
                {
                    dColor[0, i] = this.col;
                }
            }
        }

        public void setText(string t)
        {
            this.text = t;
        }

        public string getText()
        {
            return this.text;
        }

    }

    public class GuiFramedTextBox : GuiSelectable
    {

        protected string text;

        public GuiFramedTextBox(int ind, string startText, int X, int Y, int len, Color C) : base(ind, X, Y, C)
        {
            if (startText.Length > len)
            {
                throw new Exception("Start text for textBox can't be longer than length");
            }
            this.text = startText;
            this.Xsize = len + 2;
            this.Ysize = 3;
            updateText();
			this.checkForExceptions();
        }
		
		public string getText(){
			return this.text;
		}
		
		public void setText(string t){
			this.text = t;
		}

        override public void updateDraw()
        {
			if (this.selected)
            {
                this.Xsize += 2;
                this.X--;
                updateText();
            }
            else
            {
                this.Xsize -= 2;
                this.X++;
                updateText();
            }
        }

        public void updateText()
        {
            if (this.selected)
            {
                this.draw = new char[Ysize, Xsize];
                this.dColor = new Color[Ysize, Xsize];

                this.draw[0, 0] = ' ';
                this.draw[0, 1] = '┌';
                for (int i = 2; i < Xsize - 2; i++)
                {
                    this.draw[0, i] = '─';
                }
                this.draw[0, Xsize - 2] = '┐';
                this.draw[0, Xsize - 1] = ' ';

                this.draw[1, 0] = '>';
                this.draw[1, 1] = '│';
                for (int i = 2; i < text.Length + 2; i++)
                {
                    this.draw[1, i] = text.ToCharArray()[i - 2];
                }
                if (text.Length < Xsize - 4)
                {
                    for (int i = text.Length+2; i < Xsize - 2; i++)
                    {
                        this.draw[1, i] = ' ';
                    }
                }
                this.draw[1, Xsize - 2] = '│';
                this.draw[1, Xsize - 1] = '<';

                this.draw[2, 0] = ' ';
                this.draw[2, 1] = '└';
                for (int i = 2; i < Xsize - 2; i++)
                {
                    this.draw[2, i] = '─';
                }
                this.draw[2, Xsize - 2] = '┘';
                this.draw[2, Xsize - 1] = ' ';

                for (int j = 0; j < Ysize; j++)
                {
                    for (int k = 0; k < Xsize; k++)
                    {
                        dColor[j, k] = col;
                    }
                }
            }
            else
            {

                this.draw = new char[Ysize, Xsize];
                this.dColor = new Color[Ysize, Xsize];

                this.draw[0, 0] = '┌';
                for (int i = 1; i < Xsize - 1; i++)
                {
                    this.draw[0, i] = '─';
                }
                this.draw[0, Xsize - 1] = '┐';

                this.draw[1, 0] = '│';
                for (int i = 0; i < text.Length; i++)
                {
                    this.draw[1, i + 1] = text.ToCharArray()[i];
                }
                if (text.Length < Xsize - 2)
                {
                    for (int i = text.Length + 1; i <= Xsize - 2; i++)
                    {
                        this.draw[1, i] = ' ';
                    }
                }
                this.draw[1, Xsize - 1] = '│';

                this.draw[2, 0] = '└';
                for (int i = 1; i < Xsize - 1; i++)
                {
                    this.draw[2, i] = '─';
                }
                this.draw[2, Xsize - 1] = '┘';

                for (int j = 0; j < Ysize; j++)
                {
                    for (int k = 0; k < Xsize; k++)
                    {
                        dColor[j, k] = col;
                    }
                }
            }
        }

    }
}