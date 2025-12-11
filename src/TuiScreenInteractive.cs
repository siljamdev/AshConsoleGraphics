using System;
using AshLib;
using AshLib.Formatting;

namespace AshConsoleGraphics.Interactive;

//Manager elements

/// <summary>
/// Interactive screen with selectables and key actions
/// </summary>
public class TuiScreenInteractive : TuiScreen{
	
	/// <summary>
	/// 2d matrix of the elements, ordered and positioned in the way it is wanted
	/// </summary>
	public TuiSelectable[,] SelectionMatrix{get; set{
		if(value == null){
			return;
		}
		if(field != null){
			SetSelected(false);
		}
		field = value;
		SetSelected(true);
	}}
	
	/// <summary>
	/// X index of the selected element in the matrix
	/// </summary>
	public virtual uint MatrixPointerX {get; set{
		if(value < SelectionMatrix.GetLength(1) && MatrixPointerY < SelectionMatrix.GetLength(0) && SelectionMatrix[MatrixPointerY, value] != null){
			SetSelected(false);
			field = value;
			SetSelected(true);
		}
	}}
	
	/// <summary>
	/// Y index of the selected element in the matrix
	/// </summary>
	public virtual uint MatrixPointerY {get; set{
		if(value < SelectionMatrix.GetLength(0) && MatrixPointerX < SelectionMatrix.GetLength(1) && SelectionMatrix[value, MatrixPointerX] != null){
			SetSelected(false);
			field = value;
			SetSelected(true);
		}
	}}
	
	/// <summary>
	/// Currently selected element
	/// </summary>
	public TuiSelectable Selected{get{
		if(MatrixPointerY >= SelectionMatrix.GetLength(0) || MatrixPointerX >= SelectionMatrix.GetLength(1)){
			return null;
		}
		return SelectionMatrix[MatrixPointerY, MatrixPointerX];
	}}
	
	/// <summary>
	/// If the screen is playing or not
	/// </summary>
	public bool Playing {get; private set;}
	
	/// <summary>
	/// The key actions
	/// </summary>
	public Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiScreenInteractive, ConsoleKeyInfo>> KeyFunctions {get; private set;}
	
	/// <summary>
	/// If the screen will wait for a key press before updating
	/// </summary>
	public bool WaitForKey = false;
	
	/// <summary>
	/// Event called at the end of each play cycle
	/// </summary>
	public event EventHandler OnFinishPlayCycle;
	
	/// <summary>
	/// Initializes a new interactive screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="sm">Selectable matrix</param>
	/// <param name="startX">Start X index in the selectable matrix</param>
	/// <param name="startY">Start Y index in the selectable matrix</param>
	/// <param name="f">The default format</param>
	/// <param name="e">Additional elements</param>
	public TuiScreenInteractive(int xs, int ys, TuiSelectable[,] sm, uint startX, uint startY, Placement p, int x, int y, CharFormat? f, params TuiElement[] e)
								: base(xs, ys, p, x, y, f, (e != null ? (sm != null ? e.Concat(sm.Cast<TuiSelectable>().ToArray()).ToArray() : e) : (sm != null ? sm.Cast<TuiSelectable>().ToArray() : e))){
		if(sm == null){
			SelectionMatrix = new TuiSelectable[0,0];
		}else{
			SelectionMatrix = sm;
		}
		MatrixPointerX = (uint) Math.Max(startX, 0);
		MatrixPointerY = (uint) Math.Max(startY, 0);
		SetSelected(true);
		
		KeyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiScreenInteractive, ConsoleKeyInfo>>();
		
		SetDefaultKeys();
	}
	
	/// <summary>
	/// Initializes a new interactive screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="sm">Selectable matrix</param>
	/// <param name="startX">Start X index in the selectable matrix</param>
	/// <param name="startY">Start Y index in the selectable matrix</param>
	/// <param name="f">The default format</param>
	/// <param name="e">Additional elements</param>
	public TuiScreenInteractive(int xs, int ys, TuiSelectable[,] sm, uint startX, uint startY, CharFormat? f, params TuiElement[] e)
								: base(xs, ys, f, (e != null ? (sm != null ? e.Concat(sm.Cast<TuiSelectable>().ToArray()).ToArray() : e) : (sm != null ? sm.Cast<TuiSelectable>().ToArray() : e))){
		if(sm == null){
			SelectionMatrix = new TuiSelectable[0,0];
		}else{
			SelectionMatrix = sm;
		}
		MatrixPointerX = (uint) Math.Max(startX, 0);
		MatrixPointerY = (uint) Math.Max(startY, 0);
		SetSelected(true);
		
		KeyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiScreenInteractive, ConsoleKeyInfo>>();
		
		SetDefaultKeys();
	}
	
