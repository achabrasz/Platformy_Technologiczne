using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Windows;
using Application2 = System.Windows.Application;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using Button = System.Windows.Controls.Button;

namespace Lab10
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            List<Car> myCars = new List<Car>()
            {
                new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };

            Application2 app = new Application2();
            MainWindow mainWindow = new MainWindow(myCars);
            app.Run(mainWindow);
        }
    }

    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        private SortableBindingList<Car> myCarsBindingList;
        private List<Car> myCars;

        Comparison<Car> arg1 = new Comparison<Car>((car1, car2) => car2.Motor.HorsePower.CompareTo(car1.Motor.HorsePower));
        Predicate<Car> arg2 = new Predicate<Car>(car => car.Motor.FuelType == "TDI");
        Action<Car> arg3 = new Action<Car>(car => MessageBox.Show(car.Model + ", " + car.Motor.Model + ", " + car.Year));

        public MainWindow(List<Car> Cars)
        {
            myCars = Cars;
            InitializeComponent();
            AllocConsole();

            var elementsQuerySyntax =
                from car in myCars
                where car.Model == "A6"
                group car by car.Motor.FuelType == "TDI" ? "diesel" : "petrol" into carGroup
                select new
                {
                    engineType = carGroup.Key,
                    avgHPPL = carGroup.Average(car => car.Motor.HorsePower / car.Motor.Displacement)
                } into result
                orderby result.avgHPPL descending
                select result;

            var elementsMethodSyntax = myCars
                .Where(car => car.Model == "A6")
                .GroupBy(car => car.Motor.FuelType == "TDI" ? "diesel" : "petrol")
                .Select(carGroup => new
                {
                    engineType = carGroup.Key,
                    avgHPPL = carGroup.Average(car => car.Motor.HorsePower / car.Motor.Displacement)
                })
                .OrderByDescending(result => result.avgHPPL);

            Console.WriteLine("Method Syntax:");
            foreach (var e in elementsMethodSyntax)
            {
                Console.WriteLine(e.engineType + ": " + e.avgHPPL);
            }

            Console.WriteLine("\nQuery Syntax:");
            foreach (var e in elementsQuerySyntax)
            {
                Console.WriteLine(e.engineType + ": " + e.avgHPPL);
            }

            Grid mainGrid = new Grid();
            this.Content = mainGrid;

            RowDefinition row1 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row2 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            mainGrid.RowDefinitions.Add(row1);
            mainGrid.RowDefinitions.Add(row2);

            DataGrid dataGrid = new DataGrid
            {
                AutoGenerateColumns = true,
                ItemsSource = myCars
            };
            Grid.SetRow(dataGrid, 1);
            mainGrid.Children.Add(dataGrid);

            myCarsBindingList = new SortableBindingList<Car>(myCars);
            dataGrid.ItemsSource = myCarsBindingList;

            ToolStrip toolStrip = new ToolStrip();
            ToolStripComboBox cbProperties = new ToolStripComboBox();
            ToolStripTextBox tbSearchValue = new ToolStripTextBox();
            ToolStripComboBox cbType = new ToolStripComboBox();
            ToolStripButton btnSearch = new ToolStripButton("Search");
            ToolStripButton btnAdd = new ToolStripButton("Add");
            ToolStripButton btnDelete = new ToolStripButton("Delete");
            ToolStripButton btnEdit = new ToolStripButton("Edit");


            toolStrip.Items.Add(cbProperties);
            toolStrip.Items.Add(tbSearchValue);
            toolStrip.Items.Add(cbType);
            toolStrip.Items.Add(btnSearch);
            toolStrip.Items.Add(btnAdd);
            toolStrip.Items.Add(btnDelete);
            toolStrip.Items.Add(btnEdit);

            System.Windows.Forms.Integration.WindowsFormsHost windowsFormsHost = new System.Windows.Forms.Integration.WindowsFormsHost
            {
                Child = toolStrip
            };
            Grid.SetRow(windowsFormsHost, 0);
            mainGrid.Children.Add(windowsFormsHost);

            cbProperties.Enter += (s, e) =>
            {
                cbProperties.Items.Clear();
                var properties = TypeDescriptor.GetProperties(typeof(Car));
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(typeof(Car)))
                {
                    if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int))
                    {
                        cbProperties.Items.Add(prop.Name);
                    }
                }
                cbProperties.Items.Add("Motor");
            };

            cbType.Enter += (s, e) =>
            {
                cbType.Items.Clear();
                cbType.Items.Add("String");
                cbType.Items.Add("Int32");
            };

            btnSearch.Click += (s, e) =>
            {
                string propertyName = cbProperties.SelectedItem as string;
                string searchValue = tbSearchValue.Text;
                string type = cbType.SelectedItem as string;

                if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(searchValue))
                {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(Car))[propertyName];
                    int index = myCarsBindingList.Find(prop, searchValue, type);

                    if (index >= 0)
                    {
                        dataGrid.SelectedItem = myCarsBindingList[index];
                        dataGrid.ScrollIntoView(myCarsBindingList[index]);
                    }
                    else
                    {
                        MessageBox.Show("Item not found");
                    }
                }
            };

            btnAdd.Click += (s, e) =>
            {
                var newCar = new Car("NewModel", new Engine(0, 0, "FuelType"), 0);
                myCarsBindingList.Add(newCar);
                dataGrid.ScrollIntoView(newCar);
                dataGrid.SelectedItem = newCar;
            };

            btnDelete.Click += (s, e) =>
            {
                if (dataGrid.SelectedItem is Car selectedCar)
                {
                    myCarsBindingList.Remove(selectedCar);
                }
                else
                {
                    MessageBox.Show("No item selected");
                }
            };

            btnEdit.Click += (s, e) =>
            {
                if (dataGrid.SelectedItem is Car selectedCar)
                {
                    var editWindow = new EditWindow(selectedCar);
                    editWindow.ShowDialog();
                    myCarsBindingList.ResetBindings();
                }
                else
                {
                    MessageBox.Show("No item selected");
                }
            };

            myCars.Sort(new Comparison<Car>(arg1));
            myCars.FindAll(arg2).ForEach(arg3);
        }
    }

    public partial class EditWindow : Window
    {
        private TextBox tbModel;
        private TextBox tbMotorDisplacement;
        private TextBox tbMotorHorsePower;
        private TextBox tbMotorFuelType;
        private TextBox tbYear;

        public EditWindow(Car car)
        {
            InitializeComponent();

            Grid mainGrid = new Grid();
            this.Content = mainGrid;

            RowDefinition row1 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row2 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row3 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row4 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row5 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row6 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            RowDefinition row7 = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
            mainGrid.RowDefinitions.Add(row1);
            mainGrid.RowDefinitions.Add(row2);
            mainGrid.RowDefinitions.Add(row3);
            mainGrid.RowDefinitions.Add(row4);
            mainGrid.RowDefinitions.Add(row5);
            mainGrid.RowDefinitions.Add(row6);
            mainGrid.RowDefinitions.Add(row7);

            tbModel = new TextBox { Text = car.Model };
            tbMotorDisplacement = new TextBox { Text = car.Motor.Displacement.ToString() };
            tbMotorHorsePower = new TextBox { Text = car.Motor.HorsePower.ToString() };
            tbMotorFuelType = new TextBox { Text = car.Motor.FuelType };
            tbYear = new TextBox { Text = car.Year.ToString() };

            Grid.SetRow(tbModel, 0);
            Grid.SetRow(tbMotorDisplacement, 1);
            Grid.SetRow(tbMotorHorsePower, 2);
            Grid.SetRow(tbMotorFuelType, 3);
            Grid.SetRow(tbYear, 4);
            mainGrid.Children.Add(tbModel);
            mainGrid.Children.Add(tbMotorDisplacement);
            mainGrid.Children.Add(tbMotorHorsePower);
            mainGrid.Children.Add(tbMotorFuelType);
            mainGrid.Children.Add(tbYear);

            Button btnOk = new Button { Content = "OK" };
            Button btnCancel = new Button { Content = "Cancel" };
            Grid.SetRow(btnOk, 5);
            Grid.SetRow(btnCancel, 6);
            mainGrid.Children.Add(btnOk);
            mainGrid.Children.Add(btnCancel);

            btnOk.Click += (s, e) =>
            {
                car.Model = tbModel.Text;
                car.Motor = new Engine(
                                       Convert.ToDouble(tbMotorDisplacement.Text),
                                                          Convert.ToInt32(tbMotorHorsePower.Text),
                                                                             tbMotorFuelType.Text
                                                                                            );
                car.Year = Convert.ToInt32(tbYear.Text);
                this.Close();
            };

            btnCancel.Click += (s, e) =>
            {
                this.Close();
            };


        }
    }

    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection;
        private PropertyDescriptor _sortProperty;

        public SortableBindingList(IList<T> list) : base(list) { }

        protected override bool SupportsSortingCore => true;
        protected override bool SupportsSearchingCore => true;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (prop.PropertyType.GetInterface("IComparable") != null)
            {
                _sortDirection = direction;
                _sortProperty = prop;

                List<T> items = this.Items as List<T>;
                if (items != null)
                {
                    items.Sort(Compare);
                    _isSorted = true;
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }

        private int Compare(T lhs, T rhs)
        {
            int result = CompareValue(lhs, rhs);
            return _sortDirection == ListSortDirection.Ascending ? result : -result;
        }

        private int CompareValue(T lhs, T rhs)
        {
            object lhsValue = _sortProperty.GetValue(lhs);
            object rhsValue = _sortProperty.GetValue(rhs);

            if (lhsValue == null) return rhsValue == null ? 0 : -1;
            if (rhsValue == null) return 1;
            return ((IComparable)lhsValue).CompareTo(rhsValue);
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _sortProperty = null;
            _sortDirection = ListSortDirection.Ascending;
        }

        protected override int FindCore(PropertyDescriptor prop, object? key)
        {
            if (prop.Name == "Motor")
            {
                string[] stringKey = key.ToString().Split(' ');
                Engine engine = new Engine(0, Convert.ToInt32(stringKey[1]), stringKey[0]);
                for (int i = 0; i < Count; i++)
                {
                    Engine item = prop.GetValue(this[i]) as Engine;
                    if (item != null && item.CompareTo(engine) == 0)
                        return i;
                }
            }
            for (int i = 0; i < Count; i++)
            {
                object item = prop.GetValue(this[i]);
                if (item != null && item.Equals(key))
                    return i;
            }
            return -1;
        }

        public int Find(PropertyDescriptor prop, object key, string type)
        {
            if (type == "Int32")
            {
                return FindCore(prop, Convert.ToInt32(key));
            }
            return FindCore(prop, key);
        }
    }

    public class Car
    {
        public string Model { get; set; }
        public Engine Motor { get; set; }
        public int Year { get; set; }

        public Car(string model, Engine motor, int year)
        {
            Model = model;
            Motor = motor;
            Year = year;
        }
    }

    public class Engine : IComparable, IComparable<Engine>
    {
        public double Displacement { get; }
        public int HorsePower { get; }
        public string FuelType { get; }
        public string Model { get; }

        public Engine(double displacement, int horsePower, string fuelType)
        {
            Displacement = displacement;
            HorsePower = horsePower;
            FuelType = fuelType;
            Model = $"{fuelType} {horsePower}HP";
        }

        public int CompareTo(Engine? other)
        {
            if (other == null) return 1;
            return HorsePower.CompareTo(other.HorsePower);
        }

        public override string ToString()
        {
            return Model;
        }

        public int CompareTo(object? obj)
        {
            return this.CompareTo(obj as Engine);
        }
    }
}