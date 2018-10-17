using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileDateClassifier
{
	/// <summary>
	/// DateTimeFormatDialog.xaml の相互作用ロジック
	/// </summary>
	public partial class DateTimeFormatDialog : Window
	{
		public DateTimeFormatDialog()
		{
			InitializeComponent();
		}
		public string Format { get; set; }
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Format = TBFormat.Text;
			Close();
		}


		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (TKResult != null)
			{
				try
				{
					TKResult.Text = DateTime.Now.ToString(TBFormat.Text);
				}
				catch (Exception)
				{
					TKResult.Text = "使用不可";
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			TBFormat.Text = Format;
		}
	}
}
