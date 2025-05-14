using System;
using AshLib;
using AshLib.Formatting;

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
			}else if(keyInfo.Key == ConsoleKey.Enter){
				b = WriteChar('\n');
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
		if(c == '\n'){
			return false;
		}
		if(Text.Length + 1 > Length){
			return false;
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
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Not selected text charachter format
	/// </summary>
	public CharFormat? TextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected text charachter format
	/// </summary>
	public CharFormat? SelectedTextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public CharFormat? SelectorFormat {get;
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
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string chars, string t, uint l, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf)
							: base(t, l, p, x, y){
		if(chars == null || chars.Length != 8){
			chars = "┌┐└┘──││";
		}
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		
		FrameChars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new textbox with the same colors when selected and not selected
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string chars, string t, uint l, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(chars, t, l, p, x, y, ff, ff, tf, tf, pf){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string t, uint l, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf)
							: this(null, t, l, p, x, y, ff, sff, tf, stf, pf){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string t, uint l, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(t, l, p, x, y, ff,ff, tf, tf, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(Length + 4, 3);
			b.SetChar(0, 1, '>', SelectorFormat);
			b.SetChar((int) Length + 3, 1, '<', SelectorFormat);
			
			for(int i = 2; i < Length + 2; i++){
				b.SetChar(i, 0, FrameChars[4], SelectedFrameFormat);
				b.SetChar(i, 2, FrameChars[5], SelectedFrameFormat);
			}
			
			b.SetChar(1, 1, FrameChars[6], SelectedFrameFormat);
			b.SetChar((int) Length + 2, 1, FrameChars[7], SelectedFrameFormat);
			
			b.SetChar(1, 0, FrameChars[0], SelectedFrameFormat);
			b.SetChar((int) Length + 2, 0, FrameChars[1], SelectedFrameFormat);
			b.SetChar(1, 2, FrameChars[2], SelectedFrameFormat);
			b.SetChar((int) Length + 2, 2, FrameChars[3], SelectedFrameFormat);
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(2 + i, 1, Text[i], SelectedTextFormat);
			}
			
			for(int i = Text.Length; i < Length; i++){
				b.SetChar(2 + i, 1, ' ', SelectedTextFormat);
			}
		}else{
			b = new Buffer(Length + 2, 3);
			
			for(int i = 1; i < Length + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FrameFormat);
				b.SetChar(i, 2, FrameChars[5], FrameFormat);
			}
			
			b.SetChar(0, 1, FrameChars[6], FrameFormat);
			b.SetChar((int) Length + 1, 1, FrameChars[7], FrameFormat);
			
			b.SetChar(0, 0, FrameChars[0], FrameFormat);
			b.SetChar((int) Length + 1, 0, FrameChars[1], FrameFormat);
			b.SetChar(0, 2, FrameChars[2], FrameFormat);
			b.SetChar((int) Length + 1, 2, FrameChars[3], FrameFormat);
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + i, 1, Text[i], TextFormat);
			}
			
			for(int i = Text.Length; i < Length; i++){
				b.SetChar(1 + i, 1, ' ', TextFormat);
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
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Not selected text charachter format
	/// </summary>
	public CharFormat? TextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected text charachter format
	/// </summary>
	public CharFormat? SelectedTextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public CharFormat? SelectorFormat {get;
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
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string chars, string t, uint l, uint bl, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf) : base(t, l, p, x, y){
		if(chars == null || chars.Length != 8){
			chars = "┌┐└┘──││";
		}
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		
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
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string chars, string t, uint l, uint bl, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(chars, t, l, bl, p, x, y, ff, ff, tf, tf, pf){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string t, uint l, uint bl, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf)
							: this(null, t, l, bl, p, x, y, ff, sff, tf, stf, pf){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string t, uint l, uint bl, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(t, l, bl, p, x, y, ff, ff, tf, tf, pf){}
	
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
			b.SetChar(0, 1, '>', SelectorFormat);
			b.SetChar((int) BoxXsize + 3, 1, '<', SelectorFormat);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				b.SetChar(i, 0, FrameChars[4], SelectedFrameFormat);
				b.SetChar(i, 2, FrameChars[5], SelectedFrameFormat);
			}
			
			b.SetChar(1, 1, FrameChars[6], SelectedFrameFormat);
			b.SetChar((int) BoxXsize + 2, 1, FrameChars[7], SelectedFrameFormat);
			
			b.SetChar(1, 0, FrameChars[0], SelectedFrameFormat);
			b.SetChar((int) BoxXsize + 2, 0, FrameChars[1], SelectedFrameFormat);
			b.SetChar(1, 2, FrameChars[2], SelectedFrameFormat);
			b.SetChar((int) BoxXsize + 2, 2, FrameChars[3], SelectedFrameFormat);
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(2 + i, 1, te[i], SelectedTextFormat);
			}
			
			for(int i = te.Length; i < BoxXsize; i++){
				b.SetChar(2 + i, 1, ' ', SelectedTextFormat);
			}
		}else{
			b = new Buffer(BoxXsize + 2, 3);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FrameFormat);
				b.SetChar(i, 2, FrameChars[5], FrameFormat);
			}
			
			b.SetChar(0, 1, FrameChars[6], FrameFormat);
			b.SetChar((int) BoxXsize + 1, 1, FrameChars[7], FrameFormat);
			
			b.SetChar(0, 0, FrameChars[0], FrameFormat);
			b.SetChar((int) BoxXsize + 1, 0, FrameChars[1], FrameFormat);
			b.SetChar(0, 2, FrameChars[2], FrameFormat);
			b.SetChar((int) BoxXsize + 1, 2, FrameChars[3], FrameFormat);
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(1 + i, 1, te[i], TextFormat);
			}
			
			for(int i = te.Length; i < BoxXsize; i++){
				b.SetChar(1 + i, 1, ' ', TextFormat);
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
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Not selected text charachter format
	/// </summary>
	public CharFormat? TextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected text charachter format
	/// </summary>
	public CharFormat? SelectedTextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public CharFormat? SelectorFormat {get;
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
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string chars, string t, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf) : base(t, xs * ys, p, x, y){
		if(chars == null || chars.Length != 8){
			chars = "┌┐└┘──││";
		}
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		
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
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string chars, string t, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(chars, t, xs, ys, p, x, y, ff, ff, tf, tf, pf){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string t, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf)
							: this(null, t, xs, ys, p, x, y, ff, sff, tf, stf, pf){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string t, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(t, xs, ys, p, x, y, ff, ff, tf, tf, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(BoxXsize + 4, BoxYsize + 2);
			b.SetChar(0, (int) BoxYsize / 2, '>', SelectorFormat);
			b.SetChar((int) BoxXsize + 3, (int) BoxYsize / 2, '<', SelectorFormat);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				b.SetChar(i, 0, FrameChars[4], SelectedFrameFormat);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], SelectedFrameFormat);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(1, i, FrameChars[6], SelectedFrameFormat);
				b.SetChar((int) BoxXsize + 2, i, FrameChars[7], SelectedFrameFormat);
			}
			
			b.SetChar(1, 0, FrameChars[0], SelectedFrameFormat);
			b.SetChar((int) BoxXsize + 2, 0, FrameChars[1], SelectedFrameFormat);
			b.SetChar(1, (int) BoxYsize + 1, FrameChars[2], SelectedFrameFormat);
			b.SetChar((int) BoxXsize + 2, (int) BoxYsize + 1, FrameChars[3], SelectedFrameFormat);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', SelectedTextFormat);
				}
			}
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(2 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), Text[i], SelectedTextFormat);
			}
		}else{
			b = new Buffer(BoxXsize + 2, BoxYsize + 2);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FrameFormat);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], FrameFormat);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(0, i, FrameChars[6], FrameFormat);
				b.SetChar((int) BoxXsize + 1, i, FrameChars[7], FrameFormat);
			}
			
			b.SetChar(0, 0, FrameChars[0], FrameFormat);
			b.SetChar((int) BoxXsize + 1, 0, FrameChars[1], FrameFormat);
			b.SetChar(0, (int) BoxYsize + 1, FrameChars[2], FrameFormat);
			b.SetChar((int) BoxXsize + 1, (int) BoxYsize + 1, FrameChars[3], FrameFormat);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', TextFormat);
				}
			}
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), Text[i], TextFormat);
			}
		}
		return b;
	}
	
	public override bool WriteChar(char c){
		if(Text.Length + 1 > Length){
			return false;
		}
		if(c == '\n'){
			int m = (int) BoxXsize - ((Text.Length - 1) % (int) BoxXsize) - 1;
			Text = Text + new string(' ', m == 0 ? (int) BoxXsize : m);
			return true;
		}
		Text = Text + c;
		return true;
	}
}

