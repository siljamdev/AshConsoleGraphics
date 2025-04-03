using System;
using AshLib;

namespace AshConsoleGraphics.Interactive;

//Classes for text input

/// <summary>
/// Elements where you can write
/// </summary>
public abstract class TuiWritable : TuiSelectable{
	
	/// <summary>
	/// The written text
	/// </summary>
	public string Text {get;
	set{
		if(value.Length > Length){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The maximum length you can write
	/// </summary>
	public uint Length {get;
	set{
		if(Text != null && Text.Length > value){
			Text = Text.Substring(0, (int) value);
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Base constructor
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">length</param>
	protected TuiWritable(string t, uint l, Placement p, int x, int y) : base(p, x, y){
		Length = l;
		Text = t;
	}
	
	public override bool HandleKey(ConsoleKeyInfo keyInfo){
		char c = keyInfo.KeyChar;
		bool b = false;
		if(char.IsControl(c)){
			if(keyInfo.Key == ConsoleKey.Backspace){
				b = DelChar();
			}
		}else{
			b = WriteChar(c);
		}
		
		if(!b){
			if(keyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
				keyFunctions[(keyInfo.Key, keyInfo.Modifiers)].Invoke(this, keyInfo);
				return true;
			}
			return false;
		}else{
			return true;
		}
	}
	
	/// <summary>
	/// Attempts to write a char to the end of the text
	/// </summary>
	/// <param name="c">Char to write</param>
	/// <returns>If it was posible to write</returns>
	public virtual bool WriteChar(char c){
		if(Text.Length + 1 > Length){
			return true;
		}
		Text = Text + c;
		return true;
	}
	
	/// <summary>
	/// Attempts to delete a char from the end of the text
	/// </summary>
	/// <returns>If it was posible to delete</returns>
	public virtual bool DelChar(){
		if(Text.Length != 0){
			Text = Text.Substring(0, Text.Length - 1);
			return true;
		}
		return true;
	}
}

/// <summary>
/// A textbox where you can write inside a frame
/// </summary>
public class TuiFramedTextBox : TuiWritable{
	
	/// <summary>
	/// Foreground frame color when not selected
	/// </summary>
	public Color3? FgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when not selected
	/// </summary>
	public Color3? BgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground frame color when selected
	/// </summary>
	public Color3? FgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when selected
	/// </summary>
	public Color3? BgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when not selected
	/// </summary>
	public Color3? FgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when not selected
	/// </summary>
	public Color3? BgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when selected
	/// </summary>
	public Color3? FgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when selected
	/// </summary>
	public Color3? BgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? FgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? BgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get;
	set{
		if(value == null || value.Length != 8){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new textbox
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 8 chars long</exception>
	public TuiFramedTextBox(string chars, string t, uint l, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb)
							: base(t, l, p, x, y){
		if(chars == null || chars.Length != 8){
			throw new ArgumentException("String must be 8 chars long");
		}
		
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		FrameChars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new textbox with the same colors when selected and not selected
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(string chars, string t, uint l, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(chars, t, l, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new textbox with the same colors for frame and text
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(string chars, string t, uint l, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(chars, t, l, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new textbox with general colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiFramedTextBox(string chars, string t, uint l, Placement p, int x, int y, Color3? f, Color3? b)
							: this(chars, t, l, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new textbox with null colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	public TuiFramedTextBox(string chars, string t, uint l, Placement p, int x, int y)
							: this(chars, t, l, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new textbox with each individual frame char
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(t, l, p, x, y){
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		FrameChars = new char[]{topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right};
	}
	
	/// <summary>
	/// Initializes a new textbox with each individual frame char and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new textbox with each individual frame char and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, Placement p, int x, int y,
							Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new textbox with each individual frame char with general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, Placement p, int x, int y,
							Color3? f, Color3? b)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new textbox with each individual frame char and null colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	public TuiFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, Placement p, int x, int y)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(string t, uint l, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb)
							: this('┌', '┐', '└', '┘', '─', '─', '│', '│', t, l, p, x, y, ff, fb, fsf, fsb, tf, tb, tsf, tsb, sf, sb){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(string t, uint l, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(t, l, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││') and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedTextBox(string t, uint l, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(t, l, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││') and general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiFramedTextBox(string t, uint l, Placement p, int x, int y, Color3? f, Color3? b)
							: this(t, l, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││') and null colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	public TuiFramedTextBox(string t, uint l, Placement p, int x, int y) : this(t, l, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(Length + 4, 3);
			b.SetChar(0, 1, '>', FgSelectorColor, BgSelectorColor);
			b.SetChar((int) Length + 3, 1, '<', FgSelectorColor, BgSelectorColor);
			
			for(int i = 2; i < Length + 2; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameSelectedColor, BgFrameSelectedColor);
				b.SetChar(i, 2, FrameChars[5], FgFrameSelectedColor, BgFrameSelectedColor);
			}
			
			b.SetChar(1, 1, FrameChars[6], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) Length + 2, 1, FrameChars[7], FgFrameSelectedColor, BgFrameSelectedColor);
			
			b.SetChar(1, 0, FrameChars[0], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) Length + 2, 0, FrameChars[1], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(1, 2, FrameChars[2], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) Length + 2, 2, FrameChars[3], FgFrameSelectedColor, BgFrameSelectedColor);
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(2 + i, 1, Text[i], FgTextSelectedColor, BgTextSelectedColor);
			}
			
			for(int i = Text.Length; i < Length; i++){
				b.SetChar(2 + i, 1, ' ', null, BgTextSelectedColor);
			}
		}else{
			b = new Buffer(Length + 2, 3);
			
			for(int i = 1; i < Length + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameColor, BgFrameColor);
				b.SetChar(i, 2, FrameChars[5], FgFrameColor, BgFrameColor);
			}
			
			b.SetChar(0, 1, FrameChars[6], FgFrameColor, BgFrameColor);
			b.SetChar((int) Length + 1, 1, FrameChars[7], FgFrameColor, BgFrameColor);
			
			b.SetChar(0, 0, FrameChars[0], FgFrameColor, BgFrameColor);
			b.SetChar((int) Length + 1, 0, FrameChars[1], FgFrameColor, BgFrameColor);
			b.SetChar(0, 2, FrameChars[2], FgFrameColor, BgFrameColor);
			b.SetChar((int) Length + 1, 2, FrameChars[3], FgFrameColor, BgFrameColor);
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + i, 1, Text[i], FgTextColor, BgTextColor);
			}
			
			for(int i = Text.Length; i < Length; i++){
				b.SetChar(1 + i, 1, ' ', null, BgTextColor);
			}
		}
		return b;
	}
}

/// <summary>
/// A textbox where you can write inside a frame and it lets you write longer than the visible length of the box
/// </summary>
public class TuiFramedScrollingTextBox : TuiWritable{
	
	/// <summary>
	/// Foreground frame color when not selected
	/// </summary>
	public Color3? FgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when not selected
	/// </summary>
	public Color3? BgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground frame color when selected
	/// </summary>
	public Color3? FgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when selected
	/// </summary>
	public Color3? BgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when not selected
	/// </summary>
	public Color3? FgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when not selected
	/// </summary>
	public Color3? BgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when selected
	/// </summary>
	public Color3? FgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when selected
	/// </summary>
	public Color3? BgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? FgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? BgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get;
	set{
		if(value == null || value.Length != 8){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible X size of the box
	/// </summary>
	public uint BoxXsize {get; set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new scrolling textbox
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 8 chars long</exception>
	public TuiFramedScrollingTextBox(string chars, string t, uint l, uint bl, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(t, l, p, x, y){
		if(chars == null || chars.Length != 8){
			throw new ArgumentException("String must be 8 chars long");
		}
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		BoxXsize = bl;
		
		FrameChars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new scrolling textbox with the same colors when selected and not selected
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(string chars, string t, uint l, uint bl, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(chars, t, l, bl, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with the same colors for frame and text
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(string chars, string t, uint l, uint bl, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(chars, t, l, bl, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with general colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiFramedScrollingTextBox(string chars, string t, uint l, uint bl, Placement p, int x, int y, Color3? f, Color3? b)
							: this(chars, t, l, bl, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with null colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	public TuiFramedScrollingTextBox(string chars, string t, uint l, uint bl, Placement p, int x, int y)
							: this(chars, t, l, bl, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with each individual frame char
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint bl, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(t, l, p, x, y){
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		BoxXsize = bl;
		
		FrameChars = new char[]{topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right};
	}
	
	/// <summary>
	/// Initializes a new scrolling textbox with each individual frame char and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint bl, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, bl, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with each individual frame char and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint bl, Placement p, int x, int y,
							Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, bl, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with each individual frame char with general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiFramedScrollingTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint bl, Placement p, int x, int y,
							Color3? f, Color3? b)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, bl, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with each individual frame char with null colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	public TuiFramedScrollingTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint bl, Placement p, int x, int y)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, bl, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(string t, uint l, uint bl, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb)
							: this('┌', '┐', '└', '┘', '─', '─', '│', '│', t, l, bl, p, x, y, ff, fb, fsf, fsb, tf, tb, tsf, tsb, sf, sb){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(string t, uint l, uint bl, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(t, l, bl, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││') and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedScrollingTextBox(string t, uint l, uint bl, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(t, l, bl, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││') and general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiFramedScrollingTextBox(string t, uint l, uint bl, Placement p, int x, int y, Color3? f, Color3? b)
							: this(t, l, bl, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││') and null colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	public TuiFramedScrollingTextBox(string t, uint l, uint bl, Placement p, int x, int y) : this(t, l, bl, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		
		string te;
		if(Text.Length > BoxXsize){
			te = "…" + Text.Substring(Text.Length - (int) BoxXsize + 1);
		}else{
			te = Text;
		}
		
		if(Selected){
			b = new Buffer(BoxXsize + 4, 3);
			b.SetChar(0, 1, '>', FgSelectorColor, BgSelectorColor);
			b.SetChar((int) BoxXsize + 3, 1, '<', FgSelectorColor, BgSelectorColor);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameSelectedColor, BgFrameSelectedColor);
				b.SetChar(i, 2, FrameChars[5], FgFrameSelectedColor, BgFrameSelectedColor);
			}
			
			b.SetChar(1, 1, FrameChars[6], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) BoxXsize + 2, 1, FrameChars[7], FgFrameSelectedColor, BgFrameSelectedColor);
			
			b.SetChar(1, 0, FrameChars[0], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) BoxXsize + 2, 0, FrameChars[1], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(1, 2, FrameChars[2], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) BoxXsize + 2, 2, FrameChars[3], FgFrameSelectedColor, BgFrameSelectedColor);
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(2 + i, 1, te[i], FgTextSelectedColor, BgTextSelectedColor);
			}
			
			for(int i = te.Length; i < BoxXsize; i++){
				b.SetChar(2 + i, 1, ' ', null, BgTextSelectedColor);
			}
		}else{
			b = new Buffer(BoxXsize + 2, 3);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameColor, BgFrameColor);
				b.SetChar(i, 2, FrameChars[5], FgFrameColor, BgFrameColor);
			}
			
			b.SetChar(0, 1, FrameChars[6], FgFrameColor, BgFrameColor);
			b.SetChar((int) BoxXsize + 1, 1, FrameChars[7], FgFrameColor, BgFrameColor);
			
			b.SetChar(0, 0, FrameChars[0], FgFrameColor, BgFrameColor);
			b.SetChar((int) BoxXsize + 1, 0, FrameChars[1], FgFrameColor, BgFrameColor);
			b.SetChar(0, 2, FrameChars[2], FgFrameColor, BgFrameColor);
			b.SetChar((int) BoxXsize + 1, 2, FrameChars[3], FgFrameColor, BgFrameColor);
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(1 + i, 1, te[i], FgTextColor, BgTextColor);
			}
			
			for(int i = te.Length; i < BoxXsize; i++){
				b.SetChar(1 + i, 1, ' ', null, BgTextColor);
			}
		}
		return b;
	}
}

/// <summary>
/// A textbox where you can write inside a frame that can be several lines in height
/// </summary>
public class TuiMultiLineFramedTextBox : TuiWritable{
	
	/// <summary>
	/// Foreground frame color when not selected
	/// </summary>
	public Color3? FgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when not selected
	/// </summary>
	public Color3? BgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground frame color when selected
	/// </summary>
	public Color3? FgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when selected
	/// </summary>
	public Color3? BgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when not selected
	/// </summary>
	public Color3? FgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when not selected
	/// </summary>
	public Color3? BgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when selected
	/// </summary>
	public Color3? FgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when selected
	/// </summary>
	public Color3? BgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? FgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? BgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get;
	set{
		if(value == null || value.Length != 8){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible X size of the box. Length is xsize * ysize
	/// </summary>
	public uint BoxXsize {get;
	set{
		field = value;
		Length = BoxXsize * BoxYsize;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible Y size of the box. Length is xsize * ysize
	/// </summary>
	public uint BoxYsize {get;
	set{
		field = value;
		Length = BoxXsize * BoxYsize;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new multiline textbox
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 8 chars long</exception>
	public TuiMultiLineFramedTextBox(string chars, string t, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(t, xs * ys, p, x, y){
		if(chars == null || chars.Length != 8){
			throw new ArgumentException("String must be 8 chars long");
		}
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		BoxXsize = xs;
		BoxYsize = ys;
		
		Text = t;
		
		FrameChars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new multiline textbox with the same colors when selected and not selected
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(string chars, string t, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(chars, t, xs, ys, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with the same colors for frame and text
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(string chars, string t, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(chars, t, xs, ys, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with general colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiMultiLineFramedTextBox(string chars, string t, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b)
							: this(chars, t, xs, ys, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new multiline textbox with null colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	public TuiMultiLineFramedTextBox(string chars, string t, uint xs, uint ys, Placement p, int x, int y)
							: this(chars, t, xs, ys, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint xs, uint ys, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(t, xs * ys, p, x, y){
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		BoxXsize = xs;
		BoxYsize = ys;
		
		Text = t;
		
		FrameChars = new char[]{topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right};
	}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint xs, uint ys, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, xs, ys, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint xs, uint ys, Placement p, int x, int y,
							Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, xs, ys, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char with general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiMultiLineFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint xs, uint ys, Placement p, int x, int y,
							Color3? f, Color3? b)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, xs, ys, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char with null colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	public TuiMultiLineFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint xs, uint ys, Placement p, int x, int y)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, xs, ys, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(string t, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb)
							: this('┌', '┐', '└', '┘', '─', '─', '│', '│', t, xs, ys, p, x, y, ff, fb, fsf, fsb, tf, tb, tsf, tsb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(string t, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(t, xs, ys, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineFramedTextBox(string t, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(t, xs, ys, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiMultiLineFramedTextBox(string t, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b)
							: this(t, xs, ys, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	public TuiMultiLineFramedTextBox(string t, uint xs, uint ys, Placement p, int x, int y) : this(t, xs, ys, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(BoxXsize + 4, BoxYsize + 2);
			b.SetChar(0, (int) BoxYsize / 2, '>', FgSelectorColor, BgSelectorColor);
			b.SetChar((int) BoxXsize + 3, (int) BoxYsize / 2, '<', FgSelectorColor, BgSelectorColor);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameSelectedColor, BgFrameSelectedColor);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], FgFrameSelectedColor, BgFrameSelectedColor);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(1, i, FrameChars[6], FgFrameSelectedColor, BgFrameSelectedColor);
				b.SetChar((int) BoxXsize + 2, i, FrameChars[7], FgFrameSelectedColor, BgFrameSelectedColor);
			}
			
			b.SetChar(1, 0, FrameChars[0], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) BoxXsize + 2, 0, FrameChars[1], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(1, (int) BoxYsize + 1, FrameChars[2], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) BoxXsize + 2, (int) BoxYsize + 1, FrameChars[3], FgFrameSelectedColor, BgFrameSelectedColor);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', null, BgTextSelectedColor);
				}
			}
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(2 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), Text[i], FgTextSelectedColor, BgTextSelectedColor);
			}
		}else{
			b = new Buffer(BoxXsize + 2, BoxYsize + 2);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameColor, BgFrameColor);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], FgFrameColor, BgFrameColor);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(0, i, FrameChars[6], FgFrameColor, BgFrameColor);
				b.SetChar((int) BoxXsize + 1, i, FrameChars[7], FgFrameColor, BgFrameColor);
			}
			
			b.SetChar(0, 0, FrameChars[0], FgFrameColor, BgFrameColor);
			b.SetChar((int) BoxXsize + 1, 0, FrameChars[1], FgFrameColor, BgFrameColor);
			b.SetChar(0, (int) BoxYsize + 1, FrameChars[2], FgFrameColor, BgFrameColor);
			b.SetChar((int) BoxXsize + 1, (int) BoxYsize + 1, FrameChars[3], FgFrameColor, BgFrameColor);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', null, BgTextColor);
				}
			}
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), Text[i], FgTextColor, BgTextColor);
			}
		}
		return b;
	}
}

/// <summary>
/// A textbox where you can write inside a frame that can be several lines in height and scrolls
/// </summary>
public class TuiMultiLineScrollingFramedTextBox : TuiWritable{
	
	/// <summary>
	/// Foreground frame color when not selected
	/// </summary>
	public Color3? FgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when not selected
	/// </summary>
	public Color3? BgFrameColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground frame color when selected
	/// </summary>
	public Color3? FgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background frame color when selected
	/// </summary>
	public Color3? BgFrameSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when not selected
	/// </summary>
	public Color3? FgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when not selected
	/// </summary>
	public Color3? BgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground text color when selected
	/// </summary>
	public Color3? FgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color when selected
	/// </summary>
	public Color3? BgTextSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? FgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public Color3? BgSelectorColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get;
	set{
		if(value == null || value.Length != 8){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible X size of the box. Length is xsize * ysize
	/// </summary>
	public uint BoxXsize {get;
	set{
		field = value;
		Length = BoxXsize * BoxYsize;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible Y size of the box. Length is xsize * ysize
	/// </summary>
	public uint BoxYsize {get;
	set{
		field = value;
		Length = BoxXsize * BoxYsize;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new multiline textbox
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 8 chars long</exception>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb)
											: base(t, l, p, x, y){
		if(chars == null || chars.Length != 8){
			throw new ArgumentException("String must be 8 chars long");
		}
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		BoxXsize = xs;
		BoxYsize = ys;
		
		Length = l;
		
		Text = t;
		
		FrameChars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new multiline textbox with the same colors when selected and not selected
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(chars, t, l, xs, ys, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with the same colors for frame and text
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(chars, t, l, xs, ys, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with general colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b)
							: this(chars, t, l, xs, ys, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new multiline textbox with null colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, uint l, uint xs, uint ys, Placement p, int x, int y)
							: this(chars, t, l, xs, ys, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint xs, uint ys, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(t, l, p, x, y){
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgTextColor = tf;
		BgTextColor = tb;
		FgTextSelectedColor = tsf;
		BgTextSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		BoxXsize = xs;
		BoxYsize = ys;
		
		Length = l;
		
		Text = t;
		
		FrameChars = new char[]{topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right};
	}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint xs, uint ys, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, xs, ys, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint xs, uint ys, Placement p, int x, int y,
							Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, xs, ys, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char with general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiMultiLineScrollingFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint xs, uint ys, Placement p, int x, int y,
							Color3? f, Color3? b)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, xs, ys, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new multiline textbox with each individual frame char with null colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	public TuiMultiLineScrollingFramedTextBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, string t, uint l, uint xs, uint ys, Placement p, int x, int y)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, t, l, xs, ys, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected text color</param>
	/// <param name="tb">Background not selected text color</param>
	/// <param name="tsf">Foreground selected text color</param>
	/// <param name="tsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb)
							: this('┌', '┐', '└', '┘', '─', '─', '│', '│', t, l, xs, ys, p, x, y, ff, fb, fsf, fsb, tf, tb, tsf, tsb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground text color</param>
	/// <param name="tb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(t, l, xs, ys, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same color for frame and text
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame and text color</param>
	/// <param name="b">Background frame and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiMultiLineScrollingFramedTextBox(string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
									: this(t, l, xs, ys, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="f">Foreground frame color</param>
	/// <param name="b">Background frame color</param>
	public TuiMultiLineScrollingFramedTextBox(string t, uint l, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b)
									: this(t, l, xs, ys, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and general colors
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	public TuiMultiLineScrollingFramedTextBox(string t, uint l, uint xs, uint ys, Placement p, int x, int y)
									: this(t, l, xs, ys, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		
		string te;
		if(Text.Length > BoxXsize * BoxYsize){
			te = "…" + Text.Substring(Text.Length - (int) (BoxXsize * BoxYsize) + 1);
		}else{
			te = Text;
		}
		
		if(Selected){
			b = new Buffer(BoxXsize + 4, BoxYsize + 2);
			b.SetChar(0, (int) BoxYsize / 2, '>', FgSelectorColor, BgSelectorColor);
			b.SetChar((int) BoxXsize + 3, (int) BoxYsize / 2, '<', FgSelectorColor, BgSelectorColor);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameSelectedColor, BgFrameSelectedColor);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], FgFrameSelectedColor, BgFrameSelectedColor);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(1, i, FrameChars[6], FgFrameSelectedColor, BgFrameSelectedColor);
				b.SetChar((int) BoxXsize + 2, i, FrameChars[7], FgFrameSelectedColor, BgFrameSelectedColor);
			}
			
			b.SetChar(1, 0, FrameChars[0], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) BoxXsize + 2, 0, FrameChars[1], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(1, (int) BoxYsize + 1, FrameChars[2], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) BoxXsize + 2, (int) BoxYsize + 1, FrameChars[3], FgFrameSelectedColor, BgFrameSelectedColor);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', null, BgTextSelectedColor);
				}
			}
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(2 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), te[i], FgTextSelectedColor, BgTextSelectedColor);
			}
		}else{
			b = new Buffer(BoxXsize + 2, BoxYsize + 2);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FgFrameColor, BgFrameColor);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], FgFrameColor, BgFrameColor);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(0, i, FrameChars[6], FgFrameColor, BgFrameColor);
				b.SetChar((int) BoxXsize + 1, i, FrameChars[7], FgFrameColor, BgFrameColor);
			}
			
			b.SetChar(0, 0, FrameChars[0], FgFrameColor, BgFrameColor);
			b.SetChar((int) BoxXsize + 1, 0, FrameChars[1], FgFrameColor, BgFrameColor);
			b.SetChar(0, (int) BoxYsize + 1, FrameChars[2], FgFrameColor, BgFrameColor);
			b.SetChar((int) BoxXsize + 1, (int) BoxYsize + 1, FrameChars[3], FgFrameColor, BgFrameColor);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', null, BgTextColor);
				}
			}
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(1 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), te[i], FgTextColor, BgTextColor);
			}
		}
		return b;
	}
}