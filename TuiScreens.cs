using System;
using System.Linq;
using AshLib.Formatting;
using AshLib;

namespace AshConsoleGraphics;

//Manager elements

/// <summary>
/// Element that has other elements and shows them all
/// </summary>
public class TuiScreen : TuiElement, IEnumerable<TuiElement>{
	
	/// <summary>
	/// List of child elements
	/// </summary>
	public List<TuiElement> Elements {get; private set;}
    public uint Xsize {get;
	set{
		field = value;
		needToGenBuffer = true;
		DoResize();
	}}
	
    public uint Ysize {get;
	set{
		field = value;
		needToGenBuffer = true;
		DoResize();
	}}
	
	/// <summary>
	/// Action that takes place when the screen resizes
	/// </summary>
	public Action<TuiScreen>? OnResize {internal get; set;}
	
	/// <summary>
	/// If set to true, the screen will auto resize to the whole console size
	/// </summary>
	public bool AutoResize {get; set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Deafult char format
	/// </summary>
    public CharFormat? DefFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The default format</param>
	/// <param name="e">The elements</param>
    public TuiScreen(uint xs, uint ys, Placement p, int x, int y, CharFormat? f, params TuiElement[] e) : base(p, x, y){
		if(e == null){
			Elements = new List<TuiElement>();
		}else{
			Elements = new List<TuiElement>(e.Where(x => x != null).Distinct());
		}
		
		DefFormat = f;
		
		Xsize = xs;
		Ysize = ys;
	}
	
	/// <summary>
	/// Initializes a new screen with top left placement
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The default format</param>
	/// <param name="e">The elements</param>
	public TuiScreen(uint xs, uint ys, CharFormat? f, params TuiElement[] e) : this(xs, ys, Placement.TopLeft, 0, 0, f, e){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		foreach(TuiElement e in Elements){
			int x = 0;
			int y = 0;
			Buffer eb = e.Buffer;
			switch(e.Placement){
				default:
				case Placement.TopLeft:
					x = e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.TopRight:
					x = (int) Xsize - (int) eb.Xsize - e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.BottomLeft:
					x = e.OffsetX;
					y = (int) Ysize - (int) eb.Ysize - e.OffsetY;
					break;
				case Placement.BottomRight:
					x = (int) Xsize - (int) eb.Xsize - e.OffsetX;
					y = (int) Ysize - (int) eb.Ysize - e.OffsetY;
					break;
				case Placement.Center:
					x = (int) Xsize / 2 - (int) eb.Xsize / 2 + e.OffsetX;
					y = (int) Ysize / 2 - (int) eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.CenterLeft:
					x = e.OffsetX;
					y = (int) Ysize / 2 - (int) eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.CenterRight:
					x = (int) Xsize - (int) eb.Xsize - e.OffsetX;
					y = (int) Ysize / 2 - (int) eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.TopCenter:
					x = (int) Xsize / 2 - (int) eb.Xsize / 2 + e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.BottomCenter:
					x = (int) Xsize / 2 - (int) eb.Xsize / 2 + e.OffsetX;
					y = (int) Ysize - (int) eb.Ysize - e.OffsetY;
					break;
			}
			b.AddBuffer(x, y, eb);
		}
		b.ReplaceNull(DefFormat);
		SetAllNoNeedGenerateBuffer();
		return b;
	}
	
	//IEnumerable method
	public IEnumerator<TuiElement> GetEnumerator(){
		foreach(TuiElement e in Elements){
			yield return e;
		}
	}
	
	//IEnumerable method
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){
		return GetEnumerator();
	}
	
	/// <summary>
	/// Prints the screen into the console
	/// </summary>
    public void Print(){
		//Console.Clear();
		try{
			Console.SetCursorPosition(0, 0);
		}catch(Exception e){}
		//Console.Write("\x1b[0;0H");
		FastConsole.Write(Buffer.ToString(' ', DefFormat));
		FastConsole.Flush();
    }
	
	protected override bool BufferNeedsToBeGenerated(){
		if(needToGenBuffer){
			return true;
		}
		
		try{ //If terminals is not interactive it will throw exception
			if(AutoResize && (Console.WindowWidth != Xsize || Console.WindowHeight - 1 != Ysize)){
				Xsize = (uint) Console.WindowWidth;
				Ysize = (uint) Console.WindowHeight - 1;
				return true;
			}
		}catch(Exception e){} //What can we do? Nothing.
		
		foreach(TuiElement e in Elements){
			if(e.ScreenNeedsToBeGenerated()){
				return true;
			}
		}
		return false;
	}
	
	internal override bool ScreenNeedsToBeGenerated(){	
		if(needToGenScreenBuffer){
			return true;
		}
		
		foreach(TuiElement e in Elements){
			if(e.ScreenNeedsToBeGenerated()){
				needToGenBuffer = true;
				return true;
			}
		}
		return false;
	}
	
