using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace Chat.ViewModels
{
    public class ListBoxScrollIntoViewBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (listBox?.SelectedItem != null)
            {
                listBox.Dispatcher.Invoke(() =>
                {
                    listBox.UpdateLayout();
                    listBox.ScrollIntoView(listBox.SelectedItem);
                });
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }
    }
}
