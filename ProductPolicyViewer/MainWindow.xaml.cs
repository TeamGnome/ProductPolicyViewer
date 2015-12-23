using ProductPolicyViewer.ProductPolicy;
using System.Windows;

namespace ProductPolicyViewer
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         var data = new PPObject();
         dgOutput.ItemsSource = data.Policies;
      }
   }
}
