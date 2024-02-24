using System;
using System.Drawing;
using Pastel;
using System.Text;

namespace AshConsoleGraphics
{
	
	public struct Col{
		int R;
		int G;
		int B;
		
		public Col(int r, int g, int b){
			this.R = r;
			this.G = g;
			this.B = b;
		}
		
		public Color toCol(){
			return Color.FromArgb(this.R, this.G, this.B);
		}
		
		public static bool operator==(Col a, Col b)
        {
            return ((a.R == b.R) && (a.G == b.G) && (a.B == b.B));
        }
		
		public static bool operator!=(Col a, Col b)
        {
            return !(a == b);
        }
		
		public override string ToString()
        {
            return string.Format("R: {0}; G: {1}; B: {2}", R, G, B);
        }

        public override bool Equals(object? obj)
        {
            var objectToCompare = obj as Col?;
            if ( objectToCompare == null )
                return false;

            return this.ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
	}
	
	public static class FastConsole
	{
		static readonly BufferedStream str;
	
		static FastConsole()
		{
			Console.OutputEncoding = Encoding.Unicode;
	
			str = new BufferedStream(Console.OpenStandardOutput(), 0x15000);
		}
	
		public static void Write(String s)
		{
			var rgb = new byte[s.Length << 1];
			Encoding.Unicode.GetBytes(s, 0, s.Length, rgb, 0);
	
			lock (str)   // (optional, can omit if appropriate)
				str.Write(rgb, 0, rgb.Length);
		}
	
		public static void Flush() { 
		lock (str) str.Flush(); }
	}

}