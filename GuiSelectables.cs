using System;
using System.Drawing;
using Pastel;
using System.Text;

namespace AshConsoleGraphics
{
	
	public abstract class GuiSelectable : GuiElement
    {
        protected int index;
        protected bool selected = false;
        protected Col? col;

        public GuiSelectable(int ind, int X, int Y, Col? C)
        {
            this.index = ind;
            this.X = X;
            this.Y = Y;
            this.col = C;
        }
		
		public GuiSelectable(int ind, int X, int Y) : this(ind, X, Y, null){}

        public void setSel(bool s)
        {
			bool a = this.selected;
			this.selected = s;
            updateDraw(a);
        }

        public bool getSel()
        {
            return this.selected;
        }

        public int getIndex()
        {
            return this.index;
        }

        public abstract void updateDraw(bool a);
    }

    public class GuiButton : GuiSelectable
    {

        protected string text;

        public GuiButton(int ind, string text, int X, int Y, Col? C) : base(ind, X, Y, C)
        {
            this.text = text;
            this.Xsize = text.Length;
            this.Ysize = 1;
			updateText();
			this.checkForExceptions();
        }
		public GuiButton(int ind, string text, int X, int Y) : this(ind, text, X, Y, null){}

        override public void updateDraw(bool a)
        {
            if (this.selected && !a)
            {
                this.Xsize += 2;
                this.X--;
                updateText();
            }
            else if(!this.selected && a)
            {
                this.Xsize -= 2;
                this.X++;
                updateText();
            }
        }
		
		protected void updateText()
        {
            if (this.selected)
            {
                this.draw = new char[1, Xsize];
                this.dColor = new Col?[1, Xsize];

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
                this.dColor = new Col?[1, Xsize];

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
			this.Xsize = text.Length;
			if(this.selected){
				this.Xsize += 2;
                this.X--;
			}
			updateText();
			this.checkForExceptions();
        }

        public string getText()
        {
            return this.text;
        }
    }
	
	public abstract class GuiWritable : GuiSelectable
	{
		protected string text;
		protected int length;
		
		public GuiWritable(int ind, string startText, int X, int Y, int len, Col? C) : base(ind, X, Y, C)
        {
            if (startText.Length > len)
            {
                throw new Exception("Start text for writable, that has a length of:"+startText.Length+" can't be longer than length, wich is:"+len);
            }
            this.text = startText;
			this.length = len;
        }
		
		public GuiWritable(int ind, string startText, int X, int Y, int len) : this(ind, startText, X, Y, len, null){}
		
		public override abstract void updateDraw(bool a);
		
		public void setText(string s){
			if (!(s.Length > this.length))
            {
                this.text = s;
				dynamic d = this;
				d.updateDraw(this.selected);
            }
		}
		
		public string getText(){
			return this.text;
		}
		
		public int getLength(){
			return this.length;
		}
		
		public void write(char c){
			this.setText(this.text + c);
		}
		
		public void delChar(){
			if(this.text.Length != 0){
				this.setText(this.text.Substring(0, this.text.Length - 1));
			}
		}
	}

    public class GuiFramedTextBox : GuiWritable
    {

        public GuiFramedTextBox(int ind, string startText, int X, int Y, int len, Col? C) : base(ind, startText, X, Y, len, C)
        {
            this.Xsize = len + 2;
            this.Ysize = 3;
            updateText();
			this.checkForExceptions();
        }
		
		public GuiFramedTextBox(int ind, string startText, int X, int Y, int len) : this(ind, startText, X, Y, len, null){}

        override public void updateDraw(bool a)
        {
			if (this.selected && !a)
            {
                this.Xsize += 2;
                this.X--;
            }
            else if(!this.selected && a)
            {
                this.Xsize -= 2;
                this.X++;
            }
			updateText();
        }

        public void updateText()
        {
            if (this.selected)
            {
                this.draw = new char[Ysize, Xsize];
                this.dColor = new Col?[Ysize, Xsize];

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
                this.dColor = new Col?[Ysize, Xsize];

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