using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace GlyphRunIssue
{
	/// <summary>
	/// Very basic class to contain drawing visuals.  The MainWindow markup creates an instance of this
	/// and this is used to hold WPF drawing elements.
	/// </summary>
	public class VisualContainer : FrameworkElement
	{
		public VisualCollection Visuals { get; set; }


		public VisualContainer()
		{
			// Create a VisualCollection object which is defined in .NET just for this use.
			// We are simply housing it in a Framework element so it can be placed in markup.
			Visuals = new VisualCollection(this);

			DrawingVisual visual = new();
			Visuals.Add(visual);
            RenderVisual(visual);
		}



		protected override Visual GetVisualChild(int index)
		{
			return Visuals[index];
		}



		protected override int VisualChildrenCount
		{
			get
			{
				return Visuals.Count;
			}
		}



		private static void RenderVisual(DrawingVisual visual)
		{
			using DrawingContext context = visual.RenderOpen();

			//////////////////////////////////////////////////////////////////////////////////
			// Create a glyph run of 100 lower case 'i' with an arbitrary fixed glyph width
			//////////////////////////////////////////////////////////////////////////////////

			Typeface typeface = new(new FontFamily("Arial"),
				FontStyles.Normal,                          // normal weight
				FontWeight.FromOpenTypeWeight(400),         // normal weight
				FontStretch.FromOpenTypeStretch(5));        // normal stretch

			if (!typeface.TryGetGlyphTypeface(out GlyphTypeface glyphTypeface))
				return;

			ushort glyphIndex = glyphTypeface.CharacterToGlyphMap['i'];
			const double glyphHeight = 30.0;
			const double glyphWidth = 5.0;
			Point ptOrigin = new(glyphHeight, glyphHeight * 2.0);

			ushort[] glyphIndices = Enumerable.Repeat(glyphIndex, 100).ToArray();
			double[] advWidths = Enumerable.Repeat(glyphWidth, 100).ToArray();
			Point[] glyphOffsets = Enumerable.Repeat(new Point(0, 0), 100).ToArray();
			char[] chars = Enumerable.Repeat('i', 100).ToArray();
			ushort[] clusterMap = Enumerable.Range(0, 100).Select(i => (ushort)i).ToArray();

			GlyphRun gr = new(glyphTypeface,
				bidiLevel: 0,
				isSideways: false,
				renderingEmSize: glyphHeight,
				pixelsPerDip: 1F,
				glyphIndices,
				baselineOrigin: ptOrigin,
				advanceWidths: advWidths,
				glyphOffsets: glyphOffsets,
				characters: chars,
				deviceFontName: null,
				clusterMap: clusterMap,
				caretStops: null,
				language: null);

			context.DrawGlyphRun(Brushes.Black, gr);

			//////////////////////////////////////////////////
			// Draw the alignment box around the GlyphRun
			//////////////////////////////////////////////////

			Pen penAlign = new(Brushes.Red, 1.0);
			var rect = gr.ComputeAlignmentBox();
			rect.Offset(ptOrigin.X, ptOrigin.Y);
			context.DrawRectangle(null, penAlign, rect);

			//////////////////////////////////////////////////////////////////////////
			// Draw vertical tick marks under each glyph's computed starting position
			//////////////////////////////////////////////////////////////////////////

			Pen pen = new(Brushes.Blue, 1.0);
			double x = ptOrigin.X;
			for(int i=0; i<100; i++)
			{
				context.DrawLine(pen, new Point(x, ptOrigin.Y), new Point(x, ptOrigin.Y + glyphHeight));
				x += glyphWidth;
			}

			// Now draw some text to make sure that it is properly spaced.
			// When printing, an extra space is inserted between the 'fonction' and 's'
			var ft = new FormattedText("L'autre personne était incapable de prendre soin des enfants en raison d'une déficience des fonctions physiques ou mentales qui l'a obligée", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 10, Brushes.Black, 1.0);

			context.DrawText(ft, new Point(glyphHeight, glyphHeight*4));
		}
	}
}
