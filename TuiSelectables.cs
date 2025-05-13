using System;
using AshLib;
using AshLib.Formatting;

namespace AshConsoleGraphics.Interactive;

//Classes for interactive screens

/// <summary>
/// An interactive element that can be selected and does things
/// </summary>
public abstract class TuiSelectable : TuiElement{
	/// <summary>
	/// If it is selected or not
	/// </summary>
	public bool Selected {get;
	internal set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Keybinds and their actions
	/// </summary>
	protected Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiSelectable, ConsoleKeyInfo>> keyFunctions {get; private set;}
	
	/// <summary>
	/// Initializes a new selectable element
	/// </summary>
	/// <param name="p">The relative placement method</param>
	/// <param name="x">The relative x offset</param>
	/// <param name="y">The relative x offset</param>
	protected TuiSelectable(Placement p, int x, int y) : base(p, x, y){
		keyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiSelectable, ConsoleKeyInfo>>();
	}
	
	/// <summary>
	/// Subscribes a new key event
	/// </summary>
	/// <param name="k">The key pressed</param>
	/// <param name="m">The key modifiers (cntrl, shift)</param>
	/// <param name="keyFunction">The action to call</param>
	public TuiSelectable SubKeyEvent(ConsoleKey k, ConsoleModifiers m, Action<TuiSelectable, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, m)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Subscribes a new key event with no modifiers
	/// </summary>
	/// <param name="k">The key pressed</param>
	/// <param name="keyFunction">The action to call</param>
	public TuiSelectable SubKeyEvent(ConsoleKey k, Action<TuiSelectable, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, ConsoleModifiers.None)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Deletes all key events
	/// </summary>
	public void DeleteAllKeyEvents(){
		this.keyFunctions.Clear();
	}
	
	/// <summary>
	/// Handles a key press
	/// </summary>
	public virtual bool HandleKey(ConsoleKeyInfo keyInfo){
		if(keyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
			keyFunctions[(keyInfo.Key, keyInfo.Modifiers)].Invoke(this, keyInfo);
			return true;
		}
		return false;
	}
}

/// <summary>
/// A label pressable with enter
/// </summary>
public class TuiButton : TuiSelectable{
	/// <summary>
	/// Text to show
	/// </summary>
	public string Text {get;
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
	/// Initializes a new button with all colors
	/// </summary>
	/// <param name="t">Text to display</param>
	/// <param name="f">Not selected text format</param>
	/// <param name="sf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiButton(string t, Placement p, int x, int y, CharFormat? f, CharFormat? sf, CharFormat? pf) : base(p, x, y){
		TextFormat = f;
		SelectedTextFormat = sf;
		SelectorFormat = pf;
		Text = t;
	}
	
	/// <summary>
	/// Initializes a new button with the same text color when selected and when not
	/// </summary>
	/// <param name="t">Text to display</param>
	/// <param name="f">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiButton(string t, Placement p, int x, int y, CharFormat? f = null, CharFormat? pf = null) : this(t, p, x, y, f, f, pf){}
	
	/// <summary>
	/// Sets the enter action of the button
	/// </summary>
	/// <param name="a">Action</param>
	public TuiButton SetAction(Action<TuiSelectable, ConsoleKeyInfo> a){
		SubKeyEvent(ConsoleKey.Enter, a);
		return this;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(Text.Length + 2, 1);
			b.SetChar(0, 0, '>', SelectorFormat);
			b.SetChar(Text.Length + 1, 0, '<', SelectorFormat);
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + i, 0, Text[i], SelectedTextFormat);
			}
		}else{
			b = new Buffer(Text.Length, 1);
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(i, 0, Text[i], TextFormat);
			}
		}
		return b;
	}
}

/// <summary>
/// Lets you pick options with the lateral arrows
/// </summary>
public class TuiOptionPicker : TuiSelectable{
	/// <summary>
	/// The options 
	/// </summary>
	public string[] Options {get; private set;}
	
