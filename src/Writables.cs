using System;
using AshLib;
using AshLib.Formatting;

namespace AshConsoleGraphics.Interactive;

//Classes for text input

/// <summary>
/// A textbox where you can write inside a frame
/// </summary>
public class TuiFramedTextBox : TuiWritable{
	
	private TuiFrame frame;
	
	/// <summary>
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		if(!Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		if(Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get{
		return frame.Chars;
	}
	set{
		frame.Chars = value;
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
	/// Format of the selectors that surround the element when selcted
	/// </summary>
	public CharFormat? SelectorFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the cursor
	/// </summary>
	public CharFormat? CursorFormat {get;
	set{
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string chars, string t, int l, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf)
							: base(t, l, p, x, y){
		frame = new TuiFrame(chars, l + 2, 3, Placement.TopLeft, 0, 0, ff);
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		CursorFormat = curfor;
		
		OnLengthChange += (s, a) => {
			frame.Xsize = Length + 2;
		};
		
		OnSelection += (s, a) => {
			if(Selected){
				frame.Format = SelectedFrameFormat;
			}else{
				frame.Format = FrameFormat;
			}
		};
	}
	
	/// <summary>
	/// Initializes a new textbox with the same colors when selected and not selected
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string chars, string t, int l, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(chars, t, l, p, x, y, ff, ff, tf, tf, curfor, pf){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string t, int l, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf)
							: this(null, t, l, p, x, y, ff, sff, tf, stf, curfor, pf){}
	
	/// <summary>
	/// Initializes a new textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Textbox length</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedTextBox(string t, int l, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(t, l, p, x, y, ff,ff, tf, tf, curfor, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(Length + 4, 3);
			
			b.AddBuffer(1, 0, frame.Buffer);
			
			b.SetChar(0, 1, LeftSelector, SelectorFormat);
			b.SetChar(Length + 3, 1, RightSelector, SelectorFormat);
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(2 + i, 1, Text[i], SelectedTextFormat);
			}
			
			for(int i = Text.Length; i < Length; i++){
				b.SetChar(2 + i, 1, ' ', SelectedTextFormat);
			}
			
			if(Text.Length < Length){
				b.SetChar(Text.Length + 2, 1, Cursor, CursorFormat);
			}
		}else{
			b = new Buffer(Length + 2, 3);
			
			b.AddBuffer(0, 0, frame.Buffer);
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + i, 1, Text[i], TextFormat);
			}
			
			for(int i = Text.Length; i < Length; i++){
				b.SetChar(1 + i, 1, ' ', TextFormat);
			}
		}
		return b;
	}
	
	override protected bool BufferNeedsToBeGenerated(){
		return base.BufferNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
	
	override internal bool ScreenNeedsToBeGenerated(){
		return base.ScreenNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
}

/// <summary>
/// A textbox where you can write inside a frame and it lets you write longer than the visible length of the box
/// </summary>
public class TuiFramedScrollingTextBox : TuiWritable{
	
	private TuiFrame frame;
	
	/// <summary>
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		if(!Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		if(Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get{
		return frame.Chars;
	}
	set{
		frame.Chars = value;
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
	/// Format of the selectors that surround the element when selcted
	/// </summary>
	public CharFormat? SelectorFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the cursor
	/// </summary>
	public CharFormat? CursorFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible X size of the box
	/// </summary>
	public int BoxXsize {get{
		return frame.Xsize - 2;
	}
	set{
		if(value < 0){
			return;
		}
		frame.Xsize = value + 2;
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string chars, string t, int l, int bl, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf) : base(t, l, p, x, y){
		frame = new TuiFrame(chars, bl + 2, 3, Placement.TopLeft, 0, 0, ff);
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		CursorFormat = curfor;
		
		OnSelection += (s, a) => {
			if(Selected){
				frame.Format = SelectedFrameFormat;
			}else{
				frame.Format = FrameFormat;
			}
		};
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string chars, string t, int l, int bl, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(chars, t, l, bl, p, x, y, ff, ff, tf, tf, curfor, pf){}
	
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string t, int l, int bl, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf)
							: this(null, t, l, bl, p, x, y, ff, sff, tf, stf, curfor, pf){}
	
	/// <summary>
	/// Initializes a new scrolling textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">Max text length</param>
	/// <param name="bl">Visible textbox length</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedScrollingTextBox(string t, int l, int bl, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(t, l, bl, p, x, y, ff, ff, tf, tf, curfor, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		
		string te;
		bool showCursor = Selected && Text.Length < Length;
		if(Text.Length > BoxXsize - (showCursor ? 1 : 0)){
			te = "…" + Text.Substring(Text.Length - BoxXsize + (showCursor ? 2 : 1)) + (showCursor ? Cursor : "");
		}else{
			te = Text + (showCursor ? Cursor : "");
		}
		
		if(Selected){
			b = new Buffer(BoxXsize + 4, 3);
			
			b.SetChar(0, 1, LeftSelector, SelectorFormat);
			b.SetChar(BoxXsize + 3, 1, RightSelector, SelectorFormat);
			
			b.AddBuffer(1, 0, frame.Buffer);
			
			for(int i = 0; i < te.Length; i++){
				if(showCursor && i == te.Length - 1){
					b.SetChar(2 + i, 1, te[i], CursorFormat);
				}else{
					b.SetChar(2 + i, 1, te[i], SelectedTextFormat);
				}
			}
			
			for(int i = te.Length; i < BoxXsize; i++){
				b.SetChar(2 + i, 1, ' ', SelectedTextFormat);
			}
		}else{
			b = new Buffer(BoxXsize + 2, 3);
			
			b.AddBuffer(0, 0, frame.Buffer);
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(1 + i, 1, te[i], TextFormat);
			}
			
			for(int i = te.Length; i < BoxXsize; i++){
				b.SetChar(1 + i, 1, ' ', TextFormat);
			}
		}
		return b;
	}
	
	override protected bool BufferNeedsToBeGenerated(){
		return base.BufferNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
	
	override internal bool ScreenNeedsToBeGenerated(){
		return base.ScreenNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
}

/// <summary>
/// A textbox where you can write inside a frame that can be several lines in height
/// </summary>
public class TuiMultiLineFramedTextBox : TuiWritable{
	
	private TuiFrame frame;
	
	/// <summary>
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		if(!Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		if(Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get{
		return frame.Chars;
	}
	set{
		frame.Chars = value;
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
	/// Format of the selectors that surround the element when selected
	/// </summary>
	public CharFormat? SelectorFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the cursor
	/// </summary>
	public CharFormat? CursorFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible X size of the box. Length is xsize * ysize
	/// </summary>
	public int BoxXsize {get{
		return frame.Xsize - 2;
	}
	set{
		if(value < 0){
			return;
		}
		frame.Xsize = value + 2;
		Length = BoxXsize * BoxYsize;
	}}
	
	/// <summary>
	/// The visible Y size of the box. Length is xsize * ysize
	/// </summary>
	public int BoxYsize {get{
		return frame.Ysize - 2;
	}
	set{
		if(value < 0){
			return;
		}
		frame.Ysize = value + 2;
		Length = BoxXsize * BoxYsize;
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string chars, string t, int xs, int ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf) : base(t, xs * ys, p, x, y){
		frame = new TuiFrame(chars, xs + 2, ys + 2, Placement.TopLeft, 0, 0, ff);
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		CursorFormat = curfor;
		
		Text = t;
		
		CanWriteChar = c => {
			if(Text.Length + 1 > Length){
				return null;
			}
			
			if(c == '\n'){
				int m = (int) BoxXsize - ((Text.Length - 1) % (int) BoxXsize) - 1;
				return new string(' ', m == 0 ? (int) BoxXsize : m);
			}
			
			return c.ToString();
		};
		
		bool b = false;
		
		OnLengthChange += (s, a) => {
			if(!b){ //Prevent infinite loop
				b = true;
				Length = BoxXsize * BoxYsize; //No length resize is allowed
				b = false;
			}
		};
		
		OnSelection += (s, a) => {
			if(Selected){
				frame.Format = SelectedFrameFormat;
			}else{
				frame.Format = FrameFormat;
			}
		};
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string chars, string t, int xs, int ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(chars, t, xs, ys, p, x, y, ff, ff, tf, tf, curfor, pf){}
	
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string t, int xs, int ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf)
							: this(null, t, xs, ys, p, x, y, ff, sff, tf, stf, curfor, pf){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineFramedTextBox(string t, int xs, int ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(t, xs, ys, p, x, y, ff, ff, tf, tf, curfor, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(BoxXsize + 4, BoxYsize + 2);
			
			b.SetChar(0, 1 + BoxYsize / 2, LeftSelector, SelectorFormat);
			b.SetChar(BoxXsize + 3, 1 + BoxYsize / 2, RightSelector, SelectorFormat);
			
			b.AddBuffer(1, 0, frame.Buffer);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', SelectedTextFormat);
				}
			}
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(2 + (i % BoxXsize), 1 + (i / BoxXsize), Text[i], SelectedTextFormat);
			}
			
			if(Text.Length < Length){
				b.SetChar(2 + (Text.Length % BoxXsize), 1 + (Text.Length / BoxXsize), Cursor, CursorFormat);
			}
		}else{
			b = new Buffer(BoxXsize + 2, BoxYsize + 2);
			
			b.AddBuffer(0, 0, frame.Buffer);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', TextFormat);
				}
			}
			
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + (i % BoxXsize), 1 + (i / BoxXsize), Text[i], TextFormat);
			}
		}
		return b;
	}
	
	override protected bool BufferNeedsToBeGenerated(){
		return base.BufferNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
	
	override internal bool ScreenNeedsToBeGenerated(){
		return base.ScreenNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
}

/// <summary>
/// A textbox where you can write inside a frame that can be several lines in height and scrolls
/// </summary>
public class TuiMultiLineScrollingFramedTextBox : TuiWritable{
	
	/// <summary>
	/// Not selected frame charachter format
	private TuiFrame frame;
	
	/// <summary>
	/// Not selected frame charachter format
	/// </summary>
	public CharFormat? FrameFormat {get;
	set{
		field = value;
		if(!Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Selected frame charachter format
	/// </summary>
	public CharFormat? SelectedFrameFormat {get;
	set{
		field = value;
		if(Selected){
			frame.Format = value;
		}
	}}
	
	/// <summary>
	/// Frame charachters. An example would be '┌┐└┘──││'
	/// </summary>
	public char[] FrameChars {get{
		return frame.Chars;
	}
	set{
		frame.Chars = value;
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
	/// Format of the selectors that surround the element when selcted
	/// </summary>
	public CharFormat? SelectorFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Format of the cursor
	/// </summary>
	public CharFormat? CursorFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The visible X size of the box.
	/// </summary>
	public int BoxXsize {get{
		return frame.Xsize - 2;
	}
	set{
		if(value < 0){
			return;
		}
		frame.Xsize = value + 2;
	}}
	
	/// <summary>
	/// The visible Y size of the box
	/// </summary>
	public int BoxYsize {get{
		return frame.Ysize - 2;
	}
	set{
		if(value < 0){
			return;
		}
		frame.Ysize = value + 2;
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, int l, int xs, int ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf)
											: base(t, l, p, x, y){
		frame = new TuiFrame(chars, xs + 2, ys + 2, Placement.TopLeft, 0, 0, ff);
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		CursorFormat = curfor;
		
		Length = l;
		
		Text = t;
		
		OnSelection += (s, a) => {
			if(Selected){
				frame.Format = SelectedFrameFormat;
			}else{
				frame.Format = FrameFormat;
			}
		};
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
	public TuiMultiLineScrollingFramedTextBox(string chars, string t, int l, int xs, int ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(chars, t, l, xs, ys, p, x, y, ff, ff, tf, tf, curfor, pf){}
	
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
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineScrollingFramedTextBox(string t, int l, int xs, int ys, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? tf, CharFormat? stf, CharFormat? curfor, CharFormat? pf)
							: this(null, t, l, xs, ys, p, x, y, ff, sff, tf, stf, curfor, pf){}
	
	/// <summary>
	/// Initializes a new multiline textbox with default frame chars ('┌┐└┘──││') and same colors for selected and not selected
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="xs">Box X size</param>
	/// <param name="ys">Box Y size</param>
	/// <param name="ff">Frame format</param>
	/// <param name="tf">Text format</param>
	/// <param name="curfor">Cursor format</param>
	/// <param name="pf">Selector format</param>
	public TuiMultiLineScrollingFramedTextBox(string t, int l, int xs, int ys, Placement p, int x, int y, CharFormat? ff = null, CharFormat? tf = null, CharFormat? curfor = null, CharFormat? pf = null)
							: this(t, l, xs, ys, p, x, y, ff, ff, tf, tf, curfor, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		
		string te;
		bool showCursor = Selected && Text.Length < Length;
		if(Text.Length > BoxXsize * BoxYsize - (showCursor ? 1 : 0)){
			te = "…" + Text.Substring(Text.Length - (BoxXsize * BoxYsize) + (showCursor ? 2 : 1)) + (showCursor ? Cursor : "");
		}else{
			te = Text + (showCursor ? Cursor : "");
		}
		
		if(Selected){
			b = new Buffer(BoxXsize + 4, BoxYsize + 2);
			b.SetChar(0, 1 + BoxYsize / 2, LeftSelector, SelectorFormat);
			b.SetChar(BoxXsize + 3, 1 + BoxYsize / 2, RightSelector, SelectorFormat);
			
			b.AddBuffer(1, 0, frame.Buffer);
			
			for(int i = 2; i < BoxXsize + 2; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', SelectedTextFormat);
				}
			}
			
			for(int i = 0; i < te.Length; i++){
				if(showCursor && i == te.Length - 1){
					b.SetChar(2 + (i % BoxXsize), 1 + (i / BoxXsize), te[i], CursorFormat);
				}else{
					b.SetChar(2 + (i % BoxXsize), 1 + (i / BoxXsize), te[i], SelectedTextFormat);
				}
			}
		}else{
			b = new Buffer(BoxXsize + 2, BoxYsize + 2);
			
			b.AddBuffer(0, 0, frame.Buffer);
			
			for(int i = 1; i < BoxXsize + 1; i++){
				for(int j = 1; j < BoxYsize + 1; j++){
					b.SetChar(i, j, ' ', TextFormat);
				}
			}
			
			for(int i = 0; i < te.Length; i++){
				b.SetChar(1 + (i % BoxXsize), 1 + (i / BoxXsize), te[i], TextFormat);
			}
		}
		return b;
	}
	
	override protected bool BufferNeedsToBeGenerated(){
		return base.BufferNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
	
	override internal bool ScreenNeedsToBeGenerated(){
		return base.ScreenNeedsToBeGenerated() || frame.ScreenNeedsToBeGenerated();
	}
}