/// <summary>
/// A textbox where you can write inside a frame that can be several lines in height and scrolls
/// </summary>
public class TuiMultiLineScrollingFramedTextBox : TuiWritable{
	
	/// <summary>
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Not selected text charachter format
	/// </summary>
	public CharFormat? TextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected text charachter format
	/// </summary>
	public CharFormat? SelectedTextFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the selector pads '&gt;' '&lt;' that surround the element when selcted
	/// </summary>
	public CharFormat? SelectorFormat {get;
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
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, uint l, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf)
											: base(t, l, p, x, y){
		if(chars == null || chars.Length != 8){
			chars = "┌┐└┘──││";
		}
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		
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
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, uint l, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(chars, t, l, xs, ys, p, x, y, ff, ff, tf, tf, pf){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineScrollingFramedTextBox(string t, uint l, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? pf)
							: this(null, t, l, xs, ys, p, x, y, ff, sff, tf, stf, pf){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineScrollingFramedTextBox(string t, uint l, uint xs, uint ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(t, l, xs, ys, p, x, y, ff, ff, tf, tf, pf){}
	
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
			b.SetChar(0, (int) BoxYsize / 2, '>', SelectorFormat);
			b.SetChar((int) BoxXsize + 3, (int) BoxYsize / 2, '<', SelectorFormat);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				b.SetChar(i, 0, FrameChars[4], SelectedFrameFormat);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], SelectedFrameFormat);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(1, i, FrameChars[6], SelectedFrameFormat);
				b.SetChar((int) BoxXsize + 2, i, FrameChars[7], SelectedFrameFormat);
			}
			
			b.SetChar(1, 0, FrameChars[0], SelectedFrameFormat);
			b.SetChar((int) BoxXsize + 2, 0, FrameChars[1], SelectedFrameFormat);
			b.SetChar(1, (int) BoxYsize + 1, FrameChars[2], SelectedFrameFormat);
			b.SetChar((int) BoxXsize + 2, (int) BoxYsize + 1, FrameChars[3], SelectedFrameFormat);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', SelectedTextFormat);
				}
			}
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(2 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), te[i], SelectedTextFormat);
			}
		}else{
			b = new Buffer(BoxXsize + 2, BoxYsize + 2);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				b.SetChar(i, 0, FrameChars[4], FrameFormat);
				b.SetChar(i, (int) BoxYsize + 1, FrameChars[5], FrameFormat);
			}
			
			for(int i = 1; i < BoxYsize + 1; i++){
				b.SetChar(0, i, FrameChars[6], FrameFormat);
				b.SetChar((int) BoxXsize + 1, i, FrameChars[7], FrameFormat);
			}
			
			b.SetChar(0, 0, FrameChars[0], FrameFormat);
			b.SetChar((int) BoxXsize + 1, 0, FrameChars[1], FrameFormat);
			b.SetChar(0, (int) BoxYsize + 1, FrameChars[2], FrameFormat);
			b.SetChar((int) BoxXsize + 1, (int) BoxYsize + 1, FrameChars[3], FrameFormat);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', TextFormat);
				}
			}
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(1 + (i % (int) BoxXsize), 1 + (i / (int) BoxXsize), te[i], TextFormat);
			}
		}
		return b;
	}
}