	/// <summary>
	/// The index in Options of the selected option
	/// </summary>
	public uint SelectedOptionIndex {get;
	set{
		if(value >= Options.Length){
			return;
		}
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// The selected option. Get only
	/// </summary>
	public string SelectedOption {get{
		return Options[SelectedOptionIndex];
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
	/// Initializes a new option picker with all colors
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="so">Index of the selected option</param>
	/// <param name="f">Not selected text format</param>
	/// <param name="sf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	/// <exception cref="System.ArgumentException">Thrown when t is null or no option was provided (length 0)</exception>
	public TuiOptionPicker(string[] t, uint so, Placement p, int x, int y, CharFormat? f, CharFormat? sf, CharFormat? pf) : base(p, x, y){
		if(t == null || t.Length == 0){
			throw new ArgumentException("At least one option needs to be provided");
		}
		
		TextFormat = f;
		SelectedTextFormat = sf;
		SelectorFormat = pf;
		
		Options = t;
		SelectedOptionIndex = so;
		
		SubKeyEvent(ConsoleKey.LeftArrow, (s, ck) => {SelectedOptionIndex--;});
		SubKeyEvent(ConsoleKey.RightArrow, (s, ck) => {SelectedOptionIndex++;});
	}
	
	/// <summary>
	/// Initializes a new option picker with selected option 0 with all colors
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="f">Not selected text format</param>
	/// <param name="sf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiOptionPicker(string[] t, Placement p, int x, int y, CharFormat? f, CharFormat? sf, CharFormat? pf)
						: this(t, 0, p, x, y, f, sf, pf){}
	
	/// <summary>
	/// Initializes a new option picker with the same text color when selected and when not
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="so">Index of the selected option</param>
	/// <param name="f">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiOptionPicker(string[] t, uint so, Placement p, int x, int y, CharFormat? f = null, CharFormat? pf = null)
						: this(t, so, p, x, y, f, f, pf){}
	
	/// <summary>
	/// Initializes a new option picker with selected option 0 with the same text color when selected and when not
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="f">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiOptionPicker(string[] t, Placement p, int x, int y, CharFormat? f = null, CharFormat? pf = null)
						: this(t, 0, p, x, y, f, f, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(SelectedOption.Length + 2, 1);
			b.SetChar(0, 0, '<', SelectorFormat);
			b.SetChar(SelectedOption.Length + 1, 0, '>', SelectorFormat);
			for(int i = 0; i < SelectedOption.Length; i++){
				b.SetChar(1 + i, 0, SelectedOption[i], SelectedTextFormat);
			}
		}else{
			b = new Buffer(SelectedOption.Length, 1);
			for(int i = 0; i < SelectedOption.Length; i++){
				b.SetChar(i, 0, SelectedOption[i], TextFormat);
			}
		}
		return b;
	}
}

/// <summary>
/// Lets you pick between a number range with the lateral arrows
/// </summary>
public class TuiNumberPicker : TuiSelectable{
	
	/// <summary>
	/// Lower bound on the number
	/// </summary>
	public int LowerLimit {get; set{
		if(value > UpperLimit){
			return;
		}
		
		if(Number < value){
			Number = value;
		}
		
		field = value;
	}}
	
	/// <summary>
	/// Upper bound on the number
	/// </summary>
	public int UpperLimit {get; set{
		if(value < LowerLimit){
			return;
		}
		
		if(Number > value){
			Number = value;
		}
		
		field = value;
	}}
	
	/// <summary>
	/// Selected number
	/// </summary>
	public int Number {get; set{
		if(value < LowerLimit){
			field = LowerLimit;
			needToGenBuffer = true;
			return;
		}
		
		if(value > UpperLimit){
			field = UpperLimit;
			needToGenBuffer = true;
			return;
		}
		
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// What the selected number will be incremented/decremented each time an arrow is pressed
	/// </summary>
	public int Interval;
	
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
	/// Initializes a new number picker with all colors
	/// </summary>
	/// <param name="lower">Lower bound</param>
	/// <param name="upper">Upper bound</param>
	/// <param name="interval">Increment interval</param>
	/// <param name="num">Initial selected number</param>
	/// <param name="f">Not selected text format</param>
	/// <param name="sf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	/// <exception cref="System.ArgumentException">Thrown when lower isgreater than upper</exception>
	public TuiNumberPicker(int lower, int upper, int interval, int num, Placement p, int x, int y, CharFormat? f, CharFormat? sf, CharFormat? pf) : base(p, x, y){
		if(lower > upper){
			throw new ArgumentException("Lower limit cant be greater than upper");
		}
		
		TextFormat = f;
		SelectedTextFormat = sf;
		SelectorFormat = pf;
		
		UpperLimit = upper;
		LowerLimit = lower;
		Interval = interval;
		Number = num;
		
		SubKeyEvent(ConsoleKey.LeftArrow, NumberDown);
		SubKeyEvent(ConsoleKey.RightArrow, NumberUp);
	}
	
	/// <summary>
	/// Initializes a new number picker with the same text color when selected and when not
	/// </summary>
	/// <param name="lower">Lower bound</param>
	/// <param name="upper">Upper bound</param>
	/// <param name="interval">Increment interval</param>
	/// <param name="num">Initial selected number</param>
	/// <param name="f">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiNumberPicker(int lower, int upper, int interval, int num, Placement p, int x, int y, CharFormat? f = null, CharFormat? pf = null)
						: this(lower, upper, interval, num, p, x, y, f, f, pf){}
	
	/// <summary>
	/// Increments the number by the interval
	/// </summary>
	public void NumberUp(TuiSelectable s, ConsoleKeyInfo ck){
		Number += Interval;
	}
	
	/// <summary>
	/// Decrements the number by the interval
	/// </summary>
	public void NumberDown(TuiSelectable s, ConsoleKeyInfo ck){
		Number -= Interval;
	}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		string s = Number.ToString();
		if(Selected){
			b = new Buffer(s.Length + 2, 1);
			b.SetChar(0, 0, '<', SelectorFormat);
			b.SetChar(s.Length + 1, 0, '>', SelectorFormat);
			for(int i = 0; i < s.Length; i++){
				b.SetChar(1 + i, 0, s[i], SelectedTextFormat);
			}
		}else{
			b = new Buffer(s.Length, 1);
			for(int i = 0; i < s.Length; i++){
				b.SetChar(i, 0, s[i], TextFormat);
			}
		}
		return b;
	}
}

/// <summary>
/// Checkbox (on/off) with a frame
/// </summary>
public class TuiFramedCheckBox : TuiSelectable{
	
	/// <summary>
	/// If its checked or not
	/// </summary>
	public bool Checked {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
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
	/// Not selected check charachter format
	/// </summary>
	public CharFormat? CheckFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected check charachter format
	/// </summary>
	public CharFormat? SelectedCheckFormat {get;
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
	/// Check carachter when the checkbox is unchecked
	/// </summary>
	public char UnCheckedChar {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Check carachter when the checkbox is checked
	/// </summary>
	public char CheckedChar {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new framed checkbox
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="cf">Not selected check format</param>
	/// <param name="scf">Selected check format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedCheckBox(string chars, char u, char c, bool b, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? cf, CharFormat? scf, CharFormat? pf) : base(p, x, y){
		if(chars == null || chars.Length != 8){
			chars = "┌┐└┘──││";
		}
		
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		CheckFormat = cf;
		SelectedCheckFormat = scf;
		SelectorFormat = pf;
		
		UnCheckedChar = u;
		CheckedChar = c;
		Checked = b;
		
		FrameChars = chars.ToCharArray();
		
		SubKeyEvent(ConsoleKey.Enter, (s, ck) => {Checked = !Checked;});
	}
	
	/// <summary>
	/// Initializes a new framed checkbox with the same colors when selected and not selected
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Frame format</param>
	/// <param name="cf">Check format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedCheckBox(string chars, char u, char c, bool b, Placement p, int x, int y, CharFormat? ff = null, CharFormat? cf = null, CharFormat? pf = null)
							: this(chars, u, c, b, p, x, y, ff, ff, cf, cf, pf){}
	
	/// <summary>
	/// Initializes a new framed checkbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="cf">Not selected check format</param>
	/// <param name="scf">Selected check format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedCheckBox(char u, char c, bool b, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? cf, CharFormat? scf, CharFormat? pf)
							: this(null, u, c, b, p, x, y, ff, sff, cf, scf, pf){}
	
	/// <summary>
	/// Initializes a new framed checkbox with defaut frame chars and the same colors when selected and not selected
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Frame format</param>
	/// <param name="cf">Check format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedCheckBox(char u, char c, bool b, Placement p, int x, int y, CharFormat? ff = null, CharFormat? cf = null, CharFormat? pf = null)
							: this(u, c, b, p, x, y, ff, ff, cf, cf, pf){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(5, 3);
			b.SetChar(0, 1, '>', SelectorFormat);
			b.SetChar((int) 4, 1, '<', SelectorFormat);
			
			b.SetChar(2, 0, FrameChars[4], SelectedFrameFormat);
			b.SetChar(2, 2, FrameChars[5], SelectedFrameFormat);
			
			b.SetChar(1, 1, FrameChars[6], SelectedFrameFormat);
			b.SetChar((int) 3, 1, FrameChars[7], SelectedFrameFormat);
			
			b.SetChar(1, 0, FrameChars[0], SelectedFrameFormat);
			b.SetChar((int) 3, 0, FrameChars[1], SelectedFrameFormat);
			b.SetChar(1, 2, FrameChars[2], SelectedFrameFormat);
			b.SetChar((int) 3, 2, FrameChars[3], SelectedFrameFormat);
			
			b.SetChar(2, 1, Checked ? CheckedChar : UnCheckedChar, SelectedCheckFormat);
		}else{
			b = new Buffer(3, 3);
			
			b.SetChar(1, 0, FrameChars[4], FrameFormat);
			b.SetChar(1, 2, FrameChars[5], FrameFormat);
			
			b.SetChar(0, 1, FrameChars[6], FrameFormat);
			b.SetChar((int) 2, 1, FrameChars[7], FrameFormat);
			
			b.SetChar(0, 0, FrameChars[0], FrameFormat);
			b.SetChar((int) 2, 0, FrameChars[1], FrameFormat);
			b.SetChar(0, 2, FrameChars[2], FrameFormat);
			b.SetChar((int) 2, 2, FrameChars[3], FrameFormat);
			
			b.SetChar(1, 1, Checked ? CheckedChar : UnCheckedChar, CheckFormat);
		}
		return b;
	}
}

/// <summary>
/// Radio button: two options, either one or the other
/// </summary>
public class TuiFramedRadio : TuiSelectable{
	/// <summary>
	/// If the right option is checked(true) or the left one(false)
	/// </summary>
	public bool RightOptionChecked {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
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
	/// Not selected check charachter format
	/// </summary>
	public CharFormat? CheckFormat {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Selected check charachter format
	/// </summary>
	public CharFormat? SelectedCheckFormat {get;
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
	/// Check carachter when the checkbox is unchecked
	/// </summary>
	public char UnCheckedChar {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Check carachter when the checkbox is checked
	/// </summary>
	public char CheckedChar {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Left option
	/// </summary>
	public string LeftOption {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Right option
	/// </summary>
	public string RightOption {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Initializes a new radio button
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="cf">Not selected check format</param>
	/// <param name="scf">Selected check format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedRadio(string chars, char u, char c, string lo, string ro, Placement p, int x, int y,
							CharFormat? ff, CharFormat? sff, CharFormat? cf, CharFormat? scf, CharFormat? tf, CharFormat? stf, CharFormat? pf)
							: base(p, x, y){
		if(chars == null || chars.Length != 8){
			chars = "┌┐└┘──││";
		}
		FrameFormat = ff;
		SelectedFrameFormat = sff;
		CheckFormat = cf;
		SelectedCheckFormat = scf;
		TextFormat = tf;
		SelectedTextFormat = stf;
		SelectorFormat = pf;
		
		UnCheckedChar = u;
		CheckedChar = c;
		
		LeftOption = lo;
		RightOption = ro;
		
		FrameChars = chars.ToCharArray();
		
		SubKeyEvent(ConsoleKey.Enter, (s, ck) => {RightOptionChecked = !RightOptionChecked;});
	}
	
	/// <summary>
	/// Initializes a new radio button with the same colors when selected and when not
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Frame format</param>
	/// <param name="cf">Check format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedRadio(string chars, char u, char c, string lo, string ro, Placement p, int x, int y, CharFormat? ff = null, CharFormat? cf = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(chars, u, c, lo, ro, p, x, y, ff, ff, cf, cf, tf, tf, pf){}
	
	/// <summary>
	/// Initializes a new radio button with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Not selected frame format</param>
	/// <param name="sff">Selected frame format</param>
	/// <param name="cf">Not selected check format</param>
	/// <param name="scf">Selected check format</param>
	/// <param name="tf">Not selected text format</param>
	/// <param name="stf">Selected text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedRadio(char u, char c, string lo, string ro, Placement p, int x, int y, CharFormat? ff, CharFormat? sff, CharFormat? cf, CharFormat? scf, CharFormat? tf, CharFormat? stf, CharFormat? pf)
							: this(null, u, c, lo, ro, p, x, y, ff, sff, cf, scf, tf, stf, pf){}
	
	/// <summary>
	/// Initializes a new radio button with default frame chars ('┌┐└┘──││') and the same colors for selected and not selected
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Frame format</param>
	/// <param name="cf">Check format</param>
	/// <param name="tf">Text format</param>
	/// <param name="pf">Selector format</param>
	public TuiFramedRadio(char u, char c, string lo, string ro, Placement p, int x, int y, CharFormat? ff = null, CharFormat? cf = null, CharFormat? tf = null, CharFormat? pf = null)
							: this(u, c, lo, ro, p, x, y, ff, ff, cf, cf, tf, tf, pf){}
	
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(9 + LeftOption.Length + RightOption.Length, 3);
			b.SetChar(0, 1, '>', SelectorFormat);
			b.SetChar((int) b.Xsize - 1, 1, '<', SelectorFormat);
			
			int i;
			for(i = 1; i < LeftOption.Length + 1; i++){
				b.SetChar(i, 1, LeftOption[i - 1], SelectedTextFormat);
			}
			
			b.SetChar(i, 1, FrameChars[6], SelectedFrameFormat); //Left edge
			b.SetChar(i, 0, FrameChars[0], SelectedFrameFormat);
			b.SetChar(i, 2, FrameChars[2], SelectedFrameFormat);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], SelectedFrameFormat); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], SelectedFrameFormat);
			
			b.SetChar(i, 1, RightOptionChecked ? UnCheckedChar : CheckedChar, SelectedCheckFormat);
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], SelectedFrameFormat); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], SelectedFrameFormat);
			b.SetChar(i, 2, FrameChars[3], SelectedFrameFormat);
			
			i++;
			i++;
			
			for(int j = 0; j < RightOption.Length; j++, i++){
				b.SetChar(i, 1, RightOption[j], SelectedTextFormat);
			}
			
			b.SetChar(i, 1, FrameChars[6], SelectedFrameFormat); //Left edge
			b.SetChar(i, 0, FrameChars[0], SelectedFrameFormat);
			b.SetChar(i, 2, FrameChars[2], SelectedFrameFormat);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], SelectedFrameFormat); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], SelectedFrameFormat);
			
