using System;
using AshLib;
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
	
	public int ScrollX {get; private set;}
	
	public int ScrollY {get; private set;}
	
	
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
		OnResize += (s, args) => {
			updateScroll();
		};
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
		OnResize += (s, args) => {
			updateScroll();
		};
	}
	
	void updateScroll(){
		if(Selected == null){
			return;
		}
		
		(int x, int y) = Selected.GetTopLeftPosition(Xsize, Ysize);
		x += ScrollX;
		y += ScrollY;
		
		if(x < 0){
			ScrollX -= x;
		}else if(x + Selected.Buffer.Xsize > Xsize){
			ScrollX -= x + Selected.Buffer.Xsize - Xsize;
		}
		
		if(y < 0){
			ScrollY -= y;
		}else if(y + Selected.Buffer.Ysize > Ysize){
			ScrollY -= y + Selected.Buffer.Ysize - Ysize;
		}
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		foreach(TuiElement e in Elements){
			(int x, int y) = e.GetTopLeftPosition(Xsize, Ysize);
			x += ScrollX;
			y += ScrollY;
		
			b.AddBuffer(x, y, e.Buffer);
		}
		b.ReplaceNull(DefFormat);
		SetAllNoNeedGenerateBuffer();
		return b;
	}
}