using System;
using AshLib;
using AshLib.Formatting;

namespace AshConsoleGraphics;

//Graphic elements

/// <summary>
/// Element of a screen
/// </summary>
public abstract class TuiElement{
	/// <summary>
	/// Offset in the X coordinate from its relative position
	/// </summary>
	public int OffsetX {get;
	set{
		field = value;
		needToGenScreenBuffer = true;
	}}
	
	/// <summary>
	/// Offset in the Y coordinate from its relative position
	/// </summary>
	public int OffsetY {get;
	set{
		field = value;
		needToGenScreenBuffer = true;
	}}
	
	/// <summary>
	/// Relative placement to its parent screen
	/// </summary>
	public Placement Placement {get;
	set{
		field = value;
		needToGenScreenBuffer = true;
	}}
	
	private Buffer _buffer;
	public Buffer Buffer{
	get{
		if(BufferNeedsToBeGenerated()){
			_buffer = GenerateBuffer();
			needToGenBuffer = false;
		}
		return _buffer;
	}}
	
	/// <summary>
	/// Set this to true when the buffer needs to be regenerated
	/// </summary>
	protected bool needToGenBuffer {get;
	set{
		field = value;
		if(needToGenBuffer){
			needToGenScreenBuffer = true;
		}
	}} = true;
	
	internal virtual bool needToGenScreenBuffer {get;
	set{
		field = value;
	}} = true;
	
	/// <summary>
	/// Will be called when parent screen resizes
	/// </summary>
	public event EventHandler<ResizeArgs> OnParentResize;
	
	/// <summary>
	/// Initializes a new element
	/// </summary>
	/// <param name="p">The relative placement method</param>
	/// <param name="x">The relative x offset</param>
	/// <param name="y">The relative x offset</param>
	protected TuiElement(Placement p, int x, int y){
		Placement = p;
		OffsetX = x;
		OffsetY = y;
	}
	
	/// <summary>
	/// The method that generates the element's buffer
	/// </summary>
	abstract protected Buffer GenerateBuffer();
	
	public void TriggerUpdate(){
		needToGenBuffer = true;
	}
	
	/// <summary>
	/// In most cases, the base implementation is enough
	/// </summary>
	virtual protected bool BufferNeedsToBeGenerated(){
		return needToGenBuffer;
	}
	
	virtual internal bool ScreenNeedsToBeGenerated(){
		return needToGenScreenBuffer;
	}
	
	internal void RaiseParentResize(TuiScreen s, ResizeArgs args){
		OnParentResize?.Invoke(s, args);
	}
	
	public (int, int) GetTopLeftPosition(int Xsize, int Ysize){
		int x = 0;
		int y = 0;
		Buffer eb = Buffer;
		switch(Placement){
			default:
			case Placement.TopLeft:
				x = OffsetX;
				y = OffsetY;
				break;
			case Placement.TopRight:
				x = Xsize - eb.Xsize - OffsetX;
				y = OffsetY;
				break;
			case Placement.BottomLeft:
				x = OffsetX;
				y = Ysize - eb.Ysize - OffsetY;
				break;
			case Placement.BottomRight:
				x = Xsize - eb.Xsize - OffsetX;
				y = Ysize - eb.Ysize - OffsetY;
				break;
			case Placement.Center:
				x = Xsize / 2 - eb.Xsize / 2 + OffsetX;
				y = Ysize / 2 - eb.Ysize / 2 + OffsetY;
				break;
			case Placement.CenterLeft:
				x = OffsetX;
				y = Ysize / 2 - eb.Ysize / 2 + OffsetY;
				break;
			case Placement.CenterRight:
				x = Xsize - eb.Xsize - OffsetX;
				y = Ysize / 2 - eb.Ysize / 2 + OffsetY;
				break;
			case Placement.TopCenter:
				x = Xsize / 2 - eb.Xsize / 2 + OffsetX;
				y = OffsetY;
				break;
			case Placement.BottomCenter:
				x = Xsize / 2 - eb.Xsize / 2 + OffsetX;
				y = Ysize - eb.Ysize - OffsetY;
				break;
		}
		return (x, y);
	}
}