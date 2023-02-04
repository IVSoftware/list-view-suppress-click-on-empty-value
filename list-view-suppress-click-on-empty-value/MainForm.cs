using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Linq;

namespace list_view_with_empty_values
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            listView.View = View.Details;
            listView.Columns.Add("Name");
            listView.Columns.Add("Light", 80);
            listView.Columns.Add("Time", 150);
            listView.Columns[0].Width = listView.ClientRectangle.Width - 230;
            listView.Items.Add(new BoundListViewItem("LV Item 0", 50, TimeSpan.FromMinutes(5)));
            listView.Items.Add(new BoundListViewItem("LV Item 1", 75, TimeSpan.FromMinutes(10)));
            listView.Items.Add(new BoundListViewItem("LV Item 2", 100, TimeSpan.FromMinutes(15)));

            listView.SelectedIndexChanged += onListViewSelectionChanged;
        }

        private void onListViewSelectionChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                labelSelected.Text = "None";
            }
            else
            {
                labelSelected.Text = string.Join(
                        ", ",
                        listView.SelectedItems.Cast<ListViewItem>().Select(_ => _.Index));
            }
        }
    }
    class ListViewEx : ListView
    {
        const int WM_LBUTTONDOWN = 0x0201;
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == WM_LBUTTONDOWN)
            {
                if (suppressClick())
                {
                    m.Result = (IntPtr)1;
                    return;
                }
            }
            base.WndProc(ref m);
        }
        private bool suppressClick()
        {
            var hitTest = HitTest(PointToClient(MousePosition));
            if ((hitTest.Item == null) || string.IsNullOrEmpty(hitTest.Item.Text))
            {
                return true;
            }
            return false;
        }
    }
    class BoundListViewItem : ListViewItem , INotifyPropertyChanged
    {
        public BoundListViewItem(string text, double light, TimeSpan time) : this (light, time)=>
            base.Text = text;

        public BoundListViewItem(double light, TimeSpan time)
        {
            SubItems.Add(new ListViewSubItem(this, nameof(Light)) { Name = nameof(Light) });
            SubItems.Add(new ListViewSubItem(this, nameof(Time)) { Name = nameof(Time) });
            Light = light;
            Time = time; 
        }
        public new string Text
        {
            get => base.Text;
            set
            {
                if (!Equals(base.Text, value))
                {
                    base.Text = value;
                    OnPropertyChanged();
                }
            }
        }
        double _light = 0;
        public double Light
        {
            get => _light;
            set
            {
                if (!Equals(_light, value))
                {
                    _light = value;
                    OnPropertyChanged();
                    setSubItemValue(value);
                    SubItems[nameof(Light)].Text = value.ToString();
                }
            }
        }

        TimeSpan _time = TimeSpan.MaxValue;
        public TimeSpan Time
        {
            get => _time;
            set
            {
                if (!Equals(_time, value))
                {
                    _time = value;
                    setSubItemValue(value);
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void setSubItemValue(object value, [CallerMemberName] string? propertyName = null) =>
            SubItems[propertyName].Text = $"{value}";
    }
}
