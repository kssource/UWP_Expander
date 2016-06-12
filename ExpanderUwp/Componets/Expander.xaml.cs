using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ExpanderUwp.Componets
{
    public sealed partial class Expander : UserControl
    {
        public Expander()
        {
            this.InitializeComponent();
        }

        private bool isLocalExpanderAction = false;
        //private bool listLoaded = false;

        public static readonly DependencyProperty ExpanderItemsProperty =
            DependencyProperty.Register("ExpanderItems", typeof(ObservableCollection<IExpanderDataItem>), typeof(Expander),
                new PropertyMetadata(null, OnSourceChanged)
                );

        public ObservableCollection<IExpanderDataItem> ExpanderItems
        {
            get { return (ObservableCollection<IExpanderDataItem>)GetValue(ExpanderItemsProperty); }
            set { SetValue(ExpanderItemsProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e != null)
            {
                if (e.OldValue != null)
                {
                    var isSourceListType = e.OldValue.GetType() == typeof(ObservableCollection<IExpanderDataItem>);
                    if (isSourceListType == false) return;

                    Expander exComp = d as Expander;
                    var list = e.OldValue as ObservableCollection<IExpanderDataItem>;
                    foreach (INotifyPropertyChanged item in list)
                    {
                        item.PropertyChanged -= exComp.item_PropertyChanged;
                    }
                }

                if (e.NewValue != null)
                {
                    var isSourceListType = e.NewValue.GetType() == typeof(ObservableCollection<IExpanderDataItem>);
                    if (isSourceListType == false) return;

                    Expander exComp = d as Expander;
                    var list = e.NewValue as ObservableCollection<IExpanderDataItem>;
                    foreach (INotifyPropertyChanged item in list)
                    {
                        item.PropertyChanged += exComp.item_PropertyChanged;
                    }
                }
            }
        }


        private void updateUiLater()
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            #pragma warning disable 4014
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { updateUi(); });
        }

        private void updateUi()
        {
            isLocalExpanderAction = true;//disable change listener

            bool oneItemIsAlreadyExpanded = false;// only one item allowed be expanded
            // catch/repair wrong states
            for (int ind = 0; ind < ExpanderItems.Count; ++ind)
            {
                var item = ExpanderItems[ind];
                if(item.expanded )
                {
                    if (oneItemIsAlreadyExpanded) item.expanded = false;//colapse
                    else oneItemIsAlreadyExpanded = true;//mark
                }

                if(isVisualStateExpanded(item) != item.expanded)
                {
                    if (item.expanded) expandItem(item);
                    else collapseItem(item);
                }
            }

            isLocalExpanderAction = false;
        }


        public void toggleItem(int listIndex)
        {
            for (int ind = 0; ind < ExpanderItems.Count; ++ind)
            {

                var item = ExpanderItems[ind];
                if (ind == listIndex)
                {
                    item.expanded = !item.expanded;
                }
                else
                {
                    if (item.expanded)
                    {
                        item.expanded = false;
                    }
                }
            }
            updateUi();
        }


        private void itemHeader_tapped(object sender, TappedRoutedEventArgs e)
        {
            var headerComponent = sender as FrameworkElement;
            var data = headerComponent.DataContext;
            int listIndex = findListIndex(data);
            toggleItem(listIndex);
        }


        private int findListIndex(object data)
        {
            int index = 0;
            for (int ind = 0; ind < ExpanderItems.Count; ++ind)
            {
                if (data.Equals(ExpanderItems[ind]))
                {
                    index = ind;
                    break;
                }
            }

            return index;
        }

        private void expandItem(IExpanderDataItem item)
        {
            //handle arrow rotation

            //hide previous(rotaded) img
            FrameworkElement prevImage = findItemElement(item, expandedArrowComponentList);
            prevImage.Visibility = Visibility.Collapsed;

            FrameworkElement arrowComp = findItemElement(item, collapsedArrowComponentList);
            arrowComp.Visibility = Visibility.Visible;

            Visual xamlVisual = ElementCompositionPreview.GetElementVisual(arrowComp);
            var compositor = xamlVisual.Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.Duration = TimeSpan.FromSeconds(1.0d);
            animation.IterationCount = 1;
            animation.InsertKeyFrame(0.0f, 0.0f);

            float angle = 90.0f;//expand
            animation.InsertKeyFrame(1.0f, angle);
            xamlVisual.CenterPoint = new System.Numerics.Vector3(5.0f, 5.0f, 0);

            xamlVisual.StartAnimation("RotationAngleInDegrees", animation);

            // show item content
            ContentControl contentWrapper = findItemContentWrapperOfItem(item) ;
            contentWrapper.Content = contentWrapper.Tag;
            ((UIElement)contentWrapper.Content).Visibility = Visibility.Visible;

        }


        private void collapseItem(IExpanderDataItem item)
        {
            //handle arrow rotation

            //hide previous(rotaded) img
            FrameworkElement prevImage = findItemElement(item, collapsedArrowComponentList);
            prevImage.Visibility = Visibility.Collapsed;

            FrameworkElement arrowComp = findItemElement(item, expandedArrowComponentList); ;
            arrowComp.Visibility = Visibility.Visible;

            Visual xamlVisual = ElementCompositionPreview.GetElementVisual(arrowComp);
            var compositor = xamlVisual.Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.Duration = TimeSpan.FromSeconds(1.0d);
            animation.IterationCount = 1;
            animation.InsertKeyFrame(0.0f, 0.0f);

            float angle = -90.0f;//
            animation.InsertKeyFrame(1.0f, angle);
            xamlVisual.CenterPoint = new System.Numerics.Vector3(5.0f, 5.0f, 0);
            //animation.StopBehavior = AnimationStopBehavior.SetToFinalValue;

            xamlVisual.StartAnimation("RotationAngleInDegrees", animation);

            // hide item content
            ContentControl contentWrapper = findItemContentWrapperOfItem(item);
            ((UIElement)contentWrapper.Content).Visibility = Visibility.Collapsed;
            contentWrapper.Content = null;

        }


        private List<FrameworkElement> collapsedArrowComponentList = new List<FrameworkElement>();
        private void collapsedArrowLoaded(object sender, RoutedEventArgs e)
        {
            var tmp = sender as FrameworkElement;
            collapsedArrowComponentList.Add(tmp);
        }
        private void collapsedArrow_Unloaded(object sender, RoutedEventArgs e)
        {
            var tmp = sender as FrameworkElement;
            collapsedArrowComponentList.Remove(tmp);
        }



        private List<FrameworkElement> expandedArrowComponentList = new List<FrameworkElement>();
        private void expandedArrowLoaded(object sender, RoutedEventArgs e)
        {
            var tmp = sender as FrameworkElement;
            expandedArrowComponentList.Add(tmp);
        }
        private void expandedArrow_Unloaded(object sender, RoutedEventArgs e)
        {
            var tmp = sender as FrameworkElement;
            expandedArrowComponentList.Remove(tmp);
        }



        private List<ContentControl> itemContentWrappersList = new List<ContentControl>();
        private void itemContentWrapperLoaded(object sender, RoutedEventArgs e)
        {
            var contentWrapper = sender as ContentControl;
            itemContentWrappersList.Add(contentWrapper);

            //contentWrapper.Visibility = Visibility.Visible;
            contentWrapper.Tag = contentWrapper.Content;
            contentWrapper.Content = null;
            updateUiLater();
        }
        private void itemContentWrapper_Unloaded(object sender, RoutedEventArgs e)
        {
            var contentWrapper = sender as ContentControl;
            itemContentWrappersList.Remove(contentWrapper);
        }
        private ContentControl findItemContentWrapperOfItem(IExpanderDataItem item)
        {
            ContentControl elem = null;
            foreach (ContentControl fe in itemContentWrappersList)
            {
                if (item.Equals(fe.DataContext))
                {
                    elem = fe;
                    break;
                }
            }

            return elem;
        }

        private FrameworkElement findItemElement(IExpanderDataItem item, List<FrameworkElement> elementsList)
        {
            FrameworkElement elem = null;
            foreach (FrameworkElement fe in elementsList)
            {
                if (item.Equals(fe.DataContext))
                {
                    elem = fe;
                    break;
                }
            }

            return elem;
        }

        private bool isVisualStateExpanded(IExpanderDataItem item)
        {
            ContentControl contentWrapper = findItemContentWrapperOfItem(item);
            bool expanded = contentWrapper.Content != null;

            return expanded;
        }


        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isLocalExpanderAction) return;

            if (e.PropertyName.Equals("expanded"))
            {
                updateUi();
            }
        }



    }


    ///////////////////////////////////
    public interface IExpanderDataItem : INotifyPropertyChanged
    {
        string headerText { get; set; }
        string contentText { get; set; }
        bool expanded { get; set; }
    }

    public class ExpanderDataItem : IExpanderDataItem
    {
        private string header;
        private string content;
        private bool expandedValue = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public string headerText
        {
            get
            {
                return header;
            }

            set
            {
                if (header.Equals(value)) return;

                header = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("headerText"));
                }
                header = value;
            }
        }

        public string contentText
        {
            get
            {
                return content;
            }

            set
            {
                if (content.Equals(value)) return;

                content = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("contentText"));
                }

                content = value;
            }
        }

        public bool expanded
        {
            get
            {
                return expandedValue;
            }

            set
            {
                if (expandedValue == value) return;

                expandedValue = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("expanded"));
                }
            }
        }


        public ExpanderDataItem(string header, string content)
        {
            this.header = header;
            this.content = content;
        }


    }
}
