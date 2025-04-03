using System;
using System.Text;
using System.Collections;
using AshLib;
using AshLib.Formatting;

namespace AshConsoleGraphics;

/// <summary>
/// Char and color buffer used for generating elements
/// </summary>
public class Buffer{
	/// <summary>
	/// Global directive to stop generating buffers with color. Useful for terminals that dont support ansi escape sequences
	/// </summary>
	public static bool NoColor = false;
	
	public uint Xsize{get; private set;}
	public uint Ysize{get; private set;}
	
	char?[] charBuffer;
	Color3?[] fgColorBuffer;
	Color3?[] bgColorBuffer;
	
	string built;
	
	bool needToBuild = true;
	
	/// <summary>
	/// Initializes a new empty buffer with its size
	/// </summary>
	public Buffer(uint x, uint y){
		Xsize = x;
		Ysize = y;
		charBuffer = new char?[Xsize * Ysize];
		fgColorBuffer = new Color3?[Xsize * Ysize];
		bgColorBuffer = new Color3?[Xsize * Ysize];
	}
	
	public Buffer(int x, int y) : this((uint) x, (uint) y){}
	
	/// <summary>
	/// Sets a char
	/// </summary>
	/// <param name="x">The x position of the char</param>
	/// <param name="y">The y position of the char</param>
	/// <param name="c">The char</param>
	/// <param name="fg">The foreground color</param>
	/// <param name="bg">The background color</param>
	public void SetChar(int x, int y, char? c, Color3? fg, Color3? bg){
		if(x >= Xsize || x < 0 || y >= Ysize || y < 0){
			return;
		}
		
		int i = y * (int) Xsize + x;
		
		if(c != null){
			charBuffer[i] = c;
		}
		if(fg != null){
			fgColorBuffer[i] = fg;
		}
		if(bg != null){
			bgColorBuffer[i] = bg;
		}
		needToBuild = true;
	}
	
	/// <summary>
	/// Sets a char
	/// </summary>
	/// <param name="x">The x position of the char</param>
	/// <param name="y">The y position of the char</param>
	/// <param name="c">The char</param>
	/// <param name="fg">The foreground color</param>
	public void SetChar(int x, int y, char? c, Color3? fg){
		if(x >= Xsize || x < 0 || y >= Ysize || y < 0){
			return;
		}
		
		int i = y * (int) Xsize + x;
		
		if(c != null){
			charBuffer[i] = c;
		}
		if(fg != null){
			fgColorBuffer[i] = fg;
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
		
		int i = y * (int) Xsize + x;
		charBuffer[i] = c;
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
			for(int xr = 0; xr < b.Xsize; xr++, i++){
				SetChar(x + xr, y + yr, b.charBuffer[i], b.fgColorBuffer[i], b.bgColorBuffer[i]);
			}
		}
		needToBuild = true;
	}
	
	/// <summary>
	/// Replaces the null colors for printing
	/// </summary>
	/// <param name="defFgColor">The default foreground color</param>
	/// <param name="defBgColor">The default background color</param>
	public void ReplaceNull(Color3? defFgColor, Color3? defBgColor){
		for(int i = 0; i < Xsize * Ysize; i++){
			if(fgColorBuffer[i] == null){
				fgColorBuffer[i] = defFgColor;
			}
			if(bgColorBuffer[i] == null){
				bgColorBuffer[i] = defBgColor;
			}
		}
		needToBuild = true;
	}
	
	/// <summary>
	/// Generates the string representation
	/// </summary>
	/// <param name="defChar">The default char, usually space</param>
	/// <param name="defFgColor">The default foreground color</param>
	/// <param name="defBgColor">The default background color</param>
	public string ToString(char defChar, Color3? defFgColor, Color3? defBgColor){
		if(!needToBuild){
			return built;
		}
		
		if(NoColor){
			StringBuilder sb = new StringBuilder();
			int i = 0;
			for(int yr = 0; yr < Ysize; yr++){
				for(int xr = 0; xr < Xsize; xr++, i++){
					sb.Append(charBuffer[i] != null ? charBuffer[i] : defChar);
				}
				sb.Append(Environment.NewLine);
			}
			built = sb.ToString();
		}else{
			FormatString fs = new FormatString();
			int i = 0;
			bool f = (defFgColor == null);
			bool b = (defBgColor == null);
			for(int yr = 0; yr < Ysize; yr++){
				for(int xr = 0; xr < Xsize; xr++, i++){
					fs.Append((charBuffer[i] != null ? charBuffer[i] : defChar), new CharFormat((fgColorBuffer[i] != null ? fgColorBuffer[i] : defFgColor), (fgColorBuffer[i] == null && f), (bgColorBuffer[i] != null ? bgColorBuffer[i] : defBgColor), (bgColorBuffer[i] == null && b)));
				}
				fs.Append(Environment.NewLine);
			}
			built = fs.ToString();
		}
		needToBuild = false;
		return built;
	}
}

/// <summary>
/// Buffer for elements that are connected lines
/// </summary>
public class BitBuffer{
	public uint Xsize{get; private set;}
	public uint Ysize{get; private set;}
	
	BitArray buffer;
	
	/// <summary>
	/// Initializes a new empty buffer with its size
	/// </summary>
	public BitBuffer(uint x, uint y){
		Xsize = x;
		Ysize = y;
		buffer = new BitArray((int) Xsize * (int) Ysize);
	}
	
	public BitBuffer(int x, int y) : this((uint) x, (uint) y){}
	
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
		
		int i = y * (int) Xsize + x;
		
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
		
		int i = y * (int) Xsize + x;
		
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
	/// <param name="fgColor">The foreground color</param>
	/// <param name="bgColor">The background color</param>
	public Buffer ToBuffer(char[] chars, Color3? fgColor, Color3? bgColor){
		Buffer b = new Buffer(Xsize, Ysize);
		int i = 0;
		for(int yr = 0; yr < Ysize; yr++){
			for(int xr = 0; xr < Xsize; xr++, i++){
				if(buffer[i]){
					b.SetChar(xr, yr, GetConnectedChar(xr, yr, chars), fgColor, bgColor);
				}
			}
		}
		return b;
	}
}