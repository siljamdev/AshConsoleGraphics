using System;
using System.Drawing;
using Pastel;
//

namespace AshCG_0_2{
	
	public class AshConsoleGraphics
        {
            private List<string> print = new List<string>();
			private List<int> printLen = new List<int>();
            private int length;
            private int height;

            public AshConsoleGraphics(int x, int y)
            {
                length = x;
                height = y;
            }

			public void setGuiScreen(AshConsoleGraphics.GuiScreen gs) {
				this.print = gs.print();
				this.printLen = gs.printLen();
			}
			
			public int PlayGuiScreenB(AshConsoleGraphics.GuiScreenB gs) {
				bool controller = true;
				while(controller){
					this.print = gs.print();
					this.printLen = gs.printLen();
					doPrint();
					ConsoleKeyInfo keyInfo = Console.ReadKey(true);

					switch (keyInfo.Key)
					{
						case ConsoleKey.UpArrow:
							gs.move(2);
							break;
						case ConsoleKey.DownArrow:
							gs.move(3);
							break;
						case ConsoleKey.LeftArrow:
							gs.move(1);
							break;
						case ConsoleKey.RightArrow:
							gs.move(0);
							break;	
						case ConsoleKey.Enter:
							return gs.bAction();
							break;
						case ConsoleKey.Escape:
							return 0;
							break;
						case ConsoleKey.Backspace:
							gs.tbDel();
							break;
						default:
							if (Char.IsLetterOrDigit(keyInfo.KeyChar) || Char.IsPunctuation(keyInfo.KeyChar) || keyInfo.KeyChar == ' '){
								gs.tbWrite(keyInfo.KeyChar);
							}
							break;
					}
				}
				return 0;
			}

            public void setPrint(List<string> p)
            {
				this.print = p;
				for (int i = 0; i < printLen.Count(); i++){
					printLen[i] = this.length;
				}
            }
			
			public void writePrint(string p){
				if (this.print.Count == 0){
					print.Add(p);
					printLen.Add(p.Length);
				} else {
					print[print.Count-1] += p;
					printLen[printLen.Count-1] += p.Length;
				}
			}
			
			public void writePrintCol(string p, Color c){
				if (this.print.Count == 0){
					print.Add(p.Pastel(c));
					printLen.Add(p.Length);
				} else {
					print[print.Count-1] += p.Pastel(c);
					printLen[printLen.Count-1] += p.Length;
				}
			}
			
			public void writeLinePrint(string p){
				writePrint(p);
				print.Add("");
				printLen.Add(0);
			}
			
			public void writeLinePrintCol(string p, Color c){
				writePrintCol(p, c);
				print.Add("");
				printLen.Add(0);
			}

            public void doPrint()
            {
				preparePrint();
                Console.SetCursorPosition(0,0);
                Console.Write(string.Join(Environment.NewLine,print));
				print.Clear();
				printLen.Clear();
            }

            public void preparePrint()
            {
                 if (print.Count > height)
                {
					print.RemoveRange(height, print.Count - height);
					printLen.RemoveRange(height, print.Count - height);
                } 
                else
                {
                    print.AddRange(Enumerable.Repeat(new string(' ', length), height-print.Count).ToList());
					printLen.AddRange(Enumerable.Repeat(length, height-printLen.Count).ToList());
                }
				
				for (int i = 0; i < print.Count; i++)
                {
					if (printLen[i] > this.length) {
						throw new Exception("Line longer than accepted");
					}
					for(int f = 0; f < length - printLen[i]; f++){
					print[i] += " ";
					}
                } 
            }
			
			public class Gui{
				protected List<string> draw = new List<string>();
				
				public string[] print(){
					return draw.ToArray();
				}
			}
			
			public class GuiScreen : Gui{
				protected List<GuiElement> elements = new List<GuiElement>();
				private List<string> pr= new List<string>();
				private List<int> prLen= new List<int>();
				int length;
				int height;
				
