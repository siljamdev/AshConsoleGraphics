using System;
using AshLib;

namespace AshConsoleGraphics.Interactive;

//Manager elements

/// <summary>
/// Interactive screen with selectables and key actions
/// </summary>
public class TuiScreenInteractive : TuiScreen{
	
	/// <summary>
	/// 2d matrix of the elements, ordered and positioned in the way it is wanted
	/// </summary>
	public TuiSelectable[,] SelectionMatrix {get; private set;}
	
	/// <summary>
	/// X index of the selected element in the matrix
	/// </summary>
	public uint MatrixPointerX {get; set{
		if(value < SelectionMatrix.GetLength(1) && SelectionMatrix[MatrixPointerY, value] != null){
			SetSelected(false);
			field = value;
			SetSelected(true);
		}
	}}
	
	/// <summary>
	/// Y index of the selected element in the matrix
	/// </summary>
	public uint MatrixPointerY {get; set{
		if(value < SelectionMatrix.GetLength(0) && SelectionMatrix[value, MatrixPointerX] != null){
			SetSelected(false);
			field = value;
			SetSelected(true);
		}
	}}
	
	/// <summary>
	/// Currently selected element
	/// </summary>
	public TuiSelectable Selected{get{
		return SelectionMatrix[MatrixPointerY, MatrixPointerX];
	}}
	
	/// <summary>
	/// If the screen is playing or not
	/// </summary>
	public bool Playing {get; private set;}
	
	/// <summary>
	/// The key actions
	/// </summary>
	protected Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiScreenInteractive, ConsoleKeyInfo>> keyFunctions;
	
	/// <summary>
	/// If the screen will wait for a key press before updating
	/// </summary>
	public bool WaitForKey = false;
	
	/// <summary>
	/// Action called at the end of each play cycle
	/// </summary>
	public Action<TuiScreenInteractive>? FinishPlayCycleEvent {internal get; set;}
	
