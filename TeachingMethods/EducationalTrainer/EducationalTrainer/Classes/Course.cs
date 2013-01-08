using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;

namespace EducationalTrainer.Classes
{
    public class CourseTreeViewItem
    {
        public ObservableCollection<object> Items { get; set; }

        public string ImageUrl { get; set; }

        public string Title { get; set; }
    }
    [Serializable]
    [XmlRoot("Course")]
    public class Course
    {
        [XmlArray]
        public List<Test> Tests { get; set; }

        [XmlArray]
        public List<Theory> Theories { get; set; }
    }

    [Serializable]
    public class Test
    {
        [XmlElement]
        public string Title { get; set; }
        [XmlElement]
        public string Url { get; set; }
        [XmlElement]
        public int Points { get; set; }
        [XmlElement]
        public List<string> Choice { get; set; }
        [XmlAttribute]
        public int CorrectAnswer { get; set; }

        public bool Visited { get; set; }

        private List<RadioButton> _radioButtons;

        [XmlIgnore]
        public List<RadioButton> RadioButtons
        {
            get
            {
                if (_radioButtons == null)
                {
                    _radioButtons = new List<RadioButton>();
                    foreach (var choice in Choice)
                    {
                        RadioButton radioButton = new RadioButton
                                                      {
                                                          Content = choice,
                                                          Margin = new Thickness(0, 20, 0, 0),
                                                          FontSize = 16,
                                                          FontWeight = FontWeights.Bold
                                                      };
                        _radioButtons.Add(radioButton);
                    }
                }

                return _radioButtons;
            }
        }

        [XmlIgnore]
        public int ResultPoints
        {
            get
            {
                int givenAnswer = RadioButtons.IndexOf(RadioButtons.Find(rd => rd.IsChecked == true)) + 1;
                if (givenAnswer == CorrectAnswer)
                {
                    return Points;
                }
                else
                {
                    return 0;
                }
            }
        }

        [XmlIgnore]
        public string GivenAnswer
        {
            get
            {
                if (RadioButtons.Find(rd => rd.IsChecked == true) != null)
                {
                    string givenAnswer = RadioButtons.Find(rd => rd.IsChecked == true).Content.ToString();
                    return givenAnswer;
                }
                else
                {
                    return "Відсутня відповідь";
                }
            }
        }

        [XmlIgnore]
        public string CorrectStringAnswer
        {
            get { return RadioButtons[CorrectAnswer - 1].Content.ToString(); }
        }

        public Test()
        {
            Choice = new List<string>();
        }

        public Test(string title, string url, int points, List<string> choice, int correctAnswer)
        {
            Title = title;
            Url = url;
            Points = points;
            Choice = choice;
            CorrectAnswer = correctAnswer;
        }



        public string ImageUrl
        {
            get
            {
                return "../Images/paper_bag.jpg";
            }
        }
    }

    [Serializable]
    public class Theory
    {
        [XmlElement]
        public string Title { get; set; }
        [XmlElement]
        public string Url { get; set; }

        public bool Visited { get; set; }

        public Theory()
        {
            
        }

        public Theory(string title, string url)
        {
            Title = title;
            Url = url;
        }

        public string ImageUrl
        {
            get
            {
                return "../Images/book_open.jpg";
            }
        }
    }
}
