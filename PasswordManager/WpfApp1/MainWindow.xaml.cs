using ExtensionX;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private SerializedData _loadData;

        public SerializedData LoadData
        {
            get
            {
                return _loadData;
            }
            set
            {
                if (value != null)
                {
                    CommentLabel.Content = "备注:" + value.GetString("Comment");

                }
                else
                {
                    CommentLabel.Content = "备注:空";

                }



                _loadData = value;
            }
        }

        private void BuildBtn_Click(object sender, RoutedEventArgs e)
        {
            int min = 0;
            int max = 0;
            if (UseRandom.IsChecked.Value)
            {

                if (!int.TryParse(RandomMin.Text, out min))
                {
                    MessageBox.Show("随机最小值输入不正确", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(RandomMax.Text, out max))
                {
                    MessageBox.Show("随机最大值输入不正确", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            int subCount;
            if (!int.TryParse(SubCount.Text, out subCount))
            {
                MessageBox.Show("截取长度输入不正确", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (subCount < 6)
            {
                MessageBox.Show("截取长度不能小于6位", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (subCount > 18)
            {
                MessageBox.Show("截取长度不能大于18位", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = "pd";
            sf.Filter = "密码文件(*.pd)|*.pd";
            if (sf.ShowDialog().Value)
            {

                SerializedData sd = new SerializedData();

                string randomStr = "";

                if (UseRandom.IsChecked.Value)
                {
                    randomStr = new Random().Next(min, max).ToString();
                }


                sd.SetString("FixedContent", FixedContent.Text + randomStr);
                sd.SetInt("SubCount", subCount);
                sd.SetBool("UseSpecial", UseSpecial.IsChecked.Value);
                sd.SetString("SpecialContent", SpecialContent.Text);
                sd.SetBool("UseRandomCase", UseRandomCase.IsChecked.Value);
                sd.SetString("Comment", Comment.Text);


                using (MemoryStream ms = new MemoryStream())
                {
                    sd.WriteStreamData(ms);
                    File.WriteAllBytes(sf.FileName, ms.ToArray());
                }


                MessageBox.Show("生成成功:" + sf.FileName, "提示", MessageBoxButton.OK, MessageBoxImage.Information);


            }

        }

        SerializedData LoadFile(string filePath)
        {
            try
            {
                using (Stream s = File.Open(filePath, FileMode.Open))
                {
                    return Util.ReadStreamData<SerializedData>(s);
                }
            }
            catch (Exception)
            {

                return null;
            }

        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoadData == null)
            {
                MessageBox.Show("没有加载文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string fixedContent = LoadData.GetString("FixedContent");
            int subCount = LoadData.GetInt("SubCount");
            bool useSpecial = LoadData.GetBool("UseSpecial");
            string specialContent = LoadData.GetString("SpecialContent");
            bool useRandomCase = LoadData.GetBool("UseRandomCase");

            string result = fixedContent + PasswordInput.Password;
            result = SHA1(result);
            if (useSpecial)
            {
                for (int i = 0; i < specialContent.Length; i++)
                {
                    if (i >= result.Length)
                        break;
                    Random indexRandom = new Random(result[i]);
                    int index = indexRandom.Next(0, result.Length);
                    result = result.Insert(index, specialContent[i].ToString());
                }
            }

            if (useRandomCase)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    Random random = new Random(result[i]);
                    double d = random.NextDouble();
                    if (d >= 0.5f)
                    {
                        string remove = result[i].ToString();
                        result = result.Remove(i, 1);
                        result = result.Insert(i, remove.ToLower());
                    }

                }
            }

            result = result.Substring(0, subCount);

            Result.Text = result;


        }
        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }

        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="content">需要加密字符串</param>  
        /// <param name="encode">指定加密编码</param>  
        /// <returns>返回40位大写字符串</returns>  
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

        private void FileDragOver(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            LoadData = LoadFile(fileName);
            if (LoadData != null)
            {
                TipsLabel.Content = "当前加载文件:" + fileName;
            }
            else
            {
                TipsLabel.Content = "当前加载文件:空";
            }
        }


    }
}
