using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoRenamer_UI
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox1.Refresh();
            Application.DoEvents();

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                LogLine("Getting paths");

                var paths = new List<string>(ConfigurationManager.AppSettings["paths"].Split(new char[] { ';' }));

                LogLine("DONE - Getting paths");

                foreach (var path in paths)
                {
                    try
                    {
                        LogLine("Processing " + path);

                        DirectoryInfo di = new DirectoryInfo(path);
                        string nowString = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string pathRenamed = Path.Combine(path, nowString);

                        if (!Directory.Exists(pathRenamed))
                        {
                            LogLine("Creating directory " + pathRenamed);
                            Directory.CreateDirectory(pathRenamed);
                            LogLine("DONE - Creating directory " + pathRenamed);
                        }

                        var allFiles = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(s => s.EndsWith(".jpg") ||
                            s.EndsWith(".jpg") ||
                            s.EndsWith(".jpeg") ||
                            s.EndsWith(".png") ||
                            s.EndsWith(".JPG") ||
                            s.EndsWith(".JPEG") ||
                            s.EndsWith(".PNG")
                            );

                        foreach (var file in allFiles)
                        {
                            LogLine("Rename file " + file);

                            FileInfo fi = new FileInfo(file);

                            Image image = Image.FromFile(fi.FullName);

                            //Try get datetime
                            try
                            {
                                LogLine("Try get PropertyTagExifDTDigitized " + file);

                                //PropertyTagExifDTDigitized
                                var datetime = Encoding.UTF8.GetString(image.GetPropertyItem(0x9004).Value).Replace(":", "").Replace("\0", "");

                                LogLine("DONE - Success trying get PropertyTagExifDTDigitized " + file);

                                var newName = Path.Combine(pathRenamed, datetime + fi.Extension.ToLower());

                                LogLine("Renaming from " + file + " to " + newName);

                                File.Copy(fi.FullName, newName);

                                LogLine("DONE - Renaming from " + file + " to " + newName);
                            }
                            catch
                            {
                                LogLine("Fail rename by PropertyTagExifDTDigitized " + file);

                                var newName = Path.GetFileNameWithoutExtension(fi.FullName).Replace("IMG-", "").Replace("WA", "").Replace("-", "").Replace(".", "");

                                LogLine("Rename by replacing from " + file + " to " + newName);

                                File.Copy(fi.FullName, Path.Combine(pathRenamed, newName + fi.Extension.ToLower()));

                                LogLine("DONE - Rename by replacing from " + file + " to " + newName);
                            }

                            LogLine("DONE - Rename file " + file);
                        }

                        LogLine("DONE - Processing " + path);
                    }
                    //PER DIRECTORY CATCH
                    catch (Exception ex)
                    {
                        Cursor.Current = Cursors.Default;

                        LogLine("**********CRITICAL ERROR**********");
                        LogLine(ex.Message);
                        LogLine("----------------------------------");
                        LogLine(ex.StackTrace);
                        LogLine("**********CRITICAL ERROR**********");
                    }
                }

                Cursor.Current = Cursors.Default;
            }
            //GLOBAL CATCH
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;

                LogLine("**********CRITICAL ERROR GLOBAL**********");
                LogLine(ex.Message);
                LogLine("----------------------------------");
                LogLine(ex.StackTrace);
                LogLine("**********CRITICAL ERROR GLOBAL**********");
            }

            Cursor.Current = Cursors.Default;
        }

        private void LogLine(string text)
        {
            textBox1.Text += text + Environment.NewLine;
            textBox1.Text += Environment.NewLine;
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
            textBox1.Refresh();
            Application.DoEvents();
        }
    }
}
