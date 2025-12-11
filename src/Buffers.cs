using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using AshLib;
using AshLib.Formatting;

namespace AshConsoleGraphics;

/// <summary>
/// Char and color buffer used for generating elements
/// </summary>
public class Buffer{
	
	private const int  STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_PROCESSED_OUTPUT = 0x0001;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
	
	[DllImport("kernel32")]
	private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

	[DllImport("kernel32")]
	private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

	[DllImport("kernel32")]
	private static extern IntPtr GetStdHandle(int nStdHandle);
	
	static Buffer(){
		if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){ //The console needs to be prepared becayuse else the NoColor wont work
			var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
			var _ = GetConsoleMode(iStdOut, out var outConsoleMode)
			&& SetConsoleMode(iStdOut, outConsoleMode | ENABLE_PROCESSED_OUTPUT | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
		}
	}
	
	/// <summary>
	/// Global directive to stop generating buffers with format. Useful for terminals that dont support ansi escape sequences
	/// </summary>
	public static bool NoFormat = false;
	
	public int Xsize{get; private init;}
	public int Ysize{get; private init;}
	
	char?[] charBuffer;
	public CharFormat?[] formatBuffer;
	
	string built;
	
	bool needToBuild = true;
	
	/// <summary>
	/// Initializes a new empty buffer with its size
	/// </summary>
	public Buffer(int x, int y){
		Xsize = Math.Max(x, 0);
		Ysize = Math.Max(y, 0);
		charBuffer = new char?[Xsize * Ysize];
		formatBuffer = new CharFormat?[Xsize * Ysize];
	}
	
	/// <summary>
	/// Sets a char
	/// </summary>
	/// <param name="x">The x position of the char</param>
	/// <param name="y">The y position of the char</param>
	/// <param name="c">The char</param>
	/// <param name="f">The char format</param>
	public void SetChar(int x, int y, char? c, CharFormat? f){
		if(x >= Xsize || x < 0 || y >= Ysize || y < 0){
			return;
		}
		
		int i = y * (int) Xsize + x;
		
		if(c != null){
			charBuffer[i] = c;
		}
		if(f != null){
			formatBuffer[i] = f;
		}
		needToBuild = true;
	}
	
	/// <summary>
	/// Sets a char
	/// </summary>
	/// <param name="x">The x position of the char</param>
	/// <param name="y">The y position of the char</param>
	/// <param name="c">The char</param>
	public void SetChar(int x, int y, char? c){
		if(c == null || x >= Xsize || x < 0 || y >= Ysize || y < 0){
			return;
		}
		
		int i = y * Xsize + x;
		charBuffer[i] = c;
		formatBuffer[i] = null;
		needToBuild = true;
	}
	
	/// <summary>
	/// Adds a whole buffer
	/// </summary>
	/// <param name="x">The x position of the buffer, top left corner</param>
	/// <param name="y">The y position of the buffer, top left corner</param>
	/// <param name="b">The other buffer</param>
	public void AddBuffer(int x, int y, Buffer b){
		int i = 0;
		for(int yr = 0; yr < b.Ysize; yr++){
			if(y + yr < 0){
				i += b.Xsize;
				continue;
			}
			if(y + yr >= Ysize){
				break;
			}
			for(int xr = 0; xr < b.Xsize; xr++, i++){
				if(x + xr < 0){
					continue;
				}
				if(x + xr >= Xsize){
					continue;
				}
				//SetChar(x + xr, y + yr, b.charBuffer[i], b.formatBuffer[i]);
				
				int i2 = (y + yr) * (int) Xsize + (x + xr);
				
				if(b.charBuffer[i] != null){
					charBuffer[i2] = b.charBuffer[i];
				}
				if(b.formatBuffer[i] != null){
					formatBuffer[i2] = b.formatBuffer[i];
				}
			}
		}
		needToBuild = true;
	}
	
	/// <summary>
	/// Replaces the null colors for printing
	/// </summary>
	/// <param name="def">The default format</param>
	public void ReplaceNull(CharFormat? def){
		for(int i = 0; i < Xsize * Ysize; i++){
			formatBuffer[i] = combine(formatBuffer[i], def);
		}
		needToBuild = true;
	}
	
