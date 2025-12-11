using System;
using AshLib;
using AshLib.Formatting;

namespace AshConsoleGraphics;

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
	/// Charachter format
	/// </summary>
	public CharFormat? Format {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new label
	/// </summary>
	/// <param name="text">The text</param>
	/// <param name="f">The format</param>
	public TuiLabel(string text, Placement p, int x, int y, CharFormat? f = null) : base(p, x, y){
		Format = f;
		Text = text;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Text.Length, 1);
		for(int i = 0; i < Text.Length; i++){
			b.SetChar(i, 0, Text[i], Format);
		}
		return b;
	}
}

/// <summary>
/// Two labels side by side, left and right text
/// </summary>
public class TuiTwoLabels : TuiElement{
	public string LeftText {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	public string RightText {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Left label format
	/// </summary>
	public CharFormat? LeftFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Right label format
	/// </summary>
	public CharFormat? RightFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new label
	/// </summary>
	/// <param name="text">The text</param>
	/// <param name="f">The format</param>
	public TuiTwoLabels(string textl, string textr, Placement p, int x, int y, CharFormat? lf = null, CharFormat? rf = null) : base(p, x, y){
		LeftFormat = lf;
		RightFormat = rf;
		LeftText = textl;
		RightText = textr;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(LeftText.Length + RightText.Length, 1);
		for(int i = 0; i < LeftText.Length; i++){
			b.SetChar(i, 0, LeftText[i], LeftFormat);
		}
		for(int i = LeftText.Length; i < LeftText.Length + RightText.Length; i++){
			b.SetChar(i, 0, RightText[i - LeftText.Length], RightFormat);
		}
		return b;
	}
}

/// <summary>
/// Two labels side by side, left and right text
/// </summary>
public class TuiMultipleLabels : TuiElement{
	string[] Texts;
	
	CharFormat?[] Formats;
	
	/// <summary>
	/// Count of labels
	/// </summary>
	public int NumberOfLabels{
		get{
			return Texts.Length;
		}
	}
	
	/// <summary>
	/// Initializes a new label
	/// </summary>
	/// <param name="text">The text</param>
	/// <param name="f">The format</param>
	public TuiMultipleLabels(IEnumerable<string> ts, Placement p, int x, int y, IEnumerable<CharFormat?> f = null) : base(p, x, y){
		if(ts == null){
			throw new ArgumentNullException(nameof(ts));
		}
		
		Texts = ts.ToArray();
		f ??= new CharFormat?[Texts.Length];
		Formats = f.ToArray();
		
		if(Texts.Length != Formats.Length){
			throw new Exception("Texts and format enumerations must be of the same length");
		}
	}
	
	public bool SetText(int index, string text){
		if(index < 0 || index >= Texts.Length){
			return false;
		}
		
		Texts[index] = text;
		needToGenBuffer = true;
		return true;
	}
	
	public string GetText(int index){
		if(index < 0 || index >= Texts.Length){
			return null;
		}
		
		return Texts[index];
	}
	
	public bool SetFormat(int index, CharFormat? format){
		if(index < 0 || index >= Formats.Length){
			return false;
		}
		
		Formats[index] = format;
		needToGenBuffer = true;
		return true;
	}
	
	public CharFormat? GetFormat(int index){
		if(index < 0 || index >= Formats.Length){
			return null;
		}
		
		return Formats[index];
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Texts.Sum(s => s?.Length ?? 0), 1);
		int c = 0;
		for(int i = 0; i < Texts.Length; i++){
			for(int j = 0; j < Texts[i].Length; j++, c++){
				b.SetChar(c, 0, Texts[i][j], Formats[i]);
			}
		}
		return b;
	}
}

/// <summary>
/// A solid rectangle of a single charachter
/// </summary>
public class TuiRectangle : TuiElement{
	public int Xsize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	public int Ysize {get;
	set{
		if(value < 0){
			return;
		}
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
	/// Charachter format
	/// </summary>
	public CharFormat? Format {get;
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
	/// <param name="f">The format</param>
	public TuiRectangle(char c, int xs, int ys, Placement p, int x, int y, CharFormat? f = null) : base(p, x, y){
		Format = f;
		Char = c;
		Xsize = xs;
		Ysize = ys;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		for(int i = 0; i < Ysize; i++){
			for(int j = 0; j < Xsize; j++){
				b.SetChar(j, i, Char, Format);
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
	public TuiSquare(char c, int s, Placement p, int x, int y, CharFormat? f = null) : base(c, s, s, p, x, y, f){}
}

/// <summary>
/// An empty on the inside frame with 8 different charachters
/// </summary>
public class TuiFrame : TuiElement, ILineElement{
	public int Xsize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	public int Ysize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Charachter format
	/// </summary>
	public CharFormat? Format {get;
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
	/// <param name="f">The format</param>
	public TuiFrame(string chars, int xs, int ys, Placement p, int x, int y, CharFormat? f = null) : base(p, x, y){
		if(chars == null || chars.Length != 8){
			chars = "┌┐└┘──││";
		}
		
		Format = f;
		Xsize = xs;
		Ysize = ys;
		
		Chars = chars.ToCharArray();
	}
	
	/// <summary>
	/// Initializes a new frame with the 8 default chars (single line)
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="f">The format</param>
	public TuiFrame(int xs, int ys, Placement p, int x, int y, CharFormat? f = null) : this(null, xs, ys, p, x, y, f){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		
		for(int i = 1; i < Xsize - 1; i++){
			b.SetChar(i, 0, Chars[4], Format);
			b.SetChar(i, Ysize - 1, Chars[5], Format);
		}
		
		for(int i = 1; i < Ysize - 1; i++){
			b.SetChar(0, i, Chars[6], Format);
			b.SetChar(Xsize - 1, i, Chars[7], Format);
		}
		
		b.SetChar(0, 0, Chars[0], Format);
		b.SetChar(Xsize - 1, 0, Chars[1], Format);
		b.SetChar(0, Ysize - 1, Chars[2], Format);
		b.SetChar(Xsize - 1, Ysize - 1, Chars[3], Format);
		
		return b;
	}
	
	public BitBuffer GenerateBitBuffer(){
		BitBuffer b = new BitBuffer(Xsize, Ysize);
		
		for(int i = 0; i < Xsize; i++){
			b.SetBit(i, 0, true);
			b.SetBit(i, Ysize - 1, true);
		}
		
		for(int i = 0; i < Ysize; i++){
			b.SetBit(0, i, true);
			b.SetBit(Xsize - 1, i, true);
		}
		
		return b;
	}
}

/// <summary>
/// Straight horizontal line
/// </summary>
public class TuiHorizontalLine : TuiElement, ILineElement{
	public int Xsize {get;
	set{
		if(value < 0){
			return;
		}
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
	/// Charachter format
	/// </summary>
	public CharFormat? Format {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new line
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="c">The char</param>
	/// <param name="f">The format</param>
	public TuiHorizontalLine(int xs, char c, Placement p, int x, int y, CharFormat? f = null) : base(p, x, y){
		Format = f;
		Xsize = xs;
		Char = c;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, 1);
		for(int i = 0; i < Xsize; i++){
			b.SetChar(i, 0, Char, Format);
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
	public int Ysize {get;
	set{
		if(value < 0){
			return;
		}
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
	/// Charachter format
	/// </summary>
	public CharFormat? Format {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new line
	/// </summary>
	/// <param name="ys">The y size</param>
	/// <param name="c">The char</param>
	/// <param name="f">The format</param>
	public TuiVerticalLine(int ys, char c, Placement p, int x, int y, CharFormat? f = null) : base(p, x, y){
		Format = f;
		Ysize = ys;
		Char = c;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(1, Ysize);
		for(int i = 0; i < Ysize; i++){
			b.SetChar(0, i, Char, Format);
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
	public int Xsize {get;
	set{
		if(value < 0){
			return;
		}
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
		field = Math.Clamp(value, 0, 100);
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Complete Charachter format
	/// </summary>
	public CharFormat? CompleteFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Incomplete Charachter format
	/// </summary>
	public CharFormat? IncompleteFormat {get;
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
	/// <param name="cf">The filled char format</param>
	/// <param name="uf">The not filled char format</param>
	public TuiProgressBar(int xs, char c, char u, Placement p, int x, int y, CharFormat? cf, CharFormat? uf) : base(p, x, y){
		CompleteFormat = cf;
		IncompleteFormat = uf;
		
		Xsize = xs;
		CompleteChar = c;
		IncompleteChar = u;
	}
	
	/// <summary>
	/// Initializes a new progress bar with the same format for filled and not filled
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="c">The filled char</param>
	/// <param name="u">The not filled char</param>
	/// <param name="f">The format</param>
	public TuiProgressBar(int xs, char c, char u, Placement p, int x, int y, CharFormat? f = null) : this(xs, c, u, p, x, y, f, f){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, 1);
		
		int completeChars = Percentage * Xsize / 100;
		
		for(int i = 0; i < completeChars; i++){
			b.SetChar(i, 0, CompleteChar, CompleteFormat);
		}
		for(int i = completeChars; i < Xsize; i++){
			b.SetChar(i, 0, IncompleteChar, IncompleteFormat);
		}
		return b;
	}
}

/// <summary>
/// A rectangle for words that separates words into different lines
/// </summary>
public class TuiLog : TuiElement{
	public int Xsize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
		generateLines();
	}}
	
	public int Ysize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
		generateLines();
	}}
	
	private string _text = "";
	
	/// <summary>
	/// The whole text
	/// </summary>
	public string Text {get{
		return _text;
	} set{
		_text = value;
		needToGenBuffer = true;
		generateLines();
	}}
	
	private List<string> lines;
	
	/// <summary>
	/// Charachter format
	/// </summary>
	public CharFormat? Format {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// How much the text is scrolled
	/// </summary>
	public int Scroll {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new log
	/// </summary>
	public TuiLog(int xs, int ys, Placement p, int x, int y, CharFormat? f = null) : base(p, x, y){
		Format = f;
		
		Xsize = xs;
		Ysize = ys;
		
		Text = "";
	}
	
	/// <summary>
	/// Appends some text. Analog of Console.Write
	/// </summary>
	public void Append(string s){
		string[] pars = s.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
		
		//Console.WriteLine(s);
		//Console.WriteLine(pars.Length);
		//Console.WriteLine(DivideLines("", 30).Length);
		
		if(pars.Length == 1){
			string t = lines[^1] + pars[0];
			lines.RemoveAt(lines.Count - 1);
			lines.AddRange(DivideLines(t, Xsize));
		}else if(pars.Length > 1){
			string t = lines[^1] + pars[0];
			lines.RemoveAt(lines.Count - 1);
			lines.AddRange(DivideLines(t, Xsize));
			
			foreach(string l in pars.Skip(1)){
				lines.AddRange(DivideLines(l, Xsize));
			}
		}
		
		//Console.ReadKey();
		
		_text += s;
		
		needToGenBuffer = true;
	}
	
	/// <summary>
	/// Appends some text and ends the line. Analog of Console.WriteLine
	/// </summary>
	public void AppendLine(string s){
		Append(s + Environment.NewLine);
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		
		string[] pars;
		
		if(lines.Count > Ysize){
			pars = lines.GetRange(Math.Max(0, lines.Count - Ysize - Scroll), Ysize).ToArray();
		}else{
			pars = lines.ToArray();
		}
		
		for(int i = 0; i < Xsize; i++){
			for(int j = 0; j < Ysize; j++){
				b.SetChar(i, j, null, Format);
			}
		}
		
		for(int i = 0; i < pars.Length; i++){
			for(int j = 0; j < pars[i].Length; j++){
				b.SetChar(j, i, pars[i][j], Format);
			}
		}
		
		return b;
	}
	
	internal void generateLines(){
		string[] pars = _text.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
		
		List<string> ls = new List<string>();
		
		foreach(string l in pars){
			ls.AddRange(DivideLines(l, Xsize));
		}
		
		if(ls.Count == 0){
			ls.Add("");
		}
		
		lines = ls;
	}
	
	static string[] DivideLines(string input, int maxCharsPerLine){
		if(input.Length == 0){
			return new string[]{""};
		}
        List<string> lines = new List<string>();
        string[] words = input.Split(new string[]{" ", "\t"}, StringSplitOptions.None);	
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

/* /// <summary>
/// A rectangle for words that separates words into different lines and has format
/// </summary>
public class TuiFormatLog : TuiElement{
	public int Xsize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
		generateLines();
	}}
	
	public int Ysize {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
		generateLines();
	}}
	
	private FormatString _text = new FormatString();
	
	/// <summary>
	/// The whole text
	/// </summary>
	public FormatString Text {get{
		return _text;
	} set{
		_text = value;
		needToGenBuffer = true;
		generateLines();
	}}
	
	private List<FormatString> lines;
	
	/// <summary>
	/// Default format
	/// </summary>
	public CharFormat? DefFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// How much the text is scrolled
	/// </summary>
	public int Scroll {get;
	set{
		if(value < 0){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new log
	/// </summary>
	public TuiFormatLog(int xs, int ys, Placement p, int x, int y, CharFormat? f = null) : base(p, x, y){
		DefFormat = f;
		
		Xsize = xs;
		Ysize = ys;
		
		Text = "";
	}
	
	/// <summary>
	/// Appends some text. Analog of Console.Write
	/// </summary>
	public void Append(string s, CharFormat? f){
		string[] pars = s.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
		
		//Console.WriteLine(s);
		//Console.WriteLine(pars.Length);
		//Console.WriteLine(DivideLines("", 30).Length);
		
		if(pars.Length == 1){
			FormatString t = FormatString.Clone(lines[^1]);
			t.Append(pars[0], f);
			lines.RemoveAt(lines.Count - 1);
			lines.AddRange(DivideLines(t, Xsize));
		}else if(pars.Length > 1){
			FormatString t = FormatString.Clone(lines[^1]);
			t.Append(pars[0], f);
			lines.RemoveAt(lines.Count - 1);
			lines.AddRange(DivideLines(t, Xsize));
			
			foreach(string l in pars.Skip(1)){
				lines.AddRange(DivideLines(new FormatString(l, f), Xsize));
			}
		}
		
		//Console.ReadKey();
		
		_text.Append(s, f);
		
		needToGenBuffer = true;
	}
	
	/// <summary>
	/// Appends some text and ends the line. Analog of Console.WriteLine
	/// </summary>
	public void AppendLine(string s, CharFormat? f){
		Append(s + Environment.NewLine, f);
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b = new Buffer(Xsize, Ysize);
		
		FormatString[] pars;
		
		if(lines.Count > Ysize){
			pars = lines.GetRange(Math.Max(0, lines.Count - Ysize - Scroll), Ysize).ToArray();
		}else{
			pars = lines.ToArray();
		}
		
		for(int i = 0; i < Xsize; i++){
			for(int j = 0; j < Ysize; j++){
				b.SetChar(i, j, null, DefFormat);
			}
		}
		
		for(int i = 0; i < pars.Length; i++){
			int j = 0;
			foreach((char c, CharFormat? f) in pars[i]){
				b.SetChar(j, i, c, combine(f, DefFormat));
				j++;
			}
		}
		
		return b;
	}
	
	internal void generateLines(){
		FormatString[] pars = _text.SplitIntoLines();
		
		List<FormatString> ls = new List<FormatString>();
		
		foreach(FormatString l in pars){
			ls.AddRange(DivideLines(l, Xsize));
		}
		
		if(ls.Count == 0){
			ls.Add("");
		}
		
		lines = ls;
	}
	
	static FormatString[] DivideLines(FormatString input, int maxCharsPerLine){
		if(input.Length == 0){
			return new FormatString[]{""};
		}
        List<FormatString> lines = new List<FormatString>();
        FormatString[] words = splitWords(input);	
        FormatString currentLine = new FormatString();

        foreach(FormatString word in words){
            if(word.Length > maxCharsPerLine){
                // Break up long words if they exceed maxCharsPerLine
                if(currentLine.Length > 0){
                    lines.Add(currentLine);
                    currentLine = new FormatString();
                }

                for(int i = 0; i < word.Length; i += maxCharsPerLine){
                    int length = Math.Min(maxCharsPerLine, word.Length - i);
                    lines.Add(word.Substring(i, length));
                }
            }else if(currentLine.Length + word.Length + 1 <= maxCharsPerLine){
                // Add the word to the current line if it fits
                if(currentLine.Length > 0){
                    currentLine.Append(" ", (CharFormat?) null);
                }
                currentLine.Append(word);
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
	
	static FormatString[] splitWords(FormatString s){
		List<FormatString> words = new();
		FormatString current = new FormatString();
		foreach((char c, CharFormat? f) in s){
			if(c == ' ' || c == '\t'){
				words.Add(current);
				current = new FormatString();
			}else{
				current.Append(c, f);
			}
		}
		words.Add(current);
		
		return words.ToArray();
	}
	
	static CharFormat? combine(CharFormat? a, CharFormat? b){
		if(a == null){
			return b;
		}
		if(b == null){
			return a;
		}
		
		return new CharFormat(a.density ?? b.density, a.italic ?? b.italic, a.underline ?? b.underline, a.strikeThrough ?? b.strikeThrough, a.foreground ?? b.foreground, false, a.background ?? b.background, false);
	}
} */