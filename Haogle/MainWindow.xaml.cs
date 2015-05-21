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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
namespace Haogle
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

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Test wm = new Test();
            wm.Show();
            wm.WindowState = WindowState.Maximized;
            this.Close();

        }

        private void Face_Click(object sender, RoutedEventArgs e)
        {
            Face wm = new Face();
            wm.Show();
            wm.WindowState = WindowState.Maximized;
            this.Close();

        }

        private void TrackMode_Click(object sender, RoutedEventArgs e)
        {
            TrackMode wm = new TrackMode();
            wm.Show();
            wm.WindowState = WindowState.Maximized;
            this.Close();
        }

        private void SpeechControl_Click(object sender, RoutedEventArgs e)
        {
           SpeechControl wm = new SpeechControl();
            wm.Show();
            wm.WindowState = WindowState.Maximized;
            this.Close();
        }

        private void voice_Click(object sender, RoutedEventArgs e)
        {
            Voice wm = new Voice();
            wm.Show();
            wm.WindowState = WindowState.Maximized;
            this.Close();


        }

        private void GreenScreen_Click(object sender, RoutedEventArgs e)
        {
            Green wm = new Green();
            wm.Show();
            wm.WindowState = WindowState.Maximized;
            this.Close();

        }


        private void Help_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.CheckFileExists = true;
            dlg.Filter = //"PowerPoint Format (*.ppt,*.pptx)|*.ppt;*.pptx|" +
                         "All files (*.*)|*.*";

            if ((bool)dlg.ShowDialog(this))
            {
                string filePath = dlg.FileName;
                string xpsFilePath = dlg.FileName + ".xps";
                Window1 pw = new Window1();
                pw.Owner = this;
                pw.Show();
                
                pw.WindowState = WindowState.Maximized;
                var convertResults = OfficeToXps.ConvertToXps(filePath, ref xpsFilePath);
                pw.Close();
                switch (convertResults.Result)
                {
                    case ConversionResult.OK:
                        Help xps = new Help(xpsFilePath);
                        xps.Owner = this;
                        xps.Show();
                        break;

                    case ConversionResult.InvalidFilePath:
                        // 处理文件路径错误或文件不存在
                        break;
                    case ConversionResult.UnexpectedError:

                        break;
                    case ConversionResult.ErrorUnableToInitializeOfficeApp:
                        // Office2013 未安装会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToOpenOfficeFile:
                        // 文件被占用会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToAccessOfficeInterop:
                        // Office2013 未安装会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToExportToXps:
                        // 微软 OFFICE2013 Save As PDF 或 XPS  插件未安装异常
                        break;
                }
            }
        }

        private void avatar_Click(object sender, RoutedEventArgs e)
        {
            avatar wm = new avatar();
            wm.Show();

            wm.WindowState = WindowState.Maximized;
            this.Close();
        }
        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    MessageBox.Show("Are you sure for quit ");
        //}

    }
}
