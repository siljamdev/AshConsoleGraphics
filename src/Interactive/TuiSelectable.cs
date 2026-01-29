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
	/// Global left selector char for all included Selectables
	/// </summary>
	public static char? LeftSelector = '>';
	
	/// <summary>
	/// Global right selector char for all included Selectables
	/// </summary>
	public static char? RightSelector = '<';
	
	/// <summary>
	/// If it is selected or not
	/// </summary>
	public bool Selected {get;
	internal set{
		field = value;
		needToGenBuffer = true;
		OnSelection?.Invoke(this, EventArgs.Empty);
	}}
	
	public event EventHandler OnSelection;
	
	/// <summary>
	/// Keybinds and their actions
	/// </summary>
	public Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiSelectable, ConsoleKeyInfo>> KeyFunctions {get; private set;}
	
	/// <summary>
	/// Initializes a new selectable element
	/// </summary>
	/// <param name="p">The relative placement method</param>
	/// <param name="x">The relative x offset</param>
	/// <param name="y">The relative x offset</param>
	protected TuiSelectable(Placement p, int x, int y) : base(p, x, y){
		KeyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiSelectable, ConsoleKeyInfo>>();
	}
	
	/// <summary>
	/// Subscribes a new key event
	/// </summary>
	/// <param name="k">The key pressed</param>
	/// <param name="m">The key modifiers (cntrl, shift)</param>
	/// <param name="keyFunction">The action to call</param>
	public TuiSelectable SubKeyEvent(ConsoleKey k, ConsoleModifiers m, Action<TuiSelectable, ConsoleKeyInfo> keyFunction){
		KeyFunctions[(k, m)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Subscribes a new key event with no modifiers
	/// </summary>
	/// <param name="k">The key pressed</param>
	/// <param name="keyFunction">The action to call</param>
	public TuiSelectable SubKeyEvent(ConsoleKey k, Action<TuiSelectable, ConsoleKeyInfo> keyFunction){
		KeyFunctions[(k, ConsoleModifiers.None)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Deletes all key events
	/// </summary>
	public TuiSelectable DeleteAllKeyEvents(){
		this.KeyFunctions.Clear();
		return this;
	}
	
	/// <summary>
	/// Handles a key press
	/// </summary>
	public virtual bool HandleKey(ConsoleKeyInfo keyInfo){
		if(KeyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
			KeyFunctions[(keyInfo.Key, keyInfo.Modifiers)].Invoke(this, keyInfo);
			return true;
		}
		return false;
	}
}