	void DoResize(){
		if(OnResize != null){
			OnResize.Invoke(this);
		}
		
		foreach(TuiElement e in Elements){
			if(e.OnParentResize != null){
				e.OnParentResize.Invoke(this);
			}
		}
	}
	
	void SetAllNoNeedGenerateBuffer(){
		foreach(TuiElement e in Elements){
			e.needToGenScreenBuffer = false;
			if(e is TuiScreen gs){
				gs.SetAllNoNeedGenerateBuffer();
			}
		}
	}
}

/// <summary>
/// Screen with elements that connect like lines
/// </summary>
public class TuiConnectedLinesScreen : TuiScreen{
	/// <summary>
	/// The 16 connected line charachters. An example would be "·───│┌┐┬│└┘┴│├┤┼"
	/// </summary>
	public char[] Chars {get;
	set{
		if(value == null || value.Length != 16){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	List<ILineElement> LinedElements;
	
	/// <summary>
	/// Initializes a new line screen
	/// </summary>
	/// <param name="chars">The line charachters</param>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The default format</param>
	/// <param name="i">The lined elements</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 16 chars long</exception>
	public TuiConnectedLinesScreen(string chars, uint xs, uint ys, IEnumerable<ILineElement> i, Placement p, int x, int y, CharFormat? f) : base(xs, ys, p, x, y, f, i.Cast<TuiElement>().ToArray()){
		if(chars == null || chars.Length != 16){
			throw new ArgumentException("String must be 16 chars long");
		}
		
		LinedElements = new List<ILineElement>(i);
		
		Chars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new line screen with top left position
	/// </summary>
	/// <param name="chars">The line charachters</param>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The default format</param>
	/// <param name="i">The lined elements</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 16 chars long</exception>
	public TuiConnectedLinesScreen(string chars, uint xs, uint ys, IEnumerable<ILineElement> i, CharFormat? f) : base(xs, ys, f, i.Cast<TuiElement>().ToArray()){
		if(chars == null || chars.Length != 16){
			throw new ArgumentException("String must be 16 chars long");
		}
		
		LinedElements = new List<ILineElement>(i);
		
		Chars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new line screen with default chars
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The default format</param>
	/// <param name="i">The lined elements</param>
	public TuiConnectedLinesScreen(uint xs, uint ys, IEnumerable<ILineElement> i, Placement p, int x, int y, CharFormat? f) : base(xs, ys, p, x, y, f, i.Cast<TuiElement>().ToArray()){
		LinedElements = new List<ILineElement>(i);
		
		Chars = "·───│┌┐┬│└┘┴│├┤┼".ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new line screen with top left position and default chars
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The default format</param>
	/// <param name="i">The lined elements</param>
	public TuiConnectedLinesScreen(uint xs, uint ys, IEnumerable<ILineElement> i, CharFormat? f) : base(xs, ys, f, i.Cast<TuiElement>().ToArray()){
		LinedElements = new List<ILineElement>(i);
		
		Chars = "·───│┌┐┬│└┘┴│├┤┼".ToCharArray();
	}
	
	override protected Buffer GenerateBuffer(){
		BitBuffer b = new BitBuffer(Xsize, Ysize);
		
		foreach(ILineElement i in LinedElements){
			int x = 0;
			int y = 0;
			BitBuffer eb = i.GenerateBitBuffer();
			TuiElement e = (TuiElement) i;
			switch(e.Placement){
				default:
				case Placement.TopLeft:
					x = e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.TopRight:
					x = (int) Xsize - (int) eb.Xsize - e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.BottomLeft:
					x = e.OffsetX;
					y = (int) Ysize - (int) eb.Ysize - e.OffsetY;
					break;
				case Placement.BottomRight:
					x = (int) Xsize - (int) eb.Xsize - e.OffsetX;
					y = (int) Ysize - (int) eb.Ysize - e.OffsetY;
					break;
				case Placement.Center:
					x = (int) Xsize / 2 - (int) eb.Xsize / 2 + e.OffsetX;
					y = (int) Ysize / 2 - (int) eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.CenterLeft:
					x = e.OffsetX;
					y = (int) Ysize / 2 - (int) eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.CenterRight:
					x = (int) Xsize - (int) eb.Xsize - e.OffsetX;
					y = (int) Ysize / 2 - (int) eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.TopCenter:
					x = (int) Xsize / 2 - (int) eb.Xsize / 2 + e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.BottomCenter:
					x = (int) Xsize / 2 - (int) eb.Xsize / 2 + e.OffsetX;
					y = (int) Ysize - (int) eb.Ysize - e.OffsetY;
					break;
			}
			b.AddBuffer(x, y, eb);
		}
		
		return b.ToBuffer(Chars, DefFormat);
	}
}