				public GuiScreen(List<GuiElement> e, int l, int h){
					this.elements = e;
					this.length = l;
					this.height = h;
				}
			
				public void addElement(GuiElement e){
					this.elements.Add(e);
				}
				
				public void preparePrint(){
					for(int i = 0; i < this.height; i++){
						pr.Add(new string(' ',this.length));
						prLen.Add(this.length);
					}
					for(int i = 0; i < this.elements.Count; i++){
						GuiElement e = this.elements[i];
						if (e.getXsize() + e.getX()>this.length){
							break;
						}
						if (e.getYsize() + e.getY() > this.height){
							break;
						}
						for(int f = 0; f < e.getYsize(); f++){
							if (e.getX() + e.getXsize() == this.length) {
								pr[e.getY()+f] = pr[e.getY()+f].Substring(0,this.length-e.getXsize()) + e.print()[f];
							} else {
								pr[e.getY()+f] = pr[e.getY()+f].Substring(0,this.length-e.getXsize() - (this.length - (e.getX() + e.getXsize()))) + e.print()[f] + pr[e.getY()+f].Substring(e.getX()+e.getXsize(), this.length - (e.getX()+e.getXsize()));
							}
						}
					}
				}
				
				new public List<string> print(){
					preparePrint();
					return this.pr;
				}
				
				public List<int> printLen(){
					return this.prLen;
				}
				
			
			}
			
			public class GuiScreenB : GuiScreen{
				protected int bSelected = 0;
				protected int[,] bNav;
				protected int X;
				protected int Y;
				
				public GuiScreenB(List<GuiElement> e, int l, int h, int[,] bn, int startX, int startY) : base(e,l,h){
					this.bNav = bn;
					this.X = startX;
					this.Y = startY;
					if (this.elements[getIndexOfB(bNav[this.X,this.Y])] is GuiButton){
						this.bSelected = getIndexOfB(bNav[this.X,this.Y]);
						setSel(true);
					}
				}
				
				public void move(int dir){
					if (dir==0){
						if(this.X+1 < bNav.GetLength(1)){
							setSel(false);
							this.X++;
							this.bSelected = getIndexOfB(bNav[this.Y,this.X]);
							setSel(true);
						}
					} else if (dir==1){
						if(this.X != 0){
							setSel(false);
							this.X--;
							this.bSelected = getIndexOfB(bNav[this.Y,this.X]);
							setSel(true);
						}
					} else if (dir==3){
						if(this.Y+1 < bNav.GetLength(0)){
							setSel(false);
							this.Y++;
							this.bSelected = getIndexOfB(bNav[this.Y,this.X]);
							setSel(true);
						}
					} else if (dir==2){
						if(this.Y != 0){
							setSel(false);
							this.Y--;
							this.bSelected = getIndexOfB(bNav[this.Y,this.X]);
							setSel(true);
						}
					}
				}
				
				public int bAction(){
					if (this.elements[bSelected] is GuiButton b){
						return b.getIndex();
					}
					return 0;
				}
				
				public void setSel(bool bn){
					if (this.elements[bSelected] is GuiButton){
						dynamic b = this.elements[bSelected];
						b.setSel(bn);
					}
				}
				
