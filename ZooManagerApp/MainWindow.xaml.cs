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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZooManagerApp
{

    public partial class MainWindow : Window
    {
        SqlConnection sqlconnection;
        public MainWindow()
        {
            InitializeComponent();
            //creating a string for the connection of the database,we use the connection string
            //to link the database
            string connection = ConfigurationManager.ConnectionStrings["ZooManagerApp.Properties.Settings.NdlovuraymondConnectionString"].ConnectionString;
            sqlconnection = new SqlConnection(connection);
            ShowZoos();
            ShowAnimals();
        }
        //method for what happens when something in our list box lbzoos is selected
        private void lbzoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAssociatedAnimals();
            ShowselectedZoointextbox();
        }
        private void lbanimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowselectedAnimalintextbox();
        }
        //methods for my text box, so that it will dynamically adjust its data to what is selected
        private void ShowselectedZoointextbox()
        {
            try
            {
                string query = "select location from Zoo where Id=@ZooId";
                SqlCommand mycommand = new SqlCommand(query, sqlconnection);

                //sql adapter is an interface to make tables using C#
                SqlDataAdapter zooadapter = new SqlDataAdapter(mycommand);
                using (zooadapter)
                {
                    mycommand.Parameters.AddWithValue("@ZooId", lbzoos.SelectedValue);
                    DataTable zootable = new DataTable();
                    zooadapter.Fill(zootable);
                    tbbox.Text = zootable.Rows[0]["Location"].ToString();
                }
            }
            catch (Exception)
            {
                //    MessageBox.Show(whyitdidntwork.ToString());
            }
        }
        private void ShowselectedAnimalintextbox()
        {
            try
            {
                string query = "select Name from Animal where Id=@AnimalId";
                SqlCommand mycommand = new SqlCommand(query, sqlconnection);

                //sql adapter is an interface to make tables using C#
                SqlDataAdapter animaladapter = new SqlDataAdapter(mycommand);
                using (animaladapter)
                {
                    mycommand.Parameters.AddWithValue("@AnimalId", lbanimals.SelectedValue);
                    DataTable animaltable = new DataTable();
                    animaladapter.Fill(animaltable);
                    tbbox.Text = animaltable.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception)
            {
                //    MessageBox.Show(whyitdidntwork.ToString());
            }
        }
        //method to show the zoos in lbzoos
        private void ShowZoos()
        {
            try
            {
                string query = "select * from Zoo";
                //sql adapter is an interface to make tables using C#
                SqlDataAdapter zooadapter = new SqlDataAdapter(query, sqlconnection);
                using (zooadapter)
                {
                    DataTable zootable = new DataTable();
                    zooadapter.Fill(zootable);
                    lbzoos.DisplayMemberPath = "Location";
                    lbzoos.SelectedValuePath = "Id";
                    lbzoos.ItemsSource = zootable.DefaultView;
                }
            }
            catch(Exception whyitdidntwork)
            {
                MessageBox.Show(whyitdidntwork.ToString());
            }
        }
       
        //method to show associated animals with selected zoo
        private void ShowAssociatedAnimals()
        {
            try
            {
                string query = "select * from Animal a inner join ZooAnimal za on a.Id=za.AnimalId where za.ZooID " +
                    "= @ZooId";
                SqlCommand mycommand = new SqlCommand(query, sqlconnection);

                //sql adapter is an interface to make tables using C#
                SqlDataAdapter assanimaladapter = new SqlDataAdapter(mycommand);
                using (assanimaladapter)
                {
                    mycommand.Parameters.AddWithValue("@ZooId", lbzoos.SelectedValue);
                    DataTable animal = new DataTable();
                    assanimaladapter.Fill(animal);
                    lbassanimals.DisplayMemberPath = "Name";
                    lbassanimals.SelectedValuePath = "Id";
                    lbassanimals.ItemsSource = animal.DefaultView;
                }
            }
            catch (Exception)
            {
            //    MessageBox.Show(whyitdidntwork.ToString());
            }
        }
        //method for displaying all animals in my database
        private void ShowAnimals()
        {
            try
            {
                string query = "select * from Animal";
                //sql adapter is an interface to make tables using C#
                SqlDataAdapter animaladapter = new SqlDataAdapter(query, sqlconnection);
                using (animaladapter)
                {
                    DataTable animaltable = new DataTable();
                    animaladapter.Fill(animaltable);
                    lbanimals.DisplayMemberPath = "Name";
                    lbanimals.SelectedValuePath = "Id";
                    lbanimals.ItemsSource = animaltable.DefaultView;
                }
            }
            catch (Exception)
            {
             //   MessageBox.Show(whyitdidntwork.ToString());
            }
        }
        //methods for what the different button clicks do
        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where Id = @ZooId";
                SqlCommand delzoocomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                delzoocomm.Parameters.AddWithValue("@ZooId", lbzoos.SelectedValue);
                delzoocomm.ExecuteScalar();
            }
            catch (Exception)
            {
              //this is just so if the user clicks without selecting anything or leaves it null
              //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after edit
                ShowZoos();
            }
        }

        private void Remove_Animal1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from ZooAnimal where Id=@Animalid";
                SqlCommand remanimalcomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                remanimalcomm.Parameters.AddWithValue("@AnimalId", lbassanimals.SelectedValue);
                remanimalcomm.ExecuteScalar();
            }
            catch (Exception)
            {
                //this is just so if the user clicks without selecting anything or leaves it null
                //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after edit
                ShowAssociatedAnimals();
            }
        }

        private void Add_Zoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand delzoocomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                delzoocomm.Parameters.AddWithValue("@Location",tbbox.Text);
                delzoocomm.ExecuteScalar();
            }
            catch (Exception)
            {
                //this is just so if the user clicks without selecting anything or leaves it null
                //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after edit
                ShowZoos();
            }
        }

        private void Add_animal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@AnimalId)";
                SqlCommand addtozoocomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                addtozoocomm.Parameters.AddWithValue("@AnimalId", tbbox.Text);
                addtozoocomm.ExecuteScalar();
            }
            catch (Exception)
            {
                //this is just so if the user clicks without selecting anything or leaves it null
                //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after edit
                ShowAnimals();
            }
        }

        private void Delete_Animal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where Id = @ZooId";
                SqlCommand delanimalcomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                delanimalcomm.Parameters.AddWithValue("@ZooId", lbanimals.SelectedValue);
                delanimalcomm.ExecuteScalar();
            }
            catch (Exception)
            {
                //this is just so if the user clicks without selecting anything or leaves it null
                //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after edit
                ShowAnimals();
            }
        }

        private void Add_animaltoZoo__Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand addtozoocomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                addtozoocomm.Parameters.AddWithValue("@ZooId", lbzoos.SelectedValue);
                addtozoocomm.Parameters.AddWithValue("@AnimalId", lbanimals.SelectedValue);
                addtozoocomm.ExecuteScalar();
            }
            catch(Exception)
            {
                //this is just so if the user clicks without selecting anything or leaves it null
                //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after edit
                ShowAssociatedAnimals();
            }
        }
        //methods for updating values that alread exist
        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Zoo Set Location = @Location where Id = @ZooId";
                SqlCommand updatezoocomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                updatezoocomm.Parameters.AddWithValue("@ZooId", lbzoos.SelectedValue);
                updatezoocomm.Parameters.AddWithValue("@Location", tbbox.Text);
                updatezoocomm.ExecuteScalar();
            }
            catch (Exception)
            {
                //this is just so if the user clicks without selecting anything or leaves it null
                //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after an update
                ShowZoos();
            }
        }

        private void Update_Animal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Animal Set Name = @Name where Id = @AnimalId";
                SqlCommand updatezoocomm = new SqlCommand(query, sqlconnection);
                sqlconnection.Open();
                updatezoocomm.Parameters.AddWithValue("@AnimalId", lbanimals.SelectedValue);
                updatezoocomm.Parameters.AddWithValue("@Name", tbbox.Text);
                updatezoocomm.ExecuteScalar();
            }
            catch (Exception)
            {
                //this is just so if the user clicks without selecting anything or leaves it null
                //the program will not crash:)
            }
            finally
            {
                sqlconnection.Close();
                //to make sure user sees update list after an update
                ShowAnimals();
            }
        }
    }
}
