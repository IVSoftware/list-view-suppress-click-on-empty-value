One way to suppress suppress the click behavior under those conditions is to make a custom `ListViewEx` class that overrides `WndProc` and performs a `HitTest` when the mouse is clicked. (You'll have to go to the designer file and swap out the `ListView` references of course.)

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