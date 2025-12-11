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
			field = value.Substring(0, Length);
		}else{
			field = value;
			needToGenBuffer = true;
		}
	}}
	
	/// <summary>
	/// The maximum length you can write
	/// </summary>
	public int Length {get;
	set{
		if(value < 0){
			return;
		}
		if(Text != null && Text.Length > value){
			Text = Text.Substring(0, value);
		}
		field = value;
		needToGenBuffer = true;
		OnLengthChange?.Invoke(this, EventArgs.Empty);
	}}
	
	/// <summary>
	/// Determines if a char can be written, and returns the string to add to text if possible, null otherwise
	/// </summary>
	public Func<char, string?> CanWriteChar {get; set;}
	
	/// <summary>
	/// Will be called when length changes
	/// </summary>
	public event EventHandler OnLengthChange;
	
	/// <summary>
	/// Base constructor
	/// </summary>
	/// <param name="t">Initial text</param>
	/// <param name="l">length</param>
	protected TuiWritable(string t, int l, Placement p, int x, int y) : base(p, x, y){
		Length = l;
		Text = t;
		
		//Base implementation
		CanWriteChar = c => {
			if(c == '\n'){
				return null;
			}
			if(Text.Length + 1 > Length){
				return null;
			}
			return c.ToString();
		};
	}
	
	public override sealed bool HandleKey(ConsoleKeyInfo keyInfo){
		char c = keyInfo.KeyChar;
		bool b = false;
		if(char.IsControl(c)){
			if(keyInfo.Key == ConsoleKey.Backspace){
				b = DelChar();
			}else if(keyInfo.Key == ConsoleKey.Enter){
				string? r = CanWriteChar?.Invoke('\n');
				if(r != null){
					Text += r;
					b = true;
				}
			}
		}else{
			string? r = CanWriteChar?.Invoke(c);
			if(r != null){
				Text += r;
				b = true;
			}
		}
		
		if(!b){
			if(KeyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
				KeyFunctions[(keyInfo.Key, keyInfo.Modifiers)].Invoke(this, keyInfo);
				return true;
			}
			return false;
		}else{
			return true;
		}
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