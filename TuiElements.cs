using System;
using AshLib;

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
	
	internal bool needToGenScreenBuffer = true;
	
	/// <summary>
	/// Will be called when parent screen resizes
	/// </summary>
	public Action<TuiScreen>? OnParentResize {internal get; set;}
	
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
	
	/// <summary>
	/// In most cases, the base implementation is enough
	/// </summary>
	virtual protected bool BufferNeedsToBeGenerated(){
		return needToGenBuffer;
	}
	
	virtual internal bool ScreenNeedsToBeGenerated(){
		return needToGenScreenBuffer;
	}
}

/// <summary>
/// Just some text
/// </summary>
public class TuiLabel : TuiElement{
	public string Text {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color
	/// </summary>
	public Color3? FgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color
	/// </summary>
	public Color3? BgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new label
	/// </summary>
	/// <param name="text">The text</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiLabel(string text, Placement p, int x, int y, Color3? f, Color3? b) : base(p, x, y){
		FgColor = f;
		BgColor = b;
		Text = text;
	}
	
	/// <summary>
	/// Initializes a new label with null colors
	/// </summary>
	/// <param name="text">The text</param>
	public TuiLabel(string text, Placement p, int x, int y) : this(text, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Text.Length, 1);
		for(int i = 0; i < Text.Length; i++){
			b.SetChar(i, 0, Text[i], FgColor, BgColor);
		}
		return b;
	}
}

/// <summary>
/// A solid rectangle of a single charachter
/// </summary>
public class TuiRectangle : TuiElement{
	public uint Xsize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	public uint Ysize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The charachter of the whole rectangle
	/// </summary>
	public char Char {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color
	/// </summary>
	public Color3? FgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color
	/// </summary>
	public Color3? BgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new rectangle
	/// </summary>
	/// <param name="c">The charachter</param>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiRectangle(char c, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b) : base(p, x, y){
		FgColor = f;
		BgColor = b;
		Char = c;
		Xsize = xs;
		Ysize = ys;
	}
	
	/// <summary>
	/// Initializes a new rectangle with null colors
	/// </summary>
	/// <param name="c">The charachter</param>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiRectangle(char c, uint xs, uint ys, Placement p, int x, int y) : this(c, xs, ys, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		for(int i = 0; i < Ysize; i++){
			for(int j = 0; j < Xsize; j++){
				b.SetChar(j, i, Char, FgColor, BgColor);
			}
		}
		return b;
	}
}

/// <summary>
/// A solid square of a single charachter
/// </summary>
public class TuiSquare : TuiRectangle{
	/// <summary>
	/// Initializes a new square
	/// </summary>
	/// <param name="s">The x and y size</param>
	public TuiSquare(char c, uint s, Placement p, int x, int y, Color3? f, Color3? b) : base(c, s, s, p, x, y, f, b){}
	
	/// <summary>
	/// Initializes a new square with null colors
	/// </summary>
	/// <param name="s">The x and y size</param>
	public TuiSquare(char c, uint s, Placement p, int x, int y) : this(c, s, p, x, y, null, null){}
}

