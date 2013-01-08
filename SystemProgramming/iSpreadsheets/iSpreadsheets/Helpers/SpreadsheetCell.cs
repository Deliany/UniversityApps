using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace iSpreadsheets.Helpers
{
    /// <summary>
    /// Class to represent spreadsheet cell
    /// </summary>
    [Serializable]
    public class SpreadsheetCell : INotifyPropertyChanged
    {
        protected string _content;

        /// <summary>
        /// Gets or sets cell content
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        /// <summary>
        /// Parent DataTable of cell
        /// </summary>
        [XmlIgnore]
        public DataTable Table { get; set; }
        
        /// <summary>
        /// Gets or sets arbitrary object value that can be used to store custom information
        /// </summary>
        public SpreadsheetCellCustomInfo Tag { get; set; }


        public SpreadsheetCell() { }

        /// <summary>
        /// Initializes new instance of SpreadsheetCell class using specified content and custom information
        /// </summary>
        /// <param name="content">String used to initialize the value of cell content</param>
        /// <param name="table">Parent DataTable of cell</param>
        /// <param name="tag">Object value used to initialize cell custom information</param>
        public SpreadsheetCell(string content, DataTable table, SpreadsheetCellCustomInfo tag)
        {
            this.Table = table;
            this.Tag = tag;
            this.Content = content ?? string.Empty;
        }

        public void UpdateAllConsequentialCells()
        {
            foreach (var consequentialCell in this.Tag.ConsequentialCells)
            {
                var cell = this.Table.GetSpreadsheetCell(consequentialCell.Row, consequentialCell.Col);
                if (cell != null)
                {
                    Logger.WriteLogInfo(string.Format("Updating consequential cell [{0}], old content \"{1}\"", consequentialCell.FullName, cell.Content));
                    cell.Tag.Calculate(cell.Tag.Formula, this.Table);
                    cell.Content = cell.Tag.Value;
                    Logger.WriteLogInfo(string.Format("Updated consequential cell [{0}], new content \"{1}\"", consequentialCell.FullName, cell.Content));

                    cell.UpdateAllConsequentialCells();
                }
            }
        }

        public void UpdateAllDependentCells()
        {
            foreach (var dependentCell in this.Tag.DependentCells)
            {
                var cell = this.Table.GetSpreadsheetCell(dependentCell.Row, dependentCell.Col);
                if (cell != null)
                {
                    Logger.WriteLogInfo(string.Format("Updating dependent cell [{0}]", dependentCell.FullName));
                    if (cell.Tag.ConsequentialCells.Contains(this.Tag.CellName))
                    {
                        cell.Tag.ConsequentialCells.Remove(this.Tag.CellName);
                        Logger.WriteLogInfo(string.Format("Removed cell [{0}] from consequential list of cell [{1}] ",
                                                          dependentCell.FullName, this.Tag.CellName.FullName));
                    }
                }
            }
            this.Tag.DependentCells.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
