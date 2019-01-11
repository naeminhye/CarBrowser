using HelixToolkit.Wpf;
using Microsoft.Win32;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_21122018
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentModel = "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Models\\Dragon.stl";
        private Material currentMaterial = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromRgb(205, 25, 1)));
        private List<UploadedItem> currentUploadedList = null;

        private List<string> TEXTURE_LIST = new List<string>()
        {
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-001.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-002.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-003.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-004.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-005.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-006.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-007.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Textures\\texture-008.jpg"
        };

        private List<string> MODEL_LIST = new List<string>()
        {
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Models\\cone.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Models\\cube.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Models\\cylinder.jpg",
            "C:\\Users\\Hieu Nguyen\\documents\\visual studio 2015\\Projects\\Project_21122018\\Project_21122018\\Models\\sphere.jpg"
        };

        public MainWindow()

        {
            InitializeComponent();

            viewPort3d.Children.Add(new ModelVisual3D { Content = display3D(currentModel, currentMaterial) });
            createListOfImageButton(TEXTURE_LIST, 10);
            createListOfImageButton(MODEL_LIST, 300);

        }

        private Image createImageButtonWithStyle(double leftMargin, string sourcePath)
        {
            Image imageBtn = new Image();
            imageBtn.Height = 31;
            imageBtn.Width = 31;
            imageBtn.Margin = new Thickness(leftMargin, 10, 0, 0);
            imageBtn.HorizontalAlignment = HorizontalAlignment.Left;
            imageBtn.VerticalAlignment = VerticalAlignment.Top;
            imageBtn.Stretch = Stretch.Fill;
            imageBtn.Source = new BitmapImage(new Uri(sourcePath, UriKind.Absolute));

            Style effect = createImageEffect();
            imageBtn.Style = effect;

            return imageBtn;
        }

        private Style createImageEffect()
        {
            var effect = new DropShadowEffect()
            {
                ShadowDepth = 0,
                Color = Color.FromRgb(0, 51, 102),
                Opacity = 1,
                BlurRadius = 1
            };

            var setter = new Setter()
            {
                Property = UIElement.EffectProperty,
                Value = effect
            };

            var trigger = new Trigger()
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true,
                Setters = { setter }
            };

            var style = new Style()
            {
                Triggers = { trigger }
            };

            return style;
        }

        private void createListOfImageButton(List<string> pathList, double startPosition)
        {
            foreach (string path in pathList)
            {
                Image imageBtn = createImageButtonWithStyle(startPosition, path);
                imageBtn.Name = "btn_" + startPosition;
                grid.Children.Add(imageBtn);
                startPosition += 31;

                imageBtn.MouseDown += (object sender, MouseButtonEventArgs e) =>
                {
                    Material material = this.currentMaterial;
                    if (Int32.Parse(imageBtn.Name.Split('_')[1]) >= 300)
                    {
                        this.currentModel = path.Split('.')[0] + ".stl";
                    }
                    else
                    {
                        material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(path))));
                    }

                    refreshViewport(this.currentModel, material);
                    this.currentMaterial = material;
                };
            }
        }

        private Model3D display3D(string path, Material material)
        {
            Model3D model = null;

            try
            {
                // add rotate gesture
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                // import 3D model
                ModelImporter import = new ModelImporter() { DefaultMaterial = material };
                model = import.Load(path);
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Error: " + e.StackTrace);
            }

            return model;
        }

        private void refreshViewport(string path, Material material)
        {
            viewPort3d.Children.Clear();

            viewPort3d.Children.Add(new SunLight());

            viewPort3d.Children.Add(new ModelVisual3D { Content = display3D(path, material) });
        }

        private void btnTexture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.DefaultExt = ".jpg";
            dialog.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                this.currentMaterial = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(dialog.FileName))));

                refreshViewport(this.currentModel, this.currentMaterial);
                addToRecentUploaded(dialog);
            }

        }

        private void btnModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.DefaultExt = ".stl";
            dialog.Filter = "STL Files (*.stl)|*.stl";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                this.currentModel = dialog.FileName;

                refreshViewport(this.currentModel, this.currentMaterial);
                addToRecentUploaded(dialog);
            }
        }

        private void addToRecentUploaded(OpenFileDialog file)
        {
            string path = file.FileName;

            if (file.DefaultExt == "stl")
            {
                path = file.FileName.Split('.')[0] + ".jpg";
            }

            this.listView.Items.Insert(0, new UploadedItem() { thumbnailPath = path, fileName = file.SafeFileName, fullPath = file.FileName });
            this.currentUploadedList = this.listView.Items.Cast<UploadedItem>().ToList();
        }

        private void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;

            if(item != null && item.IsSelected)
            {
                var content = item.Content as UploadedItem;
                if (content.fileName.Split('.')[1] == "stl")
                {
                    this.currentModel = content.fullPath;
                    refreshViewport(this.currentModel, this.currentMaterial);
                }
                else
                {
                    this.currentMaterial = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(content.fullPath))));
                    refreshViewport(this.currentModel, this.currentMaterial);
                }

            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox).Text;

            this.listView.Items.Clear();
            if (this.currentUploadedList != null)
            {
                foreach (var item in this.currentUploadedList)
                {
                    if (searchText == "" || searchText == null)
                    {
                        listView.Items.Add(item);
                    }
                    else
                    {
                        if (item.fileName.Contains(searchText))
                        {
                            listView.Items.Add(item);
                        }
                    }
                }
            }
        }
    }
}
