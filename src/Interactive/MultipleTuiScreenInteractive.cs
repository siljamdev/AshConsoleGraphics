using System;
using AshLib;
using AshLib.Lists;
using AshLib.Formatting;

namespace AshConsoleGraphics.Interactive;

//Manager elements

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
		field?.SetSelected(false);
		field = value;
		field?.SetSelected(true);
	}}
	
	/// <summary>
	/// All the interactive screens
	/// </summary>
	public ReactiveList<TuiScreenInteractive> ScreenList {get; private set;}
	
	/// <summary>
	/// If this screen is playing
	/// </summary>
	public bool Playing {get; private set;}
	
	/// <summary>
	/// The key actions
	/// </summary>
	public Dictionary<(ConsoleKey, ConsoleModifiers), Action<MultipleTuiScreenInteractive, ConsoleKeyInfo>> KeyFunctions {get; private set;}
	
	/// <summary>
	/// If the screen will wait for a key press before updating
	/// </summary>
	public bool WaitForKey = false;
	
	/// <summary>
	/// Event called at the end of each play cycle
	/// </summary>
	public EventHandler OnFinishPlayCycle;
	
	/// <summary>
	/// Initializes a new instance of a MultipleTuiScreenInteractive screen. Note that it starts with no selected screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="ss">All the interactive screens</param>
	/// <param name="f">The default format</param>
	/// <param name="e">Additional elements</param>
	public MultipleTuiScreenInteractive(int xs, int ys, IEnumerable<TuiScreenInteractive> ss, Placement p, int x, int y, CharFormat? f, params TuiElement[] e)
										: base(xs, ys, p, x, y, f, (e != null ? (ss != null ? e.Concat(ss).ToArray() : e) : (ss != null ? ss.ToArray() : e))){
		bool b = false; //Prevent calling it twice because its expensive
		if(ss == null){
			ScreenList = new ReactiveList<TuiScreenInteractive>(() => {
				if(b){
					return;
				}
				b = true;
				ScreenList.RemoveAll(x => x == null);
				b = false;
				
				foreach(TuiScreenInteractive sb in ScreenList.ToArray()){
					sb.SetSelected(false);
				}
				
				if(!ScreenList.Contains(SelectedScreen)){
					SelectedScreen = null;
				}else{
					SelectedScreen.SetSelected(true);
				}
			});
		}else{
			ScreenList = new ReactiveList<TuiScreenInteractive>(ss.Where(x => x != null).Distinct(), () => {
				if(b){
					return;
				}
				b = true;
				ScreenList.RemoveAll(x => x == null);
				b = false;
				
				foreach(TuiScreenInteractive sb in ScreenList.ToArray()){
					sb.SetSelected(false);
				}
				
				if(!ScreenList.Contains(SelectedScreen)){
					SelectedScreen = null;
				}else{
					SelectedScreen.SetSelected(true);
				}
			});
		}
		
		foreach(TuiScreenInteractive sb in ScreenList.ToArray()){
			sb.SetSelected(false);
		}
		
		KeyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<MultipleTuiScreenInteractive, ConsoleKeyInfo>>();
	}
	
	/// <summary>
	/// Initializes a new instance of a MultipleTuiScreenInteractive screen. Note that it starts with no selected screen
	/// </summary>
	/// <param name="xs">The x size</param>
	/// <param name="ys">The y size</param>
	/// <param name="ss">All the interactive screens</param>
	/// <param name="f">The default format</param>
	/// <param name="e">Additional elements</param>
	public MultipleTuiScreenInteractive(int xs, int ys, IEnumerable<TuiScreenInteractive> ss, CharFormat? f, params TuiElement[] e)
										: base(xs, ys, f, (e != null ? (ss != null ? e.Concat(ss).ToArray() : e) : (ss != null ? ss.ToArray() : e))){
		bool b = false; //Prevent calling it twice because its expensive
		if(ss == null){
			ScreenList = new ReactiveList<TuiScreenInteractive>(() => {
				if(b){
					return;
				}
				b = true;
				ScreenList.RemoveAll(x => x == null);
				b = false;
				
				foreach(TuiScreenInteractive sb in ScreenList.ToArray()){
					sb.SetSelected(false);
				}
				
				if(!ScreenList.Contains(SelectedScreen)){
					SelectedScreen = null;
				}else{
					SelectedScreen.SetSelected(true);
				}
			});
		}else{
			ScreenList = new ReactiveList<TuiScreenInteractive>(ss.Where(x => x != null).Distinct(), () => {
				if(b){
					return;
				}
				b = true;
				ScreenList.RemoveAll(x => x == null);
				b = false;
				
				foreach(TuiScreenInteractive sb in ScreenList.ToArray()){
					sb.SetSelected(false);
				}
				
				if(!ScreenList.Contains(SelectedScreen)){
					SelectedScreen = null;
				}else{
					SelectedScreen.SetSelected(true);
				}
			});
		}
		
		foreach(TuiScreenInteractive sb in ScreenList.ToArray()){
			sb.SetSelected(false);
		}
		
		KeyFunctions = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<MultipleTuiScreenInteractive, ConsoleKeyInfo>>();
	}
	
	/// <summary>
	/// Adds a new key event and returns itself
	/// </summary>
	public MultipleTuiScreenInteractive SubKeyEvent(ConsoleKey k, ConsoleModifiers m, Action<MultipleTuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		KeyFunctions[(k, m)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Adds a new key event with no modifiers and returns itself
	/// </summary>
	public MultipleTuiScreenInteractive SubKeyEvent(ConsoleKey k, Action<MultipleTuiScreenInteractive, ConsoleKeyInfo> keyFunction){
		KeyFunctions[(k, ConsoleModifiers.None)] = keyFunction;
		return this;
	}
	
	/// <summary>
	/// Clears all existent key events and returns itself
	/// </summary>
	public MultipleTuiScreenInteractive DeleteAllKeyEvents(){
		this.KeyFunctions.Clear();
		return this;
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
		if(SelectedScreen != null && SelectedScreen.HandleKey(keyInfo)){
			return true;
		}else if(KeyFunctions.ContainsKey((keyInfo.Key, keyInfo.Modifiers))){
			Action<MultipleTuiScreenInteractive, ConsoleKeyInfo> keyFunction = KeyFunctions[(keyInfo.Key, keyInfo.Modifiers)];
			keyFunction.Invoke(this, keyInfo);
			
			return true;
		}
		
		return false;
	}
	
	internal void CallFinishCycleEvent(){
		foreach(TuiScreenInteractive sb in ScreenList.ToArray()){
			sb.CallFinishCycleEvent();
		}
		
		OnFinishPlayCycle?.Invoke(this, EventArgs.Empty);
	}
	
	/// <summary>
	/// Stop a screen playing
	/// </summary>
	public static void StopPlaying(MultipleTuiScreenInteractive s, ConsoleKeyInfo ck){
		s.Playing = false;
	}
}