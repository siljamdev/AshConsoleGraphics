using System;
using AshLib;
using AshLib.Lists;
using AshLib.Formatting;

namespace AshConsoleGraphics.Interactive;

//Manager elements

/// <summary>
/// Interactive screen that scrolls if items are out of bounds
/// </summary>
public class TuiScrollingScreenInteractive : TuiScreenInteractive{
	public override uint MatrixPointerX {get{
		return base.MatrixPointerX;
	} set{
		base.MatrixPointerX = value;
		updateScroll();
	}}
	
	public override uint MatrixPointerY {get{
		return base.MatrixPointerY;
	} set{
		base.MatrixPointerY = value;
		updateScroll();
	}}
	
	public int ScrollX {get; set{
		field = value;
		needToGenBuffer = true;
	}}
	
	public int ScrollY {get; set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// List of child elements that will not be affected by scroll
	/// </summary>
	public ReactiveList<TuiElement> FixedElements {get; private set;}
	
	/// <summary>
	/// Initializes a new interactive screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="sm">Selectable matrix</param>
	/// <param name="startX">Start X index in the selectable matrix</param>
	/// <param name="startY">Start Y index in the selectable matrix</param>
	/// <param name="f">The default format</param>
	/// <param name="e">Additional elements</param>
	public TuiScrollingScreenInteractive(int xs, int ys, TuiSelectable[,] sm, uint startX, uint startY, Placement p, int x, int y, CharFormat? f, params TuiElement[] e)
								: base(xs, ys, sm, startX, startY, p, x, y, f, e){		
		FixedElements = new();
		
		FixedElements.OnChanged = () => {
			needToGenBuffer = true;
		};
		
		OnResize += (s, args) => {
			updateScroll();
		};
		
		SetDefaultKeys();
	}
	
	/// <summary>
	/// Initializes a new interactive screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="sm">Selectable matrix</param>
	/// <param name="startX">Start X index in the selectable matrix</param>
	/// <param name="startY">Start Y index in the selectable matrix</param>
	/// <param name="f">The default format</param>
	/// <param name="e">Additional elements</param>
	public TuiScrollingScreenInteractive(int xs, int ys, TuiSelectable[,] sm, uint startX, uint startY, CharFormat? f, params TuiElement[] e)
								: base(xs, ys, sm, startX, startY, f, e){		
		FixedElements = new();
		
		FixedElements.OnChanged = () => {
			needToGenBuffer = true;
		};
		
		OnResize += (s, args) => {
			updateScroll();
		};
		
		SetDefaultKeys();
	}
	
	void updateScroll(){
		if(Selected == null){
			return;
		}
		
		//In fixed elements scroll doesnt change
		if(FixedElements != null && FixedElements.Contains(Selected)){
			return;
		}
		
		(int x, int y) = Selected.GetTopLeftPosition(Xsize, Ysize);
		x += ScrollX;
		y += ScrollY;
		
		if(x < 0){
			ScrollX -= x;
		}else if(x + Selected.Buffer.Xsize > Xsize){
			int n = x + Selected.Buffer.Xsize - Xsize;
			if(x - n >= 0){
				ScrollX -= n;
			}
		}
		
		if(y < 0){
			ScrollY -= y;
		}else if(y + Selected.Buffer.Ysize > Ysize){
			int n = y + Selected.Buffer.Ysize - Ysize;
			if(y - n >= 0){
				ScrollY -= n;
			}
		}
	}
	
	override protected Buffer GenerateBuffer(){
		List<TuiElement> fix = new(FixedElements.Count);
		
		Buffer b = new Buffer(Xsize, Ysize);
		
		foreach(TuiElement e in Elements.ToArray()){
			if(FixedElements.Contains(e)){
				fix.Add(e);
				continue;
			}
			
			(int x, int y) = e.GetTopLeftPosition(Xsize, Ysize);
			x += ScrollX;
			y += ScrollY;
		
			b.AddBuffer(x, y, e.Buffer);
		}
		
		foreach(TuiElement e in fix){
			(int x, int y) = e.GetTopLeftPosition(Xsize, Ysize);
			
			b.AddBuffer(x, y, e.Buffer);
		}
		
		b.ReplaceNull(DefFormat);
		SetAllNoNeedGenerateBuffer();
		return b;
	}
	
	public static void ScrollUp(TuiScreenInteractive s, ConsoleKeyInfo ck){
		((TuiScrollingScreenInteractive) s).ScrollY++;
	}
	
	public static void ScrollDown(TuiScreenInteractive s, ConsoleKeyInfo ck){
		((TuiScrollingScreenInteractive) s).ScrollY--;
	}
	
	void SetDefaultKeys(){
		SubKeyEvent(ConsoleKey.PageUp, ScrollUp);
		SubKeyEvent(ConsoleKey.PageDown, ScrollDown);
	}
}