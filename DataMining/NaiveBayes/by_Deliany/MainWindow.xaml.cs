using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using IMDb_Scraper;
using Bayes;
using TheMovieDb;

namespace by_Deliany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NaiveBayes classifier = new NaiveBayes();
        public MainWindow()
        {
            InitializeComponent();
            //IMDB_search(textBoxMovieName.Text);
            //TmdbApi api = new TmdbApi("dfab259ff7f22ce9a3f5db118b55743f");
            //var movies = api.MovieBrowse(TmdbOrderBy.Title, TmdbOrder.Desc, page: 1, perPage: 10);

            Deserialize();
            showlog();

        }

        public void Serialize()
        {
            try
            {
                if(File.Exists("db.xml"))
                {
                    File.Delete("db.xml");
                }
                using (Stream textWriter = File.Open("db.xml", FileMode.OpenOrCreate))
                {
                    XmlSerializer serializer = new XmlSerializer(classifier.GetType());
                    serializer.Serialize(textWriter, classifier);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Deserialize()
        {
            try
            {
                File.Copy("db.xml", "db_backup.xml", true);
                using (Stream textReader = File.Open("db.xml", FileMode.Open))
                {
                    XmlSerializer deserializer = new XmlSerializer(classifier.GetType());
                    classifier = (NaiveBayes)deserializer.Deserialize(textReader);
                    textReader.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public class Record
        {
            public string Word { get; set; }
            public string Category { get; set; }
            public int Count { get; set; }
        }
        public class CategoryRecord
        {
            public string Category { get; set; }
        }
        private void showlog()
        {
            try
            {
                var database = classifier.FeatureCategoryCombinations;

                var query = database.OrderBy(d => d.Key.Key).ThenBy(d => d.Key.Value);

                ObservableCollection<Record> db_records = new ObservableCollection<Record>();
                ObservableCollection<CategoryRecord> db_categories = new ObservableCollection<CategoryRecord>();
                foreach (var item in query)
                {
                    db_records.Add(new Record { Word = item.Key.Key, Category = item.Key.Value, Count = item.Value });
                }
                foreach (var category in classifier.Categories())
                {
                    db_categories.Add(new CategoryRecord {Category = category});
                }
                dataGridDataBase.ItemsSource = db_records;
                dataGridCategories.ItemsSource = db_categories;
                labelDataBaseCount.Content = "Кількість записів в базі: " + db_records.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void classifyButton_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show(string.Format("Classified as '{0}'", classifier.Classify(textBoxDescription.Text)));
        }

        private List<string> parseGenres()
        {
            try
            {
                List<string> genres = new List<string>();
                Regex re = new Regex(@"[\w'-]+", RegexOptions.Compiled);

                Match m = re.Match(textBoxGenre.Text);
                while (m.Success)
                {
                    genres.Add(m.Groups[0].Value);
                    m = m.NextMatch();
                }
                return genres;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<string>();
            }

        }

        private void buttonTrain_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxDescription.Text.Length > 0 && textBoxGenre.Text.Length > 0)
                {
                    List<string> genresArray = parseGenres();
                    if(genresArray.Count == 0)
                    {
                        throw new Exception("Genres not found!");
                    }
                    StringBuilder genres = new StringBuilder();
                    foreach (var genre in genresArray)
                    {
                        genres.Append("'" + genre + "'");
                        if (genre != genresArray.Last())
                        {
                            genres.Append("; ");
                        }
                        classifier.Train(textBoxDescription.Text, genre);
                    }
                    showlog();
                    MessageBox.Show("Train successfull!\n" + "Trained genres: " + genres);

                    textBoxGenre.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void buttonSerialize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Serialize();
                MessageBox.Show("Serialized!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void IMDB_search(string movieName)
        {
            IMDb imdb = new IMDb(movieName);

            StringBuilder description = new StringBuilder();
            description.Append(imdb.Title + "\n");

            description.Append(imdb.Plot + "\n");


            description.Append(imdb.Storyline);

            textBoxDescription.Text = description.ToString();

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < imdb.Genres.Count; ++i)
            {
                str.Append(imdb.Genres[i]);
                if (i != imdb.Genres.Count - 1)
                {
                    str.Append(", ");
                }
            }
            textBoxGenre.Text = str.ToString();
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxMovieName.Text.Length > 0)
                {
                    IMDB_search(textBoxMovieName.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        private void textBoxDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(textBoxDescription.Text.Length > 0)
            {
                labelPossibleGenre.Content = "Можливий жанр: " + classifier.Classify(textBoxDescription.Text);
            }
            else
            {
                labelPossibleGenre.Content = "Можливий жанр: ";
            }
        }

        private void dataGridDataBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedRow = dataGridDataBase.GetSelectedRow();
                var word = (dataGridDataBase.GetCell(selectedRow, 0).Content as TextBlock).Text;
                var category = (dataGridDataBase.GetCell(selectedRow, 1).Content as TextBlock).Text;

                textBoxWord.Text = word;
                textBoxCategory.Text = category;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            
        }

        private void dataGridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedRow = dataGridCategories.GetSelectedRow();
                var category = (dataGridCategories.GetCell(selectedRow, 0).Content as TextBlock).Text;

                textBoxCategory.Text = category;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(textBoxWord.Text.Length > 0 && textBoxCategory.Text.Length > 0)
                {
                    if(classifier.DeleteFeatureCategoryCombination(textBoxWord.Text,textBoxCategory.Text))
                    {
                        MessageBox.Show("Successfull removed from data base!");
                        showlog();
                    }
                    else
                    {
                        MessageBox.Show("Such combination don't exist!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
