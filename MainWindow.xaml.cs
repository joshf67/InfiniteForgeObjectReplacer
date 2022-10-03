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
using InfiniteForgeConstants.MaterialSettings.Grime;
using InfiniteForgeConstants.MaterialSettings.Region;
using InfiniteForgeConstants.ObjectSettings;
using InfiniteForgePacker.XML;
using InfiniteForgePacker.XML.Application;
using Microsoft.Win32;

namespace InfiniteReplacer
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
        
        //Object
        public static readonly DependencyProperty ObjectToReplaceProperty 
            = DependencyProperty.Register("ObjectToReplace", typeof(ObjectId), typeof(MainWindow));

        public ObjectId ObjectToReplace
        {
            get{return (ObjectId)GetValue(ObjectToReplaceProperty);}
            set{SetValue(ObjectToReplaceProperty,value);}
        }
        
        public static readonly DependencyProperty ObjectReplacerProperty 
            = DependencyProperty.Register("ObjectReplacer", typeof(ObjectId), typeof(MainWindow));

        public ObjectId ObjectReplacer
        {
            get{return (ObjectId)GetValue(ObjectReplacerProperty);}
            set{SetValue(ObjectReplacerProperty,value);}
        }     
        
        //Material
        public static readonly DependencyProperty MaterialToReplaceProperty 
            = DependencyProperty.Register("MaterialToReplace", typeof(SwatchIdCombination), typeof(MainWindow));
        
        public SwatchIdCombination MaterialToReplace
        {
            get{return (SwatchIdCombination)GetValue(MaterialToReplaceProperty);}
            set{SetValue(MaterialToReplaceProperty,value);}
        }
                
        public static readonly DependencyProperty MaterialReplacerProperty 
            = DependencyProperty.Register("MaterialReplacer", typeof(SwatchIdCombination), typeof(MainWindow));
        
        public SwatchIdCombination MaterialReplacer
        {
            get{return (SwatchIdCombination)GetValue(MaterialReplacerProperty);}
            set{SetValue(MaterialReplacerProperty,value);}
        }     
                
        //Grime
        public static readonly DependencyProperty GrimeToReplaceProperty 
            = DependencyProperty.Register("GrimeToReplace", typeof(GrimeIdCombination), typeof(MainWindow));
                        
        public GrimeIdCombination GrimeToReplace
        {
            get{return (GrimeIdCombination)GetValue(GrimeToReplaceProperty);}
            set{SetValue(GrimeToReplaceProperty,value);}
        }
                                
        public static readonly DependencyProperty GrimeReplacerProperty 
            = DependencyProperty.Register("GrimeReplacer", typeof(GrimeIdCombination), typeof(MainWindow));
                        
        public GrimeIdCombination GrimeReplacer
        {
            get{return (GrimeIdCombination)GetValue(GrimeReplacerProperty);}
            set{SetValue(GrimeReplacerProperty,value);}
        }     
        
        public static readonly DependencyProperty BiomeReplacerProperty 
            = DependencyProperty.Register("BiomeReplacer", typeof(Biome), typeof(MainWindow));
                        
        public Biome BiomeReplacer
        {
            get{return (Biome)GetValue(BiomeReplacerProperty);}
            set{SetValue(BiomeReplacerProperty,value);}
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
                MessageBoxResult messageBoxLoad = System.Windows.MessageBox.Show("Please load a valid XML file",
                    "Continue", System.Windows.MessageBoxButton.OK);
                return;
            }

            Replacer.ConvertType(FilePath, new[] { (ObjectToReplace, ObjectReplacer) });

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Your file has been modified",
                "Continue", System.Windows.MessageBoxButton.OK);
        }

        private void btnReplaceBiomesObjects_Click(object sender, RoutedEventArgs e)
        {
            if (FilePath is "" or "Please load an XML file")
            {
                MessageBoxResult messageBoxLoad = System.Windows.MessageBox.Show("Please load a valid XML file", "Continue", System.Windows.MessageBoxButton.OK);
                return;
            }
            
            List<(ObjectId ObjectToReplace, ObjectId Replacer)> replacers =
                new List<(ObjectId ObjectToReplace, ObjectId Replacer)>();
            
            var toReplaceName = Enum.GetName(typeof(Biome), BiomeReplacer);
            
            if (toReplaceName is null) 
            {
                MessageBoxResult messageBoxLoad = System.Windows.MessageBox.Show("Please enter a valid Biome type", "Continue", System.Windows.MessageBoxButton.OK);
                return;
            }
            
            foreach (var objectIdName in Enum.GetNames(typeof(ObjectId)))
            {
                foreach (var biomeName in Enum.GetNames(typeof(Biome)))
                {
                    if (biomeName == toReplaceName || !objectIdName.Contains(biomeName)) continue;
                    
                    var nameWithNewBiome = objectIdName.Replace(biomeName, toReplaceName);
                    ObjectId replacerId;
                    if (ObjectId.TryParse(nameWithNewBiome, out replacerId))
                    {
                        replacers.Add(((ObjectId)Enum.Parse(typeof(ObjectId), objectIdName), replacerId));
                    }
                }
            }

            Replacer.ConvertType(FilePath, replacers.ToArray());
            
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
            
            //.ConvertType(FilePath, replacers.ToArray());

            var xml = XDocument.Load(FilePath);
            XMLHelper.GetFolderStruct(xml)?.Remove();
            xml.Save(FilePath);
            
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Your file has been modified", "Continue", System.Windows.MessageBoxButton.OK);
        }
        
        private void btnReplaceMaterialObject_Click(object sender, RoutedEventArgs e)
        {
            if (FilePath is "" or "Please load an XML file")
            {
                MessageBoxResult messageBoxLoad = System.Windows.MessageBox.Show("Please load a valid XML file", "Continue", System.Windows.MessageBoxButton.OK);
                return;
            }

            Replacer.ConvertMaterial(FilePath, new []{ ((int)MaterialToReplace, (int)MaterialReplacer)});
            
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Your file has been modified", "Continue", System.Windows.MessageBoxButton.OK);
        }
        
        private void btnReplaceGrimeObject_Click(object sender, RoutedEventArgs e)
        {
            if (FilePath is "" or "Please load an XML file")
            {
                MessageBoxResult messageBoxLoad = System.Windows.MessageBox.Show("Please load a valid XML file", "Continue", System.Windows.MessageBoxButton.OK);
                return;
            }

            Replacer.ConvertGrime(FilePath, new []{ ((int)GrimeToReplace, (int)GrimeReplacer)});
            
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Your file has been modified", "Continue", System.Windows.MessageBoxButton.OK);
        }
    }
}