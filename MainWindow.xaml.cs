using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace GlyphRunIssue
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}



		private void btnPrint_Click(object sender, RoutedEventArgs e)
		{
			DebugRenderToXpsFile();
			//return;

			PrintDialog printDialog = new();
			if (printDialog.ShowDialog() == false)
				return;

			PrintQueue queue = printDialog.PrintQueue;
			XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(queue);

			RenderToXpsDocument(writer);
		}



		protected void DebugRenderToXpsFile()
		{
			var fileXpsDebug = @"c:\Temp\Test.xps";
			File.Delete(fileXpsDebug);
			XpsDocument xpsDocument = new XpsDocument(fileXpsDebug, FileAccess.ReadWrite);
			XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

			RenderToXpsDocument(writer);

			xpsDocument.Close();
		}



		private void RenderToXpsDocument(XpsDocumentWriter writer)
		{
			// Begin rendering to the XPS document writer
			System.Windows.Documents.Serialization.SerializerWriterCollator collator = writer.CreateVisualsCollator();
			collator.BeginBatchWrite();

			collator.Write(visualContainer);

			// Complete XPS document
			collator.EndBatchWrite();
		}
	}
}