	/// <summary>
	/// Initializes a new interactive screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="sm">Selectable matrix</param>
	/// <param name="startX">Start X index in the selectable matrix</param>
	/// <param name="startY">Start Y index in the selectable matrix</param>
	/// <param name="f">The default foreground color</param>
	/// <param name="b">The default background color</param>
	/// <param name="e">Additional elements</param>
	public TuiScreenInteractive(uint xs, uint ys, TuiSelectable[,] sm, uint startX, uint startY, Placement p, int x, int y, Color3? f, Color3? b, params TuiElement[] e)
								: base(xs, ys, p, x, y, f, b, (e != null ? (sm != null ? e.Concat(sm.Cast<TuiSelectable>().ToArray()).ToArray() : e) : (sm != null ? sm.Cast<TuiSelectable>().ToArray() : e))){
		if(sm == null){
			SelectionMatrix = new TuiSelectable[1,1];
		}else{
			SelectionMatrix = sm;
		}
		MatrixPointerX = startX;
		MatrixPointerY = startY;
		SetSelected(true);
		
		keyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiScreenInteractive, ConsoleKeyInfo>>();
		
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
	/// <param name="f">The default foreground color</param>
	/// <param name="b">The default background color</param>
	/// <param name="e">Additional elements</param>
	public TuiScreenInteractive(uint xs, uint ys, TuiSelectable[,] sm, uint startX, uint startY, Color3? f, Color3? b, params TuiElement[] e)
								: base(xs, ys, f, b, (e != null ? (sm != null ? e.Concat(sm.Cast<TuiSelectable>().ToArray()).ToArray() : e) : (sm != null ? sm.Cast<TuiSelectable>().ToArray() : e))){
		if(sm == null){
			SelectionMatrix = new TuiSelectable[1,1];
		}else{
			SelectionMatrix = sm;
		}
		MatrixPointerX = startX;
		MatrixPointerY = startY;
		SetSelected(true);
		
		keyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<TuiScreenInteractive, ConsoleKeyInfo>>();
		
		SetDefaultKeys();
	}
	
	internal void SetSelected(bool b){
		if(Selected != null){
			Selected.Selected = b;
		}
	}
	
	/// <summary>
	/// Adds a new key event
	/// </summary>
	public void SubKeyEvent(ConsoleKey k, ConsoleModifiers m, Action<TuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, m)] = keyFunction;
	}
	
	/// <summary>
	/// Adds a new key event with no modifiers
	/// </summary>
	public void SubKeyEvent(ConsoleKey k, Action<TuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, ConsoleModifiers.None)] = keyFunction;
	}
	
	/// <summary>
	/// Clears all existent key events. Useful because there are some default ones
	/// </summary>
	public void DeleteAllKeyEvents(){
		this.keyFunctions.Clear();
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
			
			if(!WaitForKey && !Console.KeyAvailable){
				CallFinishCycleEvent();
				continue;
			}
			
			ConsoleKeyInfo keyInfo = Console.ReadKey(true);
			
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
		
		if(keyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
			Action<TuiScreenInteractive, ConsoleKeyInfo> keyFunction = keyFunctions[(keyInfo.Key, keyInfo.Modifiers)];
			keyFunction.Invoke(this, keyInfo);
			return true;
		}
		return false;
	}
	
	internal void CallFinishCycleEvent(){
		if(FinishPlayCycleEvent != null){
			FinishPlayCycleEvent.Invoke(this);
		}
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

/// <summary>
/// Screen aimed at handling having multiple interactive screens at the same time, and only controlling one
/// </summary>
public class MultipleTuiScreenInteractive : TuiScreen{
	
	/// <summary>
	/// The curently selected screen
	/// </summary>
	public TuiScreenInteractive SelectedScreen {get; set{
		if(value != null && !ScreenList.Contains(value)){
			return;
		}
		if(field != null){
			field.SetSelected(false);
		}
		field = value;
		if(field != null){
			field.SetSelected(true);
		}
	}}
	
	/// <summary>
	/// All the interactive screens
	/// </summary>
	protected List<TuiScreenInteractive> ScreenList;
	
	/// <summary>
	/// If this screen is playing
	/// </summary>
	public bool Playing {get; private set;}
	
	/// <summary>
	/// The key actions
	/// </summary>
	protected Dictionary<(ConsoleKey, ConsoleModifiers), Action<MultipleTuiScreenInteractive, ConsoleKeyInfo>> keyFunctions;
	
	/// <summary>
	/// If the screen will wait for a key press before updating
	/// </summary>
	public bool WaitForKey = false;
	
	/// <summary>
	/// Action called at the end of each play cycle
	/// </summary>
	public Action<MultipleTuiScreenInteractive>? FinishPlayCycleEvent {internal get; set;}
	
	/// <summary>
	/// Initializes a new instance of a MultipleTuiScreenInteractive screen. Note that it starts with no selected screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="ss">All the interactive screens</param>
	/// <param name="f">The default foreground color</param>
	/// <param name="b">The default background color</param>
	/// <param name="e">Additional elements</param>
	public MultipleTuiScreenInteractive(uint xs, uint ys, IEnumerable<TuiScreenInteractive> ss, Placement p, int x, int y, Color3? f, Color3? b, params TuiElement[] e)
										: base(xs, ys, p, x, y, f, b, (e != null ? (ss != null ? e.Concat(ss).ToArray() : e) : (ss != null ? ss.ToArray() : e))){
		if(ss == null){
			ScreenList = new List<TuiScreenInteractive>();
		}else{
			ScreenList = ss.ToList();
		}
		
		foreach(TuiScreenInteractive sb in ScreenList){
			sb.SetSelected(false);
		}
		
		keyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<MultipleTuiScreenInteractive, ConsoleKeyInfo>>();
	}
	
	/// <summary>
	/// Initializes a new instance of a MultipleTuiScreenInteractive screen. Note that it starts with no selected screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="ss">All the interactive screens</param>
	/// <param name="f">The default foreground color</param>
	/// <param name="b">The default background color</param>
	/// <param name="e">Additional elements</param>
	public MultipleTuiScreenInteractive(uint xs, uint ys, IEnumerable<TuiScreenInteractive> ss, Color3? f, Color3? b, params TuiElement[] e)
										: base(xs, ys, f, b, (e != null ? (ss != null ? e.Concat(ss).ToArray() : e) : (ss != null ? ss.ToArray() : e))){
		if(ss == null){
			ScreenList = new List<TuiScreenInteractive>();
		}else{
			ScreenList = ss.ToList();
		}
		
		foreach(TuiScreenInteractive sb in ScreenList){
			sb.SetSelected(false);
		}
		
		keyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<MultipleTuiScreenInteractive, ConsoleKeyInfo>>();
	}
	
	/// <summary>
	/// Adds a new key event
	/// </summary>
	public void SubKeyEvent(ConsoleKey k, ConsoleModifiers m, Action<MultipleTuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, m)] = keyFunction;
	}
	
	/// <summary>
	/// Adds a new key event with no modifiers
	/// </summary>
	public void SubKeyEvent(ConsoleKey k, Action<MultipleTuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		keyFunctions[(k, ConsoleModifiers.None)] = keyFunction;
	}
	
	/// <summary>
	/// Clears all existent key events
	/// </summary>
	public void DeleteAllKeyEvents(){
		this.keyFunctions.Clear();
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
			
			if(!WaitForKey && !Console.KeyAvailable){
				CallFinishCycleEvent();
				continue;
			}
			
			ConsoleKeyInfo keyInfo = Console.ReadKey(true);
			
			if(SelectedScreen != null){
				if(!SelectedScreen.HandleKey(keyInfo) && keyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
					Action<MultipleTuiScreenInteractive, ConsoleKeyInfo> keyFunction = keyFunctions[(keyInfo.Key, keyInfo.Modifiers)];
					keyFunction.Invoke(this, keyInfo);
				}
			}else if(keyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
				Action<MultipleTuiScreenInteractive, ConsoleKeyInfo> keyFunction = keyFunctions[(keyInfo.Key, keyInfo.Modifiers)];
				keyFunction.Invoke(this, keyInfo);
			}
			
			foreach(TuiScreenInteractive sb in ScreenList){
				sb.CallFinishCycleEvent();
			}
			
			CallFinishCycleEvent();
		}
	}
	
	internal void CallFinishCycleEvent(){
		if(FinishPlayCycleEvent != null){
			FinishPlayCycleEvent.Invoke(this);
		}
	}
	
	/// <summary>
	/// Stop a screen playing
	/// </summary>
	public static void StopPlaying(MultipleTuiScreenInteractive s, ConsoleKeyInfo ck){
		s.Playing = false;
	}
}