using System;
using System.Linq;
using AshLib.Formatting;
using AshLib;

namespace AshConsoleGraphics;

//Manager elements

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
	public TuiConnectedLinesScreen(string chars, int xs, int ys, IEnumerable<ILineElement> i, Placement p, int x, int y, CharFormat? f)
		: base(xs, ys, p, x, y, f, i.Cast<TuiElement>().ToArray()){
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
	public TuiConnectedLinesScreen(string chars, int xs, int ys, IEnumerable<ILineElement> i, CharFormat? f) : base(xs, ys, f, i.Cast<TuiElement>().ToArray()){
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
	public TuiConnectedLinesScreen(int xs, int ys, IEnumerable<ILineElement> i, Placement p, int x, int y, CharFormat? f) : base(xs, ys, p, x, y, f, i.Cast<TuiElement>().ToArray()){
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
	public TuiConnectedLinesScreen(int xs, int ys, IEnumerable<ILineElement> i, CharFormat? f) : base(xs, ys, f, i.Cast<TuiElement>().ToArray()){
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
					x = Xsize - eb.Xsize - e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.BottomLeft:
					x = e.OffsetX;
					y = Ysize - eb.Ysize - e.OffsetY;
					break;
				case Placement.BottomRight:
					x = Xsize - eb.Xsize - e.OffsetX;
					y = Ysize - eb.Ysize - e.OffsetY;
					break;
				case Placement.Center:
					x = Xsize / 2 - eb.Xsize / 2 + e.OffsetX;
					y = Ysize / 2 - eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.CenterLeft:
					x = e.OffsetX;
					y = Ysize / 2 - eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.CenterRight:
					x = Xsize - eb.Xsize - e.OffsetX;
					y = Ysize / 2 - eb.Ysize / 2 + e.OffsetY;
					break;
				case Placement.TopCenter:
					x = Xsize / 2 - eb.Xsize / 2 + e.OffsetX;
					y = e.OffsetY;
					break;
				case Placement.BottomCenter:
					x = Xsize / 2 - eb.Xsize / 2 + e.OffsetX;
					y = Ysize - eb.Ysize - e.OffsetY;
					break;
			}
			b.AddBuffer(x, y, eb);
		}
		
		return b.ToBuffer(Chars, DefFormat);
	}
}