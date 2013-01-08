using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using TreeContainer;
using System.Windows;

namespace by_Deliany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] Headers;
        private string Result;
        DataGrid dataGrid = new DataGrid();
        private DecisionNode tree;

        private void AddDataGrid()
        {
            dataGrid.Width = groupBoxDecisionTree.Width;
            dataGrid.Height = groupBoxDecisionTree.Height;
            dataGrid.VerticalAlignment = groupBoxDecisionTree.VerticalAlignment;
            dataGrid.HorizontalAlignment = groupBoxDecisionTree.HorizontalAlignment;
            dataGrid.Margin = groupBoxDecisionTree.Margin;
            dataGrid.Visibility = Visibility.Hidden;
            dataGrid.FontFamily = new FontFamily("Verdana");
            dataGrid.FontSize = 16;
            dataGrid.AutoGenerateColumns = true;
            dataGrid.IsReadOnly = true;
            mainGrid.Children.Add(dataGrid);
        }
        public MainWindow()
        {
            InitializeComponent();
            AddDataGrid();
            listBoxZoo.Visibility = Visibility.Hidden;

            //simple_example();
            zoo_example();
        }

        class Animal
        {
            public string AnimalName { get; set; }
            public string Hair { get; set; }
            public string Feathers { get; set; }
            public string Eggs { get; set; }
            public string Milk { get; set; }
            public string Airbourne { get; set; }
            public string Aquatic { get; set; }
            public string Predator { get; set; }
            public string Toothed { get; set; }
            public string Backbone { get; set; }
            public string Breathes { get; set; }
            public string Venomous { get; set; }
            public string Fins { get; set; }
            public int Legs { get; set; }
            public string Tail { get; set; }
            public string Domestic { get; set; }
            public string Catsize { get; set; }
            public int Type { get; set; }
        }
        class ClassifyAnimal
        {
            public string Hair { get; set; }
            public string Feathers { get; set; }
            public string Eggs { get; set; }
            public string Milk { get; set; }
            public string Airbourne { get; set; }
            public string Aquatic { get; set; }
            public string Predator { get; set; }
            public string Toothed { get; set; }
            public string Backbone { get; set; }
            public string Breathes { get; set; }
            public string Venomous { get; set; }
            public string Fins { get; set; }
            public int Legs { get; set; }
            public string Tail { get; set; }
            public string Domestic { get; set; }
            public string Catsize { get; set; }
        }
        class SimpleExample
        {
            public string Refferer { get; set; }
            public string Location { get; set; }
            public string ReadFAQ { get; set; }
            public int PageViewed { get; set; }
            public string ServiceChosen { get; set; }
        }
        class ClassifySimpleExample
        {
            public string Refferer { get; set; }
            public string Location { get; set; }
            public string ReadFAQ { get; set; }
            public int PageViewed { get; set; }
        }

        private void zoo_example()
        {
            string path = @"zoo.txt";
            Headers = new string[]{
                                        "Hair", "Feathers", "Eggs", "Milk", "Airborne", "Aquatic",
                                        "Predator", "Toothed", "Backbone", "Breathes", "Venomous", "Fins", "Legs", "Tail",
                                        "Domestic", "Catsize"
                                    };
            Result = "Animal type";

            List<string[]> rows = new List<string[]>();
            List<string[]> dummy = new List<string[]>();
            foreach (var animalData in File.ReadAllLines(path))
            {
                List<string> data = new List<string>();
                string[] line = animalData.Split(',');
                for (int i = 0; i < line.Length; ++i)
                {
                    if(i==13 || i==17 || i==0)
                    {
                        data.Add(line[i]);
                    }
                    else
                    {
                        data.Add(BoolParser.Parse(line[i]).ToString());
                    }
                }
                dummy.Add(data.ToArray());
                data.RemoveAt(0);
                rows.Add(data.ToArray());
            }
            UpdateDataBaseZoo(dummy);
            tcMain.Clear();
            tree = DecisionTreeTools.BuildTree(rows);
            DecisionTreeTools.Prune(tree.NextFalseNode.NextTrueNode.NextFalseNode.NextTrueNode,1.01);
            DecisionTreeTools.Prune(tree.NextFalseNode.NextTrueNode.NextTrueNode.NextFalseNode,1.01);
            PrintTree(tree);
            

            listBoxZoo.Items.Clear();
            listBoxZoo.Items.Add(
                "1 -- (41) aardvark, antelope, bear, boar, buffalo, calf, cavy, cheetah, deer, dolphin, elephant, fruitbat, giraffe, girl, goat, gorilla, hamster, hare, leopard, lion, lynx, mink, mole, mongoose, opossum, oryx, platypus, polecat, pony, porpoise, puma, pussycat, raccoon, reindeer, seal, sealion, squirrel, vampire, vole, wallaby,wolf ");
            listBoxZoo.Items.Add(
                "2 -- (20) chicken, crow, dove, duck, flamingo, gull, hawk, kiwi, lark, ostrich, parakeet, penguin, pheasant, rhea, skimmer, skua, sparrow, swan, vulture, wren ");
            listBoxZoo.Items.Add("3 -- (5) pitviper, seasnake, slowworm, tortoise, tuatara");
            listBoxZoo.Items.Add(
                "4 -- (13) bass, carp, catfish, chub, dogfish, haddock, herring, pike, piranha, seahorse, sole, stingray, tuna ");
            listBoxZoo.Items.Add("5 -- (4) frog, frog, newt, toad ");
            listBoxZoo.Items.Add("6 -- (8) flea, gnat, honeybee, housefly, ladybird, moth, termite, wasp ");
            listBoxZoo.Items.Add(
                "7 -- (10) clam, crab, crayfish, lobster, octopus, scorpion, seawasp, slug, starfish, worm");
            
            listBoxZoo.Visibility = Visibility.Visible;
        }
        private void simple_example()
        {
            string path = @"decision_tree_example.txt";
            Headers = new string[]{"Refferer", "Location", "Read FAQ", "Pages viewed"};
            Result = "Service";

            List<string[]> rows = new List<string[]>();
            foreach (var word in File.ReadAllLines(path))
            {
                rows.Add(word.Split('\t'));
            }

            tcMain.Clear();
            tree = DecisionTreeTools.BuildTree(rows);
            PrintTree(tree);
            UpdateDataBaseSimpleExample(rows);

            listBoxZoo.Visibility = Visibility.Hidden;
        }

        private void UpdateDataBaseZoo(List<string[]> rows)
        {
            ObservableCollection<Animal> collection = new ObservableCollection<Animal>();
            foreach (var row in rows)
            {
                collection.Add(new Animal
                                   {
                                       AnimalName = row[0],
                                       Hair = BoolParser.Parse(row[1])?"+":"-",
                                       Feathers = BoolParser.Parse(row[2]) ? "+" : "-",
                                       Eggs = BoolParser.Parse(row[3]) ? "+" : "-",
                                       Milk = BoolParser.Parse(row[4]) ? "+" : "-",
                                       Airbourne = BoolParser.Parse(row[5]) ? "+" : "-",
                                       Aquatic = BoolParser.Parse(row[6]) ? "+" : "-",
                                       Predator = BoolParser.Parse(row[7]) ? "+" : "-",
                                       Toothed = BoolParser.Parse(row[8]) ? "+" : "-",
                                       Backbone = BoolParser.Parse(row[9]) ? "+" : "-",
                                       Breathes = BoolParser.Parse(row[10]) ? "+" : "-",
                                       Venomous = BoolParser.Parse(row[11]) ? "+" : "-",
                                       Fins = BoolParser.Parse(row[12]) ? "+" : "-",
                                       Legs = int.Parse(row[13]),
                                       Tail = BoolParser.Parse(row[14]) ? "+" : "-",
                                       Domestic = BoolParser.Parse(row[15]) ? "+" : "-",
                                       Catsize = BoolParser.Parse(row[16]) ? "+" : "-",
                                       Type = int.Parse(row[17])
                                   });
            }
            ObservableCollection<ClassifyAnimal> classify = new ObservableCollection<ClassifyAnimal>();
            classify.Add(new ClassifyAnimal());
            dataGridClassify.ItemsSource = classify;
            dataGrid.ItemsSource = collection;
        }
        private void UpdateDataBaseSimpleExample(List<string[]> rows)
        {
            ObservableCollection<SimpleExample> collection = new ObservableCollection<SimpleExample>();
            foreach (var row in rows)
            {
                collection.Add(new SimpleExample
                {
                    Refferer = row[0],
                    Location = row[1],
                    ReadFAQ = row[2],
                    PageViewed = int.Parse(row[3]),
                    ServiceChosen = row[4]
                });
            }
            ObservableCollection<ClassifySimpleExample> classify = new ObservableCollection<ClassifySimpleExample>();
            classify.Add(new ClassifySimpleExample());
            dataGridClassify.ItemsSource = classify;
            dataGrid.ItemsSource = collection;
        }
        
        private void PrintTree(DecisionNode tree, TreeNode tnControl = null)
        {

            TreeNode tnSubtreeRoot;

            Label label = new Label();
            label.BorderBrush = Brushes.White;
            label.BorderThickness = new Thickness(0.5);
            label.Foreground = Brushes.Orange;


            // Is this a leaf node?
            if (tree.Results != null)
            {
                StringBuilder str = new StringBuilder();
                foreach (var res in tree.Results)
                {
                   str.Append("{"+Result+": '" + res.Key + "'}");
                }
                label.Content = str.ToString();
                label.Foreground = Brushes.YellowGreen;
                if(tnControl == null)
                {
                    tnSubtreeRoot = tcMain.AddRoot(label);
                }
                else
                {
                    tnSubtreeRoot = tcMain.AddNode(label, tnControl);
                }
            }
            else
            {
                string name = Headers[tree.Column] + " : " + tree.Value + " ?";
                label.Content = name;
                if(tnControl == null)
                {
                    tnSubtreeRoot = tcMain.AddRoot(label);
                }
                else
                {
                    tnSubtreeRoot = tcMain.AddNode(label, tnControl);
                }

                PrintTree(tree.NextFalseNode, tnSubtreeRoot);
                PrintTree(tree.NextTrueNode, tnSubtreeRoot);

            }
        }

        private bool mode = true;
        private void checkBoxDataBase_Click(object sender, RoutedEventArgs e)
        {
            groupBoxDecisionTree.Visibility = mode ? Visibility.Hidden : Visibility.Visible;
            
            dataGrid.Visibility = !mode ? Visibility.Hidden : Visibility.Visible;
            
            mode = !mode;
        }
        
        /// <summary>
        /// Parse strings into true or false bools using relaxed parsing rules
        /// </summary>
        public static class BoolParser
        {
            /// <summary>
            /// Get the boolean value for this string
            /// </summary>
            public static bool Parse(string value)
            {
                return IsTrue(value);
            }

            /// <summary>
            /// Determine whether the string is not True
            /// </summary>
            public static bool IsFalse(string value)
            {
                return !IsTrue(value);
            }

            /// <summary>
            /// Determine whether the string is equal to True
            /// </summary>
            public static bool IsTrue(string value)
            {
                try
                {
                    // 1
                    // Avoid exceptions
                    if (value == null)
                    {
                        return false;
                    }

                    // 2
                    // Remove whitespace from string
                    value = value.Trim();

                    // 3
                    // Lowercase the string
                    value = value.ToLower();

                    // 4
                    // Check for word true
                    if (value == "true")
                    {
                        return true;
                    }

                    // 5
                    // Check for letter true
                    if (value == "t")
                    {
                        return true;
                    }

                    // 6
                    // Check for one
                    if (value == "1")
                    {
                        return true;
                    }

                    // 7
                    // Check for word yes
                    if (value == "yes")
                    {
                        return true;
                    }

                    // 8
                    // Check for letter yes
                    if (value == "y")
                    {
                        return true;
                    }

                    // 9
                    // It is false
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void buttonSimpleExample_Click(object sender, RoutedEventArgs e)
        {
            simple_example();
        }

        private void buttonZooExample_Click(object sender, RoutedEventArgs e)
        {
            zoo_example();
        }

        private void dataGridClassify_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var selectedRow = dataGridClassify.GetSelectedRow();
                List<string> data = new List<string>();
               // StringBuilder str = new StringBuilder();
                for(int i = 0; i< dataGridClassify.Columns.Count; ++i)
                {
                    TextBlock textBlock = dataGridClassify.GetCell(selectedRow, i).Content as TextBlock;
                    
                    if(textBlock != null)
                    {
                        //str.Append((textBlock.Text == string.Empty?"null":textBlock.Text)+"\n");
                        data.Add(textBlock.Text == string.Empty ? null : (textBlock.Text=="+")?"True":"False");
                    }
                    else
                    {
                        CheckBox checkBox = dataGridClassify.GetCell(selectedRow, i).Content as CheckBox;
                        if(checkBox!= null)
                        {
                            data.Add(checkBox.IsChecked.ToString());
                            //str.Append(checkBox.IsChecked.ToString()+"\n");
                        }
                        else
                        {
                            var x = dataGridClassify.GetCell(selectedRow, i).Content as TextBox;
                            data.Add(x.Text);
                        }
                    }
                }
                var classification = DecisionTreeTools.MissingDataClassify(data.ToArray(), tree);
                StringBuilder classificationData = new StringBuilder();
                foreach (var c in classification)
                {
                    classificationData.Append("{"+Result+" '" + c.Key + "' : " + c.Value + "}");
                }

                MessageBox.Show(classificationData.ToString());
            }
        }
    }
}