	/// <summary>
	/// Generates the string representation
	/// </summary>
	/// <param name="defChar">The default char, usually space</param>
	/// <param name="def">The default format</param>
	public string ToString(char defChar, CharFormat? def){
		if(!needToBuild){
			return built;
		}
		
		if(NoFormat){
			StringBuilder sb = new StringBuilder();
			int i = 0;
			for(int yr = 0; yr < Ysize; yr++){
				for(int xr = 0; xr < Xsize; xr++, i++){
					sb.Append(charBuffer[i] ?? defChar);
				}
				sb.Append(Environment.NewLine);
			}
			built = sb.ToString();
		}else{
			FormatString fs = new FormatString();
			int i = 0;
			for(int yr = 0; yr < Ysize; yr++){
				for(int xr = 0; xr < Xsize; xr++, i++){
					fs.Append((charBuffer[i] ?? defChar), fix(combine(formatBuffer[i], def)));
				}
				fs.Append(Environment.NewLine);
			}
			built = fs.ToString();
		}
		needToBuild = false;
		return built;
	}
	
	static CharFormat? fix(CharFormat? a){
		CharFormat r = CharFormat.ResetAll;
		if(a == null){
			return r;
		}
		return new CharFormat(a.density ?? r.density, a.italic ?? r.italic, a.underline ?? r.underline, a.strikeThrough ?? r.strikeThrough, a.foreground, a.foreground == null, a.background, a.background == null);
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
}

/// <summary>
/// Buffer for elements that are connected lines
/// </summary>
public class BitBuffer{
	public int Xsize{get; private init;}
	public int Ysize{get; private init;}
	
	BitArray buffer;
	
	/// <summary>
	/// Initializes a new empty buffer with its size
	/// </summary>
	public BitBuffer(int x, int y){
		Xsize = Math.Max(x, 0);
		Ysize = Math.Max(y, 0);
		buffer = new BitArray(Xsize * Ysize);
	}
	
	/// <summary>
	/// Sets the whole buffer to true and returns itself
	/// </summary>
	public BitBuffer SetAllTrue(){
		for(int i = 0; i < buffer.Length; i++){
			buffer[i] = true;
		}
		
		return this;
	}
	
	/// <summary>
	/// Sets a bit
	/// </summary>
	/// <param name="x">The x position of the bit</param>
	/// <param name="y">The y position of the bit</param>
	/// <param name="b">The bit</param>
	public void SetBit(int x, int y, bool b){
		if(x >= Xsize || x < 0 || y >= Ysize || y < 0){
			return;
		}
		
		int i = y * Xsize + x;
		
		buffer[i] = b;
	}
	
	/// <summary>
	/// Adds a whole buffer
	/// </summary>
	/// <param name="x">The x position of the buffer, top left corner</param>
	/// <param name="y">The y position of the buffer, top left corner</param>
	/// <param name="b">The other buffer</param>
	public void AddBuffer(int x, int y, BitBuffer b){
		int i = 0;
		for(int yr = 0; yr < b.Ysize; yr++){
			for(int xr = 0; xr < b.Xsize; xr++, i++){
				if(b.buffer[i]){
					SetBit(x + xr, y + yr, b.buffer[i]);
				}
			}
		}
	}
	
	bool GetBit(int x, int y){
		if(x >= Xsize || x < 0 || y >= Ysize || y < 0){
			return false;
		}
		
		int i = y * Xsize + x;
		
		return buffer[i];
	}
	
	char GetConnectedChar(int x, int y, char[] chars){
		bool arriba = GetBit(x, y - 1);
		bool abajo = GetBit(x, y + 1);
		bool izquierda = GetBit(x - 1, y);
		bool derecha = GetBit(x + 1, y);
		
		int final = 0;
		
		if(arriba){
			final += 8;
		}
		if(abajo){
			final += 4;
		}
		if(izquierda){
			final += 2;
		}
		if(derecha){
			final += 1;
		}
		
		return chars[final];
	}
	
	/// <summary>
	/// Transforms itself to a normal buffer of connected lines
	/// </summary>
	/// <param name="chars">16 charachters that are the coneccted line. An example would be "·───│┌┐┬│└┘┴│├┤┼"</param>
	/// <param name="def">The default format</param>
	public Buffer ToBuffer(char[] chars, CharFormat? def){
		Buffer b = new Buffer(Xsize, Ysize);
		int i = 0;
		for(int yr = 0; yr < Ysize; yr++){
			for(int xr = 0; xr < Xsize; xr++, i++){
				if(buffer[i]){
					b.SetChar(xr, yr, GetConnectedChar(xr, yr, chars), def);
				}
			}
		}
		return b;
	}
}