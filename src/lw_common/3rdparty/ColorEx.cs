using System;
using System.Drawing;

// 8 Feb 2016 - yeah, this turned out to be a very stupid idea - much better to use the HSL from http://www.codeproject.com/Articles/19045/Manipulating-colors-in-NET-Part
//
// imported from http://www.codeproject.com/Articles/1001468/Color-Scheme-Selector
//
// modified to use System.Drawing
namespace ColorSchemeExtension_DO_NOT_USE
{
	public class ColorEx
	{
		public static short MaxHue = 360;
		public static short MaxSaturation = 100;

		private bool _IsRGBDirty = false;
		private bool _IsHSVDirty = false;

		public ColorEx ( )
		{
			RGB2HSV( );
		}

		public ColorEx ( Color c) {
		    _r = c.R;
		    _b = c.B;
		    _g = c.G;
			RGB2HSV( );
		}

	    private byte _r = 128, _g = 128, _b = 128;

		public ColorEx Clone ( )
		{
			return ( new ColorEx( )
			{
				R = R,
				G = G,
				B = B
			} );
		}


		public Color Color
		{
			get
			{
				if ( _IsRGBDirty )
				{
					HSV2RGB( );
				}

				return ( Color.FromArgb(_r,_g,_b) );
			}
		}

		public byte R
		{
			get
			{
				if ( _IsRGBDirty )
				{
					HSV2RGB( );
				}

				return ( _r );
			}
			set
			{
				_r = value;

				_IsHSVDirty = true;
			}
		}

		public byte G
		{
			get
			{
				if ( _IsRGBDirty )
				{
					HSV2RGB( );
				}

				return ( _g );
			}
			set
			{
				_g = value;

				_IsHSVDirty = true;
			}
		}

		public byte B
		{
			get
			{
				if ( _IsRGBDirty )
				{
					HSV2RGB( );
				}

				return ( _b );
			}
			set
			{
				_b = value;

				_IsHSVDirty = true;
			}
		}

		private short _H = 0;

		public short H
		{
			get
			{
				if ( _IsHSVDirty )
				{
					RGB2HSV( );
				}

				return ( _H );
			}
			set
			{
				// Hue is circular (degree)
				_H = ( short )( ( value < 0 ? 360 : 0 ) + ( value % 360 ) );

				_IsRGBDirty = true;
			}
		}

		private byte _S = 0;

		public byte S
		{
			get
			{
				if ( _IsHSVDirty )
				{
					RGB2HSV( );
				}

				return ( _S );
			}
			set
			{
				if ( value >= 0 && value <= 100 )
				{
					_S = value;

					_IsRGBDirty = true;
				}
			}
		}

		private byte _V = 0;

		public byte V
		{
			get
			{
				if ( _IsHSVDirty )
				{
					RGB2HSV( );
				}

				return ( _V );
			}
			set
			{
				if ( value >= 0 && value <= 100 )
				{
					_V = value;

					_IsRGBDirty = true;
				}
			}
		}


		#region Helpers

		private void RGB2HSV ( )
		{
			double nR = _r / ( double )255;
			double nG = _g / ( double )255;
			double nB = _b / ( double )255;
			double nCmax = Math.Max( nR, Math.Max( nG, nB ) );
			double nCmin = Math.Min( nR, Math.Min( nG, nB ) );
			double nDelta = nCmax - nCmin;
			double nH = 0;
			double nS = 0;
			double nV = nCmax;

			if ( nDelta != 0 )
			{
				if ( nCmax == nR )
				{
					nH = ( ( nG - nB ) / nDelta ) % 6.0;
				}
				else if ( nCmax == nG )
				{
					nH = ( ( nB - nR ) / nDelta ) + 2.0;
				}
				else if ( nCmax == nB )
				{
					nH = ( ( nR - nG ) / nDelta ) + 4.0;
				}
			}

			nH *= 60.0;

			if ( nH < 0 )
			{
				nH += 360;
			}

			if ( nDelta != 0 )
			{
				nS = nDelta / nCmax;
			}

			nS *= ( double )100;
			nV *= ( double )100;

			_H = ( short )( nH );
			_S = ( byte )( nS );
			_V = ( byte )( nV );

			_IsHSVDirty = false;
		}

		private void HSV2RGB ( )
		{
			double nS = _S / ( double )100;
			double nV = _V / ( double )100;
			double nDelta = nV * nS;
			double nH = _H / 60.0;
			double nX = nDelta * ( 1 - Math.Abs( ( nH % 2 ) - 1 ) );
			double nM = nV - nDelta;
			double nR = 0;
			double nG = 0;
			double nB = 0;

			if ( nH >= 0 && nH < 1 )
			{
				nR = nDelta;
				nG = nX;
			}
			else if ( nH >= 1 && nH < 2 )
			{
				nR = nX;
				nG = nDelta;
			}
			else if ( nH >= 2 && nH < 3 )
			{
				nG = nDelta;
				nB = nX;
			}
			else if ( nH >= 3 && nH < 4 )
			{
				nG = nX;
				nB = nDelta;
			}
			else if ( nH >= 4 && nH < 5 )
			{
				nR = nX;
				nB = nDelta;
			}
			else
			{
				nR = nDelta;
				nB = nX;
			}

			nR += nM;
			nG += nM;
			nB += nM;

			nR *= 255;
			nG *= 255;
			nB *= 266;

			_r = ( byte )( nR );
			_g = ( byte )( nG );
			_b = ( byte )( nB );

			_IsRGBDirty = false;
		}

		#endregion
	}
}
