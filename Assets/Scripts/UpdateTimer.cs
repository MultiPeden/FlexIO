using System.Diagnostics;
using System.IO;

namespace Assets.Scripts
{
    class UpdateTimer
    {

        private Stopwatch updateTimer = new Stopwatch();
        private TextWriter writer;

        public UpdateTimer()
        {

            writer = new StreamWriter(@"C:\test\timer\UpdateTimer.csv", append: true);
            long FileLength = new FileInfo(@"C:\test\timer\UpdateTimer.csv").Length;


            ///    timerRes = new TimerRes();
            if (FileLength == 0)
            {
                writer.WriteLine("Elapsed milisec, Number of Points");
            }
        }


        public void StartUpdateTimer()
        {
            updateTimer.Reset();
            updateTimer.Start();
        }



        public void StopUpdateTimer(int numbOfPoints)
        {
            updateTimer.Stop();
            double elapsedmili = updateTimer.Elapsed.TotalMilliseconds;

            writer.WriteLine(elapsedmili + "," + numbOfPoints);

        }


        public void FlushTimer()
        {
            writer.Flush();
            writer.Close();
        }

    }
}
