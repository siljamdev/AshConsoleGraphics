using System;
using System.Linq;
using AshLib.Formatting;
using AshLib.Lists;
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
	public ReactiveList<TuiElement> Elements {get; private set;}
	
    public int Xsize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
		DoResize();
	}}
	
    public int Ysize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
		DoResize();
	}}
	
	/// <summary>
	/// Action that takes place when the screen resizes
	/// </summary>
	public event EventHandler<ResizeArgs> OnResize;
	
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
    public TuiScreen(int xs, int ys, Placement p, int x, int y, CharFormat? f, params TuiElement[] e) : base(p, x, y){
		if(e == null){
			Elements = new ReactiveList<TuiElement>();
		}else{
			Elements = new ReactiveList<TuiElement>(e.Where(x => x != null).Distinct());
		}
		
		Elements.OnChanged = () => {
			Elements.RemoveAll(e => e == null); //No null elements ever
			needToGenBuffer = true;
		};
		
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
	public TuiScreen(int xs, int ys, CharFormat? f, params TuiElement[] e) : this(xs, ys, Placement.TopLeft, 0, 0, f, e){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		foreach(TuiElement e in Elements){
			(int x, int y) = e.GetTopLeftPosition(Xsize, Ysize);
			b.AddBuffer(x, y, e.Buffer);
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
		try{ //If terminals is not interactive it will throw exception
			if(AutoResize && (Console.WindowWidth != Xsize || Console.WindowHeight - 1 != Ysize)){
				Xsize = Console.WindowWidth;
				Ysize = Console.WindowHeight - 1;
				return true;
			}
		}catch(Exception e){} //What can we do? Nothing.
		
		if(needToGenBuffer){
			return true;
		}
		
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
		OnResize?.Invoke(this, new ResizeArgs(this.Xsize, this.Ysize));
		
		foreach(TuiElement e in Elements){
			e.RaiseParentResize(this, new ResizeArgs(this.Xsize, this.Ysize));
		}
	}
	
	protected internal void SetAllNoNeedGenerateBuffer(){
		foreach(TuiElement e in Elements){
			e.needToGenScreenBuffer = false;
			if(e is TuiScreen gs){
				gs.SetAllNoNeedGenerateBuffer();
			}
		}
	}
}