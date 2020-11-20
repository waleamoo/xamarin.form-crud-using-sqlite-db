using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using UsingSQLite.Persistence;
using Xamarin.Forms;

namespace UsingSQLite
{

    public class Recipe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; // raise this event to notify our subscriber e.g. listView

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _name; // backing field

        [Unique, MaxLength(255)]
        public string Name
        {
            get { return _name;  }
            set
            {
                if (_name == value)
                    return;
                _name = value;

                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // if PropertyChanged is empty return null else return its value 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        //public string Name { get; set; }


    }

    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Recipe> _recipes;

        public MainPage()
        {
            InitializeComponent();

            // get the dependency service of the Interface 
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); // similar to DbContext in EF for CRUD
            
        }

        protected override async void OnAppearing()
        {
            await _connection.CreateTableAsync<Recipe>(); // creates the table

            var recipes = await _connection.Table<Recipe>().ToListAsync(); // get all recipes

            // put the result set in the observable collection 
            _recipes = new ObservableCollection<Recipe>(recipes);

            recipesListView.ItemsSource = _recipes;

            base.OnAppearing();
        }

        // we need to add observable collection to make the changes appear in our list view 
        async void OnAdd(object sender, System.EventArgs e)
        {
            var recipe = new Recipe { Name = "Recipe " + DateTime.Now.Ticks };
            await _connection.InsertAsync(recipe);
            _recipes.Add(recipe);
        }

        async void OnUpdate(object sender, System.EventArgs e)
        {
            var recipe = _recipes[0];
            recipe.Name += " UPDATED";
            await _connection.UpdateAsync(recipe);
            
        }

        async void OnDelete(object sender, System.EventArgs e)
        {
            var recipe = _recipes[0];

            await _connection.DeleteAsync(recipe);

            _recipes.Remove(recipe);
        }

    }
}
