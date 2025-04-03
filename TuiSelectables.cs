using System;
using AshLib;

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
	public void SubKeyEvent(ConsoleKey k, ConsoleModifiers m, Action<TuiSelectable, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, m)] = keyFunction;
	}
	
	/// <summary>
	/// Subscribes a new key event with no modifiers
	/// </summary>
	/// <param name="k">The key pressed</param>
	/// <param name="keyFunction">The action to call</param>
	public void SubKeyEvent(ConsoleKey k, Action<TuiSelectable, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, ConsoleModifiers.None)] = keyFunction;
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
	/// Foreground text color
	/// </summary>
	public Color3? FgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color
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
	/// Initializes a new button with all colors
	/// </summary>
	/// <param name="t">Text to display</param>
	/// <param name="f">Not selected text foreground color</param>
	/// <param name="b">Not selected text background color</param>
	/// <param name="sf">Selected text foreground color</param>
	/// <param name="sb">Selected text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	public TuiButton(string t, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb, Color3? pf, Color3? pb) : base(p, x, y){
		FgTextColor = f;
		BgTextColor = b;
		FgTextSelectedColor = sf;
		BgTextSelectedColor = sb;
		FgSelectorColor = pf;
		BgSelectorColor = pb;
		Text = t;
	}
	
	/// <summary>
	/// Initializes a new button with the same text color when selected and when not
	/// </summary>
	/// <param name="t">Text to display</param>
	/// <param name="f">Text foreground color</param>
	/// <param name="b">Text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	public TuiButton(string t, Placement p, int x, int y, Color3? f, Color3? b, Color3? pf, Color3? pb) : this(t, p, x, y, f, b, f, b, pf, pb){}
	
	/// <summary>
	/// Initializes a new button with general colors
	/// </summary>
	/// <param name="t">Text to display</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiButton(string t, Placement p, int x, int y, Color3? f, Color3? b) : this(t, p, x, y, f, b, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new button with null colors
	/// </summary>
	/// <param name="t">Text to display</param>
	public TuiButton(string t, Placement p, int x, int y) : this(t, p, x, y, null, null, null, null, null, null){}
	
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
			b.SetChar(0, 0, '>', FgSelectorColor, BgSelectorColor);
			b.SetChar(Text.Length + 1, 0, '<', FgSelectorColor, BgSelectorColor);
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(1 + i, 0, Text[i], FgTextSelectedColor, BgTextSelectedColor);
			}
		}else{
			b = new Buffer(Text.Length, 1);
			for(int i = 0; i < Text.Length; i++){
				b.SetChar(i, 0, Text[i], FgTextColor, BgTextColor);
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
	/// Foreground text color
	/// </summary>
	public Color3? FgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color
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
	/// Initializes a new option picker with all colors
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="so">Index of the selected option</param>
	/// <param name="f">Not selected text foreground color</param>
	/// <param name="b">Not selected text background color</param>
	/// <param name="sf">Selected text foreground color</param>
	/// <param name="sb">Selected text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	/// <exception cref="System.ArgumentException">Thrown when t is null or no option was provided (length 0)</exception>
	public TuiOptionPicker(string[] t, uint so, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb, Color3? pf, Color3? pb) : base(p, x, y){
		if(t == null || t.Length == 0){
			throw new ArgumentException("At least one option needs to be provided");
		}
		
		FgTextColor = f;
		BgTextColor = b;
		FgTextSelectedColor = sf;
		BgTextSelectedColor = sb;
		FgSelectorColor = pf;
		BgSelectorColor = pb;
		
		Options = t;
		SelectedOptionIndex = so;
		
		SubKeyEvent(ConsoleKey.LeftArrow, (s, ck) => {SelectedOptionIndex--;});
		SubKeyEvent(ConsoleKey.RightArrow, (s, ck) => {SelectedOptionIndex++;});
	}
	
	/// <summary>
	/// Initializes a new option picker with selected option 0 with all colors
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="f">Not selected text foreground color</param>
	/// <param name="b">Not selected text background color</param>
	/// <param name="sf">Selected text foreground color</param>
	/// <param name="sb">Selected text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	public TuiOptionPicker(string[] t, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb, Color3? pf, Color3? pb)
						: this(t, 0, p, x, y, f, b, sf, sb, pf, pb){}
	
	/// <summary>
	/// Initializes a new option picker with the same text color when selected and when not
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="so">Index of the selected option</param>
	/// <param name="f">Text foreground color</param>
	/// <param name="b">Text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	public TuiOptionPicker(string[] t, uint so, Placement p, int x, int y, Color3? f, Color3? b, Color3? pf, Color3? pb)
						: this(t, so, p, x, y, f, b, f, b, pf, pb){}
	
	/// <summary>
	/// Initializes a new option picker with selected option 0 with the same text color when selected and when not
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="f">Text foreground color</param>
	/// <param name="b">Text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	public TuiOptionPicker(string[] t, Placement p, int x, int y, Color3? f, Color3? b, Color3? pf, Color3? pb)
						: this(t, 0, p, x, y, f, b, f, b, pf, pb){}
	
	/// <summary>
	/// Initializes a new option picker with general colors
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="so">Index of the selected option</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiOptionPicker(string[] t, uint so, Placement p, int x, int y, Color3? f, Color3? b)
						: this(t, so, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new option picker with selected option 0 with general colors
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiOptionPicker(string[] t, Placement p, int x, int y, Color3? f, Color3? b)
						: this(t, 0, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new option picker with null colors
	/// </summary>
	/// <param name="t">All options</param>
	/// <param name="so">Index of the selected option</param>
	public TuiOptionPicker(string[] t, uint so, Placement p, int x, int y)
						: this(t, so, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new option picker with selected option 0 with all colors
	/// </summary>
	/// <param name="t">All options</param>
	public TuiOptionPicker(string[] t, Placement p, int x, int y)
						: this(t, 0, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(SelectedOption.Length + 2, 1);
			b.SetChar(0, 0, '<', FgSelectorColor, BgSelectorColor);
			b.SetChar(SelectedOption.Length + 1, 0, '>', FgSelectorColor, BgSelectorColor);
			for(int i = 0; i < SelectedOption.Length; i++){
				b.SetChar(1 + i, 0, SelectedOption[i], FgTextSelectedColor, BgTextSelectedColor);
			}
		}else{
			b = new Buffer(SelectedOption.Length, 1);
			for(int i = 0; i < SelectedOption.Length; i++){
				b.SetChar(i, 0, SelectedOption[i], FgTextColor, BgTextColor);
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
	/// Foreground text color
	/// </summary>
	public Color3? FgTextColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background text color
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
	/// Initializes a new number picker with all colors
	/// </summary>
	/// <param name="lower">Lower bound</param>
	/// <param name="upper">Upper bound</param>
	/// <param name="interval">Increment interval</param>
	/// <param name="num">Initial selected number</param>
	/// <param name="f">Not selected text foreground color</param>
	/// <param name="b">Not selected text background color</param>
	/// <param name="sf">Selected text foreground color</param>
	/// <param name="sb">Selected text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	/// <exception cref="System.ArgumentException">Thrown when lower isgreater than upper</exception>
	public TuiNumberPicker(int lower, int upper, int interval, int num, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb, Color3? pf, Color3? pb) : base(p, x, y){
		if(lower > upper){
			throw new ArgumentException("Lower limit cant be greater than upper");
		}
		
		FgTextColor = f;
		BgTextColor = b;
		FgTextSelectedColor = sf;
		BgTextSelectedColor = sb;
		FgSelectorColor = pf;
		BgSelectorColor = pb;
		
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
	/// <param name="f">Text foreground color</param>
	/// <param name="b">Text background color</param>
	/// <param name="pf">Selector foreground color</param>
	/// <param name="pb">Selector background color</param>
	public TuiNumberPicker(int lower, int upper, int interval, int num, Placement p, int x, int y, Color3? f, Color3? b, Color3? pf, Color3? pb)
						: this(lower, upper, interval, num, p, x, y, f, b, f, b, pf, pb){}
	
	/// <summary>
	/// Initializes a new number picker with general colors
	/// </summary>
	/// <param name="lower">Lower bound</param>
	/// <param name="upper">Upper bound</param>
	/// <param name="interval">Increment interval</param>
	/// <param name="num">Initial selected number</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiNumberPicker(int lower, int upper, int interval, int num, Placement p, int x, int y, Color3? f, Color3? b)
						: this(lower, upper, interval, num, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new number picker with null colors
	/// </summary>
	/// <param name="lower">Lower bound</param>
	/// <param name="upper">Upper bound</param>
	/// <param name="interval">Increment interval</param>
	/// <param name="num">Initial selected number</param>
	public TuiNumberPicker(int lower, int upper, int interval, int num, Placement p, int x, int y)
						: this(lower, upper, interval, num, p, x, y, null, null){}
	
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
			b.SetChar(0, 0, '<', FgSelectorColor, BgSelectorColor);
			b.SetChar(s.Length + 1, 0, '>', FgSelectorColor, BgSelectorColor);
			for(int i = 0; i < s.Length; i++){
				b.SetChar(1 + i, 0, s[i], FgTextSelectedColor, BgTextSelectedColor);
			}
		}else{
			b = new Buffer(s.Length, 1);
			for(int i = 0; i < s.Length; i++){
				b.SetChar(i, 0, s[i], FgTextColor, BgTextColor);
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
	/// Foreground color of the check when not selected
	/// </summary>
	public Color3? FgCheckColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the check when not selected
	/// </summary>
	public Color3? BgCheckColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground color of the check when selected
	/// </summary>
	public Color3? FgCheckSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background color of the check when selected
	/// </summary>
	public Color3? BgCheckSelectedColor {get;
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
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected check color</param>
	/// <param name="tb">Background not selected check color</param>
	/// <param name="tsf">Foreground selected check color</param>
	/// <param name="tsb">Background selected check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 8 chars long</exception>
	public TuiFramedCheckBox(string chars, char u, char c, bool b, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(p, x, y){
		if(chars == null || chars.Length != 8){
			throw new ArgumentException("String must be 8 chars long");
		}
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgCheckColor = tf;
		BgCheckColor = tb;
		FgCheckSelectedColor = tsf;
		BgCheckSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
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
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground check color</param>
	/// <param name="tb">Background check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(string chars, char u, char c, bool b, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(chars, u, c, b, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new framed checkbox with the same colors for frame and check char
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="bo">If the checkbox is initially checked or not</param>
	/// <param name="f">Foreground frame and check color</param>
	/// <param name="b">Background frame and check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(string chars, char u, char c, bool bo, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(chars, u, c, bo, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new framed checkbox with general colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="bo">If the checkbox is initially checked or not</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiFramedCheckBox(string chars, char u, char c, bool bo, Placement p, int x, int y, Color3? f, Color3? b)
							: this(chars, u, c, bo, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new framed checkbox with null colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	public TuiFramedCheckBox(string chars, char u, char c, bool b, Placement p, int x, int y)
							: this(chars, u, c, b, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new framed checkbox with each specific frame char
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected check color</param>
	/// <param name="tb">Background not selected check color</param>
	/// <param name="tsf">Foreground selected check color</param>
	/// <param name="tsb">Background selected check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, bool b, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb) : base(p, x, y){
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgCheckColor = tf;
		BgCheckColor = tb;
		FgCheckSelectedColor = tsf;
		BgCheckSelectedColor = tsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		UnCheckedChar = u;
		CheckedChar = c;
		Checked = b;
		
		FrameChars = new char[]{topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right};
		
		SubKeyEvent(ConsoleKey.Enter, (s, ck) => {Checked = !Checked;});
	}
	
	/// <summary>
	/// Initializes a new framed checkbox with each specific frame char and the same colors when selected and not selected
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground check color</param>
	/// <param name="tb">Background check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, bool b, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, b, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new framed checkbox with each specific frame char and the same colors for frame and check char
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="bo">If the checkbox is initially checked or not</param>
	/// <param name="f">Foreground frame and check color</param>
	/// <param name="b">Background frame and check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, bool bo, Placement p, int x, int y,
							Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, bo, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new framed checkbox with each specific frame char and general colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="bo">If the checkbox is initially checked or not</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiFramedCheckBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, bool bo, Placement p, int x, int y,
							Color3? f, Color3? b)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, bo, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new framed checkbox with each specific frame char and null colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	public TuiFramedCheckBox(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, bool b, Placement p, int x, int y)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, b, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new framed checkbox with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected check color</param>
	/// <param name="tb">Background not selected check color</param>
	/// <param name="tsf">Foreground selected check color</param>
	/// <param name="tsb">Background selected check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(char u, char c, bool b, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? sf, Color3? sb)
							: this('┌', '┐', '└', '┘', '─', '─', '│', '│', u, c, b, p, x, y, ff, fb, fsf, fsb, tf, tb, tsf, tsb, sf, sb){}
	
	/// <summary>
	/// Initializes a new framed checkbox with defaut frame chars and the same colors when selected and not selected
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground check color</param>
	/// <param name="tb">Background check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(char u, char c, bool b, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? sf, Color3? sb)
							: this(u, c, b, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, sf, sb){}
	
	/// <summary>
	/// Initializes a new framed checkbox with default frame chars the same colors for frame and check char
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="bo">If the checkbox is initially checked or not</param>
	/// <param name="f">Foreground frame and check color</param>
	/// <param name="b">Background frame and check color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedCheckBox(char u, char c, bool bo, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(u, c, bo, p, x, y, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new framed checkbox with default frame chars and general colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="bo">If the checkbox is initially checked or not</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiFramedCheckBox(char u, char c, bool bo, Placement p, int x, int y, Color3? f, Color3? b)
							: this(u, c, bo, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new framed checkbox with default frame chars and null colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="b">If the checkbox is initially checked or not</param>
	public TuiFramedCheckBox(char u, char c, bool b, Placement p, int x, int y) : this(u, c, b, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(5, 3);
			b.SetChar(0, 1, '>', FgSelectorColor, BgSelectorColor);
			b.SetChar((int) 4, 1, '<', FgSelectorColor, BgSelectorColor);
			
			b.SetChar(2, 0, FrameChars[4], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(2, 2, FrameChars[5], FgFrameSelectedColor, BgFrameSelectedColor);
			
			b.SetChar(1, 1, FrameChars[6], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) 3, 1, FrameChars[7], FgFrameSelectedColor, BgFrameSelectedColor);
			
			b.SetChar(1, 0, FrameChars[0], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) 3, 0, FrameChars[1], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(1, 2, FrameChars[2], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar((int) 3, 2, FrameChars[3], FgFrameSelectedColor, BgFrameSelectedColor);
			
			if(Checked){
				b.SetChar(2, 1, CheckedChar, FgCheckSelectedColor, BgCheckSelectedColor);
			}else{
				b.SetChar(2, 1, UnCheckedChar, FgCheckSelectedColor, BgCheckSelectedColor);
			}
		}else{
			b = new Buffer(3, 3);
			
			b.SetChar(1, 0, FrameChars[4], FgFrameColor, BgFrameColor);
			b.SetChar(1, 2, FrameChars[5], FgFrameColor, BgFrameColor);
			
			b.SetChar(0, 1, FrameChars[6], FgFrameColor, BgFrameColor);
			b.SetChar((int) 2, 1, FrameChars[7], FgFrameColor, BgFrameColor);
			
			b.SetChar(0, 0, FrameChars[0], FgFrameColor, BgFrameColor);
			b.SetChar((int) 2, 0, FrameChars[1], FgFrameColor, BgFrameColor);
			b.SetChar(0, 2, FrameChars[2], FgFrameColor, BgFrameColor);
			b.SetChar((int) 2, 2, FrameChars[3], FgFrameColor, BgFrameColor);
			
			if(Checked){
				b.SetChar(1, 1, CheckedChar, FgCheckColor, BgCheckColor);
			}else{
				b.SetChar(1, 1, UnCheckedChar, FgCheckColor, BgCheckColor);
			}
		}
		return b;
	}
}

/// <summary>
/// Radio button: two options, either one of the other
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
	/// Foreground check color when not selected
	/// </summary>
	public Color3? FgCheckColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background check color when not selected
	/// </summary>
	public Color3? BgCheckColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Foreground check color when selected
	/// </summary>
	public Color3? FgCheckSelectedColor {get;
	set{
		field = value;
		needToGenBuffer = true;
	}}
	
	/// <summary>
	/// Background check color when not selected
	/// </summary>
	public Color3? BgCheckSelectedColor {get;
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
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected check color</param>
	/// <param name="tb">Background not selected check color</param>
	/// <param name="tsf">Foreground selected check color</param>
	/// <param name="tsb">Background selected check color</param>
	/// <param name="otf">Foreground not selected text color</param>
	/// <param name="otb">Background not selected text color</param>
	/// <param name="otsf">Foreground selected text color</param>
	/// <param name="otsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	/// <exception cref="System.ArgumentException">Thrown when chars is null or it is not 8 chars long</exception>
	public TuiFramedRadio(string chars, char u, char c, string lo, string ro, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? otf, Color3? otb, Color3? otsf, Color3? otsb, Color3? sf, Color3? sb)
							: base(p, x, y){
		if(chars == null || chars.Length != 8){
			throw new ArgumentException("String must be 8 chars long");
		}
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgCheckColor = tf;
		BgCheckColor = tb;
		FgCheckSelectedColor = tsf;
		BgCheckSelectedColor = tsb;
		
		FgTextColor = otf;
		BgTextColor = otb;
		FgTextSelectedColor = otsf;
		BgTextSelectedColor = otsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
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
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground check color</param>
	/// <param name="tb">Background check color</param>
	/// <param name="otf">Foreground text color</param>
	/// <param name="otb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(string chars, char u, char c, string lo, string ro, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? otf, Color3? otb, Color3? sf, Color3? sb)
							: this(chars, u, c, lo, ro, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, otf, otb, otf, otb, sf, sb){}
	
	/// <summary>
	/// Initializes a new radio button with the same colors for frame, check and text
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="f">Foreground frame, check and text color</param>
	/// <param name="b">Background frame, check and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(string chars, char u, char c, string lo, string ro, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(chars, u, c, lo, ro, p, x, y, f, b, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new radio button with general colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiFramedRadio(string chars, char u, char c, string lo, string ro, Placement p, int x, int y, Color3? f, Color3? b)
							: this(chars, u, c, lo, ro, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new radio button with null colors
	/// </summary>
	/// <param name="chars">Frame charchters</param>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	public TuiFramedRadio(string chars, char u, char c, string lo, string ro, Placement p, int x, int y)
							: this(chars, u, c, lo, ro, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new radio button with each specific frame char
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected check color</param>
	/// <param name="tb">Background not selected check color</param>
	/// <param name="tsf">Foreground selected check color</param>
	/// <param name="tsb">Background selected check color</param>
	/// <param name="otf">Foreground not selected text color</param>
	/// <param name="otb">Background not selected text color</param>
	/// <param name="otsf">Foreground selected text color</param>
	/// <param name="otsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, string lo, string ro, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? otf, Color3? otb, Color3? otsf, Color3? otsb, Color3? sf, Color3? sb) : base(p, x, y){
		FgFrameColor = ff;
		BgFrameColor = fb;
		FgFrameSelectedColor = fsf;
		BgFrameSelectedColor = fsb;
		
		FgCheckColor = tf;
		BgCheckColor = tb;
		FgCheckSelectedColor = tsf;
		BgCheckSelectedColor = tsb;
		
		FgTextColor = otf;
		BgTextColor = otb;
		FgTextSelectedColor = otsf;
		BgTextSelectedColor = otsb;
		
		FgSelectorColor = sf;
		BgSelectorColor = sb;
		
		UnCheckedChar = u;
		CheckedChar = c;
		
		LeftOption = lo;
		RightOption = ro;
		
		FrameChars = new char[]{topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right};
		
		SubKeyEvent(ConsoleKey.Enter, (s, ck) => {RightOptionChecked = !RightOptionChecked;});
	}
	
	/// <summary>
	/// Initializes a new radio button with each specific frame char and the same colors for selected and not selected
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground check color</param>
	/// <param name="tb">Background check color</param>
	/// <param name="otf">Foreground text color</param>
	/// <param name="otb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, string lo, string ro, Placement p, int x, int y,
							Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? otf, Color3? otb, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, lo, ro, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, otf, otb, otf, otb, sf, sb){}
	
	/// <summary>
	/// Initializes a new radio button with each specific frame char and same colors for frame, check and text
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="f">Foreground frame, check and text color</param>
	/// <param name="b">Background frame, check and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, string lo, string ro, Placement p, int x, int y,
							Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, lo, ro, p, x, y, f, b, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new radio button with general colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiFramedRadio(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, string lo, string ro, Placement p, int x, int y,
							Color3? f, Color3? b)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, lo, ro, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new radio button with null colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	public TuiFramedRadio(char topLeft, char topRight, char bottomLeft, char bottomRight, char top, char bottom, char left, char right, char u, char c, string lo, string ro, Placement p, int x, int y)
							: this(topLeft, topRight, bottomLeft, bottomRight, top, bottom, left, right, u, c, lo, ro, p, x, y, null, null){}
	
	/// <summary>
	/// Initializes a new radio button with default frame chars ('┌┐└┘──││')
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Foreground not selected frame color</param>
	/// <param name="fb">Background not selected frame color</param>
	/// <param name="fsf">Foreground selected frame color</param>
	/// <param name="fsb">Background selected frame color</param>
	/// <param name="tf">Foreground not selected check color</param>
	/// <param name="tb">Background not selected check color</param>
	/// <param name="tsf">Foreground selected check color</param>
	/// <param name="tsb">Background selected check color</param>
	/// <param name="otf">Foreground not selected text color</param>
	/// <param name="otb">Background not selected text color</param>
	/// <param name="otsf">Foreground selected text color</param>
	/// <param name="otsb">Background selected text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(char u, char c, string lo, string ro, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? fsf, Color3? fsb, Color3? tf, Color3? tb, Color3? tsf, Color3? tsb, Color3? otf, Color3? otb, Color3? otsf, Color3? otsb, Color3? sf, Color3? sb)
							: this('┌', '┐', '└', '┘', '─', '─', '│', '│', u, c, lo, ro, p, x, y, ff, fb, fsf, fsb, tf, tb, tsf, tsb, otf, otb, otsf, otsb, sf, sb){}
	
	/// <summary>
	/// Initializes a new radio button with default frame chars ('┌┐└┘──││') and the same colors for selected and not selected
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="ff">Foreground frame color</param>
	/// <param name="fb">Background frame color</param>
	/// <param name="tf">Foreground check color</param>
	/// <param name="tb">Background check color</param>
	/// <param name="otf">Foreground text color</param>
	/// <param name="otb">Background text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(char u, char c, string lo, string ro, Placement p, int x, int y, Color3? ff, Color3? fb, Color3? tf, Color3? tb, Color3? otf, Color3? otb, Color3? sf, Color3? sb)
							: this(u, c, lo, ro, p, x, y, ff, fb, ff, fb, tf, tb, tf, tb, otf, otb, otf, otb, sf, sb){}
	
	/// <summary>
	/// Initializes a new radio button with default frame chars ('┌┐└┘──││') and the same colors for frame, check and text
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="f">Foreground frame, check and text color</param>
	/// <param name="b">Background frame, check and text color</param>
	/// <param name="sf">Foreground selector color</param>
	/// <param name="sb">Background selector color</param>
	public TuiFramedRadio(char u, char c, string lo, string ro, Placement p, int x, int y, Color3? f, Color3? b, Color3? sf, Color3? sb)
							: this(u, c, lo, ro, p, x, y, f, b, f, b, f, b, sf, sb){}
	
	/// <summary>
	/// Initializes a new radio button with default frame chars ('┌┐└┘──││') and general colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	/// <param name="f">Foreground color</param>
	/// <param name="b">Background color</param>
	public TuiFramedRadio(char u, char c, string lo, string ro, Placement p, int x, int y, Color3? f, Color3? b)
							: this(u, c, lo, ro, p, x, y, f, b, f, b){}
	
	/// <summary>
	/// Initializes a new radio button with default frame chars ('┌┐└┘──││') and null colors
	/// </summary>
	/// <param name="u">Unchecked char</param>
	/// <param name="c">Checked char</param>
	/// <param name="lo">Left option</param>
	/// <param name="ro">Right option</param>
	public TuiFramedRadio(char u, char c, string lo, string ro, Placement p, int x, int y) : this(u, c, lo, ro, p, x, y, null, null){}
	
	override protected Buffer GenerateBuffer(){
		Buffer b;
		if(Selected){
			b = new Buffer(9 + LeftOption.Length + RightOption.Length, 3);
			b.SetChar(0, 1, '>', FgSelectorColor, BgSelectorColor);
			b.SetChar((int) b.Xsize - 1, 1, '<', FgSelectorColor, BgSelectorColor);
			
			int i;
			for(i = 1; i < LeftOption.Length + 1; i++){
				b.SetChar(i, 1, LeftOption[i - 1], FgTextSelectedColor, BgTextSelectedColor);
			}
			
			b.SetChar(i, 1, FrameChars[6], FgFrameSelectedColor, BgFrameSelectedColor); //Left edge
			b.SetChar(i, 0, FrameChars[0], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(i, 2, FrameChars[2], FgFrameSelectedColor, BgFrameSelectedColor);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], FgFrameSelectedColor, BgFrameSelectedColor); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], FgFrameSelectedColor, BgFrameSelectedColor);
			
			if(!RightOptionChecked){
				b.SetChar(i, 1, CheckedChar, FgCheckSelectedColor, BgCheckSelectedColor);
			}else{
				b.SetChar(i, 1, UnCheckedChar, FgCheckSelectedColor, BgCheckSelectedColor);
			}
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], FgFrameSelectedColor, BgFrameSelectedColor); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(i, 2, FrameChars[3], FgFrameSelectedColor, BgFrameSelectedColor);
			
			i++;
			i++;
			
			for(int j = 0; j < RightOption.Length; j++, i++){
				b.SetChar(i, 1, RightOption[j], FgTextSelectedColor, BgTextSelectedColor);
			}
			
			b.SetChar(i, 1, FrameChars[6], FgFrameSelectedColor, BgFrameSelectedColor); //Left edge
			b.SetChar(i, 0, FrameChars[0], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(i, 2, FrameChars[2], FgFrameSelectedColor, BgFrameSelectedColor);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], FgFrameSelectedColor, BgFrameSelectedColor); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], FgFrameSelectedColor, BgFrameSelectedColor);
			
			if(RightOptionChecked){
				b.SetChar(i, 1, CheckedChar, FgCheckSelectedColor, BgCheckSelectedColor);
			}else{
				b.SetChar(i, 1, UnCheckedChar, FgCheckSelectedColor, BgCheckSelectedColor);
			}
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], FgFrameSelectedColor, BgFrameSelectedColor); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], FgFrameSelectedColor, BgFrameSelectedColor);
			b.SetChar(i, 2, FrameChars[3], FgFrameSelectedColor, BgFrameSelectedColor);
		}else{
			b = new Buffer(7 + LeftOption.Length + RightOption.Length, 3);
			
			int i;
			for(i = 0; i < LeftOption.Length; i++){
				b.SetChar(i, 1, LeftOption[i], FgTextColor, BgTextColor);
			}
			
			b.SetChar(i, 1, FrameChars[6], FgFrameColor, BgFrameColor); //Left edge
			b.SetChar(i, 0, FrameChars[0], FgFrameColor, BgFrameColor);
			b.SetChar(i, 2, FrameChars[2], FgFrameColor, BgFrameColor);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], FgFrameColor, BgFrameColor); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], FgFrameColor, BgFrameColor);
			
			if(!RightOptionChecked){
				b.SetChar(i, 1, CheckedChar, FgCheckColor, BgCheckColor);
			}else{
				b.SetChar(i, 1, UnCheckedChar, FgCheckColor, BgCheckColor);
			}
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], FgFrameColor, BgFrameColor); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], FgFrameColor, BgFrameColor);
			b.SetChar(i, 2, FrameChars[3], FgFrameColor, BgFrameColor);
			
			i++;
			i++;
			
			for(int j = 0; j < RightOption.Length; j++, i++){
				b.SetChar(i, 1, RightOption[j], FgTextColor, BgTextColor);
			}
			
			b.SetChar(i, 1, FrameChars[6], FgFrameColor, BgFrameColor); //Left edge
			b.SetChar(i, 0, FrameChars[0], FgFrameColor, BgFrameColor);
			b.SetChar(i, 2, FrameChars[2], FgFrameColor, BgFrameColor);
			
			i++;
			
			b.SetChar(i, 0, FrameChars[4], FgFrameColor, BgFrameColor); //Upper and lower edges
			b.SetChar(i, 2, FrameChars[5], FgFrameColor, BgFrameColor);
			
			if(RightOptionChecked){
				b.SetChar(i, 1, CheckedChar, FgCheckColor, BgCheckColor);
			}else{
				b.SetChar(i, 1, UnCheckedChar, FgCheckColor, BgCheckColor);
			}
			
			i++;
			
			b.SetChar(i, 1, FrameChars[7], FgFrameColor, BgFrameColor); //Right edge
			
			b.SetChar(i, 0, FrameChars[1], FgFrameColor, BgFrameColor);
			b.SetChar(i, 2, FrameChars[3], FgFrameColor, BgFrameColor);
		}
		return b;
	}
}