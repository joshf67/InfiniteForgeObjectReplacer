using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Xml.Linq;
using InfiniteForgeConstants.ObjectSettings;
using InfiniteForgePacker.XML;
using InfiniteForgePacker.XML.Application;
using Microsoft.Win32;

namespace ObjectReplacer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty FilePathProperty 
            = DependencyProperty.Register("FilePath", typeof(string), typeof(MainWindow));

        public string FilePath
        {
            get{return (string)GetValue(FilePathProperty);}
            set{SetValue(FilePathProperty,value);}
        }
        
        public static readonly DependencyProperty ObjectToReplaceProperty 
            = DependencyProperty.Register("ObjectToReplace", typeof(ObjectId), typeof(MainWindow));

        public ObjectId ObjectToReplace
        {
            get{return (ObjectId)GetValue(ObjectToReplaceProperty);}
            set{SetValue(ObjectToReplaceProperty,value);}
        }
        
        public static readonly DependencyProperty ReplacerProperty 
            = DependencyProperty.Register("Replacer", typeof(ObjectId), typeof(MainWindow));

        public ObjectId Replacer
        {
            get{return (ObjectId)GetValue(ReplacerProperty);}
            set{SetValue(ReplacerProperty,value);}
        }     
        
        public MainWindow()
        {
            InitializeComponent();
            FilePath = "Please load an XML file";
        }
        
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
            }
        }

        private void btnReplaceObject_Click(object sender, RoutedEventArgs e)
        {
            if (FilePath is "" or "Please load an XML file")
            {
                MessageBoxResult messageBoxLoad = System.Windows.MessageBox.Show("Please load a valid XML file", "Continue", System.Windows.MessageBoxButton.OK);
                return;
            }

            ReplaceType.Convert(FilePath, new []{ (ObjectToReplace, Replacer)});
            
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Your file has been modified", "Continue", System.Windows.MessageBoxButton.OK);
        }

        private void btnReplaceForRetail_Click(object sender, RoutedEventArgs e)
        {
            if (FilePath is "" or "Please load an XML file")
            {
                MessageBoxResult messageBoxLoad = System.Windows.MessageBox.Show("Please load a valid XML file", "Continue", System.Windows.MessageBoxButton.OK);
                return;
            }

            List<(ObjectId ObjectToReplace, ObjectId Replacer)> replacers =
                new List<(ObjectId ObjectToReplace, ObjectId Replacer)>();
            
            foreach (var objectIdName in Enum.GetNames(typeof(ObjectId)))
            {
                if (objectIdName.Contains("_MP"))
                {
                    var nameWithoutMP = objectIdName.Replace("_MP", "");
                    ObjectId replacerId;
                    if (ObjectId.TryParse(nameWithoutMP, out replacerId))
                    {
                        replacers.Add(((ObjectId)Enum.Parse(typeof(ObjectId), objectIdName), replacerId));
                    }
                }

                if (objectIdName.Contains("FX"))
                {
                    replacers.Add(((ObjectId)Enum.Parse(typeof(ObjectId), objectIdName), ObjectId.POINTER));
                }
                
                if (objectIdName.Contains("GRASS"))
                {
                    replacers.Add(((ObjectId)Enum.Parse(typeof(ObjectId), objectIdName), ObjectId.POINTER));
                }
            }

            replacers.Add((ObjectId.FORERUNNER_CONE, ObjectId.PRIMITIVE_CONE));
            replacers.Add((ObjectId.FORERUNNER_BLOCK, ObjectId.PRIMITIVE_BLOCK));
            replacers.Add((ObjectId.FORERUNNER_CYLINDER, ObjectId.PRIMITIVE_CYLINDER));
            replacers.Add((ObjectId.FORERUNNER_PYRAMID, ObjectId.PRIMITIVE_PYRAMID));
            replacers.Add((ObjectId.FORERUNNER_RING_FULL, ObjectId.PRIMITIVE_RING_FULL));
            replacers.Add((ObjectId.FORERUNNER_RING_HALF, ObjectId.PRIMITIVE_RING_HALF));
            replacers.Add((ObjectId.FORERUNNER_RING_QUARTER, ObjectId.PRIMITIVE_RING_QUARTER));
            replacers.Add((ObjectId.FORERUNNER_RING_EIGHTH, ObjectId.PRIMITIVE_RING_EIGHTH));
            replacers.Add((ObjectId.FORERUNNER_SPHERE, ObjectId.PRIMITIVE_SPHERE));
            replacers.Add((ObjectId.FORERUNNER_TRAPEZOID, ObjectId.PRIMITIVE_TRAPEZOID));
            replacers.Add((ObjectId.FORERUNNER_TRIANGLE, ObjectId.PRIMITIVE_TRIANGLE));
            
            ReplaceType.Convert(FilePath, replacers.ToArray());

            var xml = XDocument.Load(FilePath);
            XMLHelper.GetFolderStruct(xml)?.Remove();
            xml.Save(FilePath);
            
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Your file has been modified", "Continue", System.Windows.MessageBoxButton.OK);
        }
    }
}