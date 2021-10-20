using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            App();
        }
        public void App()
        {
            CreateGraphics().DrawRectangle(new Pen(Color.White), StartButton.Location.X + StartButton.Size.Width, StartButton.Location.Y, Width - StartButton.Size.Width, StartButton.Size.Height);

            int numberOfWorkers = 5;
            int xAxisValue = 0;
            int pointRadius = 10;
            int pointScaler = 100;
            int yUpper = 0;
            Random rnd = new Random();
            Dictionary<int, Color> threadColor = new Dictionary<int, Color> { };
            for (int i = 0; i < numberOfWorkers; i++)
            {
                threadColor.Add(i, Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
            }

            List<WorkerWithQueue<string>> workers = new List<WorkerWithQueue<string>> { };
            for (int i = 0; i < numberOfWorkers; i++)
            {
                workers.Add(new WorkerWithQueue<string>((string a) =>
                {
                    Thread.Sleep(500);
                    int currentThreadId = Thread.CurrentThread.ManagedThreadId;
     
                    if (xAxisValue>=Width)
                    {
                        xAxisValue = 0;
                        //yUpper += rnd.Next(pointRadius*2);
                    }
                    for (int j = 0; j < 100; j++)
                    {
                        double radian = (xAxisValue * Math.PI) / 180;
                        CreateGraphics().FillEllipse(new SolidBrush(threadColor[currentThreadId-3]), xAxisValue, (int)(Math.Sin(radian) * pointScaler+Height/3+yUpper), pointRadius, pointRadius);
                        if (CurrentThreadLabel.InvokeRequired)
                        {
                            CurrentThreadLabel.Invoke(new MethodInvoker(delegate {CurrentThreadLabel.Text = $"Current thread ID: {currentThreadId}"; }));
                            CurrentThreadLabel.Invoke(new MethodInvoker(delegate { CurrentThreadLabel.ForeColor = threadColor[currentThreadId - 3]; }));
                        }
                        xAxisValue += 1;
                    }

                }));
            }

            for (int i = 0; i < numberOfWorkers - 1; i++)
            {
                workers[i].StartExecuting(workers[i + 1]);
            }
            workers[numberOfWorkers - 1].StartExecuting(workers[0]);

            workers[0].EnqueueTask("");
        }

    }
}
