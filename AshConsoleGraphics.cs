using System;
using System.Text;

namespace AshConsoleGraphics;

internal static class FastConsole{
	static readonly BufferedStream str;

	static FastConsole(){
		Console.OutputEncoding = Encoding.Unicode;

		str = new BufferedStream(Console.OpenStandardOutput(), 0x15000);
	}

	public static void Write(String s){
		var rgb = new byte[s.Length << 1];
		Encoding.Unicode.GetBytes(s, 0, s.Length, rgb, 0);

		lock (str)	 // (optional, can omit if appropriate)
			str.Write(rgb, 0, rgb.Length);
	}

	public static void Flush() { 
	lock (str) str.Flush(); }
}

/// <summary>
/// Elements that can work as connected lines
/// </summary>
public interface ILineElement{
    public BitBuffer GenerateBitBuffer();
}

/// <summary>
/// The relative placement of TuiElements respect to its parent Screen
/// </summary>
public enum Placement{
	Center, TopLeft, TopRight, BottomLeft, BottomRight, CenterLeft, CenterRight, TopCenter, BottomCenter
}