using System;
using System.Drawing;
using Pastel;
using System.Text;

namespace AshConsoleGraphics
{

 public class GuiScreen
    {
		protected List<GuiElement> elements = new List<GuiElement>();
        protected char[,] draw;
        protected Col[,] dColor;
        int length;
        int height;
        protected Col defCol;

        public GuiScreen(List<GuiElement> e, int l, int h, Col d)
        {
            this.elements = e;
            this.length = l;
            this.height = h;
            this.defCol = d;
        }

        protected void preparePrint()
        {
            this.draw = new char[height, length];
            this.dColor = new Col[height, length];
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
						if(e.pCol()[j, k] != null){
							this.dColor[e.getY() + j, e.getX() + k] = (Col) e.pCol()[j, k];
						}
                    }
                }
            }
        }

        public void doPrint()
        {
            
			preparePrint();
            StringBuilder final = new StringBuilder();
			StringBuilder acum = new StringBuilder();
            for (int i = 0; i < this.height; i++)
            {
				for (int j = 0; j < this.length; j++)
                {
                    if (this.draw[i, j] == ' '){
						final.Append(acum+this.draw[i, j].ToString());
						acum.Clear();
					} else if(j+1 == this.length){
						final.Append((acum+this.draw[i, j].ToString()).Pastel(this.dColor[i, j].toCol()));
						acum.Clear();
					}else if(this.dColor[i, j] == this.dColor[i, j+1] && this.draw[i, j+1] != ' '){
						acum.Append(this.draw[i, j].ToString());
					} else {
						final.Append((acum+this.draw[i, j].ToString()).Pastel(this.dColor[i, j].toCol()));
						acum.Clear();
					}
					
                }
                final.AppendLine("");
            }
            Console.SetCursorPosition(0, 0);
			FastConsole.Write(final.ToString());
			FastConsole.Flush();
        }

        public void setElem(int index, GuiElement e)
        {
            elements[index] = e;
        }
		
		public GuiElement getElem(int index){
			return this.elements[index];
		}
		
		public void addElem(GuiElement e){
			this.elements.Add(e);
		}
    }
	
	public class GuiScreenB : GuiScreen{
		protected int bSelected = 0;
		protected int[,] bMatrix;
		protected int bX;
		protected int bY;
		
		protected bool controller = false;
		
		protected Dictionary<int, Action<GuiElement, GuiScreenB>> bFunctions = new Dictionary<int, Action<GuiElement, GuiScreenB>>();
		protected Dictionary<ConsoleKey, Action<GuiScreenB, ConsoleKey>> keyFunctions = new Dictionary<ConsoleKey, Action<GuiScreenB, ConsoleKey>>();
		
		protected bool waitForKey = true;
		protected Action<GuiScreenB>? finishPlayCycleEvent = null;
		protected bool allowWrite = true;
		
		public GuiScreenB(List<GuiElement> e, int l, int h, int[,] bn, int startX, int startY, Col c) : base(e,l,h, c){
			this.bMatrix = bn;
			this.bX = startX;
			this.bY = startY;
			if (this.elements[getIndexOfB(bMatrix[this.bY,this.bX])] is GuiSelectable){
				this.bSelected = getIndexOfB(bMatrix[this.bY,this.bX]);
				setSel(true);
			}
			this.setDefKeys();
		}
		
		public void subBevent(int bIndex, Action<GuiElement, GuiScreenB> bFunction)
		{
			bFunctions[bIndex] = bFunction;
		}
		
		public void subKeyEvent(ConsoleKey bIndex, Action<GuiScreenB, ConsoleKey> keyFunction)
		{
			keyFunctions[bIndex] = keyFunction;
		}
		
		public void delAllKeyEvents(){
			this.keyFunctions.Clear();
		}
		
		public void setFinishPlayCycleEvent(Action<GuiScreenB>? finishFunction){
			finishPlayCycleEvent = finishFunction;
		}
		
		public void setWaitForKey(bool b){
			this.waitForKey = b;
		}
		
		public void setAllowWrite(bool b){
			this.allowWrite = b;
		}
		
		public bool tbWrite(ConsoleKeyInfo ck){
			char c = ck.KeyChar;
			if (this.elements[bSelected] is GuiWritable w){
				if (!(Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || c == ' ')){
					if(ck.Key == ConsoleKey.Backspace){
						tbDelPriv(w);
						return true;
					}
					return false;
				}
				if (w.getText().Length == w.getLength()){
					return false;
				}
				w.write(c);
				return true;
			}
			return false;
		}
		
		protected void tbDelPriv(GuiWritable w){
			w.delChar();
		}
		
		public void tbDel(){
			if (this.elements[bSelected] is GuiWritable w){
				w.delChar();
			}
		}
		
		private void setDefKeys(){
			subKeyEvent(ConsoleKey.UpArrow, this.moveUp);
			subKeyEvent(ConsoleKey.DownArrow, this.moveDown);
			subKeyEvent(ConsoleKey.LeftArrow, this.moveLeft);
			subKeyEvent(ConsoleKey.RightArrow, this.moveRight);
			subKeyEvent(ConsoleKey.Enter, this.pressCurrentB);
			subKeyEvent(ConsoleKey.Escape, this.stopPlay);
		}
		
		public void play() {
			controller = true;
			while(controller){
				this.doPrint();
				if(!this.waitForKey && !Console.KeyAvailable){
					continue;
				}
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);
				
				if(allowWrite && tbWrite(keyInfo)){
					continue;
				}
				
				if (keyFunctions.ContainsKey(keyInfo.Key))
				{
					Action<GuiScreenB, ConsoleKey> keyFunction = keyFunctions[keyInfo.Key];
					keyFunction.Invoke(this, keyInfo.Key);
				}
				
				if(!(finishPlayCycleEvent is null)){
					finishPlayCycleEvent.Invoke(this);
				}
			}
			return;
		}
		
		public void bPressed(int bIndex)
		{
			if (bFunctions.ContainsKey(bIndex))
			{
				Action<GuiElement, GuiScreenB> bFunction = bFunctions[bIndex];
				bFunction.Invoke(this.elements[bSelected], this);
			}
		}
		
		public void pressCurrentB(GuiScreenB s, ConsoleKey ck){
			this.bPressed(this.bGetInd());
		}
		
		public void stopPlay(GuiScreenB s, ConsoleKey ck){
			this.controller = false;
		}
		
		public void moveRight(GuiScreenB s, ConsoleKey ck){
			if(this.bX+1 < bMatrix.GetLength(1)){
				setSel(false);
				this.bX++;
				this.bSelected = getIndexOfB(bMatrix[this.bY,this.bX]);
				setSel(true);
			}
		}
		
		public void moveLeft(GuiScreenB s, ConsoleKey ck){
			if(this.bX != 0){
				setSel(false);
				this.bX--;
				this.bSelected = getIndexOfB(bMatrix[this.bY,this.bX]);
				setSel(true);
			}
		}
		
		public void moveUp(GuiScreenB s, ConsoleKey ck){
			if(this.bY+1 < bMatrix.GetLength(0)){
				setSel(false);
				this.bY++;
				this.bSelected = getIndexOfB(bMatrix[this.bY,this.bX]);
				setSel(true);
			}
		}
		
		public void moveDown(GuiScreenB s, ConsoleKey ck){
			if(this.bY != 0){
					setSel(false);
					this.bY--;
					this.bSelected = getIndexOfB(bMatrix[this.bY,this.bX]);
					setSel(true);
				}
		}
		
		//--------
		
		protected int bGetInd(){
			if (this.elements[bSelected] is GuiSelectable b){
				return b.getIndex();
			}
			return 0;
		}
		
		protected void setSel(bool bn){
			if (this.elements[bSelected] is GuiSelectable b){
				b.setSel(bn);
			}
		}
		
		protected int getIndexOfB(int n){
			for (int i = 0; i < this.elements.Count; i++){
				if(this.elements[i] is GuiSelectable b){
					if(b.getIndex() == n){
						return i;
					}
				} else {
					continue;
				}
			}
			throw new Exception("No button with Index of " + n + " was found");
		}
	}

}