				public int getIndexOfB(int n){
					for (int i = 0; i < this.elements.Count; i++){
						if(this.elements[i] is GuiButton b){
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
				
				public GuiElement getElement(int index){
					return this.elements[index];
				}
			}
			
			public class GuiElement : Gui{
				protected int X;
				protected int Y;
				protected int Xsize;
				protected int Ysize;
				
				public int getX(){
					return X;
				}
				
				public int getY(){
					return Y;
				}
				
				public int getXsize(){
					return Xsize;
				}
				
				public int getYsize(){
					return Ysize;
				}
			}

			public class GuiLabel : GuiElement{
				public GuiLabel(string text, int X, int Y){
					this.draw.Add(text);
					this.X = X;
					this.Y = Y;
					this.Xsize = text.Length;
					this.Ysize = 1;
				}
			}
			public class GuiColoredLabel : GuiElement{
				public GuiColoredLabel(string text, int X, int Y, int R, int G, int B){
					this.draw.Add(text.Pastel(Color.FromArgb(R,G,B)));
					this.X = X;
					this.Y = Y;
					this.Xsize = text.Length;
					this.Ysize = 1;
				}
			}
			public class GuiSquare : GuiElement{
				public GuiSquare(char t, int X, int Y, int Xsize, int Ysize){
					this.X = X;
					this.Y = Y;
					this.Xsize = Xsize;
					this.Ysize = Ysize;
					for (int i = 0; i < Ysize; i++){
						draw.Add(new string(t,this.Xsize));
					}
				}
			}
			
			public class GuiButton : GuiElement{
				protected int index;
				protected bool selected = false;
				protected string text;
				
				public GuiButton(int ind, string text, int X, int Y){
					this.draw.Add(text);
					this.text = text;
					this.X = X;
					this.Y = Y;
					this.Xsize = text.Length;
					this.Ysize = 1;
					this.index = ind;
				}
				
				public void setSel(bool s){
					this.selected = s;
					if (this.selected){
						this.draw[0] = ">"+this.text+"<";
						this.Xsize += 2;
						this.X--;
					} else{
						this.draw[0] = this.text;
						this.Xsize -= 2;
						this.X++;
					}
				}
				
				public bool getSel(){
					return this.selected;
				}
				
				public void setText(string t){
					this.text = t;
				}
				
				public string getText(){
					return this.text;
				}
				
				public int getIndex(){
					return this.index;
				}
			}
			
			public class GuiFramedTextBox : GuiButton{
				
				public GuiFramedTextBox(int ind, string startText, int X, int Y, int len) : base(ind, startText, X, Y){
					if(startText.Length > len){
						throw new Exception("Start text for textBox can't be longer than length");
					}
					this.draw.Clear();
					this.draw.Add("┌" + new string('─',len)+"┐");
					this.draw.Add("│" + startText + lenStr(' ',len-startText.Length) + "│");
					this.draw.Add("└" + new string('─',len)+"┘");
					this.text = startText;
					this.X = X;
					this.Y = Y;
					this.Xsize = len + 2;
					this.Ysize = 3;
				}
				
				new public void setSel(bool s){
					this.selected = s;
					if (this.selected){
						this.draw[0] = " ┌" + new string('─', Xsize-2) + "┐";
						this.draw[1] = ">│" + this.text + lenStr(' ',Xsize-2-text.Length) + "│<";
						this.draw[2] = " └" + new string('─', Xsize-2) + "┘";
						this.Xsize += 2;
						this.X--;
					} else{
						this.draw[0] = "┌" + new string('─', Xsize-4) + "┐";
						this.draw[1] = "│" + this.text + lenStr(' ',Xsize-4-text.Length) + "│";
						this.draw[2] = "└" + new string('─', Xsize-4) + "┘";
						this.Xsize -= 2;
						this.X++;
					}
				}
				
				public void updateText(){
					if (this.selected){
						this.draw[0] = " ┌" + new string('─', Xsize-4) + "┐";
						this.draw[1] = ">│" + this.text + lenStr(' ',Xsize-4-text.Length) + "│<";
						this.draw[2] = " └" + new string('─', Xsize-4) + "┘";
					} else{
						this.draw[0] = "┌" + new string('─', Xsize-2) + "┐";
						this.draw[1] = "│" + this.text + lenStr(' ',Xsize-2-text.Length) + "│";
						this.draw[2] = "└" + new string('─', Xsize-2) + "┘";
					}
				}
				
				private string lenStr(char c, int len){
					if(len <= 0){
						return "";
					} else {
						return new string(c,len);
					}
				}
			}
        }
}