	internal void SetSelected(bool b){
		if(Selected != null){
			Selected.Selected = b;
		}
	}
	
	/// <summary>
	/// Adds a new key event and returns itself
	/// </summary>
	public TuiScreenInteractive SubKeyEvent(ConsoleKey k, ConsoleModifiers m, Action<TuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		KeyFunctions[(k, m)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Adds a new key event with no modifiers and returns itself
	/// </summary>
	public TuiScreenInteractive SubKeyEvent(ConsoleKey k, Action<TuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		KeyFunctions[(k, ConsoleModifiers.None)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Clears all existent key events. Useful because there are some default ones. Returns itself
	/// </summary>
	public TuiScreenInteractive DeleteAllKeyEvents(){
		this.KeyFunctions.Clear();
		return this;
	}
	
	void SetDefaultKeys(){
		SubKeyEvent(ConsoleKey.UpArrow, MoveUp);
		SubKeyEvent(ConsoleKey.DownArrow, MoveDown);
		SubKeyEvent(ConsoleKey.LeftArrow, MoveLeft);
		SubKeyEvent(ConsoleKey.RightArrow, MoveRight);
		SubKeyEvent(ConsoleKey.Escape, StopPlaying);
	}
	
	/// <summary>
	/// Plays the interactive screen. Handles all key presses
	/// </summary>
	public void Play(){
		if(Playing){
			return;
		}
		Playing = true;
		while(Playing){
			Print();
			
			try{ //Sometimes in non interactive terminals Console.KeyAvailable gets error
				if(!WaitForKey && !Console.KeyAvailable){
					CallFinishCycleEvent();
					continue;
				}
			}catch(Exception e){}
			
			ConsoleKeyInfo keyInfo;
			try{
				keyInfo = Console.ReadKey(true);
			}catch(Exception e){
				char c = (char) Console.Read();
				keyInfo = new ConsoleKeyInfo(c, (ConsoleKey)Enum.Parse(typeof(ConsoleKey), c.ToString(), true), false, false, false);
			}
			
			HandleKey(keyInfo);
			
			CallFinishCycleEvent();
		}
	}
	
	/// <summary>
	/// Handle a key press
	/// </summary>
	public bool HandleKey(ConsoleKeyInfo keyInfo){
		if(Selected != null && Selected.HandleKey(keyInfo)){
			return true;
		}
		
		if(KeyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
			Action<TuiScreenInteractive, ConsoleKeyInfo> keyFunction = KeyFunctions[(keyInfo.Key, keyInfo.Modifiers)];
			keyFunction.Invoke(this, keyInfo);
			return true;
		}
		return false;
	}
	
	internal void CallFinishCycleEvent(){
		OnFinishPlayCycle?.Invoke(this, EventArgs.Empty);
	}
	
	/// <summary>
	/// Stop playing
	/// </summary>
	public void Stop(){
		Playing = false;
	}
	
	/// <summary>
	/// Stop a screen playing
	/// </summary>
	public static void StopPlaying(TuiScreenInteractive s, ConsoleKeyInfo ck){
		s.Stop();
	}
	
	/// <summary>
	/// Move the selected pointer right (x positive)
	/// </summary>
	public static void MoveRight(TuiScreenInteractive s, ConsoleKeyInfo ck){
		s.MatrixPointerX++;
	}
	
	/// <summary>
	/// Move the selected pointer left (x negative)
	/// </summary>
	public static void MoveLeft(TuiScreenInteractive s, ConsoleKeyInfo ck){
		s.MatrixPointerX--;
	}
	
	/// <summary>
	/// Move the selected pointer down (y positive)
	/// </summary>
	public static void MoveDown(TuiScreenInteractive s, ConsoleKeyInfo ck){
		s.MatrixPointerY++;
	}
	
	/// <summary>
	/// Move the selected pointer up (y negative)
	/// </summary>
	public static void MoveUp(TuiScreenInteractive s, ConsoleKeyInfo ck){
		s.MatrixPointerY--;
	}
}