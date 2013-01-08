using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace by_Deliany
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int OlympiadID { get; set; }
        private string connStr = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=SchoolOlympiads;Integrated Security=SSPI";

        public MainWindow()
        {
            InitializeComponent();

            OlympiadID = 1;
            SchoolNumber = 18;
            UpdateOlympiads();
            UpdateOlympiadID();
            UpdateMainTab();
        }

        #region UpdateOlympiads
        //-------------------------------------------------

        private void UpdateOlympiads()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = "SELECT * FROM [Olympiads]";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);
                DataTable dataTable = new DataTable();

                sqlDataAdapter.Fill(dataTable);
                dataGridOlympiads.ItemsSource = dataTable.DefaultView;
            }
        }

        private DataGridRow previousRow;
        private void UpdateOlympiadID()
        {
            var selectedRow = dataGridOlympiads.GetSelectedRow();

            if (previousRow != selectedRow)
            {
                previousRow = selectedRow;
            }
            else
            {
                return;
            }

            if (selectedRow == null)
            {
                //throw new Exception("Please, select proper Olympiad");
                return;
            }
            var columnCell = dataGridOlympiads.GetCell(selectedRow, 0).Content;

            int olyID;
            int.TryParse((columnCell as TextBlock).Text,out olyID);
            OlympiadID = olyID;
            GenerateOlympiadResults();
        }

        private void dataGridOlympiads_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                int oldOlyID = OlympiadID;

                UpdateOlympiadID();
                if (oldOlyID == OlympiadID)
                {
                    return;
                }

                if (tabItemMain.IsSelected)
                {
                    UpdateMainTab();
                }
                else if(tabItemOlympiads.IsSelected)
                {
                    UpdateOlympiadsTab();
                }
                else if (tabItemOlympiads2.IsSelected)
                {
                    UpdateOlympiadsTab2();
                }
                else if(tabItemQuery.IsSelected)
                {
                    UpdateQueries();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------
        #endregion

        #region Main Tab
        //---------------------------------------------------

        private void GenerateOlympiadResults()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "EXECUTE GenerateReport @olympID=" + OlympiadID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void ShowParticipants()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery ="SELECT * FROM [OlympiadSummary]";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridOlympiadSummary.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ShowWinners()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();

                string selectQuery = "SELECT * FROM [Winners]";
                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridWinners.ItemsSource = dataTable.DefaultView;
            }
        }

        private void UpdateMainTab()
        {
            GenerateOlympiadResults();
            ShowParticipants();
            ShowWinners();
        }

        //---------------------------------------------------
        #endregion

        #region OlympiadsTab
        //--------------------------------------------------------

        #region Prepare input
        //--------------------------------------------------------

        private void PrepareAuditoryInput()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("AuditoryNumber", typeof(int)));
            dataTable.Columns.Add(new DataColumn("NumberOfComputers", typeof(int)));
            dataTable.Columns.Add(new DataColumn("ComputersType", typeof(string)));
            dataTable.Rows.Add(dataTable.NewRow());

            dataGridAddAuditory.CanUserAddRows = false;
            dataGridAddAuditory.ItemsSource = dataTable.DefaultView;
        }

        private void PrepareOlympiadInput()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("Subject", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Grade", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Date", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("AuditoryNumber", typeof(int)));
            dataTable.Rows.Add(dataTable.NewRow());

            dataGridAddOlympiad.CanUserAddRows = false;
            dataGridAddOlympiad.ItemsSource = dataTable.DefaultView;
        }

        private void PrepareTaskInput()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("OlympiadID", typeof(int)));
            dataTable.Columns.Add(new DataColumn("TaskNumber", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Condition", typeof(string)));
            dataTable.Columns.Add(new DataColumn("MaximalScore", typeof(double)));
            var row = dataTable.NewRow();
            row["OlympiadID"] = OlympiadID;
            dataTable.Rows.Add(row);

            dataGridAddTask.CanUserAddRows = false;
            dataGridAddTask.ItemsSource = dataTable.DefaultView;
        }

        private void PrepareExaminerInput()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("ExaminerName", typeof(string)));
            dataTable.Columns.Add(new DataColumn("ExaminerSurname", typeof(string)));
            dataTable.Columns.Add(new DataColumn("ExaminerSecondName", typeof(string)));
            dataTable.Columns.Add(new DataColumn("BirthDate", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("Position", typeof(string)));
            var row = dataTable.NewRow();
            dataTable.Rows.Add(row);

            dataGridAddExaminer.CanUserAddRows = false;
            dataGridAddExaminer.ItemsSource = dataTable.DefaultView;
        }

        //----------------------------------------------------------
        #endregion

        #region Show different tables
        //---------------------------------------------------------

        private void ShowAuditories()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = "SELECT * FROM [Auditories]";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridAuditories.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ShowTasks()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = "SELECT * FROM [Tasks] WHERE OlympiadID = " + OlympiadID;

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridTasks.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ShowExaminers()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = @"SELECT *
                                        FROM [Examiners]";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridExaminers.ItemsSource = dataTable.DefaultView;
            }
        }

        //---------------------------------------------------------
        #endregion

        #region Add record
        //--------------------------------------------------------

        private void AddAuditory()
        {
            var row = dataGridAddAuditory.GetRow(0);
            var cell = dataGridAddAuditory.GetCell(row, 0).Content;

            int auditoryNum;
            int.TryParse((cell as TextBlock).Text, out auditoryNum);

            cell = dataGridAddAuditory.GetCell(row, 1).Content;
            int compNum;
            int.TryParse((cell as TextBlock).Text, out compNum);

            cell = dataGridAddAuditory.GetCell(row, 2).Content;
            string compDesc = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery = "INSERT INTO [Auditories] (AuditoryNumber,NumberOfComputers,ComputersType) " +
                                     "VALUES (" + auditoryNum + "," + compNum + ",'" + compDesc + "')";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void AddOlympiad()
        {
            var row = dataGridAddOlympiad.GetRow(0);
            var cell = dataGridAddOlympiad.GetCell(row, 0).Content;

            string subject = (cell as TextBlock).Text;

            cell = dataGridAddOlympiad.GetCell(row, 1).Content;
            int grade;
            int.TryParse((cell as TextBlock).Text, out grade);

            cell = dataGridAddOlympiad.GetCell(row, 2).Content;
            string date = (cell as TextBlock).Text;

            cell = dataGridAddOlympiad.GetCell(row, 3).Content;
            int auditoryNum;
            int.TryParse((cell as TextBlock).Text, out auditoryNum);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery = "INSERT INTO [Olympiads] (Subject,Grade,Date,AuditoryNumber) " +
                                     "VALUES ('" + subject + "'," + grade + ",'" + date + "'," + auditoryNum + ")";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void AddTask()
        {
            var row = dataGridAddTask.GetRow(0);
            var cell = dataGridAddTask.GetCell(row, 0).Content;

            int olyID;
            int.TryParse((cell as TextBlock).Text, out olyID);

            cell = dataGridAddTask.GetCell(row, 1).Content;
            int taskNumber;
            int.TryParse((cell as TextBlock).Text, out taskNumber);

            cell = dataGridAddTask.GetCell(row, 2).Content;
            string condition = (cell as TextBlock).Text;

            cell = dataGridAddTask.GetCell(row, 3).Content;
            double maximalScore;
            double.TryParse((cell as TextBlock).Text, out maximalScore);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery = "INSERT INTO [Tasks] (OlympiadID,TaskNumber,Condition,MaximalScore) " +
                                     "VALUES (" + olyID + "," + taskNumber + ",'" + condition + "'," + maximalScore + ")";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void AddExaminer()
        {
            var row = dataGridAddExaminer.GetRow(0);
            var cell = dataGridAddExaminer.GetCell(row, 0).Content;

            string examinerName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 1).Content;
            string examinerSurname = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 2).Content;
            string examinerSecondName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 3).Content;
            string birthDate = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 4).Content;
            string position = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery =
                    "INSERT INTO [Examiners] (ExaminerName,ExaminerSurname,ExaminerSecondname,BirthDate,Position) " +
                    "VALUES ('" + examinerName + "','" + examinerSurname + "','" + examinerSecondName + "','" + birthDate +
                    "','" + position + "')";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        //----------------------------------------------------------
        #endregion

        #region Button add events
        //---------------------------------------------------------

        private void buttonAddAuditory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddAuditory();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddOlympiad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddOlympiad();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTask();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddExaminer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddExaminer();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        #endregion

        private void UpdateOlympiadsTab()
        {
            UpdateOlympiads();
            UpdateOlympiadID();
            ShowAuditories();
            ShowTasks();
            ShowExaminers();
            PrepareAuditoryInput();
            PrepareOlympiadInput();
            PrepareTaskInput();
            PrepareExaminerInput();
        }

        //----------------------------------------------------------
        #endregion

        #region OlympiadsTab2
        //--------------------------------------------------------

        #region Prepare input
        //--------------------------------------------------------

        private void PrepareSchoolInput()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("SchoolNumber", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Region", typeof(string)));
            dataTable.Rows.Add(dataTable.NewRow());

            dataGridAddSchool.CanUserAddRows = false;
            dataGridAddSchool.ItemsSource = dataTable.DefaultView;
        }

        private void PrepareParticipantInput()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("ParticipantName", typeof(string)));
            dataTable.Columns.Add(new DataColumn("ParticipantSurname", typeof(string)));
            dataTable.Columns.Add(new DataColumn("ParticipantSecondname", typeof(string)));
            dataTable.Columns.Add(new DataColumn("Birthdate", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("SchoolNumber", typeof(int)));
            dataTable.Columns.Add(new DataColumn("TeacherSurname", typeof(string)));
            dataTable.Rows.Add(dataTable.NewRow());

            dataGridAddParticipant.CanUserAddRows = false;
            dataGridAddParticipant.ItemsSource = dataTable.DefaultView;
        }

        private void PrepareExaminerInput2()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("ExaminerName", typeof(string)));
            dataTable.Columns.Add(new DataColumn("ExaminerSurname", typeof(string)));
            dataTable.Columns.Add(new DataColumn("ExaminerSecondName", typeof(string)));
            dataTable.Columns.Add(new DataColumn("BirthDate", typeof(DateTime)));
            dataTable.Columns.Add(new DataColumn("Position", typeof(string)));
            var row = dataTable.NewRow();
            dataTable.Rows.Add(row);

            dataGridAddExaminer2.CanUserAddRows = false;
            dataGridAddExaminer2.ItemsSource = dataTable.DefaultView;
        }

        private void PrepareOlympiadDetail()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("OlympiadID", typeof(int)));
            dataTable.Columns.Add(new DataColumn("ParticipantID", typeof(int)));
            dataTable.Columns.Add(new DataColumn("TaskNumber", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Score", typeof(double)));
            dataTable.Columns.Add(new DataColumn("ExaminerID", typeof(double)));
            var row = dataTable.NewRow();
            row["OlympiadID"] = OlympiadID;
            row["ParticipantID"] = ParticipantID;
            row["ExaminerID"] = ExaminerID;
            dataTable.Rows.Add(row);

            dataGridAddOlympiadDetail.CanUserAddRows = false;
            dataGridAddOlympiadDetail.ItemsSource = dataTable.DefaultView;
        }

        //----------------------------------------------------------
        #endregion

        #region Show different tables
        //---------------------------------------------------------

        private void ShowOlympiadDetails()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = "SELECT * FROM [OlympiadDetails] WHERE OlympiadID = " + OlympiadID;

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridOlympiadDetails.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ShowSchools()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = "SELECT * FROM [Schools] ";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridSchools.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ShowParticipants2()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery =
                    "SELECT * " +
                    "FROM [Participants] ";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridParticipants2.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ShowExaminers2()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = @"SELECT *
                                        FROM [Examiners]";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridExaminers2.ItemsSource = dataTable.DefaultView;
            }
        }

        //---------------------------------------------------------
        #endregion

        #region OlympiadDetails HelperAdder
        //--------------------------------------------------------

        public int ParticipantID { get; set; }
        private DataGridRow previousRowParticID;
        private void UpdateParticipantID()
        {
            var selectedRow = dataGridParticipants2.GetSelectedRow();

            if (previousRowParticID != selectedRow)
            {
                previousRow = selectedRow;
            }
            else
            {
                return;
            }

            if (selectedRow == null)
            {
                return;
            }
            var columnCell = dataGridParticipants2.GetCell(selectedRow, 0).Content;

            int partID;
            int.TryParse((columnCell as TextBlock).Text, out partID);
            ParticipantID = partID;
        }

        public int ExaminerID { get; set; }
        private DataGridRow previousRowExamID;
        private void UpdateExaminerID()
        {
            var selectedRow = dataGridExaminers2.GetSelectedRow();

            if (previousRowParticID != selectedRow)
            {
                previousRow = selectedRow;
            }
            else
            {
                return;
            }

            if (selectedRow == null)
            {
                return;
            }
            var columnCell = dataGridExaminers2.GetCell(selectedRow, 0).Content;

            int examID;
            int.TryParse((columnCell as TextBlock).Text, out examID);
            ExaminerID = examID;
        }

        private void dataGridParticipants2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                UpdateParticipantID();
                PrepareOlympiadDetail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridExaminers2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UpdateExaminerID();
                PrepareOlympiadDetail();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        #endregion

        #region Add record
        //--------------------------------------------------------

        private void AddExaminer2()
        {
            var row = dataGridAddExaminer2.GetRow(0);
            var cell = dataGridAddExaminer2.GetCell(row, 0).Content;

            string examinerName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 1).Content;
            string examinerSurname = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 2).Content;
            string examinerSecondName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 3).Content;
            string birthDate = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 4).Content;
            string position = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery =
                    "INSERT INTO [Examiners] (ExaminerName,ExaminerSurname,ExaminerSecondname,BirthDate,Position) " +
                    "VALUES ('" + examinerName + "','" + examinerSurname + "','" + examinerSecondName + "','" + birthDate +
                    "','" + position + "')";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void AddSchool()
        {
            var row = dataGridAddSchool.GetRow(0);
            var cell = dataGridAddSchool.GetCell(row, 0).Content;

            int schoolNumber;
            int.TryParse((cell as TextBlock).Text, out schoolNumber);

            cell = dataGridAddSchool.GetCell(row, 1).Content;
            string region = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery =
                    "INSERT INTO [Schools] (SchoolNumber,Region) " +
                    "VALUES (" + schoolNumber + ",'" + region + "')";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void AddParticipant2()
        {
            var row = dataGridAddParticipant.GetRow(0);
            var cell = dataGridAddParticipant.GetCell(row, 0).Content;

            string partName = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 1).Content;
            string partSurname = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 2).Content;
            string partSecondname = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 3).Content;
            string birthdate = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 4).Content;
            int schoolNumber;
            int.TryParse((cell as TextBlock).Text, out schoolNumber);

            cell = dataGridAddParticipant.GetCell(row, 5).Content;
            string teacherSurname = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery =
                    @"INSERT INTO [Participants] (ParticipantName,ParticipantSurname,ParticipantSecondName,
                                                    BirthDate,SchoolNumber,TeacherSurname) " +
                    "VALUES ('" + partName + "','" + partSurname + "','" + partSecondname + "','" + birthdate + "'," +
                    schoolNumber + ",'" + teacherSurname + "')";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void AddOlympiadDetail()
        {
            var row = dataGridAddOlympiadDetail.GetRow(0);
            var cell = dataGridAddOlympiadDetail.GetCell(row, 0).Content;

            int olyID;
            int.TryParse((cell as TextBlock).Text, out olyID);

            cell = dataGridAddOlympiadDetail.GetCell(row, 1).Content;
            int partID;
            int.TryParse((cell as TextBlock).Text, out partID);

            cell = dataGridAddOlympiadDetail.GetCell(row, 2).Content;
            int taskNum;
            int.TryParse((cell as TextBlock).Text, out taskNum);

            cell = dataGridAddOlympiadDetail.GetCell(row, 3).Content;
            double score;
            double.TryParse((cell as TextBlock).Text, out score);

            cell = dataGridAddOlympiadDetail.GetCell(row, 4).Content;
            int examID;
            int.TryParse((cell as TextBlock).Text, out examID);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string insertQuery =
                    "INSERT INTO [OlympiadDetails] (OlympiadID,ParticipantID,TaskNumber,Score,ExaminerID) " +
                    "VALUES (" + olyID + "," + partID + "," + taskNum + "," + score + "," + examID + ")";

                SqlCommand command = new SqlCommand(insertQuery, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        //--------------------------------------------------------
        #endregion

        #region Button add Events
        //--------------------------------------------------------

        private void buttonAddSchool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddSchool();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddParticipant_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddParticipant2();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddExaminer2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddExaminer2();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddOlympiadDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddOlympiadDetail();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------
        #endregion

        private void UpdateOlympiadsTab2()
        {
            PrepareSchoolInput();
            PrepareParticipantInput();
            PrepareExaminerInput2();
            PrepareOlympiadDetail();

            ShowOlympiadDetails();
            ShowSchools();
            ShowParticipants2();
            ShowExaminers2();
        }

        //--------------------------------------------------------
        #endregion

        #region Tabs changed
        //-------------------------------------------

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.Source is TabControl)
            {
                if(tabItemMain.IsSelected)
                {
                    if (gridOlympiads.Children.Contains(dataGridOlympiads) && gridOlympiads.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads.Children.Remove(dataGridOlympiads);
                        gridOlympiads.Children.Remove(labelOlympiads);
                    }
                    else if (gridQuery.Children.Contains(dataGridOlympiads) && gridQuery.Children.Contains(labelOlympiads))
                    {
                        gridQuery.Children.Remove(dataGridOlympiads);
                        gridQuery.Children.Remove(labelOlympiads);
                    }
                    else if (gridOlympiads2.Children.Contains(dataGridOlympiads) && gridOlympiads2.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads2.Children.Remove(dataGridOlympiads);
                        gridOlympiads2.Children.Remove(labelOlympiads);
                    }


                    if (!gridMain.Children.Contains(dataGridOlympiads) && !gridMain.Children.Contains(labelOlympiads))
                    {
                        gridMain.Children.Add(dataGridOlympiads);
                        gridMain.Children.Add(labelOlympiads);

                        UpdateOlympiads();
                        UpdateOlympiadID();
                        UpdateMainTab();
                    }
                }
                if(tabItemOlympiads.IsSelected)
                {
                    if (gridMain.Children.Contains(dataGridOlympiads) && gridMain.Children.Contains(labelOlympiads))
                    {
                        gridMain.Children.Remove(dataGridOlympiads);
                        gridMain.Children.Remove(labelOlympiads);
                    }
                    else if (gridQuery.Children.Contains(dataGridOlympiads) && gridQuery.Children.Contains(labelOlympiads))
                    {
                        gridQuery.Children.Remove(dataGridOlympiads);
                        gridQuery.Children.Remove(labelOlympiads);
                    }
                    else if (gridOlympiads2.Children.Contains(dataGridOlympiads) && gridOlympiads2.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads2.Children.Remove(dataGridOlympiads);
                        gridOlympiads2.Children.Remove(labelOlympiads);
                    }


                    if (!gridOlympiads.Children.Contains(dataGridOlympiads) && !gridOlympiads.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads.Children.Add(dataGridOlympiads);
                        gridOlympiads.Children.Add(labelOlympiads);

                        UpdateOlympiads();
                        UpdateOlympiadID();
                        UpdateOlympiadsTab();
                    }
                }
                if (tabItemOlympiads2.IsSelected)
                {
                    if (gridMain.Children.Contains(dataGridOlympiads) && gridMain.Children.Contains(labelOlympiads))
                    {
                        gridMain.Children.Remove(dataGridOlympiads);
                        gridMain.Children.Remove(labelOlympiads);
                    }
                    else if (gridOlympiads.Children.Contains(dataGridOlympiads) && gridOlympiads.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads.Children.Remove(dataGridOlympiads);
                        gridOlympiads.Children.Remove(labelOlympiads);
                    }
                    else if (gridQuery.Children.Contains(dataGridOlympiads) && gridQuery.Children.Contains(labelOlympiads))
                    {
                        gridQuery.Children.Remove(dataGridOlympiads);
                        gridQuery.Children.Remove(labelOlympiads);
                    }

                    if (!gridOlympiads2.Children.Contains(dataGridOlympiads) && !gridOlympiads2.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads2.Children.Add(dataGridOlympiads);
                        gridOlympiads2.Children.Add(labelOlympiads);

                        UpdateOlympiads();
                        UpdateOlympiadID();
                        UpdateOlympiadsTab2();
                    }
                }
                if(tabItemQuery.IsSelected)
                {
                    if (gridMain.Children.Contains(dataGridOlympiads) && gridMain.Children.Contains(labelOlympiads))
                    {
                        gridMain.Children.Remove(dataGridOlympiads);
                        gridMain.Children.Remove(labelOlympiads);
                    }
                    else if (gridOlympiads.Children.Contains(dataGridOlympiads) && gridOlympiads.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads.Children.Remove(dataGridOlympiads);
                        gridOlympiads.Children.Remove(labelOlympiads);
                    }
                    else if (gridOlympiads2.Children.Contains(dataGridOlympiads) && gridOlympiads2.Children.Contains(labelOlympiads))
                    {
                        gridOlympiads2.Children.Remove(dataGridOlympiads);
                        gridOlympiads2.Children.Remove(labelOlympiads);
                    }

                    if (!gridQuery.Children.Contains(dataGridOlympiads) && !gridQuery.Children.Contains(labelOlympiads))
                    {
                        gridQuery.Children.Add(dataGridOlympiads);
                        gridQuery.Children.Add(labelOlympiads);

                        UpdateOlympiads();
                        UpdateOlympiadID();
                        UpdateQueries();
                    }
                }

                if (tabItemLog.IsSelected)
                {
                    UpdateLog();
                }
                
            }
        }

        //--------------------------------------------
        #endregion

        private void UpdateLog()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string selectQuery = "SELECT * FROM [SummaryLog]";

                var sqlDataAdapter = new SqlDataAdapter(selectQuery, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridLog.ItemsSource = dataTable.DefaultView;
            }
        }


        #region Queries
        //------------------------------------------------------

        private void Query1()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = @"SELECT ParticipantSurname, ParticipantName, ParticipantSecondName, Region, SummaryScore
                                FROM (SELECT Region,MAX(SumScore) as SummaryScore
		                                FROM [OlympiadSummary] as OS
		                                JOIN [Schools] as S
		                                ON OS.SchoolNumber = s.SchoolNumber
		                                GROUP BY Region) as t1
                                JOIN [OlympiadSummary] as OS
                                ON OS.SumScore = t1.SummaryScore";

                var sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridQuery1.ItemsSource = dataTable.DefaultView;
            }
        }

        private void Query2()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = @"SELECT P.ParticipantSurname, 
	                                    P.ParticipantName, 
	                                    P.ParticipantSecondName, 
	                                    P.TeacherSurname,
	                                    W.SumScore
                                    FROM [Winners] as W
                                    JOIN [Participants] as P
                                    ON W.ParticipantName = P.ParticipantName AND
	                                    W.ParticipantSurname = P.ParticipantSurname AND
	                                    W.ParticipantSecondName = p.ParticipantSecondName
                                    ORDER BY SumScore DESC";

                var sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridQuery2.ItemsSource = dataTable.DefaultView;
            }
        }

        private void Query3()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query =
                    @"SELECT O.TaskNumber, T.Condition, COUNT(O.TaskNumber) as PeopleNotSolved
                                FROM [OlympiadDetails] as O 
                                JOIN [Tasks] as T
                                ON O.TaskNumber = T.TaskNumber AND O.OlympiadID = T.OlympiadID
                                WHERE O.OlympiadID = " + OlympiadID + @" AND Score = 0
                                GROUP BY O.TaskNumber, T.Condition";

                var sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridQuery3.ItemsSource = dataTable.DefaultView;
            }
        }

        private void Query4()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query =
                    @"SELECT SchoolNumber,
	                             ParticipantName,
	                             ParticipantSurname,
	                             ParticipantSecondName,
	                             Place,	 
	                             Subject,
	                             Date
                            FROM (SELECT ParticipantName,
	                             ParticipantSurname,
	                             ParticipantSecondName,
	                             SchoolNumber,
	                             ROW_NUMBER() OVER (ORDER BY SumScore DESC) as Place
                            FROM [OlympiadSummary]) as Places
                            JOIN [Olympiads] as Olymps
                            ON Olymps.OlympiadID = " + OlympiadID +
                    @" WHERE SchoolNumber = " + SchoolNumber;

                var sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridQuery4.ItemsSource = dataTable.DefaultView;
            }
        }

        public int SchoolNumber { get; set; }
        private void buttonApplySchool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SchoolNumber = int.Parse(textBoxSchool.Text);
                Query4();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateQueries()
        {
            try
            {
                Query1();
                Query2();
                Query3();
                Query4();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        //------------------------------------------------------
        #endregion

        #region Deletion and Updates
        //-----------------------------------------------------------

        private void DeleteOlympiad()
        {
            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [Olympiads] WHERE OlympiadID = " + OlympiadID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateOlympiad()
        {
            var row = dataGridAddOlympiad.GetRow(0);
            var cell = dataGridAddOlympiad.GetCell(row, 0).Content;

            string subject = (cell as TextBlock).Text;

            cell = dataGridAddOlympiad.GetCell(row, 1).Content;
            int grade;
            int.TryParse((cell as TextBlock).Text, out grade);

            cell = dataGridAddOlympiad.GetCell(row, 2).Content;
            string date = (cell as TextBlock).Text;

            cell = dataGridAddOlympiad.GetCell(row, 3).Content;
            int auditoryNum;
            int.TryParse((cell as TextBlock).Text, out auditoryNum);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = @"UPDATE [Olympiads] SET Subject ='" + subject + "',Grade=" + grade + ",Date='" + date +
                               "',AuditoryNumber=" + auditoryNum +
                               " WHERE OlympiadID = " + OlympiadID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteAuditory()
        {
            var row = dataGridAuditories.GetSelectedRow();
            var cell = dataGridAuditories.GetCell(row, 0).Content;

            int auditoryNumber;
            int.TryParse((cell as TextBlock).Text, out auditoryNumber);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [Auditories] WHERE AuditoryNumber = " + auditoryNumber;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateAuditory()
        {
            var row = dataGridAuditories.GetSelectedRow();
            var cell = dataGridAuditories.GetCell(row, 0).Content;

            int auditoryNumber;
            int.TryParse((cell as TextBlock).Text, out auditoryNumber);

            row = dataGridAddAuditory.GetRow(0);

            cell = dataGridAddAuditory.GetCell(row, 0).Content;
            int newAuditoryNumber;
            int.TryParse((cell as TextBlock).Text, out newAuditoryNumber);

            int numOfComputers;
            cell = dataGridAddAuditory.GetCell(row, 1).Content;
            int.TryParse((cell as TextBlock).Text, out numOfComputers);

            
            cell = dataGridAddAuditory.GetCell(row, 2).Content;
            string computersType = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "UPDATE [Auditories] SET AuditoryNumber = " + newAuditoryNumber + ",NumberOfComputers=" +
                               numOfComputers + ",ComputersType='" + computersType + "'" +
                               "WHERE AuditoryNumber = " + auditoryNumber;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteExaminer()
        {
            var row = dataGridExaminers.GetSelectedRow();
            var cell = dataGridExaminers.GetCell(row, 0).Content;

            int examinerID;
            int.TryParse((cell as TextBlock).Text, out examinerID);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [Examiners] WHERE ExaminerID = " + examinerID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateExaminer()
        {
            var row = dataGridExaminers.GetSelectedRow();
            var cell = dataGridExaminers.GetCell(row, 0).Content;

            int examinerID;
            int.TryParse((cell as TextBlock).Text, out examinerID);

            row = dataGridAddExaminer.GetRow(0);

            cell = dataGridAddExaminer.GetCell(row, 0).Content;
            string examinerName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 1).Content;
            string examinerSurname = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 2).Content;
            string examinerSecondName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 3).Content;
            string birthDate = (cell as TextBlock).Text;

            cell = dataGridAddExaminer.GetCell(row, 4).Content;
            string position = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "UPDATE [Examiners] SET ExaminerName = '" + examinerName + "',ExaminerSurname='" +
                               examinerSurname + "',ExaminerSecondName='" + examinerSecondName + "',BirthDate='" +
                               birthDate + "',Position='" + position + "'" +
                               " WHERE ExaminerID = " + examinerID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteTask()
        {
            var row = dataGridTasks.GetSelectedRow();
            var cell = dataGridTasks.GetCell(row, 0).Content;

            int olyID;
            int.TryParse((cell as TextBlock).Text, out olyID);

            cell = dataGridTasks.GetCell(row, 1).Content;
            int taskNum;
            int.TryParse((cell as TextBlock).Text, out taskNum);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [Tasks] WHERE OlympiadID = " + olyID + " AND TaskNumber = " + taskNum;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateTask()
        {
            var row = dataGridTasks.GetSelectedRow();
            var cell = dataGridTasks.GetCell(row, 0).Content;

            int olyID;
            int.TryParse((cell as TextBlock).Text, out olyID);

            cell = dataGridTasks.GetCell(row, 1).Content;
            int taskNum;
            int.TryParse((cell as TextBlock).Text, out taskNum);

            row = dataGridAddTask.GetRow(0);

            cell = dataGridAddTask.GetCell(row, 0).Content;
            int newOlyID;
            int.TryParse((cell as TextBlock).Text, out newOlyID);

            int newTaskNum;
            cell = dataGridAddTask.GetCell(row, 1).Content;
            int.TryParse((cell as TextBlock).Text, out newTaskNum);


            cell = dataGridAddTask.GetCell(row, 2).Content;
            string condition = (cell as TextBlock).Text;

            int maximalScore;
            cell = dataGridAddTask.GetCell(row, 3).Content;
            int.TryParse((cell as TextBlock).Text, out maximalScore);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "UPDATE [Tasks] SET OlympiadID=" + newOlyID + ",TaskNumber=" + newTaskNum +
                               ",Condition='" + condition + "',MaximalScore=" + maximalScore +
                               " WHERE OlympiadID = " + olyID + " AND TaskNumber=" + taskNum;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteExaminer2()
        {
            var row = dataGridExaminers2.GetSelectedRow();
            var cell = dataGridExaminers2.GetCell(row, 0).Content;

            int examinerID;
            int.TryParse((cell as TextBlock).Text, out examinerID);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [Examiners] WHERE ExaminerID = " + examinerID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateExaminer2()
        {
            var row = dataGridExaminers2.GetSelectedRow();
            var cell = dataGridExaminers2.GetCell(row, 0).Content;

            int examinerID;
            int.TryParse((cell as TextBlock).Text, out examinerID);

            row = dataGridAddExaminer2.GetRow(0);

            cell = dataGridAddExaminer2.GetCell(row, 0).Content;
            string examinerName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer2.GetCell(row, 1).Content;
            string examinerSurname = (cell as TextBlock).Text;

            cell = dataGridAddExaminer2.GetCell(row, 2).Content;
            string examinerSecondName = (cell as TextBlock).Text;

            cell = dataGridAddExaminer2.GetCell(row, 3).Content;
            string birthDate = (cell as TextBlock).Text;

            cell = dataGridAddExaminer2.GetCell(row, 4).Content;
            string position = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "UPDATE [Examiners] SET ExaminerName = '" + examinerName + "',ExaminerSurname='" +
                               examinerSurname + "',ExaminerSecondName='" + examinerSecondName + "',BirthDate='" +
                               birthDate + "',Position='" + position + "'" +
                               " WHERE ExaminerID = " + examinerID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteParticipant()
        {
            var row = dataGridParticipants2.GetSelectedRow();
            var cell = dataGridParticipants2.GetCell(row, 0).Content;

            int participantID;
            int.TryParse((cell as TextBlock).Text, out participantID);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [Participants] WHERE ParticipantID = " + participantID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateParticipant()
        {
            var row = dataGridParticipants2.GetSelectedRow();
            var cell = dataGridParticipants2.GetCell(row, 0).Content;

            int participantID;
            int.TryParse((cell as TextBlock).Text, out participantID);

            row = dataGridAddParticipant.GetRow(0);

            cell = dataGridAddParticipant.GetCell(row, 0).Content;
            string participantName = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 1).Content;
            string participantSurname = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 2).Content;
            string participantSecondName = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 3).Content;
            string birthDate = (cell as TextBlock).Text;

            cell = dataGridAddParticipant.GetCell(row, 4).Content;
            int schoolNumber;
            int.TryParse((cell as TextBlock).Text, out schoolNumber);

            cell = dataGridAddParticipant.GetCell(row, 5).Content;
            string teacherSurname = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "UPDATE [Participants] SET ParticipantName = '" + participantName +
                               "',ParticipantSurname='" +participantSurname + "',ParticipantSecondName='" + participantSecondName +
                               "',BirthDate='" +birthDate + "',SchoolNumber=" + schoolNumber + ",TeacherSurname='" + teacherSurname + "'" +
                               " WHERE ParticipantID = " + participantID;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteSchool()
        {
            var row = dataGridSchools.GetSelectedRow();
            var cell = dataGridSchools.GetCell(row, 0).Content;

            int schoolNum;
            int.TryParse((cell as TextBlock).Text, out schoolNum);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [Schools] WHERE SchoolNumber = " + schoolNum;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateSchool()
        {
            var row = dataGridSchools.GetSelectedRow();
            var cell = dataGridSchools.GetCell(row, 0).Content;

            int schoolNum;
            int.TryParse((cell as TextBlock).Text, out schoolNum);

            row = dataGridAddSchool.GetRow(0);

            cell = dataGridAddSchool.GetCell(row, 0).Content;
            int newSchoolNum;
            int.TryParse((cell as TextBlock).Text, out newSchoolNum);

            cell = dataGridAddSchool.GetCell(row, 1).Content;
            string region = (cell as TextBlock).Text;

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "UPDATE [Schools] SET SchoolNumber=" + newSchoolNum + ",Region='" + region + "'" +
                               " WHERE SchoolNumber = " + schoolNum;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteOlympiadDetail()
        {
            var row = dataGridOlympiadDetails.GetSelectedRow();
            var cell = dataGridOlympiadDetails.GetCell(row, 0).Content;

            int olyID;
            int.TryParse((cell as TextBlock).Text, out olyID);

            cell = dataGridTasks.GetCell(row, 1).Content;
            int partID;
            int.TryParse((cell as TextBlock).Text, out partID);

            cell = dataGridTasks.GetCell(row, 2).Content;
            int taskNum;
            int.TryParse((cell as TextBlock).Text, out taskNum);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "DELETE FROM [OlympiadDetails] WHERE OlympiadID = " + olyID + 
                    " AND ParticipantID = " + partID + " AND TaskNumber = " + taskNum;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateOlympiadDetail()
        {
            var row = dataGridOlympiadDetails.GetSelectedRow();
            var cell = dataGridOlympiadDetails.GetCell(row, 0).Content;

            int olyID;
            int.TryParse((cell as TextBlock).Text, out olyID);

            cell = dataGridOlympiadDetails.GetCell(row, 1).Content;
            int partID;
            int.TryParse((cell as TextBlock).Text, out partID);

            cell = dataGridOlympiadDetails.GetCell(row, 2).Content;
            int taskNum;
            int.TryParse((cell as TextBlock).Text, out taskNum);

            //---------------------------------------------------------------

            row = dataGridAddOlympiadDetail.GetRow(0);

            cell = dataGridAddOlympiadDetail.GetCell(row, 0).Content;
            int newOlyID;
            int.TryParse((cell as TextBlock).Text, out newOlyID);

            int newPartID;
            cell = dataGridAddOlympiadDetail.GetCell(row, 1).Content;
            int.TryParse((cell as TextBlock).Text, out newPartID);

            int newTaskNum;
            cell = dataGridAddOlympiadDetail.GetCell(row, 2).Content;
            int.TryParse((cell as TextBlock).Text, out newTaskNum);


            int score;
            cell = dataGridAddOlympiadDetail.GetCell(row, 3).Content;
            int.TryParse((cell as TextBlock).Text, out score);

            int examID;
            cell = dataGridAddOlympiadDetail.GetCell(row, 4).Content;
            int.TryParse((cell as TextBlock).Text, out examID);

            using (var sqlConnection = new SqlConnection(connStr))
            {
                sqlConnection.Open();
                string query = "UPDATE [OlympiadDetails] SET OlympiadID=" + newOlyID + ",ParticipantID=" + newPartID +
                               ",TaskNumber=" + newTaskNum +
                               ",Score=" + score + ",ExaminerID=" + ExaminerID +
                               " WHERE OlympiadID = " + olyID + " AND ParticipantID=" + partID + " AND TaskNumber=" +
                               taskNum;

                SqlCommand command = new SqlCommand(query, sqlConnection);
                command.ExecuteNonQuery();
            }
        }
        
        private void buttonDeleteOlympiad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteOlympiad();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateOlympiad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateOlympiad();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateAuditory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateAuditory();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDeleteAuditory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteAuditory();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateExaminer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateExaminer();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDeleteExaminer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteExaminer();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateTask();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteTask();
                UpdateOlympiadsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateExaminer2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateExaminer2();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDeleteExaminer2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteExaminer2();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateParticipant_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateParticipant();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDeleteParticipant_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteParticipant();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateSchool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateSchool();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDeleteSchool_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteSchool();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonUpdateOlympiadDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateOlympiadDetail();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonDeleteOlympiadDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteOlympiadDetail();
                UpdateOlympiadsTab2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridExaminers_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding.StringFormat = "{0:d}";
                }
            }
        }

        private void dataGridOlympiads_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding.StringFormat = "{0:d}";
                }
            }
        }

        private void dataGridParticipants2_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding.StringFormat = "{0:d}";
                }
            }
        }

        private void dataGridExaminers2_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding.StringFormat = "{0:d}";
                }
            }
        }

        private void dataGridQuery4_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
                {
                    dataGridTextColumn.Binding.StringFormat = "{0:d}";
                }
            }
        }

        //-----------------------------------------------------------

        #endregion
    }
}