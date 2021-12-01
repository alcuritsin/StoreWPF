using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DataModelLib;
using DbConnectionLib;

namespace Store
{
    public partial class MainWindow : Window
    {
        #region Values

        public static ObservableCollection<OrderLine> Order;
        public static ObservableCollection<Product> Products;

        #endregion

        #region Constructors


        public MainWindow()
        {
            InitializeComponent();
            InitGrid(2, 4);

            Order = new ObservableCollection<OrderLine>();
            Products = new ObservableCollection<Product>(new DataBaseLib.DataBase().GetAllProducts());
        }
        #endregion


        #endregion

        #region CreateGrid

        private void InitGrid(int amountRows, int amountColumns)
        {
            Grid_Body.RowDefinitions.Clear();
            Grid_Body.ColumnDefinitions.Clear();

            for (int i = 0; i < amountRows; i++)
            {
                Grid_Body.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int j = 0; j < amountColumns; j++)
            {
                Grid_Body.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            for (int i = 0; i < amountRows; i++)
            {
                for (int j = 0; j < amountColumns; j++)
                {
                    var card = CreateCard(new Product());
                    Grid.SetRow(card, i);
                    Grid.SetColumn(card, j);

                    Grid_Body.Children.Add(card);
                }
            }
        }

        #endregion

        #region CreateCard

        private StackPanel CreateCard(Product product)

        {
            var cardImage = new Image
            {
                Source = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\img\product.jpg")) //TODO Добавить путь к изображению скаченному с FTP
            };

            var card = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            card.Children.Add(cardImage);
            card.Children.Add(CreateCardAmount());
            card.Children.Add(CreateCardAnnotation(product.Annotation));

            return card;
        }

        private Label CreateCardAnnotation(string annotation)
        {
            var label = new Label
            {
                Content = annotation
            };

            return label;
        }

        private StackPanel CreateCardAmount()
        {
            var cardAmount = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            cardAmount.Children.Add(CreateCardAmountButton("-"));
            cardAmount.Children.Add(CreateCardAmountLabel());
            cardAmount.Children.Add(CreateCardAmountButton("+"));

            return cardAmount;
        }

        private Label CreateCardAmountLabel()
        {
            var label = new Label
            {
                Content = "0",
            };
            return label;
        }

        private Button CreateCardAmountButton(string content)
        {
            var button = new Button
            {
                Content = content,
            };
            button.Click += Button_CardAmount_OnClick;
            return button;
        }
        #endregion


        private void Button_CardAmount_OnClick(object sender, RoutedEventArgs e)
        {
            var content = ((Button)sender).Content.ToString();
            var product = ((Label)((StackPanel)((StackPanel)((Button)sender).Parent).Parent).Children[2]).Content
                .ToString();

            switch (content)
            {
                case "-":
                    ReduceFromOrder(product);
                    break;
                case "+":
                    AddToOrder(product);
                    break;
            }

            ((Label)((StackPanel)((Button)sender).Parent).Children[1]).Content = AmountProduct(product);

            Button_Basket.Content = Order.Count > 0 ? $"Корзина ({Order.Count})" : "Корзина";
        }

        #endregion


        private bool IsNewLine(string product)
        {
            // Проверяет - это новый товар в корзине? 
            return Order.All(line => line.ProductName != product);
        }

        private void AddToOrder(string product)
        {
            if (IsNewLine(product))
            {
                Order.Add(new OrderLine { ProductName = product, ProductAmount = 1 });
            }
            else
            {
                foreach (var orderLine in Order)
                {
                    if (orderLine.ProductName != product) continue;

                    orderLine.ProductAmount++;
                    break;
                }
            }
        }

        private void ReduceFromOrder(string product)
        {
            if (IsNewLine(product)) return;

            foreach (var orderLine in Order)
            {
                if (orderLine.ProductName == product && orderLine.ProductAmount > 1)
                {
                    orderLine.ProductAmount--;
                }
                else
                {
                    Order.Remove(orderLine);
                    break;
                }
            }
        }

        private void Button_Basket_Click(object sender, RoutedEventArgs e)
        {
            var basket = new WindowBasket();

            basket.Show();
        }

        private int AmountProduct(string product)
        {
            return !IsNewLine(product)
                ? (from orderLine in Order where orderLine.ProductName == product select orderLine.ProductAmount)
                .FirstOrDefault()
                : 0;
        }

        //TODO Переместить в админку
        private void Button_CreateDbConnectionTemplate_Click(object sender, RoutedEventArgs e)
        {
            var template = new DbConnection
            {
                Server = "myServerAddress",
                Database = "myDataBase",
                Uid = "myUsername",
                Pwd = "myPassword"
            };

            var fileConnection = Environment.CurrentDirectory + @"\db_connection.json";

            if (template.SerializeJson(fileConnection))
            {
                MessageBox.Show(
                    $"Успешное создание шаблонного файла JSON для подключения к БД\n" +
                    $"Шаблонный файл: {fileConnection}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}