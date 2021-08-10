using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kochi_TVM.Pages.Custom
{
    /// <summary>
    /// Interaction logic for RounderProgressBarControl.xaml
    /// </summary>
    public partial class RounderProgressBarControl : UserControl
    {
        private const string PERCENTS_TEXT = "{0}%";

        private delegate void VoidDelegete();
        private Timer timer1;

        public RounderProgressBarControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            timer1 = new Timer(80);
            timer1.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            timer1.Enabled = true;
        }

        void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            timer1.Elapsed -= OnTimerElapsed;
            timer1.Stop();
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            rotationCanvas.Dispatcher.Invoke
                (
                new VoidDelegete(
                    delegate
                    {
                        SpinnerRotate.Angle += 30;
                        if (SpinnerRotate.Angle == 360)
                        {
                            SpinnerRotate.Angle = 0;
                        }
                    }
                    ),
                null
                );
        }
    }
}