/// <summary>
/// An empty on the inside frame with 8 different charachters
/// </summary>
public class TuiFrame : TuiElement, ILineElement{
	public uint Xsize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	public uint Ysize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color
	/// </summary>
	public Color3? FgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color
	/// </summary>
	public Color3? BgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// All the 8 needed chars. An example would be "┌┐└┘──││"
	/// </summary>
	public char[] Chars {get;
	set{
		if(value == null || value.Length != 8){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new frame
	/// </summary>
	/// <param name="chars">The 8 needed chars</param>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 8 chars long</exception>
	public TuiFrame(string chars, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b) : base(p, x, y){
		if(chars == null || chars.Length != 8){
			throw new ArgumentException("String must be 8 chars long");
		}
		
		FgColor = f;
		BgColor = b;
		Xsize = xs;
		Ysize = ys;
		
		Chars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new frame with null colors
	/// </summary>
	/// <param name="chars">The 8 needed chars</param>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	public TuiFrame(string chars, uint xs, uint ys, Placement p, int x, int y) : this(chars, xs, ys, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new frame with the 8 chars directly
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiFrame(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b) : base(p, x, y){
		FgColor = f;
		BgColor = b;
		Xsize = xs;
		Ysize = ys;
		
		Chars = new char[]{topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right};
	}
	
	/// <summary>
	/// Initializes a new frame with the 8 chars directly and null colors
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	public TuiFrame(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, uint xs, uint ys, Placement p, int x, int y) : this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, xs, ys, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new frame with the 8 default chars (single line)
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiFrame(uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b) : this('┌', '┐', '└', '┘', '─', '─', '│', '│', xs, ys, p, x, y, f, b){}
	
	/// <summary>
	/// Initializes a new frame with the 8 default chars (single line) and null colors
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	public TuiFrame(uint xs, uint ys, Placement p, int x, int y) : this(xs, ys, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		
		for(int i = 1; i < Xsize - 1; i++){
			b.SetChar(i, 0, Chars[4], FgColor, BgColor);
			b.SetChar(i, (int) Ysize - 1, Chars[5], FgColor, BgColor);
		}
		
		for(int i = 1; i < Ysize - 1; i++){
			b.SetChar(0, i, Chars[6], FgColor, BgColor);
			b.SetChar((int) Xsize - 1, i, Chars[7], FgColor, BgColor);
		}
		
		b.SetChar(0, 0, Chars[0], FgColor, BgColor);
		b.SetChar((int) Xsize - 1, 0, Chars[1], FgColor, BgColor);
		b.SetChar(0, (int) Ysize - 1, Chars[2], FgColor, BgColor);
		b.SetChar((int) Xsize - 1, (int) Ysize - 1, Chars[3], FgColor, BgColor);
		
		return b;
	}
	
	public BitBuffer GenerateBitBuffer(){
		BitBuffer b = new BitBuffer(Xsize, Ysize);
		
		for(int i = 0; i < Xsize; i++){
			b.SetBit(i, 0, true);
			b.SetBit(i, (int) Ysize - 1, true);
		}
		
		for(int i = 0; i < Ysize; i++){
			b.SetBit(0, i, true);
			b.SetBit((int) Xsize - 1, i, true);
		}
		
		return b;
	}
}

/// <summary>
/// Straight horizontal line
/// </summary>
public class TuiHorizontalLine : TuiElement, ILineElement{
	public uint Xsize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The carachter the line uses
	/// </summary>
	public char Char {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color
	/// </summary>
	public Color3? FgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color
	/// </summary>
	public Color3? BgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new line
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="c">The char</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiHorizontalLine(uint xs, char c, Placement p, int x, int y, Color3? f, Color3? b) : base(p, x, y){
		FgColor = f;
		BgColor = b;
		Xsize = xs;
		Char = c;
	}
	
	/// <summary>
	/// Initializes a new line with null colors
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="c">The char</param>
	public TuiHorizontalLine(uint xs, char c, Placement p, int x, int y) : this(xs, c, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, 1);
		for(int i = 0; i < Xsize; i++){
			b.SetChar(i, 0, Char, FgColor, BgColor);
		}
		return b;
	}
	
	public BitBuffer GenerateBitBuffer(){
		return new BitBuffer(Xsize, 1).SetAllTrue();
	}
}

/// <summary>
/// Straight vertical line
/// </summary>
public class TuiVerticalLine : TuiElement, ILineElement{
	public uint Ysize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The carachter the line uses
	/// </summary>
	public char Char {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color
	/// </summary>
	public Color3? FgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color
	/// </summary>
	public Color3? BgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new line
	/// </summary>
	/// <param name="ys">The y size</param>
	/// <param name="c">The char</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiVerticalLine(uint ys, char c, Placement p, int x, int y, Color3? f, Color3? b) : base(p, x, y){
		FgColor = f;
		BgColor = b;
		Ysize = ys;
		Char = c;
	}
	
	/// <summary>
	/// Initializes a new line with null colors
	/// </summary>
	/// <param name="ys">The y size</param>
	/// <param name="c">The char</param>
	public TuiVerticalLine(uint ys, char c, Placement p, int x, int y) : this(ys, c, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(1, Ysize);
		for(int i = 0; i < Ysize; i++){
			b.SetChar(0, i, Char, FgColor, BgColor);
		}
		return b;
	}
	
	public BitBuffer GenerateBitBuffer(){
		return new BitBuffer(1, Ysize).SetAllTrue();
	}
}

/// <summary>
/// A progress bar with a percentage of filled
/// </summary>
public class TuiProgressBar : TuiElement{
	public uint Xsize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The char of the not filled part
	/// </summary>
	public char IncompleteChar {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The char of the filled part
	/// </summary>
	public char CompleteChar {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The percentage of the bar that is filled
	/// </summary>
	public int Percentage {get;
	set{
		if(value < 0 || value > 100){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color of the filled part
	/// </summary>
	public Color3? FgCompleteColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the filled part
	/// </summary>
	public Color3? BgCompleteColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color of the not filled part
	/// </summary>
	public Color3? FgIncompleteColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the not filled part
	/// </summary>
	public Color3? BgIncompleteColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new progress bar
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="c">The filled char</param>
	/// <param name="u">The not filled char</param>
	/// <param name="cf">The filled foreground color</param>
	/// <param name="cb">The filled background color</param>
	/// <param name="uf">The not filled foreground color</param>
	/// <param name="ub">The not filled background color</param>
	public TuiProgressBar(uint xs, char c, char u, Placement p, int x, int y, Color3? cf, Color3? cb, Color3? uf, Color3? ub) : base(p, x, y){
		FgCompleteColor = cf;
		BgCompleteColor = cb;
		FgIncompleteColor = uf;
		BgIncompleteColor = ub;
		
		Xsize = xs;
		CompleteChar = c;
		IncompleteChar = u;
	}
	
	/// <summary>
	/// Initializes a new progress bar with the same color for filled and not filled
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="c">The filled char</param>
	/// <param name="u">The not filled char</param>
	/// <param name="f">The foreground color</param>
	/// <param name="b">The background color</param>
	public TuiProgressBar(uint xs, char c, char u, Placement p, int x, int y, Color3? f, Color3? b) : this(xs, c, u, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new progress bar with null colors
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="c">The filled char</param>
	/// <param name="u">The not filled char</param>
	public TuiProgressBar(uint xs, char c, char u, Placement p, int x, int y) : this(xs, c, u, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, 1);
		
		int completeChars = Percentage * (int) Xsize / 100;
		
		for(int i = 0; i < completeChars; i++){
			b.SetChar(i, 0, CompleteChar, FgCompleteColor, BgCompleteColor);
		}
		for(int i = completeChars; i < Xsize; i++){
			b.SetChar(i, 0, IncompleteChar, FgIncompleteColor, BgIncompleteColor);
		}
		return b;
	}
}

/// <summary>
/// A rectangle for words that separates words into different lines and shows the last lines
/// </summary>
public class TuiLog : TuiElement{
	public uint Xsize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	public uint Ysize {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The whole text
	/// </summary>
	public string Text {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color
	/// </summary>
	public Color3? FgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color
	/// </summary>
	public Color3? BgColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new log
	/// </summary>
	public TuiLog(uint xs, uint ys, Placement p, int x, int y, Color3? f, Color3? b) : base(p, x, y){
		FgColor = f;
		BgColor = b;
		
		Xsize = xs;
		Ysize = ys;
		
		Text = "";
	}
	
	/// <summary>
	/// Initializes a new log with null colors
	/// </summary>
	public TuiLog(uint xs, uint ys, Placement p, int x, int y) : this(xs, ys, p, x, y, null, null){}
	
	/// <summary>
	/// Appends some text. Analog of Console.Write
	/// </summary>
	public void Append(string s){
		Text += s;
	}
	
	/// <summary>
	/// Appends some text and ends the line. Analog of Console.WriteLine
	/// </summary>
	public void AppendLine(string s){
		Text += s + Environment.NewLine;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		
		string[] pars = Text.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
		
		List<string> lines = new List<string>();
		
		foreach(string l in pars){
			lines.AddRange(DivideLines(l, (int) Xsize));
		}
		
		pars = lines.TakeLast((int) Ysize).ToArray();
		
		for(int i = 0; i < Xsize; i++){
			for(int j = 0; j < Ysize; j++){
				b.SetChar(i, j, null, null, BgColor);
			}
		}
		
		for(int i = 0; i < pars.Length; i++){
			for(int j = 0; j < pars[i].Length; j++){
				b.SetChar(j, i, pars[i][j], FgColor, BgColor);
			}
		}
		
		return b;
	}
	
	static string[] DivideLines(string input, int maxCharsPerLine){
        List<string> lines = new List<string>();
        string[] words = input.Split(new string[]{" ", "\n", "\r"}, StringSplitOptions.None);
        string currentLine = "";

        foreach(string word in words){
            if(word.Length > maxCharsPerLine){
                // Break up long words if they exceed maxCharsPerLine
                if(currentLine.Length > 0){
                    lines.Add(currentLine);
                    currentLine = string.Empty;
                }

                for(int i = 0; i < word.Length; i += maxCharsPerLine){
                    int length = Math.Min(maxCharsPerLine, word.Length - i);
                    lines.Add(word.Substring(i, length));
                }
            }else if(currentLine.Length + word.Length + 1 <= maxCharsPerLine){
                // Add the word to the current line if it fits
                if(currentLine.Length > 0){
                    currentLine += " ";
                }
                currentLine += word;
            }else{
                // Start a new line if the word doesn't fit
                if(currentLine.Length > 0){
                    lines.Add(currentLine);
                }
                currentLine = word;
            }
        }

        // Add the last line if not empty
        if(currentLine.Length > 0){
            lines.Add(currentLine);
        }

        return lines.ToArray();
    }
}