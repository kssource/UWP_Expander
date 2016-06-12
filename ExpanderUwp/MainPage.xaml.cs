using ExpanderUwp.Componets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace ExpanderUwp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<IExpanderDataItem> expanderItemsList;

        public MainPage()
        {
            this.InitializeComponent();
            initExpanderData();
        }

        private void initExpanderData()
        {
            expanderItemsList = new ObservableCollection<IExpanderDataItem>();
            string content = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, "
                +"sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, "
                +"sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. "
                +"Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. "
                +"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor"
                +" invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. "
                +"At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, "
                +"no sea takimata sanctus est Lorem ipsum dolor sit amet.";

            ExpanderDataItem item = new ExpanderDataItem("Header 1", content);
            expanderItemsList.Add(item);
            item = new ExpanderDataItem("Another Header", content);
            expanderItemsList.Add(item);
            item = new ExpanderDataItem("Header 3", content);
            item.expanded = true;
            expanderItemsList.Add(item);
            item = new ExpanderDataItem("Header 4", content);
            expanderItemsList.Add(item);
            item = new ExpanderDataItem("Header 5", content);
            //item.expanded = true;
            expanderItemsList.Add(item);

        }

        // tests
        private void TestButtonClicked(object sender, RoutedEventArgs e)
        {
            //testAddRemoveItems();
            //testChangeItem();
            testExpand();
        }

        private void testExpand()
        {
            var item = expanderItemsList[2];
            item.expanded = false;
            //item.expanded = true;

        }

        private void testChangeItem()
        {
            var item = expanderItemsList[2];
            item.headerText = "New Header";
            item.contentText = "Changed Content";
        }

        private void testAddRemoveItems()
        {
            ExpanderDataItem item;

            //item = new ExpanderDataItem("Header Addded Item", "Added Item");
            //item.expanded = true;
            //expanderItemsList.Add(item);

            //item = new ExpanderDataItem("Header Inserted Item", "Inserted Item");
            //item.expanded = true;
            //expanderItemsList.Insert(1, item);

            expanderItemsList.RemoveAt(2);
        }
    }
}