			b.SetChar(i, 1, RightOptionChecked ? CheckedChar : UnCheckedChar, SelectedCheckFormat);
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], SelectedFrameFormat); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], SelectedFrameFormat);
			b.SetChar(i, 2, FrameChars[3], SelectedFrameFormat);
		}else{
			b = new Buffer(7 + LeftOption.Length + RightOption.Length, 3);
			
			int i;
			for(i = 0; i < LeftOption.Length; i++){
				b.SetChar(i, 1, LeftOption[i], TextFormat);
			}
			
			b.SetChar(i, 1, FrameChars[6], FrameFormat); //Left edge
			b.SetChar(i, 0, FrameChars[0], FrameFormat);
			b.SetChar(i, 2, FrameChars[2], FrameFormat);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], FrameFormat); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], FrameFormat);
			
			b.SetChar(i, 1, RightOptionChecked ? UnCheckedChar : CheckedChar, CheckFormat);
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], FrameFormat); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], FrameFormat);
			b.SetChar(i, 2, FrameChars[3], FrameFormat);
			
			i++;
			i++;
			
			for(int j = 0; j < RightOption.Length; j++, i++){
				b.SetChar(i, 1, RightOption[j], TextFormat);
			}
			
			b.SetChar(i, 1, FrameChars[6], FrameFormat); //Left edge
			b.SetChar(i, 0, FrameChars[0], FrameFormat);
			b.SetChar(i, 2, FrameChars[2], FrameFormat);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], FrameFormat); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], FrameFormat);
			
			b.SetChar(i, 1, RightOptionChecked ? CheckedChar : UnCheckedChar, CheckFormat);
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], FrameFormat); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], FrameFormat);
			b.SetChar(i, 2, FrameChars[3], FrameFormat);
		}
		return b;
	}
}