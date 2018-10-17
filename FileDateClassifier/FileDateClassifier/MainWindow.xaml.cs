using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileDateClassifier
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		CommonOpenFileDialog dirSelFrom;
		CommonOpenFileDialog dirSelTo;
		CancellationTokenSource ts;
		Task task;
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			dirSelFrom = new CommonOpenFileDialog("振り分け元の指定...")
			{
				IsFolderPicker = true,
			};
			dirSelTo = new CommonOpenFileDialog("振り分け先の指定...")
			{
				IsFolderPicker = true,
			};
			ts = new CancellationTokenSource();
		}

		private void BSelFrom_Click(object sender, RoutedEventArgs e)
		{
			dirSelFrom.DefaultFileName = TBFrom.Text;
			if (dirSelFrom.ShowDialog() == CommonFileDialogResult.Ok)
			{
				TBFrom.Text = dirSelFrom.FileName;
			}

		}

		private void BCreateFormat_Click(object sender, RoutedEventArgs e)
		{
			var form = new DateTimeFormatDialog
			{
				Format = TBFormat.Text
			};
			form.ShowDialog();
			TBFormat.Text = form.Format;
		}

		private void BSelTo_Click(object sender, RoutedEventArgs e)
		{
			dirSelTo.DefaultFileName = TBToRoot.Text;
			if (dirSelTo.ShowDialog() == CommonFileDialogResult.Ok)
			{
				TBToRoot.Text = dirSelTo.FileName;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (task == null)
			{
				BProcess.Content = "中止";
				string format = TBFormat.Text;
				var difrom = new DirectoryInfo(TBFrom.Text);
				var dito = new DirectoryInfo(TBToRoot.Text);
				task = new Task(() => Process(format, difrom, dito), ts.Token);
				task.ContinueWith(task =>
				{
					Dispatcher.Invoke(Finished);
				});
				task.Start();
			}
			else
			{
				BProcess.Content = "実行";
				ts.Cancel();
			}
		}
		private void ReportProgress(string text, double value)
		{
			TKProgress.Dispatcher.Invoke(() =>
			{
				TKProgress.Text = text;
			});
			PBProgress.Dispatcher.Invoke(() =>
			{
				PBProgress.Value = value;
			});
		}
		private void Finished()
		{
			BProcess.Content = "実行";
			MessageBox.Show("完了しました。");
			task = null;
			PBProgress.Value = 0;
		}
		private void Process(string format, DirectoryInfo from, DirectoryInfo to)
		{
			var t = from.EnumerateFiles();
			double pmax = t.Count() / 100.0;
			double ctr = 1 / pmax;
			double progress = 0;
			var comp = new FileNameComparator();
			var res = Parallel.ForEach(t, q =>
			 {
				 ts.Token.ThrowIfCancellationRequested();
				 string toPath = Path.Combine(to.FullName, q.LastWriteTime.ToString(format));
				 var DirDest = new DirectoryInfo(toPath);
				 if (!DirDest.Exists) DirDest.Create();
				 var FDist = new FileInfo(Path.Combine(toPath, q.Name));
				 var files = DirDest.EnumerateFiles();
				 if (files.Contains(FDist,comp))
				 {
					 for (ulong i = 1; i < ulong.MaxValue; i++)
					 {
						 FDist = new FileInfo(Path.Combine(toPath, Path.GetFileNameWithoutExtension(q.FullName) + $"({i})" + q.Extension));
						 if (!files.Contains(FDist, comp)) break;
					 }
				 }
				 q.MoveTo(FDist.FullName);
				 progress += ctr;
				 ReportProgress(q.FullName, progress);
			 });
			
		}

		private void TBFormat_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}
